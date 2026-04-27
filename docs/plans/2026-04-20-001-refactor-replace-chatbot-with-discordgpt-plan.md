---
title: "refactor: Replace ChatBot with DiscordGpt libraries (bridge-only, surgical scope)"
type: refactor
status: completed
date: 2026-04-20
deepened: 2026-04-20
---

# refactor: Replace ChatBot with DiscordGpt libraries (bridge-only, surgical scope)

## Overview

Replace the entire `services/ChatBot/` chat pipeline with the in-repo `BC3Technologies.DiscordGpt.*` libraries (under `gpt/`), wired up at the `// DiscordGPT HERE` marker in `app/Program.cs:81`. The replacement is deliberately **surgical**: the only existing files modified are `app/Program.cs`, `services/ChatBot/MessageHandler.cs` (body replaced; class survives as the bridge), and `app/DiscordInterop/CommandModules/ChatCommandModule.cs` (slash-command bodies rewritten). All other `services/ChatBot/*` files are deleted; new agent/prompt/tool files are created alongside the surviving `MessageHandler.cs`.

The library's `Discord.Gateway` package is **not** used. `MessageHandler` becomes a thin gate-and-bridge that gates the inbound `IUserMessage` (DM/mention/policy) and forwards to `IDiscordEventHandler.HandleAsync` via a synthesized `MessageCreatedEvent`. This avoids dual gateway connections and keeps `DiscordInitializationService.cs`, the app's `DiscordSocketClient`, all 12 non-chat slash commands, the webhook dispatcher, and the embed pipeline untouched.

## Problem Frame

The existing `services/ChatBot/` stack is custom-built around Azure AI Foundry persistent threads, a bespoke `Conversation` abstraction, manual progress-message orchestration, and `ChatRunner` workflow handoff. The repo now has a complete first-party library set under `gpt/` (`Core`, `Discord`, `Hosting`, `Storage.TableStorage`, `Copilot.Foundry`, `Discord.Gateway`, `Discord.Webhooks`) that supplies the same capabilities with cleaner abstractions: `IInteractionPolicy`, `IConversationKeyResolver`, `IConversationStore`, `IDiscordAgent`, `IThreadTitleGenerator`, and `IDiscordEventHandler`. The user wants the library stack to fully own the chat path with **only the user's specific FRC agent and prompt definitions** carried forward.

## Requirements Trace

- **R1.** All chat functionality (DM, @-mention thread creation, guild reply) is owned by the `BC3Technologies.DiscordGpt.*` libraries.
- **R2.** Only the user's FRC-specific agent definition, prompts, and tools are layered onto the library stack.
- **R3.** Wireup is anchored at the `// DiscordGPT HERE` marker in `app/Program.cs:81`.
- **R4.** Existing files modified are limited to: `app/Program.cs`, `services/ChatBot/MessageHandler.cs`, and `app/DiscordInterop/CommandModules/ChatCommandModule.cs`. (New files may be created; obsolete files in `services/ChatBot/` are deleted.)
- **R5.** Conversation state persists in Azure Table Storage using the **`userChatAgentThreads`** table name.
- **R6.** Existing `BlobServiceClient` registration in `Program.cs` is preserved unchanged for the FRC notification artifact pipeline (and any future AF state-store use).
- **R7.** App's existing `DiscordSocketClient`, `InteractionService`, `DiscordMessageDispatcher`, and all non-chat slash commands continue to function unchanged.
- **R8.** `/chat reset` and `/chat new` slash commands continue to work, calling `IConversationStore.ClearAsync` instead of the deleted `ChatThreadResetter`.
- **R9.** Build remains zero-warning (`TreatWarningsAsErrors`); full test suite passes.
- **R10.** All tool/skill invocations are auto-approved — `CopilotToolAuthorizationOptions` is configured allow-all (`AllowAllTools = true`, `AllowAllSkills = true`, `AllowToolsInDirectMessages = true`, `AllowSkillsInDirectMessages = true`, no `AllowedUserIds`/`AllowedChannelIds`/`AllowedGuildIds` filters). The agent may still interact conversationally (ask clarifying questions), but no tool call ever pauses for human approval.

## Scope Boundaries

**In scope (modify):**

- `app/Program.cs` — DI wireup at the marker.
- `services/ChatBot/MessageHandler.cs` — body replaced with gate-and-bridge.
- `app/DiscordInterop/CommandModules/ChatCommandModule.cs` — `/chat reset` and `/chat new` rewritten to call `IConversationStore.ClearAsync`.

**In scope (create):**

- New agent/prompt/tool files alongside the surviving `MessageHandler.cs` (path TBD per Unit 3 — likely under `services/ChatBot/Agents/`, `services/ChatBot/Tools/`, `services/ChatBot/Prompts/`).
- New `services/ChatBot/DependencyInjectionExtensions.cs` (slimmed) exposing a single `AddFrcChatBot(this IServiceCollection, IConfiguration)` extension that composes the library pipeline + the FRC agent.

**In scope (delete):**

- `services/ChatBot/Agents/` (legacy — replaced by new agent file)
- `services/ChatBot/Copilot/` (Foundry persistent-thread and progress-message machinery)
- `services/ChatBot/Configuration/` (legacy ChatBot config; new options bind directly via library `*Options` types in Program.cs)
- `services/ChatBot/ChatRunner.cs`
- `services/ChatBot/ChatThreadResetter.cs`
- `services/ChatBot/Conversation.cs`
- `services/ChatBot/ConversationThreadState.cs`
- `services/ChatBot/Tools/` (legacy — replaced by new tools)
- `services/ChatBot/UserChatSynchronization.cs`
- `services/ChatBot/AzureIdentityActivityFilteringProcessor.cs`
- `services/ChatBot/agent_prompt.txt`, `services/ChatBot/meal_agent_prompt.txt`, `services/ChatBot/progress_messages.txt`
- `services/ChatBot/Diagnostics/`, `services/ChatBot/Log.cs` (replaced by library logging)

**In scope (upstream `gpt/` extension — required to make library functional):**

- `gpt/src/BC3Technologies.DiscordGpt.Discord/DiscordRestClient.cs` — add `CreateMessageAsync`, `CreateReplyAsync`, `CreateThreadAsync`, `CreateThreadMessageAsync`.
- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:111` — fill in the `SendResponseAsync` TODO stub.
- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptServiceCollectionExtensions.cs` (or sibling) — register `HttpClient` for `DiscordRestClient` with `Authorization: Bot <token>` header.

**Explicit non-goals:**

- **Do NOT use `Discord.Gateway`.** No `UseGateway()` call. The app's existing `DiscordSocketClient` continues to receive `MessageReceived` events.
- **Do NOT modify** `app/DiscordInterop/DiscordInitializationService.cs`, `app/DiscordInterop/DependencyInjectionExtensions.cs`, any other slash command module, the webhook dispatcher, or `app/FunctionApp.csproj` (beyond adding/removing project references in Units 2 and 7).
- **Do NOT migrate existing `userChatAgentThreads` rows.** Schemas don't match; the table is dropped and recreated. Functionality is unreleased so no production data is at risk.
- **Do NOT bind `BlobConversationStore` as `IConversationStore`.** `BlobServiceClient` registration in `Program.cs:122-136` is preserved as-is for the existing notification artifact pipeline.
- **Do NOT add the `Storage.Blob` library project reference.** Only `Storage.TableStorage` is needed.
- **Do NOT preserve** any `ChatBot.*` namespace symbols beyond `ChatBot.MessageHandler` (so `DiscordInitializationService`'s `using ChatBot;` and `MessageHandler? chatBot` parameter continue to resolve unchanged).
- **Do NOT migrate** legacy ChatBot configuration keys; new wireup binds library `*Options` from configuration directly.

## Context & Research

### Relevant Code and Patterns

- `app/Program.cs:78-82` — `// DiscordGPT HERE` marker inside `if (hasValidChatBotConfiguration)` block. Wireup target.
- `app/Program.cs:96-136` — Existing `TableServiceClient` factory (lines 96-120) and `BlobServiceClient` registration (lines 122-136). **Reuse**, do not duplicate. Registers a keyed `TableClient` per name in the `Constants.Configuration.Azure.Storage.Tables` config section.
- `app/DiscordInterop/DiscordInitializationService.cs:27,145-175` — Subscribes `client.MessageReceived` and routes to `chatBot.HandleUserMessageAsync` (DM) or `chatBot.TryHandleGuildMessageAsync` (guild). `MessageHandler? chatBot` is an optional ctor param. **Not modified.**
- `app/DiscordInterop/CommandModules/ChatCommandModule.cs:20-51` — `/chat reset` and `/chat new` slash commands. Currently inject `ChatThreadResetter`. To be rewritten in Unit 6.
- `services/ChatBot/MessageHandler.cs:30,50` — `MessageHandler` class. Public surface: `HandleUserMessageAsync(IUserMessage, CancellationToken)` and `TryHandleGuildMessageAsync(IUserMessage, ulong currentUserId, IReadOnlyCollection<ulong> roleIds, CancellationToken)`. **Bodies replaced; signatures preserved.**
- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:34` — `IDiscordEventHandler.HandleAsync(DiscordEvent, CancellationToken)`. Library entry point for synthesized events.
- `gpt/src/BC3Technologies.DiscordGpt.Core/MessageCreatedEvent.cs` — Concrete `DiscordEvent` subtype the bridge constructs from `IUserMessage`.
- `gpt/src/BC3Technologies.DiscordGpt.TestHelpers/MessageCreatedEventBuilder.cs` — Confirms synthesizing `MessageCreatedEvent` from primitive fields is a supported pattern (used in library's own tests).
- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:111` — `SendResponseAsync` is a TODO stub today; only logs. **Required upstream fix in Unit 1.**
- `gpt/src/BC3Technologies.DiscordGpt.Discord/DiscordRestClient.cs` — Currently exposes only `GetCurrentUserAsync` and `GetGuildMemberAsync`. **Required upstream addition in Unit 1.**
- `gpt/src/BC3Technologies.DiscordGpt.Storage.TableStorage/TableConversationStore.cs` — Per-turn `ConversationTurnEntity` (`MessageJson` column) is the modern schema; `LegacyConversationEntity` (`HistoryJson`) is the fallback path. Reuse with `TableName = "userChatAgentThreads"`.
- `gpt/src/BC3Technologies.DiscordGpt.Discord/DefaultInteractionPolicy.cs` — Default policy for "should respond" decisions. Reused via DI; FRC-specific overrides go through configuration, not subclassing.

### Institutional Learnings

- The `gpt/` libraries were built specifically to be the replacement chat stack for this bot. Their abstractions (`IInteractionPolicy`, `IConversationKeyResolver`, `IConversationStore`, `IDiscordAgent`, `IThreadTitleGenerator`) match the conceptual seams the existing `services/ChatBot/` carved out manually.
- Existing `userChatAgentThreads` rows (PartitionKey/RowKey + `AgentThreadId`/`CanonicalRowKey`/`TraceRootContext`/`MentionActivated` columns) have **no schema overlap** with the library's `ConversationTurnEntity` or `LegacyConversationEntity`. Foundry persistent threads stored history server-side; the library stores it client-side. Migration is impossible without backfilling history from Foundry, which is explicitly out of scope.

### External References

- **Microsoft Agent Framework** (`Microsoft.Agents.AI*`) — Required orchestration stack per repo convention. Library's `Copilot.Foundry` package wraps Foundry chat-completions for `IChatClient`. The FRC agent is a thin wrapper over `Microsoft.Agents.AI.AIAgent` registered as `IDiscordAgent`.
- Discord REST v10 — `POST /channels/{channel.id}/messages`, `POST /channels/{channel.id}/threads`, thread starter-message + follow-up-message semantics. Implemented in Unit 1; rate-limit handling minimal (per-route 429 + Retry-After header), bot-token auth only.

## Key Technical Decisions

- **Bridge instead of Gateway.** Reusing the app's existing `DiscordSocketClient` via `MessageHandler` as the bridge is the only way to honor "only Program.cs and the Discord MessageHandler are touched." Adding `Discord.Gateway` would require a second socket connection (the library's `DiscordGatewayService` constructs its own `DiscordSocketClient` privately at `gpt/src/BC3Technologies.DiscordGpt.Discord.Gateway/DiscordGatewayService.cs:50`) and removing the app's client would force edits across `DiscordInitializationService.cs`, `DependencyInjectionExtensions.cs`, every slash command module, and the webhook dispatcher. The bridge keeps surface area minimal and avoids dual gateway connections entirely.
- **`MessageHandler.cs` survives in place** with body replaced. This keeps `DiscordInitializationService.cs:27` (`MessageHandler? chatBot = null` ctor param) and `DiscordInitializationService.cs:147,160,173` (subscription bodies) unchanged. The class moves from "Foundry orchestrator" to "thin gate + bridge to `IDiscordEventHandler`."
- **Drop and recreate `userChatAgentThreads` table** on first deploy. Functionality is unreleased; no production data is at risk. Schemas are incompatible (no overlap between existing and `ConversationTurnEntity`/`LegacyConversationEntity`); cleanest path is operational truncate + library auto-creates fresh schema on first write. Documented as a one-time deploy step in Unit 7.
- **Upstream `gpt/` work is in scope.** The library cannot deliver replies without `SendResponseAsync` being implemented and `DiscordRestClient` having send/thread methods. This work happens first (Unit 1) and lives in the `gpt/src/*` projects, not in `app/`. Library-only constraint forces this — there's no app-side workaround.
- **`HttpClient` registration for `DiscordRestClient`** is folded into the library's `AddDiscordGpt`/`UseFoundry`/`UseTableConversationStore` composition (Unit 1 chooses the exact extension method). It uses the named-client pattern with `Authorization: Bot <token>` baked into a `DelegatingHandler` so consumers don't have to register HTTP plumbing manually.
- **Bridge synthesis routes both DM and guild paths through one method.** `MessageHandler.HandleUserMessageAsync` and `TryHandleGuildMessageAsync` both build a `MessageCreatedEvent` and call `IDiscordEventHandler.HandleAsync`. Gating logic (bot? command prefix? mention required in guilds? blocked user?) lives in `MessageHandler` because it has access to the raw `IUserMessage` and the `currentUserId`/`roleIds` context. The library's `IInteractionPolicy` runs again on the synthesized event for any policy the bridge can't decide locally.
- **Use `BC3Technologies.DiscordGpt.Copilot.AddCopilotAgent` instead of writing a bespoke `IDiscordAgent`.** The library's `CopilotDiscordAgent` already wraps `IChatClient` with `FunctionInvokingChatClient` (auto tool execution), enforces `ICopilotToolAuthorizationPolicy`, and discovers `IDiscordTool`/`IDiscordSkill` from DI. The "FRC agent" decomposes into: (a) the Foundry `IChatClient` backend via `UseFoundry`, (b) a system prompt (Unit 3 chooses the injection point — either via a custom `IChatClient` decorator or whatever extension point the library exposes), (c) `IDiscordTool` implementations registered with `AddTool<T>()`, and (d) the auth-policy configuration. No custom `IDiscordAgent` class is needed.
- **Auth policy is wide-open by configuration, not by replacement.** The default `CopilotToolAuthorizationPolicy` is "secure-by-default" (everything denied unless explicitly allowed). Per R10, the wireup flips it to allow-all via `AddCopilotAgent(o => { o.AllowAllTools = true; o.AllowAllSkills = true; o.AllowToolsInDirectMessages = true; o.AllowSkillsInDirectMessages = true; })`. This avoids subclassing or replacing `ICopilotToolAuthorizationPolicy` and keeps the configuration in one place where it's easy to audit.
- **`IConversationKeyResolver` is the single source of truth for conversation identity** across both the bridge and the slash-command handlers. `ChatCommandModule` injects the same resolver instance to derive a `ConversationKey` from the slash command's `IInteractionContext`, then calls `IConversationStore.ClearAsync(key)`.
- **Slash-command surface widens to "touched" by 1 file.** `ChatCommandModule.cs` is added to the modify list because deleting `ChatThreadResetter` would break it. Per user's confirmation, it's rewritten to use `IConversationStore.ClearAsync` directly (option B).
- **`ChatBot.csproj` survives** — slimmed to `MessageHandler.cs`, the new agent/prompt/tool files, and a slimmed `DependencyInjectionExtensions.cs`. This keeps `app/FunctionApp.csproj`'s existing `<ProjectReference Include="..\services\ChatBot\ChatBot.csproj" />` valid and avoids touching `FunctionApp.csproj` beyond the project-ref additions in Unit 2.

## Open Questions

### Resolved During Planning

- **Q: Use library `Discord.Gateway` or bridge through existing socket?** Resolved: bridge. User's "only Program.cs and MessageHandler touched" constraint rules out Gateway.
- **Q: How to handle `userChatAgentThreads` schema mismatch?** Resolved: drop and recreate the table on cutover. Unreleased functionality; no data preservation needed.
- **Q: How to handle `/chat reset` and `/chat new` when `ChatThreadResetter` is deleted?** Resolved (per user confirmation): rewrite to call `IConversationStore.ClearAsync`; `ChatCommandModule.cs` is added to the modify surface.
- **Q: Does `MessageHandler` need to keep its current public method signatures?** Resolved: yes, to keep `DiscordInitializationService.cs` untouched. `HandleUserMessageAsync(IUserMessage, CancellationToken)` and `TryHandleGuildMessageAsync(IUserMessage, ulong, IReadOnlyCollection<ulong>, CancellationToken)` are preserved.
- **Q: Where do new agent/prompt/tool files live?** Resolved: alongside the surviving `MessageHandler.cs` in the slimmed `services/ChatBot/` project (under `Agents/`, `Tools/`, `Prompts/`). Keeps `ChatBot.csproj` and its existing project ref intact; avoids any `FunctionApp.csproj`/`*.slnx` reshuffling beyond library refs.
- **Q: What does "GHCP sdk's file/state store" map to in `gpt/`?** Could not locate. Treating as preserve-existing-`BlobServiceClient` (which is needed by the FRC notification artifact pipeline regardless). No `BlobConversationStore` binding.

### Deferred to Implementation

- **Exact gate logic for guild messages.** Bridge needs to decide whether mention is required, whether prefix triggers respond, whether DMs always respond, etc. Likely lifted from existing `MessageHandler.TryHandleGuildMessageAsync` semantics; verify in Unit 5.
- **`IConversationKeyResolver` rules for slash-command events.** Slash commands don't produce `MessageCreatedEvent`s. Unit 6 implementer chooses between (a) constructing a `ConversationKey` directly from `IInteractionContext` fields or (b) synthesizing a `DiscordEvent` and routing through the resolver. Whichever path is chosen, the resolver instance is the DI-registered singleton.
- **Specific named `HttpClient` configuration** for `DiscordRestClient` (timeout, retry policy) — Unit 1 picks reasonable defaults; tuning deferred.
- **Whether the `DefaultInteractionPolicy` is sufficient** or the FRC bot needs a custom `IInteractionPolicy` implementation (e.g., to honor existing per-channel allowlists). Verify in Unit 4 by inspecting current `MessageHandler` policy logic before deletion.
- **Tool inventory.** Existing `services/ChatBot/Tools/` contents need to be enumerated and ported individually in Unit 3.

## High-Level Technical Design

> *This illustrates the intended approach and is directional guidance for review, not implementation specification. The implementing agent should treat it as context, not code to reproduce.*

### Inbound message bridge (replaces Foundry pipeline)

```
                                                      app boundary │  gpt/ library boundary
                                                                   │
 Discord                                                           │
   │                                                               │
   │ MessageReceived                                               │
   ▼                                                               │
 DiscordSocketClient ───► DiscordInitializationService             │
                          (UNCHANGED — already subscribes)         │
                                  │                                │
                                  ▼                                │
                          MessageHandler.HandleUserMessageAsync    │
                          MessageHandler.TryHandleGuildMessageAsync│
                          ┌────────────────────────────────────┐   │
                          │ 1. Gate (bot? cmd prefix? mention? │   │
                          │    blocked user?)                  │   │
                          │ 2. Build MessageCreatedEvent       │   │
                          │    from IUserMessage primitives    │   │
                          │ 3. await eventHandler.HandleAsync  │───┼──►IDiscordEventHandler
                          └────────────────────────────────────┘   │       │
                                                                   │       ▼
                                                                   │  IInteractionPolicy
                                                                   │  IConversationKeyResolver
                                                                   │  IConversationStore (Table)
                                                                   │  IDiscordAgent (FRC)
                                                                   │  IThreadTitleGenerator
                                                                   │       │
                                                                   │       ▼
                                                                   │  DiscordRestClient
                                                                   │  (NEW: Create*Async)
                                                                   │       │
                                                                   ▼       ▼
                                                                Discord REST API
```

### Slash-command path (rewrite)

```
 /chat reset / /chat new ─► ChatCommandModule
                            ├─ inject IConversationStore + IConversationKeyResolver
                            ├─ derive ConversationKey from IInteractionContext
                            └─ await store.ClearAsync(key)
```

### Notably absent

- No `Discord.Gateway` registration. No second socket client. No `UseGateway()` call.
- No `BlobConversationStore` binding. `BlobServiceClient` registration at `Program.cs:122-136` is left untouched.
- No edits to `DiscordInitializationService.cs`, `DependencyInjectionExtensions.cs`, or any non-`/chat` slash command.

## Implementation Units

- [x] **Unit 1: Upstream `gpt/` library — `DiscordRestClient` send/thread methods + `SendResponseAsync` impl + `HttpClient` registration**

**Goal:** Make the library physically capable of delivering replies. Without this, every other unit produces a non-functional bot.

**Requirements:** R1, R7

**Dependencies:** None.

**Files:**
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Discord/DiscordRestClient.cs` — add `CreateMessageAsync(ulong channelId, string content, CancellationToken)`, `CreateReplyAsync(ulong channelId, ulong replyToMessageId, string content, CancellationToken)`, `CreateThreadAsync(ulong channelId, ulong fromMessageId, string name, CancellationToken)` (returns thread channel id), `CreateThreadMessageAsync(ulong threadChannelId, string content, CancellationToken)`.
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:111` — replace `SendResponseAsync` TODO body with dispatch by `discordEvent` shape: DM (`CreateMessageAsync`), guild reply with no thread context (`CreateReplyAsync`), guild new-thread (`CreateThreadAsync` then `CreateThreadMessageAsync` for content + thread title from `IThreadTitleGenerator`), in-thread reply (`CreateMessageAsync` to thread channel).
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptServiceCollectionExtensions.cs` — extend `AddDiscordGpt` (or add a sibling registration) to register `DiscordRestClient` + a named `HttpClient` with `Authorization: Bot <token>` injected via `DelegatingHandler`. Token source: `IOptions<DiscordGptCoreOptions>` (or new `DiscordRestOptions`).
- Test: `gpt/tests/BC3Technologies.DiscordGpt.Discord.Tests/DiscordRestClientTests.cs` — add coverage for new methods (mock `HttpMessageHandler`, assert URL/body/headers/auth, success + 4xx + 429).
- Test: `gpt/tests/BC3Technologies.DiscordGpt.Hosting.Tests/DiscordGptEventHandlerTests.cs` — add coverage for `SendResponseAsync` dispatch by event type.

**Approach:**
- One internal `HttpClient` field, all REST calls go through it; URL constants centralized.
- 429 handling: read `Retry-After` header, await, single retry; surface `HttpRequestException` on persistent failure (no swallowing).
- `SendResponseAsync` decides shape from `DiscordEvent` properties (`GuildId == null` → DM, `ThreadId != null` → in-thread, `ThreadName != null` → new-thread, else → reply).
- Authorization handler reads token from `DiscordGptCoreOptions` once at construction; rejects writes if missing.

**Patterns to follow:**
- Existing `DiscordRestClient.GetCurrentUserAsync` / `GetGuildMemberAsync` for HTTP method/JSON handling style.
- Existing library DI extensions for `Add*` shape conventions.

**Test scenarios:**
- *Happy path:* `CreateMessageAsync` posts to `POST /channels/{id}/messages` with correct JSON body, bot-token auth header present.
- *Happy path:* `CreateThreadAsync` posts to `POST /channels/{id}/threads` and returns the new channel id.
- *Happy path:* `SendResponseAsync` for DM event → exactly one `CreateMessageAsync` call to channel id.
- *Happy path:* `SendResponseAsync` for guild new-thread event → `CreateThreadAsync` then `CreateThreadMessageAsync`.
- *Happy path:* `SendResponseAsync` for in-thread guild event → `CreateMessageAsync` to thread channel id.
- *Edge case:* 429 with `Retry-After: 1` → one retry, then success.
- *Edge case:* `DiscordEvent` with no recognizable shape → throws `InvalidOperationException`.
- *Error path:* missing bot token in options → constructor or first send throws.
- *Error path:* persistent 5xx → propagates `HttpRequestException`.

**Verification:**
- New library tests pass.
- `dotnet build gpt/BC3Technologies.DiscordGpt.slnx -c Release /warnaserror` succeeds with zero warnings.
- Manual sanity check: `SendResponseAsync` is no longer a TODO stub.

---

- [x] **Unit 2: Project references — add DiscordGpt library refs to `app/FunctionApp.csproj` and `FRCDiscordBot.slnx`**

**Goal:** Make library types available to `app/Program.cs` and to `services/ChatBot/MessageHandler.cs`.

**Requirements:** R1

**Dependencies:** Unit 1 (so the libraries compile cleanly with the upstream additions).

**Files:**
- Modify: `app/FunctionApp.csproj` — add `<ProjectReference>` entries for `BC3Technologies.DiscordGpt.Hosting`, `BC3Technologies.DiscordGpt.Discord`, `BC3Technologies.DiscordGpt.Storage.TableStorage`, `BC3Technologies.DiscordGpt.Copilot`, `BC3Technologies.DiscordGpt.Copilot.Foundry`, `BC3Technologies.DiscordGpt.Core` (transitively pulled, but explicit for clarity).
- Modify: `services/ChatBot/ChatBot.csproj` — add `<ProjectReference>` entries for `BC3Technologies.DiscordGpt.Hosting`, `BC3Technologies.DiscordGpt.Core`, `BC3Technologies.DiscordGpt.Discord`, `BC3Technologies.DiscordGpt.Copilot`, `BC3Technologies.DiscordGpt.Copilot.Foundry`. (Needed because `MessageHandler.cs` will reference `IDiscordEventHandler`/`MessageCreatedEvent`/`IInteractionPolicy`, and the new `Tools/*.cs` files implement `IDiscordTool`.)
- Modify: `FRCDiscordBot.slnx` — register the new library projects in the solution if not already present.

**Approach:**
- Use existing `Directory.Packages.props` for any new NuGet versions; no per-project version pins.
- Do NOT add `Storage.Blob` or `Discord.Gateway` references.
- Confirm `Directory.Build.props` settings (TreatWarningsAsErrors, EnforceCodeStyleInBuild) propagate to the consuming projects without new warnings.

**Patterns to follow:**
- Existing project-reference style in `app/FunctionApp.csproj`.

**Test scenarios:**
- Test expectation: none — pure scaffolding. Verified by Unit 8 build pass.

**Verification:**
- `dotnet restore FRCDiscordBot.slnx` succeeds.
- `dotnet build FRCDiscordBot.slnx -c Release /warnaserror` still succeeds (functionality unchanged at this point — old MessageHandler still active).

---

- [x] **Unit 3: Port FRC tools + system prompt + agent composition to new files alongside `MessageHandler.cs`**

**Goal:** Stand up the FRC-specific tool implementations, system prompt, and the `AddFrcChatBot` composition extension. The `IDiscordAgent` itself is the library's `CopilotDiscordAgent` (registered by `AddCopilotAgent`); FRC contributes only tools, prompt, the Foundry chat backend, and configuration. These are NEW files; existing legacy files in `services/ChatBot/Agents/`, `Copilot/`, `Tools/`, etc., remain untouched here and are deleted in Unit 7.

**Requirements:** R2, R10

**Dependencies:** Unit 2 (project refs available).

**Files:**
- Create: `services/ChatBot/Prompts/SystemPrompt.txt` — port content from existing `services/ChatBot/agent_prompt.txt` minus Foundry-specific instructions. Ship as embedded resource.
- Create: `services/ChatBot/Tools/*.cs` — port each tool from existing `services/ChatBot/Tools/` to `IDiscordTool` shape. Inventory at start of unit; one file per tool.
- Create: `services/ChatBot/Agents/FrcSystemPromptChatClient.cs` — a `DelegatingChatClient`-style wrapper that prepends a `ChatMessage(ChatRole.System, <SystemPrompt.txt content>)` to every request before calling the inner client. **Required, not optional** — verified no built-in system-prompt hook exists in `CopilotDiscordAgent`, `FoundryChatClientOptions`, `DiscordGptEventHandler`, `IConversationStore`, or `DiscordGptCoreOptions`. The `IChatClient` decorator is the only extension point.
- Modify: `services/ChatBot/DependencyInjectionExtensions.cs` — replace existing content with a single `AddFrcChatBot(this IServiceCollection services, IConfiguration config)` extension that:
  1. Binds `FoundryChatClientOptions` from `config` and registers the Foundry `IChatClient` (exact extension name in `gpt/src/BC3Technologies.DiscordGpt.Copilot.Foundry/`; e.g., `AddFoundryChatClient`).
  2. Wraps the registered `IChatClient` with `FrcSystemPromptChatClient` (using `IChatClientBuilder.Use(...)` or service-decoration). System prompt content read once at startup from the embedded `SystemPrompt.txt`.
  3. Calls `services.AddCopilotAgent(o => { o.AllowAllTools = true; o.AllowAllSkills = true; o.AllowToolsInDirectMessages = true; o.AllowSkillsInDirectMessages = true; });` — this is the **R10 allow-all** configuration. `AddCopilotAgent` internally adds `UseFunctionInvocation` on top of the (now-system-prompt-wrapped) `IChatClient`.
  4. Calls library composition for the rest: `services.AddDiscordGpt(...).UseTableConversationStore(opt => opt.TableName = "userChatAgentThreads")` (exact extension names confirmed against `gpt/src/*` during implementation).
  5. Registers each FRC tool via `services.AddTool<TTool>()` (constructor-injected with existing FRC API clients already in DI).
  6. Registers `MessageHandler` as singleton (so `DiscordInitializationService` resolves it).

**Approach:**
- **No custom `IDiscordAgent` class.** `AddCopilotAgent` (in `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotServiceCollectionExtensions.cs:23`) already registers `CopilotDiscordAgent` which wraps `IChatClient` with `FunctionInvokingChatClient` and enforces the auth policy.
- **Allow-all is the entire R10 implementation.** `FunctionInvokingChatClient` already auto-executes tool calls without prompting; the only "permission" surface in the library is `ICopilotToolAuthorizationPolicy.IsToolAuthorized` / `IsSkillAuthorized`. Setting `AllowAllTools`/`AllowAllSkills`/`AllowToolsInDirectMessages`/`AllowSkillsInDirectMessages = true` and leaving the `Allowed*Ids` filters empty makes the policy a tautology (always authorized) without needing a custom `ICopilotToolAuthorizationPolicy` impl.
- **System prompt injection** — the FRC `SystemPrompt.txt` is injected via the `FrcSystemPromptChatClient` decorator wrapping the Foundry `IChatClient`. **Verified during planning** by direct inspection: `CopilotDiscordAgent.RespondAsync` (`gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotDiscordAgent.cs:35-74`) passes history straight through with no system-message prepending; `FoundryChatClientOptions` has only Endpoint/DeploymentName/ApiVersion; `DiscordGptEventHandler` (`gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:51-61`) builds history from `IConversationStore` (User/Assistant only) plus the new user message; `IConversationStore` has no system-message concept. The `IChatClient` decorator is the only extension point — `Microsoft.Extensions.AI.IChatClientBuilder.Use(...)` (the same pattern `AddCopilotAgent` itself uses at `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotServiceCollectionExtensions.cs:56-58` to add `UseFunctionInvocation`).
- **Tool inventory** — first action of Unit 3 is to enumerate `services/ChatBot/Tools/` contents. Each tool gets a one-to-one port to `IDiscordTool`. Names and `[Description]` text preserved verbatim.
- **Do not introduce a custom `IInteractionPolicy`** in this unit — verify default suffices in Unit 4 first.

**Patterns to follow:**
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotServiceCollectionExtensions.cs:86-103` (`AddTool` extension shape).
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/DelegatingTool.cs` for delegate-based tool registration if any FRC tool is small enough to inline.
- Existing FRC API client registrations in `app/Program.cs:84-91` for service-graph alignment.

**Test scenarios:**
- *Happy path:* `AddFrcChatBot` registers all required services; resolving `IDiscordAgent` from a built `ServiceProvider` returns a `CopilotDiscordAgent` instance.
- *Happy path:* Resolved `ICopilotToolAuthorizationPolicy.IsToolAuthorized(<any context>, <any tool name>)` returns `true` (R10 allow-all verification).
- *Happy path:* DM context (`GuildId == null`) with arbitrary tool name → policy returns `true` (`AllowToolsInDirectMessages` honored).
- *Integration:* End-to-end DI graph resolves `IDiscordEventHandler` → `CopilotDiscordAgent` → `IChatClient` (Foundry) → tools, with no missing services.
- *Integration:* Each ported FRC tool resolves its FRC-API dependency and produces a non-error result against an in-memory fake.
- *Edge case:* `FrcSystemPromptChatClient` decorator (if used) prepends exactly one system message; never duplicates.
- *Edge case:* Empty conversation history → agent still runs, no null-ref.

**Verification:**
- New tests pass.
- DI graph resolves end-to-end in a unit-test composition root.

---

- [x] **Unit 4: Wireup at `// DiscordGPT HERE` in `app/Program.cs`**

**Goal:** Compose the library pipeline + FRC agent at the marker. Configure `TableConversationStore` to use `userChatAgentThreads`. Reuse existing `TableServiceClient` and `BlobServiceClient` registrations.

**Requirements:** R3, R5, R6

**Dependencies:** Unit 3 (`AddFrcChatBot` exists).

**Files:**
- Modify: `app/Program.cs:78-82` — replace the `// DiscordGPT HERE` marker line with a call to `host.Services.AddFrcChatBot(host.Configuration);`. Add a `using ChatBot;` (or whatever namespace `DependencyInjectionExtensions` lives in) at the top of the file.

**Approach:**
- The wireup is a single call. All composition complexity lives behind `AddFrcChatBot` (Unit 3).
- Confirm `TableServiceClient` (registered later in `Program.cs:103`) is available to the library's `TableConversationStore`. If `AddFrcChatBot` runs BEFORE the `TableServiceClient` registration in source order, the registration order is still fine because DI resolution happens at runtime — but verify the library doesn't try to access `TableServiceClient` at registration time.
- Library's `TableConversationStore` should call `CreateIfNotExists` on first use. Verify; if not, ensure the table is created via the existing `storageTables` config-driven loop at `Program.cs:108-120` by adding `userChatAgentThreads` to the config list (no code change in `Program.cs` if the table is already in the config section).

**Patterns to follow:**
- Existing `host.Services.ConfigureDiscord().ConfigureTheBlueAllianceApi()...` chain at `Program.cs:73-76` for fluent extension style.

**Test scenarios:**
- Test expectation: none — single composition call. Verified by Unit 8 smoke test (DM round-trip).

**Verification:**
- `dotnet build` succeeds with zero warnings.
- `app/Program.cs` `// DiscordGPT HERE` marker is replaced; no other lines in `Program.cs` are modified.

---

- [x] **Unit 5: Replace `MessageHandler.cs` body with gate-and-bridge to `IDiscordEventHandler`**

**Goal:** `MessageHandler` becomes a thin adapter: validate the inbound `IUserMessage` is GPT-eligible, synthesize a `MessageCreatedEvent`, hand off to `IDiscordEventHandler.HandleAsync`. Public method signatures preserved so `DiscordInitializationService.cs` is untouched.

**Requirements:** R1, R4

**Dependencies:** Unit 4 (`IDiscordEventHandler` resolvable from DI).

**Files:**
- Modify: `services/ChatBot/MessageHandler.cs` — REPLACE the entire body (preserve only the class name, namespace, and method signatures `HandleUserMessageAsync(IUserMessage, CancellationToken)` and `TryHandleGuildMessageAsync(IUserMessage, ulong currentUserId, IReadOnlyCollection<ulong> roleIds, CancellationToken)`). New body:
  - Constructor takes: `IDiscordEventHandler eventHandler`, `IInteractionPolicy policy`, `ILogger<MessageHandler> logger`. (Drops all Foundry/`Conversation`/`UserChatSynchronization` deps.)
  - Both methods delegate to a private `BridgeAsync(IUserMessage msg, GateContext ctx, CancellationToken ct)` helper.
  - `BridgeAsync` runs gate (skip bots, skip own messages, skip messages starting with command prefix, in-guild requires mention or role match unless DM), then constructs `MessageCreatedEvent` from `msg` primitives (`msg.Id`, `msg.Channel.Id`, `msg.Author.Id`, `msg.Content`, `msg.Reference?.MessageId`, guild id if present, etc.), then calls `eventHandler.HandleAsync(event, ct)`.
- Test: `services/ChatBot.Tests/MessageHandlerTests.cs` (create if not present) — gate-and-bridge unit tests with a fake `IDiscordEventHandler`.

**Approach:**
- Keep gate logic literal and small. Prefer relying on library's `IInteractionPolicy` for the substantive policy decisions (which run after the bridge dispatches the event); only filter out clearly-not-chat traffic (bots, own messages) at the bridge level.
- `MessageCreatedEvent` field-population is the most error-prone step. Consult `gpt/src/BC3Technologies.DiscordGpt.TestHelpers/MessageCreatedEventBuilder.cs` for the canonical field set.
- `TryHandleGuildMessageAsync` must NOT return `true` if the bridge skipped the message (preserve existing `bool` semantics so `DiscordInitializationService`'s telemetry stays accurate).

**Execution note:** Test-first. Write the gate scenarios as failing tests before replacing the body — this is high-risk shimming code with many edge cases.

**Patterns to follow:**
- `gpt/src/BC3Technologies.DiscordGpt.TestHelpers/MessageCreatedEventBuilder.cs` for `MessageCreatedEvent` construction.

**Test scenarios:**
- *Happy path:* DM from non-bot user → bridge constructs `MessageCreatedEvent` with `GuildId = null` and calls `eventHandler.HandleAsync` exactly once.
- *Happy path:* Guild message that mentions bot → bridge constructs event with correct `GuildId`/`ChannelId` and dispatches.
- *Happy path:* Guild message that is a reply to a thread starter → reply context is preserved on `MessageCreatedEvent`.
- *Edge case:* Bot author → bridge skips, no dispatch, returns false for guild path.
- *Edge case:* Own user id → skip.
- *Edge case:* Message starting with `/` or other command prefix → skip (slash commands handled separately).
- *Edge case:* Guild message with no mention and no role match → skip; `TryHandleGuildMessageAsync` returns false.
- *Edge case:* Empty content → skip.
- *Error path:* `eventHandler.HandleAsync` throws → exception propagates (telemetry captured by `DiscordInitializationService`).
- *Integration:* Composed with real `DiscordGptEventHandler` and fakes for `DiscordRestClient`/`IDiscordAgent`/`IConversationStore`, a DM round-trip ends with `DiscordRestClient.CreateMessageAsync` being called once.

**Verification:**
- All gate scenarios pass.
- `services/ChatBot/MessageHandler.cs` does not reference any deleted-in-Unit-7 type (no `Conversation`, `ChatRunner`, `PromptCatalog`, `UserChatSynchronization`, `ChatThreadResetter`, anything in `ChatBot.Copilot`).

---

- [x] **Unit 6: Rewrite `/chat reset` and `/chat new` to call `IConversationStore.ClearAsync`**

**Goal:** Slash commands keep working after `ChatThreadResetter` is deleted. `ChatCommandModule.cs` is the only file modified in this unit.

**Requirements:** R8

**Dependencies:** Unit 4 (`IConversationStore` and `IConversationKeyResolver` resolvable from DI).

**Files:**
- Modify: `app/DiscordInterop/CommandModules/ChatCommandModule.cs` — replace the `ChatThreadResetter` constructor injection with `IConversationStore conversationStore` + `IConversationKeyResolver conversationKeyResolver`. Both `/chat reset` and `/chat new` derive a `ConversationKey` from `Context` (slash-command interaction context) and call `conversationStore.ClearAsync(key, ct)`. Preserve the existing ephemeral-reply UX.

**Approach:**
- Two valid resolution paths for `ConversationKey`:
  - **(a)** Construct directly from `Context.User.Id` / `Context.Channel.Id` / `Context.Guild?.Id`. Simpler. Recommended.
  - **(b)** Synthesize a minimal `DiscordEvent` and pass to the resolver. More uniform but more code.
  - Pick (a) unless implementation reveals the resolver applies non-trivial channel-vs-user keying logic that the slash command also needs.
- Reset and New behave identically from the store's perspective (both clear conversation state); keep separate command names for UX continuity.
- If the original UX message wording matters, preserve verbatim.

**Patterns to follow:**
- Existing `ChatCommandModule.cs` ephemeral-reply pattern.

**Test scenarios:**
- *Happy path:* `/chat reset` with valid context → `ClearAsync` called with key derived from user+channel; ephemeral confirmation reply sent.
- *Happy path:* `/chat new` → same as reset (or differentiated only if existing UX differs).
- *Edge case:* `ClearAsync` throws → user sees ephemeral error, exception logged.
- *Integration:* End-to-end: dispatch slash command via `InteractionService`, verify `ClearAsync` invoked.

**Verification:**
- `ChatCommandModule.cs` no longer references `ChatThreadResetter`.
- New tests pass.

---

- [x] **Unit 7: Delete obsolete `services/ChatBot/*` files + drop-and-recreate `userChatAgentThreads` table**

**Goal:** Strip all legacy ChatBot code. Only `MessageHandler.cs`, `ChatBot.csproj`, the slimmed `DependencyInjectionExtensions.cs`, and the new files from Unit 3 (`Agents/FrcDiscordAgent.cs`, `Prompts/*.txt`, `Tools/*.cs`) survive. Operationally, drop and recreate the Azure Table.

**Requirements:** R4 (deletes)

**Dependencies:** Units 5 + 6 (no remaining references to deleted types).

**Files (delete):**
- `services/ChatBot/Agents/` (everything except the new `FrcDiscordAgent.cs` from Unit 3 — note: Unit 3 may have placed it in a temporary location; this unit consolidates final structure)
- `services/ChatBot/Copilot/` (all)
- `services/ChatBot/Configuration/` (all)
- `services/ChatBot/Diagnostics/` (all)
- `services/ChatBot/ChatRunner.cs`
- `services/ChatBot/ChatThreadResetter.cs`
- `services/ChatBot/Conversation.cs`
- `services/ChatBot/ConversationThreadState.cs`
- `services/ChatBot/Tools/` (everything except new tools from Unit 3)
- `services/ChatBot/UserChatSynchronization.cs`
- `services/ChatBot/AzureIdentityActivityFilteringProcessor.cs`
- `services/ChatBot/ChatBotConstants.cs`
- `services/ChatBot/Log.cs`
- `services/ChatBot/agent_prompt.txt`, `services/ChatBot/meal_agent_prompt.txt`, `services/ChatBot/progress_messages.txt`

**Files (verify untouched):**
- `services/ChatBot/MessageHandler.cs` (body replaced in Unit 5)
- `services/ChatBot/ChatBot.csproj` (project refs updated in Unit 2; no further changes)
- `services/ChatBot/DependencyInjectionExtensions.cs` (rewritten in Unit 3)
- All new files from Unit 3.

**Operational step (deploy-time, NOT a code change):**
- Drop the existing `userChatAgentThreads` Azure Table via `az storage table delete --name userChatAgentThreads ...` against the deployment storage account before first deploy. The library auto-creates the table with the new schema on first write.
- Document this as a one-line runbook entry in `docs/runbooks/` (create file if directory doesn't exist) OR inline in the PR description. **No code change** — purely operational.

**Approach:**
- Run `Remove-Item` recursively on each path; rely on Unit 8's build/test pass to surface any missed references.
- If any reference outside `services/ChatBot/` reaches into a now-deleted type, that's a planning error — fix at point of reference and document in PR.
- Verify `app/Program.cs:96-106` `storageTables` config loop still creates `userChatAgentThreads` if it's listed in config; if the library auto-creates instead, remove it from the config list (no code change required if absent already).

**Test scenarios:**
- Test expectation: none — pure deletion. Verified by Unit 8 build pass + smoke test.

**Verification:**
- `grep -r "ChatBot\.\(Copilot\|Agents\|Tools\|ChatRunner\|ChatThreadResetter\|Conversation\|UserChatSynchronization\|ChatBotConstants\)" .` returns zero results outside the deleted files themselves.
- `dotnet build` zero warnings.
- Azure Table Storage Explorer shows fresh `userChatAgentThreads` table with new schema after first chat exchange post-deploy.

---

- [x] **Unit 8: Build verification + smoke test**

**Goal:** Confirm the entire repo still builds with `TreatWarningsAsErrors`, all tests pass, and a deployed instance can complete a DM round-trip and a guild-mention thread creation.

**Requirements:** R9 + end-to-end validation of R1.

**Dependencies:** All prior units.

**Files:**
- None modified.

**Approach:**
- `dotnet build FRCDiscordBot.slnx -c Release /warnaserror`
- `dotnet test FRCDiscordBot.slnx -c Release --no-build`
- Deploy to a non-prod Container Apps revision.
- Smoke checks: (a) DM the bot — receive reply; (b) @-mention in a guild channel — receive reply in a new thread with a generated title; (c) `/chat reset` in DM — receive ephemeral confirmation; subsequent DM treats history as empty; (d) verify `userChatAgentThreads` Azure Table has fresh schema rows; (e) **R10 verification:** issue a prompt that triggers a tool invocation (e.g., a team-stats lookup) — confirm the bot responds with tool results without ever sending a Discord message asking the user to approve a tool call.

**Test scenarios:**
- Test expectation: none new — meta-verification unit. Coverage from Units 1, 3, 5, 6.

**Verification:**
- Zero build warnings.
- Zero failing tests.
- All four smoke checks pass.
- Allow-all auth posture verified observationally (no approval-prompt messages emitted to user during a tool-using prompt).

## System-Wide Impact

- **Interaction graph:**
  - `DiscordSocketClient.MessageReceived` → `DiscordInitializationService` → `MessageHandler` (NEW: → `IDiscordEventHandler`).
  - `InteractionService` slash-command dispatch → `ChatCommandModule` (NEW: → `IConversationStore.ClearAsync`).
  - All other slash commands (`/events`, `/teams`, `/matches`, `/subscriptions`, `/ping`), `DiscordMessageDispatcher`, webhook embed pipeline: **untouched.**
- **Error propagation:** `IDiscordEventHandler` exceptions propagate up through `MessageHandler` to `DiscordInitializationService`'s existing telemetry wrapper. `IConversationStore.ClearAsync` exceptions propagate to `ChatCommandModule`'s ephemeral-error UX.
- **State lifecycle risks:** Drop-and-recreate of `userChatAgentThreads` is destructive; gated by "unreleased functionality" assertion. Library auto-creates table on first write, so no race between deploy and first message.
- **API surface parity:** `MessageHandler.HandleUserMessageAsync` and `TryHandleGuildMessageAsync` signatures preserved exactly, so `DiscordInitializationService:160,173` callers compile and behave identically (return semantics included).
- **Integration coverage:** Unit 5's integration test covers the full bridge → library → fake-REST round-trip. Unit 8 smoke covers real Discord ↔ Azure Table.
- **Tool/skill authorization surface:** `ICopilotToolAuthorizationPolicy` is registered with `AllowAllTools`/`AllowAllSkills`/`AllowToolsInDirectMessages`/`AllowSkillsInDirectMessages = true`, no `Allowed*Ids` filters. Every registered `IDiscordTool` and `IDiscordSkill` is callable in every conversation context. **No human-approval prompts ever surface to the end user during tool execution.** The agent retains its conversational ability to ask clarifying questions through its normal LLM output channel, but never as a synchronous tool-approval gate.
- **Unchanged invariants:**
  - `BlobServiceClient` registration (`Program.cs:122-136`) — preserved verbatim for FRC notification artifact pipeline.
  - `TableServiceClient` factory (`Program.cs:96-120`) — preserved; the library's `TableConversationStore` consumes the same `TableServiceClient`.
  - All non-chat slash commands — zero changes.
  - `DiscordInitializationService.cs` — zero changes.
  - `DependencyInjectionExtensions.cs` (under `app/DiscordInterop/`) — zero changes.
  - App's `DiscordSocketClient` intents (`AllUnprivileged | MessageContent`) — zero changes.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| `SendResponseAsync` impl in Unit 1 dispatches to wrong shape (e.g., creates a thread when it should reply inline) | Unit 1 test scenarios enumerate every event-shape branch; Unit 5 integration test exercises the full pipeline against a fake REST handler. |
| `MessageCreatedEvent` field set in Unit 5 is incomplete; library can't resolve conversation key | Use `gpt/src/BC3Technologies.DiscordGpt.TestHelpers/MessageCreatedEventBuilder.cs` as the canonical field-population reference. Unit 5 integration test will fail loudly if a required field is missing. |
| Gate logic in Unit 5 silently filters legitimate messages (over-filters) | Test-first execution posture (per Unit 5's execution note) catches this. Smoke checks in Unit 8 catch any escape. |
| Discord REST 429 rate-limit handling in Unit 1 is too naive (no token-bucket per route) | Single retry with `Retry-After` is sufficient for low-volume bot traffic. Document as future hardening if production traffic warrants. |
| `DefaultInteractionPolicy` doesn't match existing FRC-specific allowlist behavior | Unit 4 explicitly verifies during implementation; if mismatch, custom `IInteractionPolicy` is added (in scope as a new file under `services/ChatBot/`). |
| Library's `TableConversationStore` doesn't call `CreateIfNotExists` and we forget to add the table to the existing storage-tables config loop | Unit 4 verifies at implementation time. Unit 8 smoke test catches any failure (first DM would error). |
| Operational step in Unit 7 (drop existing table) is forgotten | Document inline in PR description and as a `docs/runbooks/` entry. Unit 8 smoke test verifies new schema rows exist post-deploy — old-schema rows would cause library reads to fail. |
| Tool inventory in Unit 3 misses a tool the existing agent had | Unit 3 mandates explicit enumeration of `services/ChatBot/Tools/` contents at start; each tool gets a port. Smoke test will surface gaps if a previously-working chat behavior fails. |
| Allow-all auth policy (R10) exposes any future-added tool to any user/channel/guild without review | Documented as intentional per user requirement. Mitigation is procedural: any new `IDiscordTool` added to DI is automatically reachable, so PR review for new tool registrations carries the security weight. The allow-all flags live in one place (`AddFrcChatBot` extension in Unit 3) and are easy to audit. If future scope tightens, swap to per-tool/per-context allowlists by populating `AllowedToolNames`/`AllowedUserIds` etc. and setting `AllowAllTools = false` — no code changes outside the configurator. |
| Library extension point for system prompt unclear; Unit 3 may need decorator | **Resolved during planning.** Verified via direct inspection that no built-in hook exists anywhere in `gpt/src/`. The `IChatClient` decorator (`FrcSystemPromptChatClient`) is mandated, not optional. Library itself uses the same decoration pattern (`AddCopilotAgent` calls `rawClient.AsBuilder().UseFunctionInvocation(...).Build()`), so the approach is well-supported. |
| `services/ChatBot/Tools/*.cs` ports change tool semantics in subtle ways (different argument schemas) | Microsoft Agent Framework's `[Description]` pattern is stable; port preserves names and descriptions verbatim. Unit 3 integration tests exercise each tool. |

## Documentation / Operational Notes

- **One-time deploy step (Unit 7):** Drop the existing `userChatAgentThreads` Azure Table before deploying the cutover commit. The library auto-creates the table with the new schema on first write. Loss of any existing rows is intentional (functionality unreleased).
- **PR description must include:**
  - Note about Unit 1 upstream `gpt/` library changes (no separate PR).
  - Note about the destructive table operation and the "no production data" rationale.
  - Confirmation that no other files outside the modify list were touched.
- **Future plan opportunities (out of scope here):**
  - Migrate non-chat slash commands and webhook dispatcher to library-owned client to retire app's `DiscordSocketClient` entirely.
  - Add per-route token-bucket rate-limit handling to `DiscordRestClient`.

## Sources & References

- `app/Program.cs:78-82` (marker), `app/Program.cs:96-136` (existing storage factories)
- `services/ChatBot/MessageHandler.cs` (bridge target)
- `app/DiscordInterop/CommandModules/ChatCommandModule.cs` (slash-command rewrite)
- `app/DiscordInterop/DiscordInitializationService.cs:27,145-175` (untouched subscription)
- `gpt/src/BC3Technologies.DiscordGpt.Hosting/DiscordGptEventHandler.cs:34,111` (entry + TODO stub)
- `gpt/src/BC3Technologies.DiscordGpt.Discord/DiscordRestClient.cs` (REST surface to extend)
- `gpt/src/BC3Technologies.DiscordGpt.Core/MessageCreatedEvent.cs` (event shape)
- `gpt/src/BC3Technologies.DiscordGpt.TestHelpers/MessageCreatedEventBuilder.cs` (canonical field-population)
- `gpt/src/BC3Technologies.DiscordGpt.Storage.TableStorage/TableConversationStore.cs` (target schema)
- `azure.yaml` (Container Apps host — confirmed)
