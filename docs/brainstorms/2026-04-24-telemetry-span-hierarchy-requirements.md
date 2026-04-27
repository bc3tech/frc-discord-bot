---
date: 2026-04-24
topic: telemetry-span-hierarchy
---

# Telemetry Span Hierarchy Overhaul

## Problem Frame

The `CopilotSdk.OpenTelemetry` library produces Application Insights traces that are flat, mislabeled, and missing key spans. A conversation in App Insights shows:

- Root and turn spans both named `chat` with `ActivityKind.Server` — App Insights renders them identically as "Request chat"
- Tool calls appear as orphaned root-level spans instead of children of their turn
- No agent/sub-agent spans are emitted despite the SDK firing `SubagentStartedEvent`, `SubagentCompletedEvent`, etc.
- Tool spans carry no useful attributes about what was called (no MCP server name, no tool arguments, no external HTTP dependency correlation)
- No `conversation`-type span exists, so Azure Foundry's special conversation icon/design never activates

The library ships as a NuGet package consumed by the ChatBot service. All fixes must land in the library so consumers get correct telemetry by default.

## Target Span Hierarchy

```
conversation {conversation_id}          (Internal)  ← Azure Foundry renders with conversation icon
├── chat {model}                        (Client)    ← "Turn", renders as Dependency in App Insights
│   ├── invoke_agent {agent_name}       (Internal)  ← Sub-agent invocation
│   │   ├── execute_tool {tool_name}    (Internal)  ← Tool execution
│   │   │   └── (HTTP dependencies)     (Client)    ← Auto-parented by .NET HttpClient instrumentation
│   │   └── execute_tool {tool_name}    (Internal)
│   └── execute_tool {tool_name}        (Internal)  ← Tool outside agent scope
└── chat {model}                        (Client)    ← Second turn
    └── ...
```

## Requirements

**Span Naming & Kind**

- R1. The root span operation name MUST be `conversation` with `ActivityKind.Internal`. Azure Foundry recognizes this as a first-class conversation span type. The root span MUST have real duration (encompassing all child turns), not zero-duration-and-dispose as today.
- R2. Turn spans MUST use operation name `chat` with `ActivityKind.Client` per the GenAI OTel spec. Display name should be `chat {model}` when model is known.
- R3. Agent spans MUST use operation name `invoke_agent` with `ActivityKind.Internal`. Display name should be `invoke_agent {agent_name}`. (The SDK uses "Subagent" in event names, e.g., `SubagentStartedEvent`, but the span concept is "agent" per the GenAI OTel spec.)
- R4. Tool spans MUST use operation name `execute_tool` with `ActivityKind.Internal`. Display name should be `execute_tool {tool_name}`.

**Span Hierarchy & Parenting**

- R5. All turn spans MUST be children of the conversation span.
- R6. All tool and agent spans MUST be children of their respective turn span, not orphaned root spans. The current bug where `Activity.Current` is captured once at subscription time and becomes stale MUST be fixed. Note: the stale parent also affects `AssistantUsageEvent` and `SessionErrorEvent` handling — the fix is a **hierarchy-tracking** problem (conversation → turn → agent → tool), not just "capture the right turn."
- R7. Tool spans that occur within a sub-agent invocation MUST be children of that agent's span. Tool spans outside agent scope are direct children of the turn.
- R8. HTTP calls made during tool execution MUST auto-parent under the tool span via .NET's `AsyncLocal`-based activity propagation. No special wiring needed — fixing R6 should make this work automatically.

**Span Attributes (GenAI OTel Spec + Azure Foundry)**

- R9. All spans MUST carry `gen_ai.operation.name` matching the operation (`conversation`, `chat`, `invoke_agent`, `execute_tool`).
- R10. Turn (`chat`) spans MUST carry: `gen_ai.request.model`, `gen_ai.response.model`, `gen_ai.response.finish_reasons`, `gen_ai.provider.name` (replaces deprecated `gen_ai.system`), `server.address`.
- R11. Agent spans MUST carry: `gen_ai.agent.name`, `gen_ai.agent.id` (if available from SDK events).
- R12. Tool spans MUST carry: `gen_ai.tool.name`, `gen_ai.tool.call.id`. When the tool is an MCP tool, also include the MCP server name (attribute TBD during planning — `McpServerName` is available on `ToolExecutionStartEvent`).
- R13. The conversation span MUST carry `gen_ai.conversation.id`.
- R14. The deprecated `gen_ai.system` attribute MUST be replaced with `gen_ai.provider.name` on all spans where it is currently emitted (root, turn, and tool spans per `ConversationTracer.cs` and `CopilotSessionTelemetry.cs`).

**Token Usage**

- R15. Token usage from `AssistantUsageEvent` MUST be recorded as attributes on the turn span: `gen_ai.usage.input_tokens`, `gen_ai.usage.output_tokens`.
- R16. Token usage MUST also be emitted as OTel Metrics using the `gen_ai.client.token.usage` histogram, tagged with model and operation name.

**Sensitive Data Capture**

- R17. Prompt content (`gen_ai.content.prompt`) and completion content (`gen_ai.content.completion`) MUST be opt-in, disabled by default.
- R18. The opt-in mechanism should be a configuration flag (e.g., `RecordSensitiveData = true`) passed through the library's options/configuration.

**Library Self-Containment**

- R19. `ISessionEventSubscriber` (currently defined in `BC3Technologies.DiscordGpt.Copilot`) MUST be moved into the library so the library can own the full telemetry subscription lifecycle. The library MUST provide its own implementation that automatically wires up telemetry for all SDK events (tool, agent, usage, error).
- R20. Consumers MUST be able to get full telemetry by calling a single registration method (e.g., `services.AddCopilotSdkOpenTelemetry()`). No bridge classes or manual event subscription in the consuming app.

**Consumer Migration**

- R21. The ChatBot service's `CopilotTelemetrySessionSubscriber` bridge class becomes dead code and should be removed.

**SDK Event Coverage**

- R22. The library MUST handle these SDK events that are currently ignored: `SubagentStartedEvent`, `SubagentCompletedEvent`, `SubagentFailedEvent`, `SubagentSelectedEvent`.
- R23. `SubagentStartedEvent` → start an `invoke_agent` span. `SubagentCompletedEvent` / `SubagentFailedEvent` → end it (with error status on failure).
- R24. Tool events (`ToolExecutionStartEvent`, `ToolExecutionCompleteEvent`) must correctly parent under the active agent span when one exists, or under the turn span otherwise.
- R25. `SkillInvokedEvent` MUST be recorded as attributes or an OTel event on the active agent span (not a separate span). The GenAI OTel spec has no "skill" span type. Attributes to capture: skill name, plugin name, allowed tools list.

## Success Criteria

- A conversation in App Insights shows a tree matching the target hierarchy diagram — conversation root with turn children, agent spans under turns, tool spans under agents
- Azure Foundry renders the root span with its special conversation icon/design
- Tool spans show what was called (tool name, MCP server name) and HTTP dependencies nest underneath
- No orphaned root-level tool spans
- Token usage appears both as span attributes and as OTel metrics (the `gen_ai.client.token.usage` histogram is emitted with `model` and `operation.name` dimensions)
- Consuming app needs only `services.AddCopilotSdkOpenTelemetry()` — no manual subscriber wiring

## Scope Boundaries

- **In scope**: All changes to `lib/CopilotSdk.OpenTelemetry` (including pulling `ISessionEventSubscriber` into it), removal of bridge class from `services/ChatBot/Diagnostics/`, OTel Metrics for token usage
- **Out of scope**: KQL dashboard migration (existing queries will break when turns move to `dependencies` table — that's a known, accepted consequence). **Deployment risk**: any existing alerts built on `requests | where name == "chat"` will silently stop firing on deploy day.
- **Out of scope**: Custom AI evaluator integration, cost/billing metrics, prompt caching telemetry
- **Out of scope**: Changes to the Copilot SDK itself (we work with the events it fires)

## Key Decisions

- **Turns as Dependencies**: Turn spans use `ActivityKind.Client`, moving them from `requests` to `dependencies` in App Insights. This is correct per GenAI OTel spec and Azure Foundry conventions. KQL migration is accepted as a known consequence.
- **Library owns telemetry**: `ISessionEventSubscriber` moves into the library. The library provides the implementation and handles all event-to-span mapping. Consumers don't wire up subscribers.
- **Sensitive data opt-in**: Prompt/completion content capture gated behind a configuration flag, disabled by default.
- **gen_ai.system → gen_ai.provider.name**: Migrate to the non-deprecated attribute name.
- **Skill events as attributes**: `SkillInvokedEvent` recorded as attributes/event on the active agent span, not a separate span type. The GenAI OTel spec has no "skill" operation name.
- **OTel Metrics**: Token usage emitted as histograms alongside span attributes.

## Dependencies / Assumptions

- The Copilot SDK fires `SubagentStartedEvent`/`SubagentCompletedEvent`/`SubagentFailedEvent` with `AgentName` and `ToolCallId` properties (verified in `SessionDiagnosticsLogger.cs`)
- `ToolExecutionStartEvent` exposes `McpServerName`, `McpToolName`, `ParentToolCallId` (verified)
- .NET `AsyncLocal`-based `Activity.Current` propagation will automatically parent HTTP dependency spans under tool spans once the parent capture bug is fixed (needs verification during planning)
- Azure Foundry recognizes `conversation` as a span operation name with special rendering (confirmed by user)

## Outstanding Questions

### Deferred to Planning

- [Affects R6][Technical] What is the best mechanism to fix the stale `Activity.Current` capture? With agent spans added, this becomes a hierarchy-tracking problem (conversation → turn → agent → tool), not just "capture the right turn." Options: (a) maintain a span stack that tracks the current active span at each level, (b) capture `Activity.Current` lazily in each event handler, (c) pass the turn `Activity` through event state. The fix must also cover `AssistantUsageEvent` and `SessionErrorEvent` which use the same stale parent.
- [Affects R1][Technical] How should the conversation root span get real duration? Today it's created-and-immediately-disposed with zero duration. Options: (a) keep the Activity alive for the conversation lifetime, (b) reconstruct it with calculated duration at conversation end, (c) use a different persistence strategy. This affects `ConversationTracer`, `IConversationTraceContextStore`, and possibly the store's persistence contract.
- [Affects R12][Needs research] What attribute name should be used for MCP server name on tool spans? Check if Azure Foundry or GenAI spec defines one, otherwise use a custom attribute like `mcp.server.name`.
- [Affects R8][Needs verification] Confirm that fixing the parent capture bug is sufficient for HTTP dependency auto-parenting, or whether additional `Activity.Current` threading is needed for MCP/HTTP calls within tool execution.
- [Affects R16][Technical] What histogram bucket boundaries are appropriate for the `gen_ai.client.token.usage` metric? Check the GenAI OTel spec for recommended boundaries. Note: this is new metrics infrastructure — requires a `Meter` instance and `MeterProviderBuilder` registration alongside the existing `TracerProviderBuilder`.
- [Affects R19][Technical] What is the cleanest way to move `ISessionEventSubscriber` from `BC3Technologies.DiscordGpt.Copilot` into the library without breaking the existing `gpt` layer's other consumers of that interface?
- [Affects R3, R22][Needs research] Does `SubagentSelectedEvent` warrant its own span or just attributes on the agent span? It fires before `SubagentStartedEvent` and carries `Tools` list.

## Next Steps

→ `/ce-plan` for structured implementation planning
