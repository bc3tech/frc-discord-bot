# CopilotSdk.OpenTelemetry

OpenTelemetry GenAI tracing for the [GitHub Copilot SDK](https://www.nuget.org/packages/GitHub.Copilot.SDK).
Persists a per-conversation **root span** across turns so multi-turn agent interactions roll up
into a single Trace in Application Insights (or any OTel backend) — even when each turn runs
in a separate process / Function invocation / pod.

## Why

Out of the box, every `CopilotSession.SendAndWaitAsync(...)` call produces an isolated trace.
A multi-turn conversation between a user and an agent ends up scattered across N unrelated
traces — there's no way to view "the whole conversation" in App Insights, no way to attribute
tool-call latency to a specific user, no way to follow handoffs end-to-end.

This library fixes that by:

1. Opening a zero-duration **conversation root span** the first time it sees a conversation id.
2. Persisting that root span's `ActivityContext` (W3C trace id + span id) via a pluggable
   `IConversationTraceContextStore`.
3. On every subsequent turn, opening a **turn span** parented to that persisted root —
   automatically reusing the same `trace_id`.
4. Translating the GitHub Copilot SDK's `SessionEvent` stream into OTel GenAI child spans
   (`execute_tool` for each tool call, `gen_ai.usage.*` tags on the turn span, etc.).

The result: **one trace per conversation**, top-level span = the conversation, child spans =
each turn, grandchild spans = each tool execution. Reset the conversation → next message
opens a brand-new trace.

## Install

```sh
dotnet add package CopilotSdk.OpenTelemetry
```

Targets `net10.0`. Depends on `OpenTelemetry.Api`, `Microsoft.Extensions.Logging.Abstractions`,
`Microsoft.Extensions.DependencyInjection.Abstractions`, and `GitHub.Copilot.SDK`.

## Quick start

### 1. Register services

```csharp
using CopilotSdk.OpenTelemetry;

services.AddCopilotSdkOpenTelemetry();
// optionally swap the in-memory store for a durable one (Tables, Redis, ...)
services.Replace(ServiceDescriptor.Singleton<IConversationTraceContextStore, MyDurableStore>());
```

### 2. Register the activity source with your tracer provider

```csharp
using OpenTelemetry.Trace;

builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddSource(CopilotSdkOpenTelemetry.ActivitySourceName)
        .AddAzureMonitorTraceExporter());          // or any OTLP exporter
```

### 3. Wrap each agent turn in `BeginTurnAsync`

```csharp
public sealed class MyChatHandler(IConversationTracer tracer, ICopilotClient copilot)
{
    public async Task RespondAsync(string conversationId, string userMessage, CancellationToken ct)
    {
        await using var turn = await tracer.BeginTurnAsync(conversationId, rootTags: null, ct);

        await using var session = await copilot.CreateSessionAsync(ct);
        using var telemetry = CopilotSessionTelemetry.Subscribe(session);

        await session.SendAndWaitAsync(userMessage, ct);
    }
}
```

Two turns called with the same `conversationId` will share a `trace_id`. Calling
`store.RemoveAsync(conversationId, ct)` (e.g. when the user resets their chat) starts a fresh
trace on the next turn.

## Span model (OTel GenAI conventions)

| Span name           | Kind     | Parent             | Key tags |
|---------------------|----------|--------------------|----------|
| `chat`              | Server   | (root)             | `gen_ai.system=github.copilot`, `gen_ai.operation.name=chat`, `gen_ai.conversation.id` |
| `chat`              | Server   | conversation root  | same — represents one turn |
| `execute_tool {name}` | Internal | current turn     | `gen_ai.operation.name=execute_tool`, `gen_ai.tool.name`, `gen_ai.tool.call.id` |

The conversation root span has zero duration (started and ended atomically) but its context is
persisted so child spans always link back to it. App Insights displays this as a single trace
with the conversation root at the top.

`AssistantUsageEvent`s tag the active turn span with `gen_ai.response.model`,
`gen_ai.usage.input_tokens`, and `gen_ai.usage.output_tokens`.
`SessionErrorEvent`s set the turn span's status to `Error` and tag `error.type`.

## Public API surface

- `CopilotSdkOpenTelemetry` — constants (activity source name, GenAI attribute names, operation names).
- `ConversationTraceContext(string TraceId, string SpanId, byte TraceFlags)` — persisted W3C
  context for a conversation root.
- `IConversationTraceContextStore` — `TryGetAsync` / `SetAsync` / `RemoveAsync`. Default
  implementation is `InMemoryConversationTraceContextStore`.
- `IConversationTracer.BeginTurnAsync(conversationId, rootTags, ct)` returns
  `IConversationTurnScope` (`IAsyncDisposable`, exposes the live `Activity`).
- `CopilotSessionTelemetry.Subscribe(CopilotSession, ILogger?)` — translates session events to
  OTel spans for the duration of the returned `IDisposable`.
- `IServiceCollection.AddCopilotSdkOpenTelemetry()` — registers the in-memory store + tracer.

## Persistence

The default `InMemoryConversationTraceContextStore` is process-local. For multi-instance
deployments (Functions, App Service, Kubernetes), implement `IConversationTraceContextStore`
against a durable backend — Azure Tables, Cosmos DB, Redis, etc. — and register it after
`AddCopilotSdkOpenTelemetry()`.

## License

MIT
