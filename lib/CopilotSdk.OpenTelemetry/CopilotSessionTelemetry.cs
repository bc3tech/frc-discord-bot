namespace CopilotSdk.OpenTelemetry;

using System.Collections.Concurrent;
using System.Diagnostics;

using global::GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging;

/// <summary>
/// Subscribes to a <see cref="CopilotSession"/>'s event stream and translates the relevant events
/// into OpenTelemetry GenAI spans. Each tool execution becomes an <c>execute_tool</c> child span
/// of the currently-active turn span (which the caller is expected to have opened via
/// <see cref="IConversationTracer.BeginTurnAsync"/> immediately before creating the session).
/// </summary>
public static class CopilotSessionTelemetry
{
    /// <summary>
    /// Begin emitting OTel spans for events on <paramref name="session"/>. Returns an
    /// <see cref="IDisposable"/> that, when disposed, unsubscribes the listener and ends any
    /// in-flight tool spans.
    /// </summary>
    /// <param name="session">The Copilot session to instrument.</param>
    /// <param name="logger">Optional logger used to record non-fatal translator issues.</param>
    public static IDisposable Subscribe(CopilotSession session, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(session);

        var state = new TelemetryState(Activity.Current, logger);
        var subscription = session.On(@event => state.Handle(@event));
        return new Subscription(subscription, state);
    }

    private sealed class TelemetryState(Activity? parent, ILogger? logger)
    {
        private readonly Activity? _parent = parent;
        private readonly ILogger? _logger = logger;
        private readonly ConcurrentDictionary<string, Activity> _toolActivities = new(StringComparer.Ordinal);

        public void Handle(SessionEvent @event)
        {
            try
            {
                switch (@event)
                {
                    case ToolExecutionStartEvent start:
                        this.OnToolStart(start);
                        break;
                    case ToolExecutionCompleteEvent complete:
                        this.OnToolComplete(complete);
                        break;
                    case AssistantUsageEvent usage:
                        this.OnUsage(usage);
                        break;
                    case SessionErrorEvent error:
                        this.OnError(error);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                this._logger?.LogWarning(ex, "Copilot SDK telemetry translator failed for event type {EventType}", @event?.GetType().Name);
            }
        }

        public void DisposeAll()
        {
            foreach (var kvp in this._toolActivities)
            {
                kvp.Value.Dispose();
            }

            this._toolActivities.Clear();
        }

        private void OnToolStart(ToolExecutionStartEvent start)
        {
            var data = start.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            var name = !string.IsNullOrEmpty(data.ToolName) ? data.ToolName : "tool";
            var spanName = $"{CopilotSdkOpenTelemetry.Operations.ExecuteTool} {name}";
            var parentContext = this._parent?.Context ?? default;

            var activity = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                spanName,
                kind: ActivityKind.Internal,
                parentContext: parentContext,
                tags:
                [
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.System, CopilotSdkOpenTelemetry.GenAiSystemValue),
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.ExecuteTool),
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.ToolName, name),
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.ToolCallId, data.ToolCallId),
                ],
                links: null);

            if (activity is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.McpServerName))
            {
                activity.SetTag("gen_ai.tool.mcp.server_name", data.McpServerName);
            }

            if (!string.IsNullOrEmpty(data.McpToolName))
            {
                activity.SetTag("gen_ai.tool.mcp.tool_name", data.McpToolName);
            }

            this._toolActivities[data.ToolCallId] = activity;
        }

        private void OnToolComplete(ToolExecutionCompleteEvent complete)
        {
            var data = complete.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            if (!this._toolActivities.TryRemove(data.ToolCallId, out var activity))
            {
                return;
            }

            if (data.Success)
            {
                activity.SetStatus(ActivityStatusCode.Ok);
            }
            else
            {
                activity.SetStatus(ActivityStatusCode.Error, data.Error?.Message);
            }

            activity.Dispose();
        }

        private void OnUsage(AssistantUsageEvent usage)
        {
            var target = this._parent;
            if (target is null)
            {
                return;
            }

            var data = usage.Data;
            if (data is null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(data.Model))
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.ResponseModel, data.Model);
            }

            if (data.InputTokens is { } input)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageInputTokens, input);
            }

            if (data.OutputTokens is { } output)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageOutputTokens, output);
            }
        }

        private void OnError(SessionErrorEvent error)
        {
            var target = this._parent;
            if (target is null)
            {
                return;
            }

            var message = error.Data?.Message ?? "Copilot session error";
            target.SetStatus(ActivityStatusCode.Error, message);
            if (!string.IsNullOrEmpty(error.Data?.ErrorType))
            {
                target.SetTag("error.type", error.Data.ErrorType);
            }
        }
    }

    private sealed class Subscription(IDisposable inner, TelemetryState state) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this._disposed, 1) == 1)
            {
                return;
            }

            inner.Dispose();
            state.DisposeAll();
        }
    }
}
