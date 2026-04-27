---
date: 2026-04-24
topic: telemetry-overhaul
focus: OpenTelemetry span hierarchy, Azure Foundry/App Insights compatibility, CopilotSdk.OpenTelemetry library
---

# Ideation: Telemetry Overhaul for CopilotSdk.OpenTelemetry

## Codebase Context

**Project shape:** .NET 10 / C# Azure Functions bot on Azure Container Apps. Reusable NuGet library at `lib/CopilotSdk.OpenTelemetry` consumed by the bot and designed for external consumers. Library suite at `gpt/` provides transports, AI backends, MCP, and storage adapters.

**Current telemetry state:** The library implements a conversation→turn→tool-call span hierarchy using OTel GenAI semantic conventions. Root span (`chat`, Server kind) is zero-duration and persisted via `IConversationTraceContextStore`; turn spans parent to it; `execute_tool {name}` grandchild spans come from `CopilotSessionTelemetry.Subscribe()`. Token usage and errors tagged per GenAI conventions.

**What's broken (observed in App Insights):**
1. Top-level shows as "Request chat" — should be "Conversation" with Azure Foundry's special icon
2. Child items all show as "Request chat" — should be "Turn" spans
3. Tool calls (`execute_tool`) appear as separate root-level DEPENDENCY spans instead of children of turns
4. Tool telemetry lacks useful info (no external endpoint details, etc.)
5. No "Agent" spans logged anywhere

**Root causes identified:**
- Both root and turn spans use identical name `chat` with `ActivityKind.Server`
- `Activity.Current` captured at `Subscribe()` time (once per session), not at tool-execution time (per event) — parent link is stale/broken by the time tools fire
- `SubagentStarted/Completed/Failed` events handled by `SessionDiagnosticsLogger` (logs) but ignored by `CopilotSessionTelemetry` (spans)
- `CopilotSdkOpenTelemetry.Operations` only defines `Chat` and `ExecuteTool` — no `Conversation`, `InvokeAgent`, or `Turn` constants

**Reference patterns (from agentframework-telemetry project):**
- Span names: `{gen_ai.operation.name} {gen_ai.request.model}`
- Agent spans: `invoke_agent {name}` with `ActivityKind.Internal`
- Tool spans: `execute_tool {name}` as Internal children of model calls
- Rich attributes: `gen_ai.conversation.id`, `gen_ai.agent.name`, `gen_ai.tool.name`, `gen_ai.response.finish_reasons`, `server.address`

**Design decision:** Target Azure Foundry + App Insights conventions as the primary rendering target, PLUS all official OTel GenAI semantic conventions (which are likely a subset of what Azure supports). Azure Foundry recognizes `conversation` as a first-class span type with special icon/design in its trace viewer. All improvements should live in `CopilotSdk.OpenTelemetry` to ship out-of-band for all consumers.

**Past learnings:** `IConversationContextAccessor` with `AsyncLocal<ConversationContext?>` is the established pattern for threading context (conversation key, user, channel) through the pipeline. The `CopilotTelemetrySessionSubscriber` bridge class in `services/ChatBot/Diagnostics/` is a 5-line boilerplate that every consumer must replicate.

## Ranked Ideas

### 1. Fix the Span Hierarchy (Conversation → Turn → Agent → Tool)
**Description:** Ship as one coherent change in `CopilotSdk.OpenTelemetry`:
- (a) Rename root span from `chat` to `conversation` — Azure Foundry renders this with a special icon in its trace viewer. Add `Conversation` to `Operations` constants.
- (b) Fix the tool span parent capture bug: `CopilotSessionTelemetry.Subscribe()` captures `Activity.Current` at subscription time (line 31), but by tool-execution time the ambient parent has shifted or been disposed. Must capture the turn Activity lazily at execution time or propagate it through the event data.
- (c) Add `invoke_agent {name}` spans: Handle `SubagentStartedEvent`/`SubagentCompletedEvent`/`SubagentFailedEvent` in `CopilotSessionTelemetry.Handle()` — `SessionDiagnosticsLogger` already proves the event data is available (lines 90-121).
- (d) Fix `ActivityKind`: Root `conversation` → `Internal` (orchestration envelope). Turn `chat {model}` → `Client` (outgoing LLM call, renders as "Dependency" in App Insights). Agent `invoke_agent {name}` → `Internal`. Tool `execute_tool {name}` → `Internal`. Document the KQL migration: turn spans move from `requests` to `dependencies` table.
- (e) Add `gen_ai.response.finish_reasons` and `gen_ai.request.model` to turn spans (spec-required attributes, currently absent).
**Rationale:** Directly fixes all 5 reported App Insights issues. Both pragmatism and standards critics agreed this is the #1 priority. Tool parenting bug (#2b) is actively broken in production. Every NuGet consumer inherits the fix.
**Downsides:** Changing `Server` → `Client` for turns moves them from `requests` to `dependencies` in App Insights — existing KQL queries/alerts break. `gen_ai.request.model` can only be set late (on usage event) due to Copilot SDK event model limitations. Requires coordinated testing with Azure Foundry trace viewer.
**Confidence:** 95%
**Complexity:** Medium
**Status:** Explored

### 2. Move Telemetry Subscription into the Library
**Description:** Move the `CopilotTelemetrySessionSubscriber` bridge class from `services/ChatBot/Diagnostics/` into `CopilotSdk.OpenTelemetry`. The library should provide its own `ISessionEventSubscriber` implementation auto-registered by `AddCopilotSdkOpenTelemetry()`. Consumers get correct tool/agent span telemetry without discovering and replicating a 5-line adapter pattern.
**Rationale:** If this step is missed, tool spans either vanish or become orphaned root-level dependencies — which is exactly the current bug. Making it automatic eliminates a class of misconfiguration for all consumers.
**Downsides:** Couples the OTel library to the Copilot SDK's `ISessionEventSubscriber` interface. That coupling already exists implicitly via `CopilotSessionTelemetry`.
**Confidence:** 85%
**Complexity:** Low
**Status:** Unexplored

### 3. Rethink the Zero-Duration Conversation Root Span
**Description:** The current zero-duration `Server` root span is a phantom 0ms "Request" in App Insights that confuses operators. Options:
- (a) Change to `Internal` kind so it stops appearing as a top-level Request. Azure Foundry may still render it with the `conversation` icon if the operation name is correct.
- (b) Give it meaningful duration by keeping it open (updating its end time) on each turn — requires the persisted `ConversationTraceContext` to hold the Activity or re-open it.
- (c) Eliminate the root entirely and correlate turns via shared `gen_ai.conversation.id` attribute + KQL.
Option (a) is the safest. Option (c) loses multi-turn E2E transaction drill-down. Need to test what Azure Foundry actually renders for each option.
**Rationale:** The zero-duration Server span fights the Azure Functions host (which already creates the real Server span for the HTTP trigger) and produces a confusing 0ms "Request" entry. Standards critic confirmed this is actively misleading.
**Downsides:** Option (c) means no one-click multi-turn transaction view — need KQL `where gen_ai_conversation_id == 'X'`. Option (b) requires mutating a persisted span across function invocations, which is architecturally novel. Requires testing with Azure Foundry to see what it actually renders.
**Confidence:** 70%
**Complexity:** High
**Status:** Unexplored

### 4. Opt-in Sensitive Data Capture (Tool Arguments & Results)
**Description:** Add `CopilotSdkTelemetryOptions { RecordToolArguments, RecordToolResults }` in the library that gates attaching tool args/results as span events. The data is already available in `ToolExecutionStartData.Arguments` and `ToolExecutionCompleteData.Result` (proven by `SessionDiagnosticsLogger` lines 42-46, 73-77). Enforce 8KB max attribute value truncation to avoid silent App Insights ingestion clipping.
**Rationale:** Matches the OTel GenAI spec's Opt-In pattern exactly (`gen_ai.tool.call.arguments`, `gen_ai.tool.call.result`). When debugging wrong bot answers, operators need to see what was sent to tools and what came back — span events are directly correlated to tool spans unlike log entries that require manual correlation.
**Downsides:** Large payloads (web scraping, large API responses) truncated at 8KB. Must be off by default for PII safety. Adds an options class to configure.
**Confidence:** 75%
**Complexity:** Low
**Status:** Unexplored

### 5. OTel Metrics with GenAI Spec Names
**Description:** Add a Meter in `CopilotSdk.OpenTelemetry` emitting `gen_ai.client.token.usage` (Histogram by token type) and `gen_ai.client.operation.duration` (Histogram) using exact spec-defined metric names. The app layer already has a Meter pattern (`Constants.Telemetry.AppMeterName`); the library currently has zero metrics instrumentation.
**Rationale:** Traces answer "what happened in this conversation." Metrics answer "how is the system performing across all conversations?" Pre-aggregated metrics are cheaper to query than trace-derived KQL. The GenAI spec defines these as standard metrics. All NuGet consumers get metrics for free.
**Downsides:** At current scale, KQL over span attributes may be sufficient. Adds Meter lifecycle management. Must cache Histogram instruments (don't create per-call).
**Confidence:** 65%
**Complexity:** Medium
**Status:** Unexplored

## Rejection Summary

| # | Idea | Reason Rejected |
|---|------|-----------------|
| 1 | Rich tool span attributes (gen_ai.tool.type, server.address) | `gen_ai.tool.type` not in OTel spec; HTTP metadata belongs on auto-instrumented HTTP spans, not manually duplicated |
| 2 | Nested tool call hierarchy (ParentToolCallId) | Fabricates convention concepts; Activity parenting already captures parent→child naturally if context flows correctly |
| 3 | HTTP dependency correlation under tool spans | Free consequence of fixing tool span parent capture bug — not a separate work item |
| 4 | Thread IConversationContextAccessor into telemetry | Blanket enrichment adds noise to unrelated spans; current targeted rootTags approach is more correct |
| 5 | Conversation-level token accumulation | KQL `summarize` already does this; metric with conversation_id dimension hits cardinality limits |
| 6 | Computed cost metric (token × price) | Pricing belongs in BI/analytics layer, not instrumentation; stale prices worse than no prices |
| 7 | DiagnosticListener auto-parenting | SDK uses Rx-style `session.On()`, not DiagnosticSource — architecturally impossible without SDK changes |
| 8 | Merge SessionDiagnosticsLogger + CopilotSessionTelemetry | Couples independent OTel signals (logs vs traces); creates failure-mode coupling |
| 9 | AppContext switch for zero-config | Non-standard activation pattern; existing `AddCopilotSdkOpenTelemetry()` extension is the correct .NET/OTel idiom |
| 10 | Two ActivitySources | Azure Monitor doesn't support per-source sampling out of box; custom Sampler is complex for no concrete benefit |
| 11 | Promote TableConversationTraceContextStore to library | Adds Azure.Data.Tables hard dependency to core lib; violates minimal-dependency principle; current interface + app-level impl is correct layering |
| 12 | Detect unregistered ActivitySource at startup | Standard OTel behavior across all .NET libraries; inconsistent with ecosystem to special-case one library |
| 13 | Conventions micro-package | Already solved by existing public `CopilotSdkOpenTelemetry` constants class |
| 14 | Unified Directory.Packages.props | Off-topic — not a telemetry improvement |

## Session Log
- 2026-04-24: Initial ideation — 40 candidates generated across 5 frames (pain/friction, missing capabilities, inversion/automation, assumption-breaking, leverage/compounding), deduped to 22 unique, 5 survivors after two adversarial critique passes (pragmatism + OTel conventions/App Insights correctness). User feedback: target Azure Foundry + App Insights conventions (superset of OTel spec), keep everything in CopilotSdk.OpenTelemetry library for out-of-band shipping. `conversation` IS a recognized Azure Foundry span type with special rendering.
