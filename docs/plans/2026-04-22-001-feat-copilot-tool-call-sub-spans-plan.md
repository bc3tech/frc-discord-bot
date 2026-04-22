---
title: "feat: Copilot SDK tool-call sub-spans"
type: feat
status: active
date: 2026-04-22
---

# feat: Copilot SDK tool-call sub-spans

## Overview

Complete the conversation telemetry picture started in `feat/telemetry` by emitting
OTel GenAI `execute_tool` (and related) child spans for every `SessionEvent` raised by
`GitHub.Copilot.SDK` during a Discord turn. Today's shipped code only opens
`chat.conversation` (root) and `chat.turn` (per-turn) spans; tool calls, sub-agent
handoffs, assistant deltas, and usage emitted by the Copilot SDK are invisible in
Application Insights.

The translator (`CopilotSessionTelemetry.Subscribe(CopilotSession)`) already exists in
`lib/CopilotSdk.OpenTelemetry`. The blocker is wiring: the `CopilotSession` is created
and consumed entirely inside `gpt/` (`GitHubCopilotPromptHarness.RunPromptAsync`) and
never escapes to the FRC bot, so we have no place to call `Subscribe`.

This plan investigates a zero-touch path first (subscribe to client lifecycle events
externally and `ResumeSessionAsync` to obtain a parallel handle) and falls back to a
small, OTel-agnostic extensibility hook in `gpt/` if the lifecycle path doesn't behave.

## Problem Frame

Without tool-call spans, the Application Insights trace for a turn shows
`chat.conversation > chat.turn > [opaque]`. The user can see that a turn happened
and how long it took, but cannot see which tools the agent called, which MCP servers
were invoked, how long each tool ran, whether tool calls failed, or which sub-agent
handoff was responsible for which work. That hides the most operationally interesting
behavior of the bot.

The constraint from prior session was "OTel integration must not touch DiscordGpt."
The user clarified today that the spirit of the rule is "DiscordGpt stays
OTel-agnostic" — a small generic extensibility hook in `gpt/` that knows nothing about
OTel is acceptable as a fallback if no purely-external option works.

## Requirements Trace

- **R1.** Each Copilot tool execution observed during a Discord turn appears as an
  `execute_tool` child span of the per-turn `chat.turn` span in App Insights.
- **R2.** Tool spans carry the OTel GenAI conventions: `gen_ai.system=github.copilot`,
  `gen_ai.operation.name=execute_tool`, `gen_ai.tool.name`, `gen_ai.tool.call.id`,
  plus MCP-specific tags when the tool is an MCP tool.
- **R3.** Tool span status reflects `ToolExecutionCompleteEvent.Success` /
  `Error.Message`.
- **R4.** Assistant usage events (`AssistantUsageEvent`) annotate the `chat.turn` span
  with `gen_ai.response.model` and token counts.
- **R5.** Implementation prefers a path that does not modify `gpt/`. If that path is
  not viable, the change to `gpt/` introduces no OTel/`OpenTelemetry.*` dependency and
  no OTel-aware code into `gpt/`.
- **R6.** Multi-tool turns and out-of-order completion (concurrent tool calls in the
  same turn) are tracked correctly — no leaked or mis-parented spans.
- **R7.** Subscription disposal is deterministic; no listener outlives its session, no
  in-flight tool span is leaked when a session ends abnormally.

## Scope Boundaries

- **In scope:** tool-call spans (`ToolExecutionStartEvent` / `ToolExecutionCompleteEvent`),
  usage event tagging on the turn span, error event tagging on the turn span, the
  external wiring needed to attach `CopilotSessionTelemetry.Subscribe` to live sessions.
- **Out of scope:** assistant message-delta events as discrete spans (these are
  high-volume streaming events and would clutter traces — the turn span already
  captures the assistant turn boundary).
- **Out of scope:** sub-agent handoff spans as a *separate* OTel span hierarchy. The
  Copilot SDK does emit `SubagentStartedEvent` / `SubagentCompletedEvent`, but those
  fire under the same parent turn and adding them is additive — they're added in a
  follow-up if the trace UI proves to need them. (Will note this in the deferred list.)
- **Out of scope:** redesigning the existing `chat.conversation` / `chat.turn` model.
- **Out of scope:** changing the `IConversationTraceContextStore` persistence layer.

## Context & Research

### Relevant Code and Patterns

- `lib/CopilotSdk.OpenTelemetry/CopilotSessionTelemetry.cs` — the translator. Already
  handles `ToolExecutionStartEvent`, `ToolExecutionCompleteEvent`, `AssistantUsageEvent`,
  `SessionErrorEvent`. Captures `Activity.Current` at `Subscribe` time as the parent.
- `lib/CopilotSdk.OpenTelemetry/CopilotSdkOpenTelemetry.cs` — the `ActivitySource`,
  GenAI tag constants, system value (`github.copilot`).
- `services/ChatBot/MessageHandler.cs` — externally wraps the per-turn span via
  `_conversationTracer.BeginTurnAsync(...)` before dispatching to
  `IDiscordEventHandler.HandleAsync`. The active `Activity` lives on the async stack
  for the duration of the turn.
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/GitHubCopilotPromptHarness.cs` (lines
  87-90) — the only call site of `CopilotClient.CreateSessionAsync`. The session
  reference is local; nothing escapes.
- `gpt/src/BC3Technologies.DiscordGpt.Copilot/CopilotServiceCollectionExtensions.cs`
  — `CopilotClient` is registered as `TryAddSingleton`. A `CopilotClient` instance
  is shared across all turns and all users.
- `GitHub.Copilot.SDK` (NuGet 0.2.2) public surface confirmed last session:
  - `CopilotClient` is `sealed`. No subclassing, no `DispatchProxy`.
  - `CopilotClient.On(Action<SessionLifecycleEvent>) → IDisposable` — fires lifecycle
    notifications (start, modify, end) carrying a `SessionId`.
  - `CopilotClient.ResumeSessionAsync(string sessionId, ResumeSessionConfig, CancellationToken) → Task<CopilotSession>`
    — returns a session handle for an existing session id.
  - `CopilotSession.On(SessionEventHandler) → IDisposable` — the rich event stream.

### Institutional Learnings

- This session's prior checkpoints (`005-external-only-telemetry-refact.md` and
  `004-wiring-telemetry-into-discord.md`) document the architectural exploration of
  hooking the SDK from outside `gpt/`. The conclusion was that the `CopilotClient`
  sealed class blocks every external interception trick (proxy, decorator, subclass)
  *except* the lifecycle-event + resume path, which was never validated.
- The "merge Foundry agents + local agents" precedent (`brandonh-msft/agentframework-opentelemetry`)
  showed that emitting GenAI spans is straightforward once you have a hook into the
  underlying agent runtime — the hard part is always the hook, not the translation.

### External References

- OTel GenAI semantic conventions for `execute_tool` spans:
  <https://opentelemetry.io/docs/specs/semconv/gen-ai/gen-ai-spans/>. The translator
  already follows them; no API changes there.

## Key Technical Decisions

- **Spike before commit.** The `lifecycle-event + ResumeSessionAsync` path is
  attractive (zero `gpt/` change, full event stream) but rests on undocumented SDK
  behavior. We cannot read this from the SDK source; the only honest answer is to
  run a small spike and observe. The cost of spiking is much lower than the cost of
  building an integration on a wrong assumption.
- **Fallback is a one-line generic hook in `gpt/`.** If the spike fails, add
  `Action<CopilotSession>? OnSessionCreated { get; init; }` to `DiscordGptOptions`
  in `gpt/` and invoke it inline after `CreateSessionAsync` returns. The hook
  carries no OTel knowledge, no new dependency — `Action<CopilotSession>` is BCL.
  This is the smallest possible touch and preserves the spirit of "DiscordGpt stays
  OTel-agnostic."
- **No fork, no Harmony.** Both options were considered last session and rejected
  on maintenance and debuggability grounds. They remain rejected.
- **Wire from `services/ChatBot`, not from `lib/CopilotSdk.OpenTelemetry`.** The
  library stays a translator + primitives. The bot owns the "when to subscribe"
  decision because it owns DI and lifecycle. Library remains open-source-friendly.
- **One subscription per session, disposed on session end.** Track active
  subscriptions in a `ConcurrentDictionary<string, IDisposable>` keyed by session id.
  Dispose on `SessionLifecycleEvent.Type == "session_ended"` (or equivalent). On
  app shutdown, dispose all.

## Open Questions

### Resolved During Planning

- *How are tool spans named?* — `execute_tool {tool_name}` per OTel GenAI conventions.
  Already implemented in `CopilotSessionTelemetry`.
- *How is the tool span parented?* — `Activity.Current` at `Subscribe` time becomes
  the parent context (via `parentContext` on `StartActivity`). Already implemented.
- *Where does the wiring live?* — `services/ChatBot/Diagnostics/`, registered via
  `AddCopilotSdkOpenTelemetry()` extension in `services/ChatBot/DependencyInjectionExtensions.cs`.

### Deferred to Implementation

- *Which `SessionLifecycleEvent.Type` string indicates session-ended?* — read the
  string values during the spike, then constant-ize.
- *Does `ResumeSessionAsync` produce side effects?* — observable during spike (e.g.,
  does it appear as a new session in any logs, does the resumed handle's disposal
  affect the original session, etc.).
- *Should sub-agent handoff events get their own span hierarchy?* — defer until
  there's real production trace data showing whether the current model is sufficient.

### Spike Decision Gate (must answer before Phase 2)

- **G1.** Does `client.On(SessionLifecycleEvent)` fire for sessions created via
  `client.CreateSessionAsync` *on the same client instance*? (Not just for resumed
  or externally-started sessions.)
- **G2.** Does `client.ResumeSessionAsync(sessionId)` return a `CopilotSession`
  whose `On(SessionEventHandler)` receives the live event stream, including events
  that occur *after* the resume call?
- **G3.** Is the lifecycle callback timing such that we can subscribe to the event
  stream before the first `ToolExecutionStartEvent` fires? (i.e., is there a race
  between session creation, the lifecycle notification, and the first tool call?)
- **G4.** Does the lifecycle callback fire on a thread/async context where
  `Activity.Current` is the in-flight per-turn `chat.turn` span? (If not, the spans
  will be parentless or wrongly parented and a different correlation strategy is
  required.)

If **G1, G2, G3** are all "yes" and **G4** is "yes" *or* a viable correlation
fallback exists (e.g., AsyncLocal capture at the `BeginTurnAsync` boundary keyed
by the calling turn), proceed with Path A. Otherwise, fall back to Path B.

## High-Level Technical Design

> *This illustrates the intended approach and is directional guidance for review,
> not implementation specification. The implementing agent should treat it as
> context, not code to reproduce.*

### Path A — external lifecycle listener (preferred, pending spike)

```text
                                                       ┌────────────────────────────────────┐
MessageHandler.HandleUserMessageAsync                  │ CopilotSessionLifecycleListener    │
  │                                                    │  (singleton, IHostedService)       │
  │  IConversationTracer.BeginTurnAsync(...)           │                                    │
  │  ──────► chat.turn Activity opened ────────┐       │  ctor: client.On(LifecycleHandler) │
  │                                            │       │                                    │
  │  IDiscordEventHandler.HandleAsync          │       │  LifecycleHandler(evt):            │
  │    └─► CopilotDiscordAgent.RespondAsync    │       │    if evt.Type == "session_started"│
  │         └─► GitHubCopilotPromptHarness     │       │      session = await client        │
  │              └─► client.CreateSessionAsync │       │              .ResumeSessionAsync(  │
  │                    │                       │       │                evt.SessionId, …)   │
  │                    │  (lifecycle fires) ───┼──────►│      sub = CopilotSessionTelemetry │
  │                    │                       │       │            .Subscribe(             │
  │                    │  ◄───────── parent = Activity.Current = chat.turn ────────────────┐│
  │                    │                       │       │              session,              ││
  │                    ▼                       │       │              logger)               ││
  │              session.SendAndWaitAsync ─── tool/usage events flow ─► tool spans         ││
  │                                            │       │      _subs[evt.SessionId] = sub    ││
  │  chat.turn closes ◄────────────────────────┘       │                                    ││
  │                                                    │    if evt.Type == "session_ended"  ││
  │                                                    │      _subs.Remove(evt.SessionId)?  ││
  │                                                    │            .Dispose()              ││
  │                                                    └────────────────────────────────────┘│
  ▼                                                                                          │
                                                                                             │
  ◄─────────── tool spans appear as children of chat.turn in App Insights ──────────────────┘
```

The listener is a singleton hosted service that subscribes to the singleton
`CopilotClient`'s lifecycle stream once at app startup. Per-session subscriptions
flow naturally from there.

### Path B — generic hook in `gpt/` (fallback)

```text
gpt/.../DiscordGptOptions
   public Action<CopilotSession>? OnSessionCreated { get; init; }   ◄── ONE NEW LINE

gpt/.../GitHubCopilotPromptHarness.RunPromptAsync
   var session = await _copilotClient.CreateSessionAsync(...);
   _options.OnSessionCreated?.Invoke(session);                       ◄── ONE NEW LINE

services/ChatBot/DependencyInjectionExtensions
   .Configure<DiscordGptOptions>(o => o.OnSessionCreated =
       session => CopilotSessionTelemetry.Subscribe(session, logger));
```

`Activity.Current` at the hook invocation site is naturally the per-turn span,
because the hook fires on the same async stack that opened it. No correlation
gymnastics needed. Two lines added to `gpt/`, no OTel deps.

## Implementation Units

- [ ] **Unit 1: Spike — validate the lifecycle/resume path**

**Goal:** Answer the four decision-gate questions (G1-G4) above with empirical
evidence so we can choose Path A vs. Path B.

**Requirements:** R5 (drives the choice between the two paths).

**Dependencies:** None.

**Files:**
- Create: `lib/CopilotSdk.OpenTelemetry.Spike/CopilotSdk.OpenTelemetry.Spike.csproj`
  (xUnit, references `lib/CopilotSdk.OpenTelemetry`, `GitHub.Copilot.SDK`)
- Create: `lib/CopilotSdk.OpenTelemetry.Spike/LifecycleResumeSpike.cs`
- Modify: `FRCDiscordBot.slnx` (add the spike project so it builds in CI)

**Approach:**
- Authenticate against the real Copilot service using the same token mechanism the
  bot uses (`GITHUB_TOKEN` from environment, gated `[Trait("Category", "live")]`
  so it doesn't run by default in CI).
- In a single test method:
  1. Instantiate `CopilotClient`.
  2. Subscribe a lifecycle listener that records `(timestamp, threadId, Activity.Current?.Id, evt.Type, evt.SessionId)`.
  3. Open an `Activity` named `spike.turn` via a dedicated `ActivitySource`.
  4. Call `CreateSessionAsync` and `SendAndWaitAsync` with a prompt that
     reliably triggers a tool call (e.g., a prompt mentioning a known MCP tool
     or asking the agent to read a file).
  5. From inside the lifecycle callback for `session_started`, call
     `ResumeSessionAsync` and subscribe a second listener that records every
     `SessionEvent` it receives, with timestamps.
  6. After the prompt completes, dump both event logs.
- Assert on what was observed; record the answers to G1-G4 in the test output.
- This is a throwaway project — delete it after Path A or B is chosen and shipped.

**Execution note:** Spike — write whatever code is needed to learn. Test scenarios
below are *what to observe*, not unit assertions. The spike's output is a Markdown
note appended to this plan under a new "Spike Findings" section, not green tests.

**Test scenarios:**
- *Observation:* `SessionLifecycleEvent` callback fires for the session created by
  `CreateSessionAsync` on the same client. (G1)
- *Observation:* The resumed `CopilotSession` handle's `On` callback receives at
  least one `ToolExecutionStartEvent` and matching `ToolExecutionCompleteEvent`
  during the prompt. (G2)
- *Observation:* The lifecycle callback fires before the first
  `ToolExecutionStartEvent` (compare timestamps). (G3)
- *Observation:* `Activity.Current?.Id` inside the lifecycle callback equals the
  `spike.turn` activity's id. (G4)

**Verification:**
- A "Spike Findings" addendum is appended to this plan with a Yes/No table for
  G1-G4 and a recommended path (A or B). The spike project is then deleted in
  the unit that ships the chosen path.

---

- [ ] **Unit 2A: External lifecycle listener wiring** *(only if spike picks Path A)*

**Goal:** Implement the `CopilotSessionLifecycleListener` so tool spans appear in
App Insights for every Copilot session opened by the bot, with no `gpt/` changes.

**Requirements:** R1, R2, R3, R4, R5, R6, R7.

**Dependencies:** Unit 1 picks Path A.

**Files:**
- Create: `services/ChatBot/Diagnostics/CopilotSessionLifecycleListener.cs`
- Modify: `services/ChatBot/DependencyInjectionExtensions.cs` (register the listener
  as a singleton `IHostedService` when `AddCopilotSdkOpenTelemetry()` is called)
- Create: `app/FunctionApp.Tests/ChatBot/Diagnostics/CopilotSessionLifecycleListenerTests.cs`

**Approach:**
- Listener is `IHostedService` (started by the host) + `IDisposable`.
- Constructor takes `CopilotClient`, `ILogger<CopilotSessionLifecycleListener>`,
  optional `IConversationTracer` (for diagnostics only).
- `StartAsync` calls `_client.On(HandleLifecycle)` and stores the `IDisposable`.
- `HandleLifecycle(SessionLifecycleEvent evt)`:
  - On `session_started`: capture `Activity.Current`, call
    `_client.ResumeSessionAsync(evt.SessionId, ResumeSessionConfig.Default, ct)`,
    then `CopilotSessionTelemetry.Subscribe(session, _logger)`. Store the
    returned `IDisposable` keyed by `evt.SessionId`.
  - On `session_ended` (string verified by spike): remove and dispose the
    subscription.
- `StopAsync` / `Dispose`: dispose the lifecycle subscription, then dispose every
  remaining per-session subscription.

**Patterns to follow:**
- `services/ChatBot/Diagnostics/` (existing diagnostics components — match
  style, logging, source-generated `[LoggerMessage]` partial methods).
- Existing `IHostedService` registrations in `services/ChatBot/DependencyInjectionExtensions.cs`.

**Test scenarios:**
- *Happy path:* `session_started` event triggers `ResumeSessionAsync` + `Subscribe`,
  subscription is stored.
- *Happy path:* `session_ended` event removes and disposes the subscription.
- *Edge case:* duplicate `session_started` for the same id — second subscription
  replaces and disposes the first (no leak).
- *Edge case:* `session_ended` for an unknown id is a no-op.
- *Error path:* `ResumeSessionAsync` throws — error is logged at Warning, no
  subscription is stored, listener stays alive for subsequent events.
- *Lifecycle:* `Dispose` (host shutdown) disposes the lifecycle subscription
  *and* every remaining per-session subscription.
- *Integration:* with a fake `CopilotClient` that emits a synthetic lifecycle event
  and a fake `CopilotSession` that emits a synthetic `ToolExecutionStartEvent` /
  `ToolExecutionCompleteEvent`, an `ActivityListener` records exactly one
  `execute_tool` activity parented to the in-flight `chat.turn` activity.

**Verification:**
- A live run of the bot with App Insights connected shows `chat.turn` spans with
  `execute_tool` children for any prompt that triggers a tool call.

---

- [ ] **Unit 2B: Generic `OnSessionCreated` hook in `gpt/`** *(only if spike picks Path B)*

**Goal:** Add a non-OTel-aware extensibility hook to `gpt/` so the bot can
externally subscribe to every session created by the harness.

**Requirements:** R1, R2, R3, R4, R5, R6, R7.

**Dependencies:** Unit 1 picks Path B.

**Files (in the `gpt/` submodule, separate PR/commit):**
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/DiscordGptOptions.cs`
  — add `public Action<CopilotSession>? OnSessionCreated { get; init; }`
- Modify: `gpt/src/BC3Technologies.DiscordGpt.Copilot/GitHubCopilotPromptHarness.cs`
  — invoke the hook synchronously immediately after `_copilotClient.CreateSessionAsync(...)`
  returns, wrapped in a try/catch that logs and swallows so a misbehaving hook
  cannot break a turn.
- Modify: `gpt/tests/BC3Technologies.DiscordGpt.Copilot.Tests/GitHubCopilotPromptHarnessTests.cs`
  — add a test that verifies the hook is invoked exactly once with the created
  session.

**Files (in this repo):**
- Modify: `gpt` submodule pointer — bump to the new commit on `main`.
- Create: `services/ChatBot/Diagnostics/CopilotSessionTelemetryHook.cs` — small
  helper that resolves `IServiceProvider`, builds the `Action<CopilotSession>`
  delegate, configures it onto `DiscordGptOptions` via `IConfigureOptions<DiscordGptOptions>`.
- Modify: `services/ChatBot/DependencyInjectionExtensions.cs` — register the
  options configurator when `AddCopilotSdkOpenTelemetry()` is called.
- Create: `app/FunctionApp.Tests/ChatBot/Diagnostics/CopilotSessionTelemetryHookTests.cs`

**Approach (gpt/ side):**
- New property is opt-in: default null means today's behavior is preserved exactly.
- Invocation is synchronous, on the same async stack — `Activity.Current` is
  naturally the per-turn span.
- Failures in the hook are logged at Warning and swallowed.

**Approach (this repo side):**
- `IConfigureOptions<DiscordGptOptions>` resolves `ILogger<...>` from DI and sets
  `OnSessionCreated = session => CopilotSessionTelemetry.Subscribe(session, logger)`.
- The returned `IDisposable` from `Subscribe` is stored in a
  `ConcurrentDictionary` keyed by something stable (session id if the hook can
  read it; otherwise a `WeakReference<CopilotSession>` mapping). On
  `IConfigureOptions` disposal (or via a paired `IHostedService` for cleanup),
  dispose all.
- Simpler alternative if the lifecycle of subscriptions can ride on the session's
  GC: just let the `IDisposable` be eligible for finalization when the session
  is. Worst case: a few finalizer-time disposals. Decide at implementation time.

**Patterns to follow:**
- Existing `ISessionConfigSource` in `gpt/src/BC3Technologies.DiscordGpt.Copilot/`
  — same opt-in extensibility shape.
- `services/ChatBot/Diagnostics/` for the bot-side glue.

**Test scenarios:**
- *(gpt/) Happy path:* `OnSessionCreated` is invoked exactly once per
  `RunPromptAsync` call, with the same `CopilotSession` instance the harness uses.
- *(gpt/) Edge case:* `OnSessionCreated == null` — no invocation, no exception.
- *(gpt/) Error path:* `OnSessionCreated` throws — exception is logged, the turn
  continues normally.
- *(this repo) Happy path:* registering the configurator results in a real
  `Subscribe` call against a fake session, verified via `ActivityListener`.
- *(this repo) Lifecycle:* subscriptions are disposed on app shutdown.

**Verification:**
- Same as Unit 2A — App Insights shows nested `execute_tool` spans for tool calls.

---

- [ ] **Unit 3: Translator coverage audit + extension**

**Goal:** Confirm `CopilotSessionTelemetry` handles every `SessionEvent` subtype
that's worth representing in App Insights, and add the missing ones.

**Requirements:** R1, R2, R3, R4.

**Dependencies:** Unit 1 (the spike output enumerates the actual event subtypes
observed during a real prompt — useful sanity check).

**Files:**
- Modify: `lib/CopilotSdk.OpenTelemetry/CopilotSessionTelemetry.cs`
- Modify: `lib/CopilotSdk.OpenTelemetry/CopilotSdkOpenTelemetry.cs` (any new
  GenAI tag constants)
- Modify: `lib/CopilotSdk.OpenTelemetry.Tests/CopilotSessionTelemetryTests.cs`

**Approach:**
- Enumerate `SessionEvent` subtypes via reflection in a one-off test (or by
  inspecting the SDK assembly). Confirm the four already-handled subtypes are
  the right ones. Likely additions:
  - `AssistantTurnStartEvent` / `AssistantTurnEndEvent` — *do not* emit a
    separate span; they bracket the turn span externally. Use them only if
    the existing turn span doesn't already cover the right window.
  - `SubagentStartedEvent` / `SubagentCompletedEvent` — *deferred* per the
    Scope Boundaries section. Add a hook point but don't emit yet.
- Resist the urge to add a span for every subtype — most are streaming detail
  that would clutter traces.

**Test scenarios:**
- *Happy path:* every newly-handled event subtype produces a span (or a tag on
  the parent) with the expected GenAI attributes. Use in-memory `ActivityListener`.
- *Edge case:* unknown event subtypes are silently ignored (default branch in
  the switch); no exceptions propagate.

**Verification:**
- `dotnet test lib/CopilotSdk.OpenTelemetry.Tests` — all green, coverage on the
  new branches.

---

- [ ] **Unit 4: End-to-end regression test**

**Goal:** Lock in the behavior with an integration test that drives the full
`MessageHandler` → `IDiscordEventHandler` → `CopilotSessionTelemetry` path end
to end against a fake Copilot client/session and asserts the resulting Activity
tree.

**Requirements:** R1, R6, R7.

**Dependencies:** Unit 2A or 2B (whichever ships).

**Files:**
- Modify: `app/FunctionApp.Tests/ChatBot/DiscordGptIntegrationTests.cs` (extend
  the existing `SetupTracingMocks` helper to also drive a fake session event
  stream).
- Possibly create: `app/FunctionApp.Tests/Fakes/FakeCopilotClient.cs` and
  `FakeCopilotSession.cs` if the fakes don't already exist.

**Approach:**
- Use `ActivityListener` to capture the full Activity tree produced during a
  simulated turn.
- Assertions:
  1. One `chat.conversation` root, one `chat.turn` child, two `execute_tool`
     children of the turn (one for a synthetic tool call A, one for tool B).
  2. Tool A spans complete before tool B starts → spans nest correctly even with
     interleaved start/complete events for the same call ids.
  3. Concurrent tool calls (start A, start B, complete B, complete A) each get
     their own span, parented to the turn, with correct durations.
  4. A `ToolExecutionCompleteEvent` with `Success=false` produces a span with
     `Status.Code=Error` and the error message in `Status.Description`.
  5. After app shutdown, no Activities remain "running."

**Test scenarios:** *(see Approach — items 1-5 are the scenarios)*

**Verification:**
- `dotnet test app/FunctionApp.Tests --filter Category=Integration` — all green.

---

- [ ] **Unit 5: Documentation**

**Goal:** Update `lib/CopilotSdk.OpenTelemetry/README.md` to remove the
"tool-call spans not yet wired" caveat and document the chosen wiring path.

**Requirements:** none directly — documentation hygiene.

**Dependencies:** Unit 2A or 2B is shipped.

**Files:**
- Modify: `lib/CopilotSdk.OpenTelemetry/README.md`
- Modify: `README.md` (root) — update the link/description if it mentions the
  caveat.

**Approach:**
- Replace the "Limitations" section with a "How tool spans are attached"
  section explaining the chosen path. If Path A: document the
  `CopilotSessionLifecycleListener` and `AddCopilotSdkOpenTelemetry()` call.
  If Path B: document the `OnSessionCreated` hook and that consumers using
  this lib outside Discord wire it themselves.

**Test scenarios:**
- *Test expectation: none — documentation only.*

**Verification:**
- README renders cleanly on GitHub. No broken links.

---

- [ ] **Unit 6: Spike cleanup**

**Goal:** Delete the `lib/CopilotSdk.OpenTelemetry.Spike` project once the chosen
path is shipped.

**Requirements:** none directly — repo hygiene.

**Dependencies:** Unit 2A or 2B is shipped *and* the spike findings are
preserved (in a checkpoint or in this plan's "Spike Findings" addendum).

**Files:**
- Delete: `lib/CopilotSdk.OpenTelemetry.Spike/` (whole directory)
- Modify: `FRCDiscordBot.slnx` (remove the spike project reference)

**Test scenarios:**
- *Test expectation: none — file removal only.*

**Verification:**
- `dotnet build FRCDiscordBot.slnx` succeeds.

## System-Wide Impact

- **Interaction graph:** New listener (Path A) or new hook (Path B) attaches to
  the singleton `CopilotClient`'s event stream. No other component is affected.
  The per-turn `Activity` already exists — both paths consume it, neither
  modifies it.
- **Error propagation:** All listener / hook code is wrapped in
  try/catch + log-and-swallow. A bug in the telemetry pipeline must never break
  a Discord turn. Existing turn-level error propagation is unchanged.
- **State lifecycle risks:** Per-session subscriptions are kept in a
  `ConcurrentDictionary`. The risk is leaking subscriptions if `session_ended`
  is missed. Mitigation: dispose all on app shutdown; consider a periodic sweep
  if Path A's lifecycle events prove unreliable (defer until evidence shows
  it's needed).
- **API surface parity:** The `CopilotSessionTelemetry.Subscribe` API is
  unchanged. `AddCopilotSdkOpenTelemetry()` extension method gains an internal
  registration of the listener (Path A) or options configurator (Path B). The
  public surface of the library is unchanged.
- **Integration coverage:** Unit 4 covers the cross-component interaction.
- **Unchanged invariants:** `chat.conversation` and `chat.turn` span shape,
  parent/child relationship, `IConversationTraceContextStore` persistence,
  `/chat reset` behavior, all existing 73 FunctionApp + 15 lib tests.

## Risks & Dependencies

| Risk | Mitigation |
|------|------------|
| Spike results are inconclusive (lifecycle event timing varies, sometimes fires before/sometimes after first tool event) | Treat as a Path A failure and use Path B. Document the timing observation in the Spike Findings addendum so future SDK versions can be re-evaluated. |
| `ResumeSessionAsync` has hidden side effects on the original session (e.g., resets state, breaks streaming) | Spike covers this — observe the prompt's response correctness with and without the resume call. Failure mode: Path B. |
| `Activity.Current` is null in the lifecycle callback (Path A) — spans become parentless | Capture per-turn `Activity` via `AsyncLocal<Activity?>` set inside `BeginTurnAsync`'s scope and read by the listener. Verified during spike. |
| Multiple tool calls executing concurrently in the same turn produce mis-parented spans | `CopilotSessionTelemetry` already keys in-flight tool activities by `ToolCallId` in a `ConcurrentDictionary`. Unit 4 explicitly tests this. |
| `gpt/` submodule changes (Path B) require a separate PR + submodule bump, slowing down delivery | Acceptable cost — the `gpt/` change is two lines, easily merged. The bot-side wiring is the larger PR. |
| SDK upgrade (`GitHub.Copilot.SDK` 0.2.2 → future) breaks the lifecycle event contract | Tests in Unit 2A use synthetic events and don't lock to the SDK's internals. A real-flow smoke test (manual) before merging the SDK bump catches behavioral changes. |

## Documentation / Operational Notes

- After ship: take one screenshot of an App Insights end-to-end transaction
  view showing `chat.conversation > chat.turn > execute_tool` and add it to the
  lib README.
- No new configuration required — `AddCopilotSdkOpenTelemetry()` already exists
  and is the single entry point.
- No new secrets, no new infrastructure, no rollout coordination.

## Sources & References

- Origin: this session's user request ("plan how to implement tool-call sub-spans").
- Related code:
  - `lib/CopilotSdk.OpenTelemetry/CopilotSessionTelemetry.cs`
  - `services/ChatBot/MessageHandler.cs`
  - `gpt/src/BC3Technologies.DiscordGpt.Copilot/GitHubCopilotPromptHarness.cs`
- Related plans:
  - `docs/plans/2026-04-17-001-refactor-copilot-chat-orchestration-plan.md`
  - `docs/plans/2026-04-20-001-refactor-replace-chatbot-with-discordgpt-plan.md`
- Related work: `feat/telemetry` branch, commits `01590f1, ac829e8, 8deddb2, 65c14ff, 62bcb46`.
- External: <https://opentelemetry.io/docs/specs/semconv/gen-ai/gen-ai-spans/>
- Prior art: <https://github.com/brandonh-msft/agentframework-opentelemetry>


---

## Spike Findings (Unit 1) — 2026-04-22

Live run against `CopilotClient` 0.2.2 + GitHub Models, full output in `lib/CopilotSdk.OpenTelemetry.Spike/bin/Debug/net10.0/spike-findings.md` (preserved during Unit 1 only).


### Decision-gate results

| Gate | Result | Notes |
|------|--------|-------|
| **G1** — `client.On(SessionLifecycleEvent)` fires `session.created` for sessions opened via the same client's `CreateSessionAsync` | ✅ YES | Fires `~225ms` after `CreateSessionAsync` returns (t+2278 → t+2505). Type string is `session.created` (not `session_started` as plan guessed). |
| **G2** — `ResumeSessionAsync` returns a session whose subscription delivers the *live* event stream | ✅ YES | Both `ResumeSessionConfig.OnEvent` (atomic) and post-resume `session.On()` delivered the full stream: `SessionCustomAgentsUpdatedEvent`, `SessionToolsUpdatedEvent`, `UserMessageEvent`, `AssistantTurnStartEvent`, `SessionUsageInfoEvent`, `AssistantTurnEndEvent`, `SessionErrorEvent`, `SessionIdleEvent`. |
| **G3** — Lifecycle fires before first tool / first assistant event | ✅ YES | Lifecycle `session.created` at t+2505. First assistant event (`AssistantTurnStartEvent`) at t+3753. ~1248 ms head start — ample to subscribe before any tool span needs to be emitted. |
| **G4** — `Activity.Current` is preserved inside lifecycle callback | ✅ YES | `Activity.Current` in lifecycle handler == calling `spike.turn` activity, even though callback runs on a different thread (thread 4 → thread 9). The SDK preserves `AsyncLocal<T>`. Activity is also preserved on every event delivery via the resumed handle (across threads 4, 6, 9, 11). |

### Important nuances

- **`ResumeSessionConfig` requires `Provider` and `OnPermissionRequest`** (mirroring create-time config). The lifecycle listener must capture the same `ProviderConfig` used at `CreateSessionAsync` to call resume successfully. In production, this means our DI registration needs access to `IOptions<DiscordGptOptions>` (or whatever holds the provider/key) and likely cooperation with `gpt/` for the provider config — **OR** we let the existing `Provider` config flow naturally because the registered `CopilotClient` already has `GitHubToken` and the SDK may accept a minimal `ResumeSessionConfig` once the create-side identity is established. **Implementation-time question**: re-test resume with only `OnPermissionRequest = ApproveAll` once we are inside the production app where the underlying CLI is authenticated via `GitHubToken` (not via per-session Provider).
- **`ResumeSessionConfig.OnEvent`** is the atomic subscription — preferred over post-resume `session.On()` because it eliminates the small window between resume completion and subscribe.
- **Startup event gap.** Between `CreateSessionAsync` return (t+2278) and lifecycle firing (t+2505), a couple of bookkeeping events are emitted on the primary handle (`SessionStartEvent`, `PendingMessagesModifiedEvent`) that the resumed handle does not see. **None are tool / assistant events**, so this gap is irrelevant for tool span emission.
- **Threading.** SDK invokes handlers on threadpool threads (observed: 4, 6, 9, 10, 11). All handlers received the correct `Activity.Current`. The translator's existing `ConcurrentDictionary<string, Activity>` keyed on `ToolCallId` already handles concurrency correctly.

### Recommended path: **Path A (zero-touch lifecycle/resume)**

Proceed to **Unit 2A**:
- New `CopilotSessionLifecycleListener : IHostedService` in `lib/CopilotSdk.OpenTelemetry/`
- `StartAsync` → `_client.On(lifecycle => …)` returning the subscription handle (stored, disposed in `StopAsync`)
- On `session.created` → `_client.ResumeSessionAsync(evt.SessionId, new ResumeSessionConfig { OnPermissionRequest = ApproveAll, OnEvent = e => _translator.Handle(e) })` — passing `OnEvent` is preferred over a post-resume `.On()` call (atomic, no race window).
- `CopilotSessionTelemetry` exposes a per-session entry-point (`HandleEvent(SessionEvent)` against an internal `ConcurrentDictionary<sessionId, perSessionState>`), seeded by the lifecycle listener with `(sessionId, Activity.Current)` at lifecycle-callback time.
- **No changes to `gpt/`.** The existing `session.On(...)` subscription in `GitHubCopilotPromptHarness` continues unchanged (it powers progress dispatch + tool-call counting). Path A's resumed-handle subscription runs in parallel.

### Unresolved question for Unit 2A

Whether `ResumeSessionConfig` strictly requires `Provider`/`Model` again in the **production** wiring (where `CopilotClient` is constructed with `GitHubToken` rather than a session-level `Provider`). Two options:
1. Resume succeeds with just `OnPermissionRequest` + `OnEvent` (because the CLI knows the session and its provider config). **Test this first.**
2. Resume requires the original `Provider`. We'd need the listener to capture the create-time `SessionConfig.Provider` somehow — possibly by also subscribing to `client.On("session.updated")` to catch metadata, or by intercepting `CreateSessionAsync` calls (which would re-introduce coupling to `gpt/`). **Acceptable fallback**: register an `IConfigureOptions<DiscordGptOptions>` style hook that knows the provider config (it already lives in `gpt/` DI), and pass it explicitly to the listener.
