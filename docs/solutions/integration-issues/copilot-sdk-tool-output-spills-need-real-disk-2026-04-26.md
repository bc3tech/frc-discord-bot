---
title: Copilot SDK tool-output spills require a real on-disk filesystem to be usable
date: 2026-04-26
category: integration-issues
module: discord-gpt-copilot
problem_type: integration_issue
component: assistant
symptoms:
  - "Bot calls a tool, gets back a 'Saved to: /session-state/temp/<file>' pointer, then fails to read it"
  - "rg / cat / grep return 'IO error' on the spilled path"
  - "Agent flails (creates note.txt, retries the same tool, calls apply_patch) instead of answering the user"
  - "Discord answer falls back to 'I can't reliably look that up' even though the configured tools succeeded"
root_cause: design_mismatch
resolution_type: config_change
severity: high
related_components:
  - tooling
  - storage-blob
tags:
  - github-copilot-sdk
  - session-fs-handler
  - blob-storage
  - tool-output-spill
  - integration-trap
---

# Copilot SDK tool-output spills require a real on-disk filesystem to be usable

## Problem

When a tool returns a response larger than the SDK's inline-result threshold (~64KB observed), the SDK spills the response to a file via the registered `ISessionFsHandler` and hands the agent back a string like `Saved to: /session-state/temp/<guid>.json`. The agent then treats that string as a real OS path and shells out to read it — typically `rg <pattern> /session-state/temp/<guid>.json` or `cat /session-state/temp/<guid>.json`.

When the registered handler is virtual (e.g. `BlobSessionFsHandler` writing to Azure Blob), the path doesn't exist on local disk on either Linux or Windows hosts, so every shelled-out CLI tool returns an IO error. The agent loses access to its own tool result, gives up on the user's question, and the Discord reply degrades to "I can't reliably look that up" or similar — even though every configured tool actually succeeded.

## Symptoms

A real failure trace from a Discord turn ("who are the top 10 teams in our division at worlds by EPA"):

1. `tba_api` returns 228KB of `/events/2026` data.
2. SDK spills it: agent receives `Saved to: /session-state/temp/8d1c…json`.
3. Agent calls `rg -n "Houston|Galileo|…" /session-state/temp/8d1c…json`.
4. rg replies `rg: /session-state/temp/8d1c…json: IO error`.
5. Agent retries `tba_api`, gets the same spill, calls `apply_patch` to create a `note.txt` placeholder, and finally posts a refusal to Discord.

The smoking gun is the `rg: <virtual-path>: IO error` line — rg ran successfully, it just couldn't find the file because the path is purely virtual (only the SDK and the handler know how to resolve it).

## What Didn't Work

- **Assuming this was a Windows-vs-Linux path issue.** The error format (`rg: <path>: IO error`) is rg's own — rg ran, didn't find the file. The path is virtual on both OSes. Deploying to Linux would not change the outcome.
- **Excluding `rg` from the agent's tool surface** (via `IsolatedSessionConfigSource`). This stops the immediate symptom but takes away a useful capability and doesn't fix the root cause: any future shell tool the SDK adds will hit the same wall, and the spill content remains unreachable.
- **Adding more verbose error messages from the handler.** The handler isn't being called — the shell tool is. The SDK's spill-and-pointer flow happens at the orchestrator layer, above the FS-handler RPC.

## Solution

For deployments where cross-host `session.resume` is not required (single-instance hosts, single-replica Container Apps, dev workstations), keep the SDK's session-state and tool-output spills on **local disk** by leaving `CopilotClientOptions.SessionFs` at its defaults — empty `InitialCwd`, empty `SessionStatePath`, no virtual `ISessionFsHandler` registered.

```csharp
// gpt/src/BC3Technologies.DiscordGpt.Copilot/DiscordGptBuilderCopilotExtensions.cs
var clientOptions = new CopilotClientOptions
{
    AutoStart = options.AutoStart,
    UseStdio = options.UseStdio,
    // SessionFs intentionally left at SDK defaults: spills land on real disk
    // under ConfigDir, where shelled-out CLI tools (rg, cat) can read them.
};
```

```csharp
// services/ChatBot/DependencyInjectionExtensions.cs
.UseCopilot(c => c
    // No .WithBlobSessionStorage — see solution doc for rationale.
    .WithTableConversationSessionMap(...)
    ...)
```

`BlobSessionFsHandler` and the `WithBlobSessionStorage` extension stay in the `BC3Technologies.DiscordGpt.Storage.Blob` package for SDK consumers who do need cross-host resume — see "When to use blob session storage" below.

## Why This Works

The SDK uses the registered `ISessionFsHandler` for two things:

1. **Internal session-state files** — `session.db`, `workspace.yaml`, `checkpoints/`. The agent never touches these directly; only the SDK reads/writes them, and only over the FS-handler RPC channel. Routing these through a virtual handler is fine because nothing else needs to see them.
2. **Tool-output spill files** — `/session-state/temp/<file>` for any oversized tool result. The agent **does** touch these, by shelling out. Routing these through a virtual handler breaks the agent's ability to consume its own tool results, because shell tools don't speak the FS-handler RPC protocol.

The two surfaces share one handler, so it's all-or-nothing. If you want surface #1 on blob (for resume), you also get surface #2 on blob (which breaks shell consumption of large tool results). For single-instance deployments, surface #1 has no value — there's only ever one host process — so the cleanest fix is to leave both on local disk.

## When to Use Blob Session Storage

Keep `WithBlobSessionStorage` if **all** of these are true:

- The host runs as 2+ replicas, OR you need session continuity across container restarts.
- The agent's flows don't rely on shelling out (`rg`, `cat`, custom binaries) to consume large tool results — i.e. the tools the agent uses produce small inline results, or the agent only ever uses SDK-native tools (`view`, `grep`) which can speak FS-handler RPC.
- Conversation state is more valuable than tool-output ergonomics.

Otherwise, prefer local-disk session FS plus a separate conversation-history store (Azure Table, Cosmos, SQL — anything outside the SDK's own session.db). The bot's user-visible conversation lives in `TableConversationStore` independently, so a fresh GHCP session on restart still replies coherently.

## Prevention

- **Treat any "agent shells out to read a path" as a real-disk requirement.** Audit the agent's tool surface before introducing a virtual `ISessionFsHandler`. If `bash` / `powershell` / `rg` / `apply_patch` (or any tool that takes a path) is enabled, spills must resolve to real disk.
- **Log every spill in the harness.** If you have an `ISessionFsHandler`, log `WriteFile` calls at Info level when the path matches `*/temp/*`. A spill followed by zero `ReadFile` calls is a strong signal the agent gave up on the result.
- **Test the round trip end-to-end with a tool that produces a large response.** Single integration test: invoke a tool whose result exceeds the spill threshold, then have the agent search for a known token in the result. If the search returns no hits, the spill is unreachable.

## Related

- [`docs/solutions/integration-issues/copilot-sdk-sessionstatepath-must-be-non-empty-2026-04-24.md`](./copilot-sdk-sessionstatepath-must-be-non-empty-2026-04-24.md) — the prior fix that routed session-state through `BlobSessionFsHandler`. Correct for multi-instance topologies; superseded by this doc for single-instance Container Apps where shell-tool spill consumption matters more than cross-host resume.
- [`docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md`](../best-practices/conversation-scoped-copilot-sessions-2026-04-23.md) — broader per-conversation session architecture. The conversation history piece (Azure Table) still applies; the SDK-session-id-mapping piece still applies; the blob-backed `ISessionFsHandler` piece does not apply for single-instance deployments.
