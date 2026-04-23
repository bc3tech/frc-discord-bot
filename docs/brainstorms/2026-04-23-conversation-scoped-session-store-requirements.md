---
date: 2026-04-23
topic: conversation-scoped-session-store
---

# Conversation-Scoped Copilot Session Store

## Problem Frame

The Discord bot uses the GitHub Copilot SDK as its agent runtime. Today the bot:

1. **Leaks the host's developer Copilot history into Discord answers.** The `sql` built-in tool reads `session-store.db` (the CLI's global session index) at the root of the Copilot config directory. With no `ConfigDir` override, that resolves to the developer's `~/.copilot/session-store.db` and exposes every session the developer has ever run on that machine. A Discord question can — and has — been answered with the contents of an unrelated developer session.
2. **Has no real per-conversation memory.** `CreateSessionAsync` is called on every Discord turn, so the SDK never sees prior turns of the same conversation. Continuity is faked by stuffing the transcript into the prompt string.
3. **Configures `CopilotClient` minimally.** Only `AutoStart` and `UseStdio` are set. `UseLoggedInUser`, `GitHubToken`, `Logger`, `LogLevel`, `Telemetry`, and `SessionFs` at the client level are all unset. The blob-backed `ISessionFsHandler` is wired only per-session via `SessionConfig.CreateSessionFsHandler`, so any default-session code path bypasses blob storage.

A prior stopgap added a hard-coded list of 11 built-in tools to `IsolatedSessionConfigSource.ExcludedTools` (`sql`, `session_store_sql`, `store_memory`, `task`, `read_agent`, `list_agents`, `web_fetch`, `web_search`, `ask_user`, `fetch_copilot_cli_documentation`, `skill`) to suppress the symptoms without fixing the underlying scope problem. This work item supersedes that stopgap.

The bot already has a stable conversation identity (`ConversationContext.Key.ToStorageKey()`) and a blob-backed `ISessionFsHandler` (`BlobSessionFsHandler`). What's missing is wiring those two together at the right SDK extension points.

## Architecture

```
Discord turn for convKey "guild:123/channel:456"
        │
        ▼
┌───────────────────────────────────────────────────────────┐
│ GitHubCopilotPromptHarness.RunPromptAsync                  │
│  1. lookup IConversationSessionMap[convKey] → sessionId?   │
│  2. configure SessionConfig:                               │
│       ConfigDir = botConfigRoot/cfg/<convKey>              │
│       (SessionFs already on client default = blob)         │
│  3. if sessionId: ResumeSessionAsync(sessionId, …)         │
│     else: CreateSessionAsync(…) → store sessionId in map   │
└───────────────────────────────────────────────────────────┘
        │                                          │
        ▼                                          ▼
 ConfigDir: botConfigRoot/cfg/<convKey>     Per-session events
   ├─ session-store.db (index)              (SessionFs RPC →
   │   only THIS conv's sessions             BlobSessionFsHandler →
   └─ other CLI state                        Azure Blob, survives
                                              cold starts)
```

Key insight (corrected from initial investigation): per-session conversation data lives in **`SessionFs`** (already blob-backed), not in `ConfigDir`. `session-store.db` at the `ConfigDir` root is only the CLI's session **index** (id → cwd, summary, lastActiveAt). Isolating `ConfigDir` per Discord conversation gives us an empty per-conversation index — closing the leak — without needing to hydrate session data ourselves.

## Requirements

**CopilotClient hardening (table stakes — done regardless of scoping)**
- R1. `CopilotClient` singleton factory must set `UseLoggedInUser = false` unconditionally. The bot never wants the host's logged-in identity.
- R2. `CopilotClient` singleton factory must bind `GitHubToken` from `IConfiguration` such that the value can come from any source (Key Vault, container app secret, `appsettings`, environment variable). When the binding produces a null/empty value, the SDK property is not set — the SDK falls back to its own default. Implementation must not assume any particular secret backend.
- R3. `CopilotClient` singleton factory must allow `TelemetryConfig` to be configured from `IConfiguration` such that consumers' `appsettings.json` keys correspond to SDK property names. When no telemetry section is configured the SDK property is not set and no telemetry is emitted.
- R4. `CopilotClient` singleton factory must wire a `Logger` from the host `ILoggerFactory` (category `GitHub.Copilot.SDK`) and bind `LogLevel` from `IConfiguration` (null = SDK default).
- R5. `CopilotClient` singleton factory must wire `SessionFs` at the client level using the registered `ISessionFsHandler`, so default-session code paths use blob storage instead of local disk. (Implementation note: the per-session override in `GitHubCopilotPromptHarness` then becomes redundant; whether to remove it is a planning-time decision and not part of this requirement.)

**Per-conversation isolation**
- R7. `IsolatedSessionConfigSource` must set `SessionConfig.ConfigDir = Path.Combine(botConfigRoot, "cfg", ConversationContext.Key.ToStorageKey())` per Discord conversation. The path must be a deterministic function of the conversation key.
- R8. The `ConversationContext.Key.ToStorageKey()`-derived path segment must be safe for the host filesystem (no path separators, no reserved characters). If `ToStorageKey()` does not already guarantee this, the source must apply a deterministic, collision-resistant mapping.
- R9. The bot configuration root must be a real local path that survives the lifetime of a Functions instance and is bot-owned (not the host user's home). `Path.Combine(Path.GetTempPath(), "frc-bot-copilot")` or similar is acceptable.

**Per-conversation continuity via ResumeSessionAsync**
- R10. The bot must persist a `(Discord conversation key → Copilot session id)` mapping in durable storage. The store must be readable and writable from any Functions instance.
- R11. On each Discord turn, `GitHubCopilotPromptHarness.RunPromptAsync` must look up the mapping; if a session id is found it calls `ResumeSessionAsync(sessionId, ResumeSessionConfig)`, otherwise it calls `CreateSessionAsync(SessionConfig)` and stores the returned session id under the conversation key.
- R12. Concurrent turns on the same Discord conversation must not corrupt the mapping or trigger overlapping `Resume` calls on the same session id. Per-conversation serialization (in-process semaphore keyed on convKey, or a lease pattern in the durable store) is acceptable.
- R13. Mapping writes must be best-effort and must not fail the user-visible turn. If write fails, the next turn falls back to creating a new session (degrading to today's behavior) and logs a warning.

**Built-in tool exclusions revisited**
- R14. Once `ConfigDir` is conv-scoped, the `sql session_store` tool reads only this conversation's session index, which is the desired behavior. The exclusion of `sql` and `session_store_sql` from the prior stopgap must be removed.
- R15. Tools that have no meaning in a headless Discord context must remain excluded: at minimum `ask_user`. `web_fetch` and `web_search` are also excluded by default as a policy choice (no untrusted outbound calls from a Discord-triggered turn); the policy reason is documented in code, and re-enabling is a future config decision.
- R16. Other built-ins disabled in the prior stopgap (`store_memory`, `task`, `read_agent`, `list_agents`, `fetch_copilot_cli_documentation`, `skill`) are re-enabled. They are no longer leak vectors once R7 is in place.

## Success Criteria

- A Discord question of the form "what was the last thing the user worked on?" never returns content from a developer's local Copilot session history. Verified by re-running the leak repro from the captured failure log.
- A multi-turn Discord conversation in which turn N references "what we just talked about" produces an answer grounded in the actual prior turns of that conversation, not in conversation transcript stuffed into the prompt. Verified across at least one Functions cold start (process restart between turns).
- A Discord conversation in channel A cannot retrieve session-index rows from a Discord conversation in channel B via the `sql` built-in. Verified by adversarial prompt.
- The `CopilotClient` singleton works in three deployment shapes without code changes: (a) no `GitHubToken` configured at all, (b) `GitHubToken` bound from `appsettings.json`, (c) `GitHubToken` injected via Key Vault → `IConfiguration`. Verified by toggling configuration only.
- With no telemetry section present in configuration, no OTel spans or metrics are emitted from the SDK CLI subprocess. Verified by inspecting OTel collector output during a turn.
- Two near-simultaneous Discord turns on the same conversation produce two ordered turns against the same Copilot session id without corrupting the `(convKey → sessionId)` mapping. Verified by integration test or manual race repro.

## Scope Boundaries

- **Not in scope:** changing the underlying blob storage container layout for `BlobSessionFsHandler`. Existing layout is reused.
- **Not in scope:** garbage collection of stale per-conversation `ConfigDir` subdirectories on Functions instances. Acceptable to leak directory entries until cold start; revisit only if it becomes operationally painful.
- **Not in scope:** garbage collection of stale `(convKey → sessionId)` mapping rows. The store grows monotonically; a future ops task can prune by `lastActiveAt`.
- **Not in scope:** rewriting the prompt-stuffed transcript path (`IncludeConversationHistoryAsContext` / `BuildPrompt`). Once `ResumeSessionAsync` works, the prompt-stuffed transcript can be reduced or removed, but that decision is deferred to a follow-up.
- **Not in scope:** exposing `UseLoggedInUser` as a bot-level option. Bot-harness opinion is always `false`.

## Key Decisions

- **Per-session data lives in `SessionFs` (blob), not `ConfigDir`.** Verified from SDK XML docs (`SessionFsSetProviderRequest.SessionStatePath`: "Path within each session's SessionFs where the runtime stores files for that session"). This means cross-cold-start resume works for free as long as `SessionFs` is blob-backed and `sessionId` is persisted; we do not need to hydrate `ConfigDir` ourselves.
- **`session-store.db` is the index, not the data.** It lives at `ConfigDir` root and is what the `sql session_store` tool reads. Per-conversation `ConfigDir` therefore gives us per-conversation `sql session_store` reads — desired behavior, not a leak.
- **Bind `TelemetryConfig` directly from `IConfiguration`.** No intermediate DTO; consumer `appsettings.json` keys map straight to SDK property names.
- **`GitHubToken` is `IConfiguration`-driven, not coupled to any secret backend.** Library imposes no opinion on where the secret lives.
- **`UseLoggedInUser = false` is hardcoded.** Bot has no scenario where the host's logged-in identity is desirable.
- **`SessionFs` wired at client level.** Removes the asymmetry where only harness-created sessions get blob storage.
- **No process-level env-var defense-in-depth.** Considered setting `COPILOT_CONFIG_DIR` at the OS process level as a fallback, but rejected: with R5 (client-level `SessionFs`) and R7 (per-session `ConfigDir`) both in place, no code path falls back to the host user's `~/.copilot`. Adding global state to defend a non-existent path costs more than it pays back.
- **All exclusions except `ask_user` (and optionally `web_fetch`/`web_search` as policy) are reverted.** Per-conversation `ConfigDir` neutralizes them as leak vectors.

## Dependencies / Assumptions

- `BlobSessionFsHandler` already correctly persists per-session `SessionFs` writes to blob and reads them back across instances. (Existing wiring; no changes assumed needed.)
- `ConversationContext.Key.ToStorageKey()` produces a stable, deterministic value across turns of the same Discord conversation. (Verified in code.)
- Azure Functions instances have a writable temp directory (`Path.GetTempPath()`). True for both Consumption and Premium plans.
- The Copilot SDK's `ResumeSessionAsync` reads session events through the configured `SessionFs` provider rather than only from the local `session-store.db` index. **This is the one technical assumption that needs validation during implementation** — see Outstanding Questions.

## Outstanding Questions

### Resolve Before Planning

(none — all product decisions resolved)

### Deferred to Planning

- [Affects R10][Technical] Where to persist the `(convKey → sessionId)` mapping. Strong default: Azure Table Storage in the same account as `BlobSessionFsHandler`, mirroring the existing `TableConversationTraceContextStore` pattern. Confirm during planning.
- [Affects R11/R13][Needs research] Does `ResumeSessionAsync(sessionId, …)` succeed when the local `session-store.db` (per-conversation, freshly created on this instance) has no row for `sessionId`, but the blob-backed `SessionFs` does have the session's events? If it fails, planning must add either (a) a `session-store.db` hydration step (read the index entry from blob and write it locally before resume) or (b) a `Fork` semantics fallback. A small POC at the start of implementation answers this in <30 lines.
- [Affects R12][Technical] Concurrent-turn handling: in-process semaphore keyed on convKey, or a lease in the mapping store. In-process is simpler and likely sufficient because Functions instances are usually pinned per conversation by Discord's gateway sharding; lease is more robust. Pick during planning.
- [Affects R8][Technical] Whether `ConversationContext.Key.ToStorageKey()` is already filesystem-safe, or whether the config source needs to apply additional encoding (hash + truncate, base32, etc.).

## Next Steps

→ `/ce-plan` for structured implementation planning
