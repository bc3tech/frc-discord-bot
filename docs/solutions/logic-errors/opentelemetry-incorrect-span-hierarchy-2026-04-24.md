---
title: OpenTelemetry span hierarchy broken in CopilotSdk — flat spans, missing agents, wrong operation names
date: 2026-04-24
last_updated: 2026-04-24
category: logic-errors
module: CopilotSdk.OpenTelemetry
problem_type: logic_error
component: tooling
symptoms:
  - "Top-level spans labeled 'chat' instead of 'conversation' — loses Azure Foundry conversation icon"
  - "Chat turns not appearing as separate child spans under conversation"
  - "Tool call spans created as independent root spans instead of parented under turns"
  - "Tool telemetry missing GenAI OTel semantic convention attributes"
  - "No invoke_agent spans logged at all"
  - "Parallel tool calls cascade as deeply nested parent-child chains instead of siblings"
root_cause: logic_error
resolution_type: code_fix
severity: high
tags:
  - opentelemetry
  - application-insights
  - span-hierarchy
  - genai-otel
  - activity-source
  - azure-foundry
  - copilot-sdk
  - parallel-tools
  - activity-current
---

# OpenTelemetry span hierarchy broken in CopilotSdk — flat spans, missing agents, wrong operation names

## Problem

The `CopilotSdk.OpenTelemetry` library produced malformed telemetry spans in Azure Application Insights. Spans were flat, mislabeled, and missing attributes, preventing operators from visualizing multi-turn conversations or debugging tool invocations in App Insights trace views.

## Symptoms

- Top-level span shows as "chat" instead of "conversation" — Azure Foundry uses `conversation` as a recognized span type with a distinct icon in trace diagrams
- Individual chat turns don't appear as separate `chat` spans nested under the conversation
- Tool execution spans appear at the root level, disconnected from the turn that triggered them
- Tool spans lack GenAI semantic convention attributes (`gen_ai.tool.name`, `gen_ai.tool.call.id`, `gen_ai.provider.name`)
- Sub-agent invocations (`invoke_agent`) produce no spans at all
- All spans under a conversation share the same stale parent, even across different turns

## What Didn't Work

The prior implementation used a **static `CopilotSessionTelemetry` class** with three fundamental flaws:

1. **Captured `Activity.Current` at subscribe time** — stored the ambient Activity at handler registration and reused it as the parent for all subsequent spans. When `Activity.Current` changed between turns, the handler still referenced the stale original, causing every span to share the same incorrect parent.
2. **Handled only 4 of ~10 SDK events** — missed tool execution, agent invocation, skill invocation, and error events entirely.
3. **No explicit parent context passing** — relied on `Activity.Current` being correctly set by the caller, which doesn't propagate reliably across async boundaries in the Copilot SDK's event model.

## Solution

Two new patterns replace the static class:

### 1. Two-step conversation API (`ConversationTracer`)

`IConversationTracer` provides explicit control over conversation and turn spans:

```csharp
// Step 1: Create or resume a conversation span (persists trace context)
await using IConversationScope conversation = await conversationTracer
    .CreateOrResumeConversationAsync(conversationId, rootTags, cancellationToken);

// Step 2: Begin a per-turn chat span (automatically parented under conversation)
await using IConversationTurnScope turn = conversationTracer.BeginTurn(conversationId);
```

Key mechanism — **detach/restore pattern** prevents accidental parenting to ambient traces:

```csharp
Activity? previous = Activity.Current;
Activity.Current = null;  // Detach ambient context

Activity? conversation = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
    CopilotSdkOpenTelemetry.Operations.Conversation,
    kind: ActivityKind.Internal,
    parentContext: default,  // Explicitly root — no ambient parent
    tags: [...]);

// Restore on dispose via ConversationScope
```

A `ConcurrentDictionary<string, Activity>` tracks live conversations so `BeginTurn` can look up the correct parent by conversation ID rather than relying on `Activity.Current`:

```csharp
if (_liveConversations.TryGetValue(conversationId, out Activity? conversationActivity))
{
    turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
        CopilotSdkOpenTelemetry.Operations.Chat,
        kind: ActivityKind.Client,
        parentContext: conversationActivity.Context,  // Explicit parent
        tags: turnTags,
        links: null);
}
```

### 2. Event-driven hierarchical subscriber (`TelemetrySessionSubscriber`)

Subscribes to *all* Copilot SDK events and translates them into correctly-parented spans:

- `SubagentStartedEvent` → `invoke_agent` span with `gen_ai.agent.name`
- `ToolExecutionStartEvent` → `execute_tool` span parented under current agent (or turn)
- `AssistantUsageEvent` → token count attributes + model name on turn span
- `SessionErrorEvent` → error status on current span

Tool spans use explicit parent context from the maintained `_currentAgentActivity` field, with `Activity.Current` restoration to prevent parallel tool cascading (see [Follow-up: Parallel Tool Cascading](#follow-up-parallel-tool-cascading) below):

```csharp
Activity? agentParent = _currentAgentActivity;
ActivityContext parentContext = agentParent?.Context ?? _turnParentContext;

Activity? previous = Activity.Current;
try
{
    Activity? activity = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
        spanName,
        kind: ActivityKind.Internal,
        parentContext: parentContext,  // Never default — always explicit
        tags: tags,
        links: null);
    // ... set tags, store in _activeToolActivities ...
}
finally
{
    Activity.Current = previous;  // Prevent parallel tool cascade
}
```

### Target span hierarchy

```
conversation {conversation_id}    (Internal) ← Azure Foundry conversation icon
├── chat {model}                  (Client)   ← Dependency in App Insights
│   ├── invoke_agent {name}       (Internal)
│   │   ├── execute_tool {name}   (Internal)
│   │   │   └── (HTTP deps)       (Client)   ← Auto-parented via AsyncLocal
│   │   └── execute_tool {name}   (Internal)
│   └── execute_tool {name}       (Internal)
└── chat {model}                  (Client)
```

### DI registration

Consumers need only a single call:

```csharp
services.AddCopilotSdkOpenTelemetry();

// Or with sensitive data recording:
services.AddCopilotSdkOpenTelemetry(opts => opts.RecordSensitiveData = true);
```

## Follow-up: Parallel Tool Cascading

After the initial hierarchy fix, a second bug surfaced: when the SDK makes parallel tool calls (common for data-heavy queries), tool spans cascade as deeply nested parent-child chains instead of appearing as siblings.

### Cause

`ActivitySource.StartActivity()` treats `parentContext: default` as "use `Activity.Current` as parent," then **sets `Activity.Current`** to the newly created activity. For parallel tool starts:

1. `OnToolStart(Tool A)` → `Activity.Current` = Tool A (parent = turn ✓)
2. `OnToolStart(Tool B)` → `Activity.Current` = Tool B, **parent = Tool A** ✗
3. `OnToolStart(Tool C)` → `Activity.Current` = Tool C, **parent = Tool B** ✗

Result: A → B → C cascading chain instead of flat siblings.

### Fix (commit `d5a1d41`)

1. **Capture turn context at state construction** — `_turnParentContext` preserves the turn span context before any tool/agent activities start:
   ```csharp
   private readonly ActivityContext _turnParentContext = Activity.Current?.Context ?? default;
   ```

2. **Always use explicit parentContext** — never fall back to `default`:
   ```csharp
   ActivityContext parentContext = agentParent?.Context ?? _turnParentContext;
   ```

3. **Restore `Activity.Current` via try/finally** — prevents each handler from mutating the ambient context for the next handler (see code example in Solution section above).

The same pattern applies to `OnSubagentStarted`.

### Known limitation

HTTP calls during tool execution still parent under the turn span rather than their specific tool span. The subscriber only observes events — it cannot inject `Activity.Current` into the SDK's internal async execution context. Fixing this requires instrumenting the tool execution layer inside the Copilot SDK itself.

## Why This Works

The root causes were **stale parent context** (initial fix) and **Activity.Current mutation during parallel operations** (follow-up fix). The combined solution addresses these through four mechanisms:

1. **Explicit parent contexts via ConcurrentDictionary** — live conversation Activities are stored by ID and looked up when creating child spans. Span parenting is deterministic regardless of ambient context state.
2. **Activity.Current isolation** — the detach/restore pattern ensures conversation spans are never accidentally parented to unrelated ambient traces from the Azure Functions host.
3. **Stateful event subscriber** — `TelemetrySessionSubscriber` maintains `_currentAgentActivity` and `_activeToolActivities` dictionaries reflecting the current logical hierarchy, so each event handler knows exactly which parent to use.
4. **Activity.Current restoration** — every event handler saves and restores `Activity.Current` via try/finally, ensuring parallel operations don't cascade through the implicit ambient context.

## Prevention

- **Test span hierarchy explicitly** — unit tests verify parent-child relationships via `ParentSpanId` assertions and attribute checks. The test suite includes 25+ scenarios covering conversation lifecycle, turn nesting, and tool/agent parenting.
- **Test parallel operations** — single-tool tests won't expose cascade bugs. Always test multiple concurrent tool starts and verify they are siblings, not a parent-child chain:
  ```csharp
  // Start 3 tools in sequence (simulating parallel SDK events)
  state.Handle(new ToolExecutionStartEvent { Data = new { ToolCallId = "a", ToolName = "ToolA" } });
  state.Handle(new ToolExecutionStartEvent { Data = new { ToolCallId = "b", ToolName = "ToolB" } });
  state.Handle(new ToolExecutionStartEvent { Data = new { ToolCallId = "c", ToolName = "ToolC" } });

  // All three should be siblings under the turn span
  Assert.Equal(turn.SpanId, toolA.ParentSpanId);
  Assert.Equal(turn.SpanId, toolB.ParentSpanId);
  Assert.Equal(turn.SpanId, toolC.ParentSpanId);
  ```
- **Always pass `parentContext` explicitly** — never rely on `Activity.Current` for span parenting in event-driven code:
  ```csharp
  // ✗ Fragile: relies on Activity.Current being correct
  var span = ActivitySource.StartActivity("operation");

  // ✓ Reliable: explicit parent
  var span = ActivitySource.StartActivity("operation",
      kind: ActivityKind.Internal,
      parentContext: parentActivity.Context);
  ```
- **Save and restore `Activity.Current` in callback handlers** — use try/finally to prevent ambient context mutation:
  ```csharp
  Activity? previous = Activity.Current;
  try
  {
      // Create span with explicit parent context
  }
  finally
  {
      Activity.Current = previous;
  }
  ```
- **Capture scope context immutably at entry** — store the ambient context once when entering a logical scope (e.g., `_turnParentContext` at `SessionState` construction), not at each span creation.
- **Use constants for GenAI attribute names** — `CopilotSdkOpenTelemetry.GenAiAttributes.*` prevents typos and drift from the OTel semantic conventions.
- **Validate trace context round-tripping** — if persisting trace context across restarts, test serialization/deserialization preserves `ActivityTraceId` and `ActivitySpanId`.

## Related Issues

- `docs/plans/2026-04-24-001-refactor-telemetry-span-hierarchy-plan.md` — 6-unit implementation plan
- `docs/brainstorms/2026-04-24-telemetry-span-hierarchy-requirements.md` — R1–R25 requirements spec
- `docs/ideation/2026-04-24-telemetry-overhaul-ideation.md` — ideation and idea ranking
- `docs/plans/2026-04-22-001-feat-copilot-tool-call-sub-spans-plan.md` — predecessor plan (superseded by this work)
- `docs/solutions/best-practices/conversation-scoped-copilot-sessions-2026-04-23.md` — related session management pattern
