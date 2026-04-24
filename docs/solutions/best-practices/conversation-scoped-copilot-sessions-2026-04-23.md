---
title: Conversation-scoped GHCP SDK sessions for chat-style hosts
date: 2026-04-23
category: best-practices
module: discord-gpt-copilot
problem_type: best_practice
component: assistant
severity: high
applies_when:
  - Hosting the GitHub Copilot SDK inside a multi-tenant chat or bot process
  - The bot needs durable per-conversation memory across process restarts and scale-out
  - The host process runs alongside a developer's own Copilot CLI session and must not leak it
  - Running multiple concurrent conversations through one CopilotClient
tags:
  - github-copilot-sdk
  - session-management
  - conversation-scope
  - asynclocal
  - multi-tenant
  - security
---

# Conversation-scoped GHCP SDK sessions for chat-style hosts

## Context

The GitHub Copilot SDK's default `CreateSessionAsync` + `SessionConfig` model assumes a **single-user CLI** posture: one `~/.copilot` config dir, one current working directory, one ambient session per process. Dropping that into a multi-conversation host (a Discord bot, a Slack app, a multi-tenant chat service) creates two failure modes that look unrelated but share a root cause:

1. **Tooling leak** — when the host runs on a developer machine that also has its own Copilot CLI history, the SDK happily reads `~/.copilot/sessions/` and exposes the developer's local tools (`sql`, `session_store_sql`, custom MCP servers). Bot users see `[tool: sql]` in the trace.
2. **No conversational memory** — every Discord turn becomes a brand-new session, so the bot can't refer back to "what we just said." Users churn through context they think the bot should remember.

Both come from the same gap: the host has no concept of *whose* session it's running. The SDK was designed for "the user is the process owner." A bot has many concurrent users.

## Guidance

Treat the GHCP SDK as a **per-conversation execution sandbox**, not a process-wide singleton. Three layers, in order:

### 1. Flow conversation context via `AsyncLocal`

Introduce an `IConversationContextAccessor` with an `AsyncLocal<ConversationContext?> Current` slot. Set it once at the harness boundary (before iterating session config sources, before `CreateSessionAsync`/`ResumeSessionAsync`), clear it in `finally`. Anything downstream — your `ISessionConfigSource` implementation, your tool authorization handlers, your telemetry — reads `Current` to discover *whose* turn this is, without threading it through every method signature.

```csharp
public interface IConversationContextAccessor
{
    ConversationContext? Current { get; set; }
}

public sealed class AsyncLocalConversationContextAccessor : IConversationContextAccessor
{
    private static readonly AsyncLocal<ConversationContext?> s_current = new();
    public ConversationContext? Current
    {
        get => s_current.Value;
        set => s_current.Value = value;
    }
}
```

Set it once at the entry point — not per source, not per tool call:

```csharp
// in your harness's RunPromptCoreAsync:
_conversationContextAccessor.Current = context;
try
{
    foreach (ISessionConfigSource source in _sessionConfigSources) { ... }
    CopilotSession session = await AcquireSessionAsync(context, sessionConfig, ct);
    // ... run prompt ...
}
finally
{
    _conversationContextAccessor.Current = null;
}
```

### 2. Make `SessionConfig.ConfigDir` per-conversation, `WorkingDirectory` per-turn

The SDK reads existing sessions and tool config from `ConfigDir`. Pointing it at the developer's `~/.copilot` is the leak. Pointing it at one shared bot temp dir is the *cross-tenant* leak. The right scope is **one conversation** — stable across that conversation's turns so resumed sessions find their state, isolated from every other conversation.

`WorkingDirectory` is different — the SDK treats it as a per-invocation execution sandbox. Unique per turn is fine and avoids any chance of cross-turn file state.

```csharp
// In your ISessionConfigSource implementation:
public ValueTask ConfigureAsync(SessionConfig sessionConfig, CancellationToken ct)
{
    ConversationContext context = _accessor.Current
        ?? throw new InvalidOperationException(
            "ConversationContext is required; harness must set IConversationContextAccessor.Current.");

    string safeKey = BuildSafeConversationKey(context.Key);
    string botRoot = Path.Combine(Path.GetTempPath(), "frc-bot-copilot");

    sessionConfig.ConfigDir = Path.Combine(botRoot, "cfg", safeKey);     // stable per conversation
    sessionConfig.WorkingDirectory = Path.Combine(botRoot, "work", Guid.NewGuid().ToString("N"));
    sessionConfig.EnableConfigDiscovery = false;

    // Minimal exclusion list — the SDK exposes very little by default;
    // explicitly block the few that leak host context into tool traces.
    sessionConfig.ExcludedTools = ["ask_user", "web_fetch", "web_search"];
    return ValueTask.CompletedTask;
}

private static string BuildSafeConversationKey(ConversationKey key)
{
    byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(key.Id));
    string prefix = Convert.ToHexString(hash.AsSpan(0, 8)).ToLowerInvariant();
    return $"{(int)key.Scope}-{prefix}";  // e.g., "1-a3f9...": short, deterministic, path-safe
}
```

The hashed prefix matters on Windows: raw conversation IDs (Discord snowflakes, channel paths, thread IDs concatenated) can blow past 256-char path limits when the temp root and `cfg/` segments are added.

### 3. Persist the conversation-id → session-id mapping out-of-process

`CreateSessionAsync` returns a fresh `CopilotSession` each call; `ResumeSessionAsync(sessionId, ResumeSessionConfig, ct)` revives a prior one. Resume only works if you remember the session ID. An in-memory `Dictionary` works for one process — it dies on restart and doesn't survive horizontal scale-out, which is exactly when conversational memory matters most.

Store the mapping in your existing durable state (Azure Table Storage, DynamoDB, Postgres). The mapping is small: `(conversationKey) → sessionId`. The harness's per-turn flow becomes:

```csharp
private async Task<CopilotSession> AcquireSessionAsync(
    ConversationContext context, SessionConfig config, CancellationToken ct)
{
    string? existingId = await _sessionMap.GetAsync(context.Key, ct);
    if (existingId is not null)
    {
        try
        {
            ResumeSessionConfig resumeConfig = BuildResumeConfig(config);
            return await _client.ResumeSessionAsync(existingId, resumeConfig, ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            // Session was evicted server-side, or the SDK rejected it — fall through to create.
            LogResumeFailed(ex, context.Key, existingId);
            await _sessionMap.RemoveAsync(context.Key, ct);
        }
    }

    CopilotSession session = await _client.CreateSessionAsync(config, ct);
    await _sessionMap.SetSessionIdAsync(context.Key, session.SessionId, ct);
    return session;
}
```

Two non-obvious details:

- **`ResumeSessionConfig` is not `SessionConfig`.** They share no base type. `BuildResumeConfig` is 25 lines of property-by-property copying. If you forget any, your resumed session silently loses tool authorization, telemetry, or excluded tools.
- **Per-conversation lock.** Two concurrent turns in the same conversation racing `Resume`/`Create` will create duplicate sessions and the second `SetSessionIdAsync` overwrites the first. A `ConcurrentDictionary<string, SemaphoreSlim>` keyed on the safe conversation key serializes them. Stale entries are tolerable — the leak is bounded by conversation count, not turn count.

## Why This Matters

- **Security.** Without per-conversation `ConfigDir`, a developer running the bot locally exposes their personal Copilot history to every Discord user. Reviewers will not catch this — it requires running the bot, opening the right tool trace, and recognizing tools that don't belong.
- **UX.** Without persisted session resume, the bot cannot answer "what did you just say?" Users stop trusting it as a conversational partner and start treating it as a glorified search box.
- **Operability.** The `AsyncLocal` + accessor pattern keeps the conversation context out of every method signature. New session config sources, new tool authorization handlers, new telemetry sinks all read `_accessor.Current` instead of taking it as a parameter — there's nothing for downstream code to forget to thread through.
- **Configurability.** Bind sensitive harness options (`GitHubToken`, `Telemetry`, `CliLogLevel`) via `IConfiguration.GetSection("DiscordGpt")` so consumers control where values come from — appsettings, env vars, Key Vault references, container-app secrets — without forking the host.

## When to Apply

- Hosting the GitHub Copilot SDK in any process that handles more than one user's traffic.
- The host shares a machine or container image with a developer or operator who has their own Copilot CLI configured.
- Conversation length is more than one turn and users expect continuity.
- You scale the host horizontally or restart it during a user's session.
- You need per-conversation tool authorization (different conversations get different tool surfaces).

## Examples

### Wiring it up

```csharp
services
    .AddDiscordGpt()
    .UseCopilot(c => c
        .WithBlobSessionStorage(credential, blobUri, o => o.ContainerName = "copilot-sessions")
        .WithTableConversationSessionMap(o => o.TableName = "copilotsessions")  // durable mapping
        .WithSessionConfigSource<IsolatedSessionConfigSource>()                  // per-conv ConfigDir
        // ...
    );

// IConfiguration binding so secrets flow from any source
services.Configure<DiscordGptOptions>(configuration.GetSection("DiscordGpt"));
```

### Sharp edge: C# test namespace collision

When writing tests against types in your isolated config source, watch for this:

```csharp
// app/FunctionApp.Tests/IsolatedSessionConfigSourceTests.cs
namespace FunctionApp.Tests;
using ChatBot.Diagnostics;  // ❌ resolves as FunctionApp.Tests.ChatBot.Diagnostics first → CS0234
```

C# resolves nested namespaces relative to the current namespace before walking outward. Fix with a `global::` qualified alias:

```csharp
namespace FunctionApp.Tests;
using ChatBotIsolated = global::ChatBot.Diagnostics.IsolatedSessionConfigSource;
```

This is a recurring trap whenever your test project's namespace shares a token with the production project (`FunctionApp` vs `ChatBot` both being top-level namespaces in the same solution). Prefer making the test root namespace a sibling (e.g., `ChatBot.Tests`) when starting a new test project.

## Related

- [docs/plans/2026-04-23-001-feat-conversation-scoped-session-store-plan.md](../../plans/2026-04-23-001-feat-conversation-scoped-session-store-plan.md) — full execution plan
- [docs/brainstorms/2026-04-23-conversation-scoped-session-store-requirements.md](../../brainstorms/2026-04-23-conversation-scoped-session-store-requirements.md) — requirements doc
- [docs/solutions/best-practices/builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md](builder-scoped-di-extensions-for-harness-concerns-2026-04-22.md) — companion best-practice for `WithTableConversationSessionMap` extension shape
