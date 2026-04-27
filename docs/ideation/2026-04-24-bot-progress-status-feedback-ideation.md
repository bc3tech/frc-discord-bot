---
date: 2026-04-24
topic: bot-progress-status-feedback
focus: "Streaming progress/thinking status from Copilot SDK back to Discord during long-running responses"
---

# Ideation: Bot Progress & Status Feedback to Discord Users

## Codebase Context

**Project:** FRC Discord Bot (C#/.NET) — AI chatbot for FIRST Robotics team 2046 "Bear Metal" using GitHub Copilot SDK + Azure Foundry. Complex queries trigger multi-tool orchestration (StatBotics API, TBA API, etc.) taking 1–5 minutes.

**Key discovery:** A full progress infrastructure already exists but produces no visible output:

- `ConversationContext.OnProgress` callback wired in `DiscordGptEventHandler` (line 78)
- `TryCreateIntermediateProgressUpdate` (GitHubCopilotPromptHarness.cs:403) converts SDK events to `DiscordAgentProgressUpdate`
- `SendProgressUpdateAsync` → buffer → `SendProgressLineAsync` → Discord `SendMessageAsync` pipeline is fully built
- Separate Normal/Reasoning buffers with line-based emission and deduplication
- Typing indicator loop runs concurrently (`RunTypingIndicatorLoopAsync`)

**Why nothing reaches users:**

- `TryCreateIntermediateProgressUpdate` only maps 4 event types: `AssistantMessageDeltaEvent`, `AssistantReasoningDeltaEvent` (gated by `EmitReasoningProgress`), `AssistantIntentEvent`, `ToolExecutionProgressEvent`
- It does NOT map tool start/complete or agent start/complete events — despite these firing (confirmed by `TelemetrySessionSubscriber` and `SessionDiagnosticsLogger`)
- The SDK's `SendAndWaitAsync` may not produce delta/reasoning events for all models
- `EmitReasoningProgress` defaults to false

**Existing infrastructure to leverage:**

- `ISessionEventSubscriber` pattern — pluggable, DI-registered, multiple subscribers composable
- `TelemetrySessionSubscriber` already processes all tool/agent events with timing data
- `SessionDiagnosticsLogger` logs tool names, durations, success/failure
- `DiscordAgentProgressUpdate` record with `Message` and `Kind` (Normal/Reasoning)
- Discord.Net supports messages, embeds, reactions, edits

**Past learnings:** OpenTelemetry span hierarchy fix (2026-04-24) established the `_turnParentContext` pattern for correct span parenting under parallel tool execution — same events we'd use for progress.

## Ranked Ideas

### 1. Tool Execution Progress Subscriber
**Description:** Create a new `ISessionEventSubscriber` implementation that maps `ToolExecutionStartEvent` → "🔧 Querying StatBotics..." and `ToolExecutionCompleteEvent` → "✅ StatBotics completed (2.3s)" directly to the existing `OnProgress` pipeline. Uses the same subscriber pattern proven by `TelemetrySessionSubscriber`. ~50 lines of code.
**Rationale:** The events already fire (confirmed by telemetry and diagnostics subscribers). The progress pipeline already exists and works. This idea simply connects the two — maximum impact for minimum code. Users immediately see what tools are running during the 1–5 minute wait.
**Downsides:** Adds message volume to Discord channels; tool names may not be user-friendly without a display name mapping; parallel tools may produce rapid-fire messages.
**Confidence:** 95%
**Complexity:** Low
**Status:** Unexplored

### 2. Agent/Sub-agent Visibility
**Description:** Extend the same subscriber pattern to map `SubagentStartedEvent`/`SubagentCompletedEvent` to progress: "🤖 Reasoning Agent started" → "🤖 Reasoning Agent completed (3 tools, 4.2s)". Agent display names are available in event data (`AgentDisplayName`).
**Rationale:** Trivial extension of Idea #1. Shows higher-level orchestration activity that helps users understand the bot is delegating to specialized agents, not stuck. Same proven subscriber + progress pipeline.
**Downsides:** Agent names may not be meaningful to end users; adds noise if agents complete quickly.
**Confidence:** 90%
**Complexity:** Low
**Status:** Unexplored

### 3. Configurable Progress Verbosity
**Description:** Add `EmitToolProgress` option to `DiscordGptOptions` (paralleling existing `EmitReasoningProgress`). Default true in development, configurable per deployment. Future extension: per-channel toggle via Discord command or channel-name convention (e.g., `#debug-bot` gets full verbosity).
**Rationale:** Prevents progress spam in production user channels while keeping full visibility in debug/ops channels. Follows the established pattern of `EmitReasoningProgress` for gating progress emission.
**Downsides:** Another config knob; if defaulted to off, defeats the purpose; per-channel logic adds complexity if pursued later.
**Confidence:** 85%
**Complexity:** Low
**Status:** Unexplored

### 4. Post-Response Tool Summary
**Description:** After the final response, append a small footer: "📊 5 tools in 23s | gpt-5.4-nano | 1.2K→3.4K tokens". Aggregate tool count, total duration, model name, and token usage from session events. Toggleable via config.
**Rationale:** Retrospective transparency without real-time noise. Explains why a response took long. Helps operators and power users debug slow queries. Data is already captured by telemetry subscriber.
**Downsides:** Requires aggregating data across the entire session turn; model name and token data come from different events; formatting must respect Discord's 2000-char limit.
**Confidence:** 80%
**Complexity:** Medium
**Status:** Unexplored

### 5. Token Usage in Summary
**Description:** Fold `AssistantUsageEvent` data (input tokens, output tokens, cached tokens) into the post-response footer from Idea #4. The event already fires and is captured by `TelemetrySessionSubscriber.OnUsage()`.
**Rationale:** Near-zero incremental cost over #4. Shows cache efficiency and model cost. Validates the model configuration work done earlier in this session.
**Downsides:** May not be meaningful to typical FRC team users; token counts are model-specific.
**Confidence:** 85%
**Complexity:** Low (merges with #4)
**Status:** Unexplored

### 6. Timeout Awareness & Proactive Warning
**Description:** At 80% of `ResponseTimeout` elapsed, emit "⏱️ Still processing... timeout in 60s". On actual timeout, send a context-rich error card instead of silent `TimeoutException`. Include what tools were in progress and link to logs if available.
**Rationale:** The user hit this exact issue during this session — a 60s timeout with zero feedback. Proactive warnings set expectations; rich error cards on timeout convert hard failures into diagnosable incidents.
**Downsides:** Timer wrapping around `SendAndWaitAsync` adds complexity; partial response capture is tricky; risk of false alarms if timeout is close to normal completion time.
**Confidence:** 70%
**Complexity:** Medium
**Status:** Unexplored

## Rejection Summary

| # | Idea | Reason Rejected |
|---|------|-----------------|
| 1 | Single Evolving Status Embed | Message edit tracking + failure recovery complexity outweighs benefit vs. simple line emission |
| 2 | Reaction-based Status Indicators | Discord reactions are high-latency; emoji rotation doesn't convey progress for 1–5min queries |
| 3 | Fallback Heartbeat When Events Stall | Crutch — events already fire; silence means wiring bug that Ideas #1/#2 fix directly |
| 4 | Per-User/Channel Reasoning Toggle | Premature — reasoning not enabled by default; fix tool progress first |
| 5 | Reasoning Phase as Collapsible Spoiler | SDK may not emit reasoning events; rendering premature before enabling works |
| 6 | Tool Result Previews | Tool results are raw JSON; parsing creates fragile/misleading summaries |
| 7 | Breadcrumb Trail (Edit Single Message) | Same cost as evolving embed with worse UX |
| 8 | Error Injection Drill Mode | Niche ops feature; solve core problem first |
| 9 | Conversation Replay Log | Ops/audit, not user-facing; telemetry already captures this |
| 10 | Adaptive Tool Execution | Architecture change, not progress visibility |
| 11 | Per-Tool Timeout with Circuit Breaking | Resilience feature, not progress signaling; different problem |
| 12 | Multi-Channel Queue with Notifications | Queue management, not progress feedback |
| 13 | Session Checkpoint/Fast Resume | Session management, not progress |
| 14 | Message Chunking for Long Responses | Response formatting, orthogonal concern |
| 15 | Visual Progress Bars with ETA | Requires historical latency data; high complexity for low accuracy |
| 16 | Compact JSON Event Log | Too low-level for end users |
| 17 | Token Usage Streaming Meter (real-time) | Noisy for end users; ops have OTel; folded into post-response summary instead |

## Session Log
- 2026-04-24: Initial ideation — 48 raw ideas generated across 6 frames, deduped to ~25, 6 survivors after adversarial filtering
