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
public static partial class CopilotSessionTelemetry
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Copilot SDK telemetry translator failed for event type {EventType}")]
    private static partial void LogTranslatorFailed(ILogger logger, Exception exception, string? eventType);

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
        IDisposable subscription = session.On(state.Handle);
        return new Subscription(subscription, state);
    }

    /// <summary>
    /// Creates a translator state instance for testing that can be driven with synthetic
    /// <see cref="SessionEvent"/> values without needing a real <see cref="CopilotSession"/>.
    /// </summary>
    internal static TelemetryState CreateForTesting(Activity? parent, ILogger? logger = null) =>
        new(parent, logger);

    internal sealed class TelemetryState(Activity? parent, ILogger? logger)
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
                        OnToolStart(start);
                        break;
                    case ToolExecutionCompleteEvent complete:
                        OnToolComplete(complete);
                        break;
                    case AssistantUsageEvent usage:
                        OnUsage(usage);
                        break;
                    case SessionErrorEvent error:
                        OnError(error);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                if (_logger is not null)
                {
                    LogTranslatorFailed(_logger, ex, @event.GetType().Name);
                }
            }
        }

        public void DisposeAll()
        {
            foreach (KeyValuePair<string, Activity> kvp in _toolActivities)
            {
                kvp.Value.Dispose();
            }

            _toolActivities.Clear();
        }

        private void OnToolStart(ToolExecutionStartEvent start)
        {
            ToolExecutionStartData? data = start.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            var name = !string.IsNullOrEmpty(data.ToolName) ? data.ToolName : "tool";
            var spanName = $"{CopilotSdkOpenTelemetry.Operations.ExecuteTool} {name}";
            ActivityContext parentContext = _parent?.Context ?? default;

            Activity? activity = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
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

            _toolActivities[data.ToolCallId] = activity;
        }

        private void OnToolComplete(ToolExecutionCompleteEvent complete)
        {
            ToolExecutionCompleteData? data = complete.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            if (!_toolActivities.TryRemove(data.ToolCallId, out Activity? activity))
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
            Activity? target = _parent;
            if (target is null)
            {
                return;
            }

            AssistantUsageData? data = usage.Data;
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
            Activity? target = _parent;
            if (target is null)
            {
                return;
            }

            SessionErrorData data = error.Data;
            var message = data.Message;
            target.SetStatus(ActivityStatusCode.Error, string.IsNullOrEmpty(message) ? "Copilot session error" : message);
            if (!string.IsNullOrEmpty(data.ErrorType))
            {
                target.SetTag("error.type", data.ErrorType);
            }
        }
    }

    private sealed class Subscription(IDisposable inner, TelemetryState state) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }

            inner.Dispose();
            state.DisposeAll();
        }
    }
}
