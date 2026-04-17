namespace ChatBot;

using AgentFramework.OpenTelemetry;

using ChatBot.Copilot;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using System.Diagnostics;
using System.Runtime.CompilerServices;

internal sealed class Conversation(CopilotSessionCoordinator sessionCoordinator)
{
    internal const string UserStatusPrefix = "USER_STATUS:";

    public static Task<string> CreateConversationAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return Task.FromResult(string.Empty);
    }

    public async IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        CopilotChatState chatState,
        string message,
        IEnumerable<ChatMessage>? leadingMessages,
        Func<CopilotChatState, CancellationToken, ValueTask>? persistConversationState,
        ActivityContext? traceRootContext = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(chatState);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        using Activity? activity = traceRootContext is { } parentContext
            ? Activities.AppActivitySource.StartActivity("chat.turn", ActivityKind.Server, parentContext)
            : Activities.AppActivitySource.StartActivity("chat.turn", ActivityKind.Server);
        activity?.SetTag("gen_ai.operation.name", "chat_turn");
        activity?.SetTag("span_type", "turn");
        using IDisposable conversationParentScope = Activities.PushConversationParent(activity);

        await foreach (AgentResponseUpdate update in sessionCoordinator.RunTurnStreamingAsync(
            chatState,
            message,
            leadingMessages,
            persistConversationState,
            cancellationToken).ConfigureAwait(false))
        {
            yield return update;
        }
    }

    public IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        CopilotChatState chatState,
        string message,
        Func<CopilotChatState, CancellationToken, ValueTask>? persistConversationState,
        ActivityContext? traceRootContext = null,
        CancellationToken cancellationToken = default)
        => PostUserMessageStreamingAsync(chatState, message, null, persistConversationState, traceRootContext, cancellationToken: cancellationToken);

    public IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        CopilotChatState chatState,
        string message,
        ActivityContext? traceRootContext = null,
        CancellationToken cancellationToken = default)
        => PostUserMessageStreamingAsync(chatState, message, null, null, traceRootContext, cancellationToken: cancellationToken);

    internal static bool TryExtractUserStatusMessage(string output, out string? messageToUser)
    {
        if (!output.StartsWith(UserStatusPrefix, StringComparison.Ordinal))
        {
            messageToUser = null;
            return false;
        }

        string payload = output[UserStatusPrefix.Length..].Trim();
        messageToUser = payload.Length is 0 ? null : payload;
        return true;
    }
}
