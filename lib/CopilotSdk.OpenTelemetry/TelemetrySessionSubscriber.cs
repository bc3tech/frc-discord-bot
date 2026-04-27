namespace CopilotSdk.OpenTelemetry;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// <see cref="ISessionEventSubscriber"/> that translates Copilot SDK session events into
/// hierarchical OpenTelemetry spans following the GenAI semantic conventions.
/// </summary>
public sealed partial class TelemetrySessionSubscriber(
    IOptions<CopilotSdkOpenTelemetryOptions> options,
    ILogger<TelemetrySessionSubscriber> logger) : ISessionEventSubscriber
{
    /// <inheritdoc />
    public IDisposable Subscribe(CopilotSession session)
    {
        var state = new SessionState(options.Value, logger);
        IDisposable subscription = session.On(state.Handle);
        return new Subscription(subscription, state);
    }

    /// <summary>
    /// Creates a <see cref="SessionState"/> for unit testing without needing a real
    /// <see cref="CopilotSession"/>.
    /// </summary>
    internal static SessionState CreateStateForTesting(
        CopilotSdkOpenTelemetryOptions? options = null,
        ILogger? logger = null) =>
        new(options ?? new CopilotSdkOpenTelemetryOptions(), logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Copilot SDK telemetry translator failed for event type {EventType}")]
    private static partial void LogTranslatorFailed(ILogger logger, Exception exception, string? eventType);

    internal sealed record BufferedAgentSelection(string? AgentName, string? AgentDisplayName, string[]? Tools);

    internal sealed class SessionState(CopilotSdkOpenTelemetryOptions options, ILogger? logger)
    {
        private readonly CopilotSdkOpenTelemetryOptions _options = options;
        private readonly ILogger? _logger = logger;
        private readonly ConcurrentDictionary<string, Activity> _activeToolActivities = new(StringComparer.Ordinal);

        /// <summary>
        /// The activity context that was ambient when this session state was created (the turn/conversation span).
        /// Used as the explicit parent for tool and agent spans to prevent cascading when parallel
        /// tool starts each set <see cref="Activity.Current"/>.
        /// </summary>
        private readonly ActivityContext _turnParentContext = Activity.Current?.Context ?? default;

        private Activity? _currentAgentActivity;
        private BufferedAgentSelection? _bufferedAgentSelection;

        public void Handle(SessionEvent @event)
        {
            try
            {
                switch (@event)
                {
                    case SubagentSelectedEvent selected:
                        OnSubagentSelected(selected);
                        break;
                    case SubagentStartedEvent started:
                        OnSubagentStarted(started);
                        break;
                    case SubagentCompletedEvent completed:
                        OnSubagentCompleted(completed);
                        break;
                    case SubagentFailedEvent failed:
                        OnSubagentFailed(failed);
                        break;
                    case SubagentDeselectedEvent:
                        _bufferedAgentSelection = null;
                        break;
                    case ToolExecutionStartEvent toolStart:
                        OnToolStart(toolStart);
                        break;
                    case ToolExecutionCompleteEvent toolComplete:
                        OnToolComplete(toolComplete);
                        break;
                    case AssistantUsageEvent usage:
                        OnUsage(usage);
                        break;
                    case UserMessageEvent userMessage:
                        OnUserMessage(userMessage);
                        break;
                    case AssistantMessageEvent assistantMessage:
                        OnAssistantMessage(assistantMessage);
                        break;
                    case SkillInvokedEvent skill:
                        OnSkillInvoked(skill);
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
            foreach (KeyValuePair<string, Activity> kvp in _activeToolActivities)
            {
                kvp.Value.Dispose();
            }

            _activeToolActivities.Clear();

            Activity? agent = Interlocked.Exchange(ref _currentAgentActivity, null);
            agent?.Dispose();
        }

        private void OnSubagentSelected(SubagentSelectedEvent selected)
        {
            SubagentSelectedData? data = selected.Data;
            if (data is null)
            {
                return;
            }

            _bufferedAgentSelection = new BufferedAgentSelection(data.AgentName, data.AgentDisplayName, data.Tools);
        }

        private void OnSubagentStarted(SubagentStartedEvent started)
        {
            SubagentStartedData? data = started.Data;
            if (data is null)
            {
                return;
            }

            var agentName = !string.IsNullOrEmpty(data.AgentName) ? data.AgentName : "agent";
            var spanName = $"{CopilotSdkOpenTelemetry.Operations.InvokeAgent} {agentName}";

            Activity? previous = Activity.Current;
            try
            {
                Activity? activity = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                    spanName,
                    kind: ActivityKind.Internal,
                    parentContext: _turnParentContext,
                    tags:
                    [
                        new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.InvokeAgent),
                        new(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName, CopilotSdkOpenTelemetry.GenAiProviderValue),
                        new(CopilotSdkOpenTelemetry.GenAiAttributes.AgentName, agentName),
                    ],
                    links: null);

                if (activity is null)
                {
                    _bufferedAgentSelection = null;
                    return;
                }

                if (!string.IsNullOrEmpty(data.ToolCallId))
                {
                    activity.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.AgentId, data.ToolCallId);
                }

                BufferedAgentSelection? buffered = _bufferedAgentSelection;
                if (buffered is not null)
                {
                    ActivityTagsCollection eventTags = new()
                    {
                        ["agent.display_name"] = buffered.AgentDisplayName ?? string.Empty,
                        ["agent.tools"] = buffered.Tools is not null ? string.Join(",", buffered.Tools) : string.Empty,
                    };
                    activity.AddEvent(new ActivityEvent("agent.selected", tags: eventTags));
                    _bufferedAgentSelection = null;
                }

                _currentAgentActivity = activity;
            }
            finally
            {
                Activity.Current = previous;
            }
        }

        private void OnSubagentCompleted(SubagentCompletedEvent _)
        {
            Activity? agent = Interlocked.Exchange(ref _currentAgentActivity, null);
            if (agent is null)
            {
                return;
            }

            agent.SetStatus(ActivityStatusCode.Ok);
            agent.Dispose();
        }

        private void OnSubagentFailed(SubagentFailedEvent failed)
        {
            Activity? agent = Interlocked.Exchange(ref _currentAgentActivity, null);
            if (agent is null)
            {
                return;
            }

            string? errorMessage = failed.Data.Error;
            agent.SetStatus(ActivityStatusCode.Error, string.IsNullOrEmpty(errorMessage) ? "Sub-agent failed" : errorMessage);
            agent.Dispose();
        }

        private void OnToolStart(ToolExecutionStartEvent start)
        {
            ToolExecutionStartData? data = start.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            var toolName = !string.IsNullOrEmpty(data.ToolName) ? data.ToolName : "tool";
            var spanName = $"{CopilotSdkOpenTelemetry.Operations.ExecuteTool} {toolName}";

            KeyValuePair<string, object?>[] tags =
            [
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName, CopilotSdkOpenTelemetry.GenAiProviderValue),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.ExecuteTool),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ToolName, toolName),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ToolCallId, data.ToolCallId),
            ];

            Activity? agentParent = _currentAgentActivity;
            ActivityContext parentContext = agentParent?.Context ?? _turnParentContext;

            Activity? previous = Activity.Current;
            try
            {
                Activity? activity = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                    spanName,
                    kind: ActivityKind.Internal,
                    parentContext: parentContext,
                    tags: tags,
                    links: null);

                if (activity is null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(data.McpServerName))
                {
                    activity.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.McpServerName, data.McpServerName);
                }

                if (!string.IsNullOrEmpty(data.McpToolName))
                {
                    activity.SetTag("gen_ai.tool.mcp.tool_name", data.McpToolName);
                }

                _activeToolActivities[data.ToolCallId] = activity;
            }
            finally
            {
                Activity.Current = previous;
            }
        }

        private void OnToolComplete(ToolExecutionCompleteEvent complete)
        {
            ToolExecutionCompleteData? data = complete.Data;
            if (data is null || string.IsNullOrEmpty(data.ToolCallId))
            {
                return;
            }

            if (!_activeToolActivities.TryRemove(data.ToolCallId, out Activity? activity))
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

        private static void OnUsage(AssistantUsageEvent usage)
        {
            Activity? target = Activity.Current;
            if (target is null)
            {
                return;
            }

            AssistantUsageData? data = usage.Data;
            if (data is null)
            {
                return;
            }

            var model = data.Model;

            if (!string.IsNullOrEmpty(model))
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.RequestModel, model);
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.ResponseModel, model);
                target.DisplayName = $"{CopilotSdkOpenTelemetry.Operations.Chat} {model}";
            }

            if (data.InputTokens is { } input)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageInputTokens, input);
                RecordTokenMetric((long)input, "input", model);
            }

            if (data.OutputTokens is { } output)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageOutputTokens, output);
                RecordTokenMetric((long)output, "output", model);
            }

            if (data.CacheReadTokens is { } cacheRead)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageCacheReadTokens, cacheRead);
                RecordTokenMetric((long)cacheRead, "cache_read", model);
            }

            if (data.CacheWriteTokens is { } cacheWrite)
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.UsageCacheWriteTokens, cacheWrite);
                RecordTokenMetric((long)cacheWrite, "cache_write", model);
            }
        }

        private void OnUserMessage(UserMessageEvent userMessage)
        {
            if (!_options.RecordSensitiveData)
            {
                return;
            }

            Activity? target = Activity.Current;
            string? content = userMessage.Data.Content;
            if (target is not null && !string.IsNullOrEmpty(content))
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.ContentPrompt, content);
            }
        }

        private void OnAssistantMessage(AssistantMessageEvent assistantMessage)
        {
            if (!_options.RecordSensitiveData)
            {
                return;
            }

            Activity? target = Activity.Current;
            string? content = assistantMessage.Data.Content;
            if (target is not null && !string.IsNullOrEmpty(content))
            {
                target.SetTag(CopilotSdkOpenTelemetry.GenAiAttributes.ContentCompletion, content);
            }
        }

        private static void RecordTokenMetric(long count, string tokenType, string model)
        {
            TagList tags =
            [
                new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.Chat),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.RequestModel, model),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName, CopilotSdkOpenTelemetry.GenAiProviderValue),
                new("gen_ai.token.type", tokenType),
            ];
            CopilotSdkOpenTelemetry.TokenUsageHistogram.Record(count, tags);
        }

        private void OnSkillInvoked(SkillInvokedEvent skill)
        {
            SkillInvokedData? data = skill.Data;
            if (data is null)
            {
                return;
            }

            Activity? target = _currentAgentActivity ?? Activity.Current;
            if (target is null)
            {
                return;
            }

            ActivityTagsCollection eventTags = new()
            {
                ["skill.name"] = data.Name,
                ["skill.plugin_name"] = data.PluginName ?? string.Empty,
                ["skill.allowed_tools"] = data.AllowedTools is not null ? string.Join(",", data.AllowedTools) : string.Empty,
            };
            target.AddEvent(new ActivityEvent("gen_ai.skill.invoked", tags: eventTags));
        }

        private static void OnError(SessionErrorEvent error)
        {
            Activity? target = Activity.Current;
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

    private sealed class Subscription(IDisposable inner, SessionState state) : IDisposable
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
