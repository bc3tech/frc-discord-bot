namespace ChatBot.Agents;

using ChatBot.Agents.Models;

using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Text.Json;

internal sealed class AgentWorkflowBindings(
    ExecutorBinding foundryMessageRouter,
    ExecutorBinding foundryMessagesRouter,
    ExecutorBinding localMessageRouter,
    ExecutorBinding localMessagesRouter)
    : Tuple<ExecutorBinding, ExecutorBinding, ExecutorBinding, ExecutorBinding>(
        foundryMessageRouter,
        foundryMessagesRouter,
        localMessageRouter,
        localMessagesRouter)
{
    private static readonly Type[] s_routerSentMessageTypes = [typeof(ChatMessage), typeof(TurnToken)];
    private static readonly Type[] s_routerOutputTypes = [typeof(string)];

    public AgentWorkflowBindings(
        Func<ChatMessage, IWorkflowContext, CancellationToken, ValueTask> foundryMessageRouter,
        Func<IEnumerable<ChatMessage>, IWorkflowContext, CancellationToken, ValueTask> foundryMessagesRouter,
        Func<ChatMessage, IWorkflowContext, CancellationToken, ValueTask> localMessageRouter,
        Func<IEnumerable<ChatMessage>, IWorkflowContext, CancellationToken, ValueTask> localMessagesRouter)
        : this(
            new FunctionExecutor<ChatMessage>(
                id: "foundry-message-router",
                handlerAsync: foundryMessageRouter,
                sentMessageTypes: s_routerSentMessageTypes,
                outputTypes: s_routerOutputTypes,
                declareCrossRunShareable: true).BindExecutor(),
            new FunctionExecutor<IEnumerable<ChatMessage>>(
                id: "foundry-messages-router",
                handlerAsync: foundryMessagesRouter,
                sentMessageTypes: s_routerSentMessageTypes,
                outputTypes: s_routerOutputTypes,
                declareCrossRunShareable: true).BindExecutor(),
            new FunctionExecutor<ChatMessage>(
                id: "local-message-router",
                handlerAsync: localMessageRouter,
                sentMessageTypes: s_routerSentMessageTypes,
                outputTypes: s_routerOutputTypes,
                declareCrossRunShareable: true).BindExecutor(),
            new FunctionExecutor<IEnumerable<ChatMessage>>(
                id: "local-messages-router",
                handlerAsync: localMessagesRouter,
                sentMessageTypes: s_routerSentMessageTypes,
                outputTypes: s_routerOutputTypes,
                declareCrossRunShareable: true).BindExecutor())
    {
    }

    public ExecutorBinding FoundryMessageRouter => this.Item1;

    public ExecutorBinding FoundryMessagesRouter => this.Item2;

    public ExecutorBinding LocalMessageRouter => this.Item3;

    public ExecutorBinding LocalMessagesRouter => this.Item4;
}

internal static partial class AgentWorkflowBindingsFactory
{
    private const string InvalidJsonRetryStateKey = "foundry-invalid-json-retry-count";
    private const int MaxInvalidJsonRetries = 5;

    public static AgentWorkflowBindings Create(
        Func<FoundryAgentResult, IWorkflowContext, CancellationToken, ValueTask> routeFoundryResultAsync,
        Func<string, IWorkflowContext, CancellationToken, ValueTask> routeLocalResultAsync,
        string foundryExecutorId,
        bool emitEvents,
        PromptCatalog promptCatalog,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(routeFoundryResultAsync);
        ArgumentNullException.ThrowIfNull(routeLocalResultAsync);
        ArgumentException.ThrowIfNullOrWhiteSpace(foundryExecutorId);
        ArgumentNullException.ThrowIfNull(promptCatalog);
        ArgumentNullException.ThrowIfNull(logger);

        async ValueTask RouteFoundryPayloadAsync(string payload, IWorkflowContext context, CancellationToken cancellationToken)
        {
            Log.ReceivedFoundryPayload(logger, payload);

            FoundryAgentResult? result = await DeserializeFoundryResultAsync(payload, context, foundryExecutorId, emitEvents, promptCatalog, logger, cancellationToken).ConfigureAwait(false);
            if (result is null)
            {
                return;
            }

            await routeFoundryResultAsync(result, context, cancellationToken).ConfigureAwait(false);
        }

        async ValueTask HandleFoundryMessageAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken)
        {
            if (message.Role != ChatRole.Assistant || string.IsNullOrWhiteSpace(message.Text))
            {
                Log.IgnoredSingleMessage(logger, message.Role);
                return;
            }

            await RouteFoundryPayloadAsync(message.Text, context, cancellationToken).ConfigureAwait(false);
        }

        async ValueTask HandleFoundryMessagesAsync(IEnumerable<ChatMessage> messages, IWorkflowContext context, CancellationToken cancellationToken)
        {
            ChatMessage? message = GetLastAssistantMessage(messages);
            if (message is null)
            {
                Log.NoAssistantMessageFound(logger);
                return;
            }

            await RouteFoundryPayloadAsync(message.Text!, context, cancellationToken).ConfigureAwait(false);
        }

        async ValueTask HandleLocalMessageAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken)
        {
            if (message.Role != ChatRole.Assistant || string.IsNullOrWhiteSpace(message.Text))
            {
                Log.IgnoredSingleMessage(logger, message.Role);
                return;
            }

            Log.ReceivedLocalPayload(logger, message.Text);
            await routeLocalResultAsync(message.Text, context, cancellationToken).ConfigureAwait(false);
        }

        async ValueTask HandleLocalMessagesAsync(IEnumerable<ChatMessage> messages, IWorkflowContext context, CancellationToken cancellationToken)
        {
            ChatMessage? message = GetLastAssistantMessage(messages);
            if (message is null)
            {
                Log.NoAssistantMessageFound(logger);
                return;
            }

            Log.ReceivedLocalPayload(logger, message.Text!);
            await routeLocalResultAsync(message.Text!, context, cancellationToken).ConfigureAwait(false);
        }

        return new AgentWorkflowBindings(
            HandleFoundryMessageAsync,
            HandleFoundryMessagesAsync,
            HandleLocalMessageAsync,
            HandleLocalMessagesAsync);
    }

    private static ChatMessage? GetLastAssistantMessage(IEnumerable<ChatMessage> messages)
    {
        ChatMessage? lastAssistantMessage = null;
        foreach (ChatMessage message in messages)
        {
            if (message.Role == ChatRole.Assistant && !string.IsNullOrEmpty(message.Text))
            {
                lastAssistantMessage = message;
            }
        }

        return lastAssistantMessage;
    }

    private static async ValueTask<FoundryAgentResult?> DeserializeFoundryResultAsync(
        string payload,
        IWorkflowContext context,
        string foundryExecutorId,
        bool emitEvents,
        PromptCatalog promptCatalog,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (FoundryAgentResultParser.TryParse(payload, out FoundryAgentResult? result, out JsonException? jsonException, out bool recoveredMalformedJson))
        {
            if (recoveredMalformedJson)
            {
                Log.RecoveredMalformedJson(logger);
            }

            await context.QueueStateUpdateAsync(InvalidJsonRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
            return result;
        }

        int retryCount = await context.ReadOrInitStateAsync(InvalidJsonRetryStateKey, static () => 0, cancellationToken).ConfigureAwait(false);
        if (retryCount < MaxInvalidJsonRetries && jsonException is not null)
        {
            int nextRetryCount = retryCount + 1;
            Log.RequestingJsonRetry(logger, foundryExecutorId, nextRetryCount, MaxInvalidJsonRetries, jsonException.Message);
            await context.QueueStateUpdateAsync(InvalidJsonRetryStateKey, nextRetryCount, cancellationToken).ConfigureAwait(false);
            await context.SendMessageAsync(
                new ChatMessage(ChatRole.System, BuildInvalidJsonRetryPrompt(promptCatalog, payload, jsonException, nextRetryCount)),
                foundryExecutorId,
                cancellationToken).ConfigureAwait(false);
            await context.SendMessageAsync(new TurnToken(emitEvents: emitEvents), foundryExecutorId, cancellationToken).ConfigureAwait(false);
            return null;
        }

        Log.JsonRetryFailed(logger, retryCount, MaxInvalidJsonRetries, jsonException?.Message ?? "No JsonException message was available.");
        await context.QueueStateUpdateAsync(InvalidJsonRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
        throw new InvalidOperationException("Hosted Foundry agent returned invalid JSON after retry exhaustion.", jsonException);
    }

    private static string BuildInvalidJsonRetryPrompt(PromptCatalog promptCatalog, string payload, JsonException exception, int retryAttempt)
        => promptCatalog.Format(
            promptCatalog.InvalidJsonRetryPrompt,
            ("RETRY_ATTEMPT", retryAttempt.ToString()),
            ("MAX_INVALID_JSON_RETRIES", MaxInvalidJsonRetries.ToString()),
            ("JSON_EXCEPTION", exception.ToString()),
            ("PREVIOUS_PAYLOAD", payload));

    private static partial class Log
    {
        [LoggerMessage(EventId = 1000, Level = LogLevel.Debug, Message = "agent-router ignored ChatMessage with role {Role}")]
        public static partial void IgnoredSingleMessage(ILogger logger, ChatRole role);

        [LoggerMessage(EventId = 1001, Level = LogLevel.Debug, Message = "agent-router did not find an assistant message.")]
        public static partial void NoAssistantMessageFound(ILogger logger);

        [LoggerMessage(EventId = 1002, Level = LogLevel.Debug, Message = "agent-router received hosted Foundry payload: {Payload}")]
        public static partial void ReceivedFoundryPayload(ILogger logger, string payload);

        [LoggerMessage(EventId = 1003, Level = LogLevel.Debug, Message = "agent-router received local payload: {Payload}")]
        public static partial void ReceivedLocalPayload(ILogger logger, string payload);

        [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "Recovered malformed hosted Foundry JSON locally.")]
        public static partial void RecoveredMalformedJson(ILogger logger);

        [LoggerMessage(EventId = 1005, Level = LogLevel.Warning, Message = "Hosted Foundry agent returned malformed JSON. Requesting correction retry {RetryAttempt}/{MaxRetries} for executor {FoundryExecutorId}. Exception: {ExceptionMessage}")]
        public static partial void RequestingJsonRetry(ILogger logger, string foundryExecutorId, int retryAttempt, int maxRetries, string exceptionMessage);

        [LoggerMessage(EventId = 1006, Level = LogLevel.Error, Message = "Hosted Foundry agent returned malformed JSON after {RetryCount}/{MaxRetries} correction retries. Last exception: {ExceptionMessage}")]
        public static partial void JsonRetryFailed(ILogger logger, int retryCount, int maxRetries, string exceptionMessage);
    }
}
