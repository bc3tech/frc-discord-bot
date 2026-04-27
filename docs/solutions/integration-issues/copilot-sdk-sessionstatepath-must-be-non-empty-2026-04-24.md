---
title: Copilot SDK SessionFs.SessionStatePath must be non-empty for ISessionFsHandler to receive session-state IO
date: 2026-04-24
category: integration-issues
module: discord-gpt-copilot
problem_type: integration_issue
component: assistant
symptoms:
  - "session.resume RPC fails every turn with 'Session not found: <guid>'"
  - "Bot starts a fresh Copilot session for each Discord message, losing conversational memory"
  - "ISessionFsHandler receives only agent scratch writes (temp/, *.txt) — never session.db, workspace.yaml, or checkpoints/"
  - "Local disk under ConfigDir accumulates orphan session-state directories that the CLI cannot resume"
root_cause: config_error
resolution_type: config_change
severity: high
related_components:
  - tooling
tags:
  - github-copilot-sdk
  - session-resume
  - session-fs-handler
  - blob-storage
  - integration-trap
---

# Copilot SDK SessionFs.SessionStatePath must be non-empty for ISessionFsHandler to receive session-state IO

> **Note (2026-04-26):** This fix is correct for multi-instance topologies that need cross-host `session.resume`. For **single-instance** hosts (e.g. Container App with `maxReplicas: 1`), it has been **superseded** by [`copilot-sdk-tool-output-spills-need-real-disk-2026-04-26.md`](./copilot-sdk-tool-output-spills-need-real-disk-2026-04-26.md), which reverts to the SDK's local-disk default so that tool-output spills remain readable by shelled-out CLI tools (`rg`, `cat`). The Discord bot host now uses local-disk session FS; `BlobSessionFsHandler` / `WithBlobSessionStorage` remain available in the `BC3Technologies.DiscordGpt.Storage.Blob` package for SDK consumers who do scale out.

## Problem

The Discord bot persists Copilot SDK session state to Azure Blob Storage via a custom `ISessionFsHandler` (`BlobSessionFsHandler`). Despite the handler being correctly registered and wired onto every `SessionConfig`, the SDK was still writing `session.db` / `workspace.yaml` / `checkpoints/` to local disk under the per-conversation `ConfigDir`. Because the bot's host process is conversation-scoped (different ConfigDir per turn) and stateless across restarts, the next turn's CLI process couldn't find the prior session and `session.resume` failed with `Session not found: <guid>`. Every Discord turn became a fresh session.

## Symptoms

- `BC3Technologies.DiscordGpt.Copilot.GitHubCopilotPromptHarness: Warning: Failed to resume Copilot session <guid>; creating a new session`
- `StreamJsonRpc.RemoteInvocationException: Request session.resume failed with message: Session not found: <guid>`
- The Azurite `copilot-sessions` container only contained `<sessionId>/temp/...` and `<sessionId>/*.txt` blobs (the agent's scratch FS) — no `session.db`, no `workspace.yaml`, no `checkpoints/index.md`.
- Inspecting `%LOCALAPPDATA%\Temp\frc-bot-copilot\cfg\<conv>\session-state\` showed ~12 sessions accumulated for a single conversation, all with `workspace.yaml` and `checkpoints/`, only ~2 with `session.db`.
- `BlobSessionFsHandler` Debug logs showed zero session-state writes — only agent-scratch writes.

## What Didn't Work

- **Verifying the handler wiring.** `GitHubCopilotPromptHarness.AcquireSessionAsync` correctly sets `sessionConfig.CreateSessionFsHandler = _ => _sessionFsHandler` and `BuildResumeConfig` correctly copies the factory onto the resume config. Wiring was fine.
- **Verifying the per-conversation ConfigDir was stable.** It was — the same `ConfigDir=…\cfg\0-961df10247e387b4` appeared on consecutive turns.
- **Trusting the existing comment.** `DiscordGptBuilderCopilotExtensions.cs` initialized `SessionFs = new SessionFsConfig { SessionStatePath = string.Empty, … }` with a comment claiming `"empty SessionStatePath keeps the SDK default layout while still flipping the SDK into 'use the supplied FsHandler' mode"`. That assumption was wrong — empty `SessionStatePath` makes the SDK fall back to local disk for session metadata, even when an `ISessionFsHandler` is supplied.

## Solution

Set `CopilotClientOptions.SessionFs.SessionStatePath` to a non-empty virtual path. The SDK only routes session-state reads/writes (the `session.db`, `workspace.yaml`, `checkpoints/` files) over the FS-handler RPC channel when it has a non-empty virtual root to scope them under. Empty path → local disk. Non-empty path → handler.

```csharp
// gpt/src/BC3Technologies.DiscordGpt.Copilot/DiscordGptBuilderCopilotExtensions.cs
var clientOptions = new CopilotClientOptions
{
    AutoStart = options.AutoStart,
    UseStdio = options.UseStdio,

    // Route the CLI's session state (session.db, workspace.yaml, checkpoints)
    // through the registered ISessionFsHandler. An EMPTY SessionStatePath makes
    // the SDK fall back to local disk under ConfigDir — session.db never lands
    // in blob storage and ResumeSessionAsync fails ("Session not found")
    // whenever a different CLI process tries to pick a conversation back up.
    SessionFs = new SessionFsConfig
    {
        InitialCwd = string.Empty,
        SessionStatePath = "/session-state",
        Conventions = SessionFsSetProviderRequestConventions.Posix,
    },
};
```

`BlobSessionFsHandler.GetBlobName(sessionId, path)` maps the resulting RPC calls to blobs at `<sessionId>/session-state/session.db`, `<sessionId>/session-state/workspace.yaml`, `<sessionId>/session-state/checkpoints/index.md`. `Conventions = Posix` because the virtual path uses forward slashes regardless of host OS.

## Why This Works

The Copilot SDK has **two separate FS surfaces**:

1. **Agent scratch FS** — the working directory the agent sees when it calls `view` / `edit` / `create`. Always routed through `ISessionFsHandler` when one is supplied via `SessionConfig.CreateSessionFsHandler`.
2. **Session-state FS** — internal CLI files (`session.db`, `workspace.yaml`, `checkpoints/`) used to reconstitute a session on `ResumeSessionAsync`. Routed through the FS handler **only when `CopilotClientOptions.SessionFs.SessionStatePath` is non-empty**. Otherwise written to local disk under `ConfigDir`.

The bot's wiring was correct for surface #1 (which is why agent scratch blobs were appearing) but missing for surface #2. Because the host's per-conversation ConfigDir lives in a process-local temp directory, surface-#2 state was effectively ephemeral — and `session.resume` could never find a session created by a previous CLI process.

Setting `SessionStatePath` to a non-empty virtual path forwards every session-state file IO over the JSON-RPC channel. The handler writes them to blob, where any future host process can read them back during resume.

## Prevention

- **Don't trust comments asserting "empty == default behavior" for SDK config knobs.** Test the assumption before shipping. For SDKs without published source, use reflection (`dotnet-script`, `Type.GetProperties()`) on the assembly to enumerate the option shape, then probe behavior with a minimal repro.
- **When introducing a custom `ISessionFsHandler`, log every call.** If the handler isn't being asked to do work you expect, that's diagnostic information for free. `BlobSessionFsHandler` already logs reads/writes at Debug — checking those logs is what surfaced this gap.
- **Treat SDK opt-ins as both-or-neither.** If you opt into custom session-state persistence (e.g., blob, distributed cache, encrypted store), wire it for both FS surfaces. A half-wired persistence layer that handles agent scratch but not session metadata creates the worst-of-both-worlds: blob bloat with no resume capability.
- **Consider adding a startup smoke test** that calls `CreateSessionAsync` → `ResumeSessionAsync` against the configured handler and asserts the resume succeeds. Catches this class of regression before it reaches production.

## Related Issues

- [`docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md`](../best-practices/conversation-scoped-copilot-sessions-2026-04-23.md) — broader architecture for per-conversation Copilot SDK sessions. The fix here is the missing config knob that makes that architecture's "durable per-conversation memory across process restarts and scale-out" claim actually hold.
