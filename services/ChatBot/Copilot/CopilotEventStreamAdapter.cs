namespace ChatBot.Copilot;

using ChatBot.Tools;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using System.Text;
using System.Threading.Channels;

internal static class CopilotEventStreamAdapter
{
    private static readonly TimeSpan TurnCompletionTimeout = TimeSpan.FromMinutes(2);

    public static IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
        CopilotSession session,
        string prompt,
        CancellationToken cancellationToken = default)
        => StreamTurnAsync(session, prompt, logger: null, cancellationToken);

    public static async IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
        CopilotSession session,
        string prompt,
        Microsoft.Extensions.Logging.ILogger? logger,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        StringBuilder streamedAssistantMessage = new();
        Channel<SessionEvent> events = Channel.CreateUnbounded<SessionEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });
        using IDisposable eventSubscription = session.On(@event =>
        {
            if (@event is not null)
            {
                events.Writer.TryWrite(@event);
            }
        });

        Task<string> sendTask = Task.Run(SendPromptAsync, cancellationToken);
        Task timeoutTask = Task.Delay(TurnCompletionTimeout, cancellationToken);

        bool emittedStatus = false;
        bool turnCompleted = false;
        bool sendCompleted = false;
        string? currentAssistantTurnId = null;
        string? lastAssistantTurnIdThatRequestedTools = null;
        while (!turnCompleted)
        {
            Task<bool> waitToReadTask = events.Reader.WaitToReadAsync(cancellationToken).AsTask();
            Task completedTask = sendCompleted
                ? await Task.WhenAny(waitToReadTask, timeoutTask).ConfigureAwait(false)
                : await Task.WhenAny(waitToReadTask, sendTask, timeoutTask).ConfigureAwait(false);
            if (completedTask == timeoutTask)
            {
                TimeoutException timeout = new($"Copilot turn did not complete within {TurnCompletionTimeout}.");
                logger?.CopilotTurnFailed(session.SessionId, streamedAssistantMessage.Length, timeout);
                throw timeout;
            }

            if (completedTask == sendTask)
            {
                _ = await sendTask.ConfigureAwait(false);
                sendCompleted = true;
                if (!waitToReadTask.IsCompleted)
                {
                    continue;
                }
            }

            if (!await waitToReadTask.ConfigureAwait(false))
            {
                break;
            }

            while (events.Reader.TryRead(out SessionEvent? @event))
            {
                logger?.CopilotSessionEventObserved(session.SessionId, @event.Type);
                switch (@event)
                {
                    case AssistantTurnStartEvent assistantTurnStartEvent:
                        currentAssistantTurnId = assistantTurnStartEvent.Data.TurnId;
                        break;
                    case ToolExecutionStartEvent toolExecutionStartEvent when !emittedStatus:
                        emittedStatus = true;
                        yield return CreateStatusUpdate(BuildToolStatusMessage(toolExecutionStartEvent.Data.ToolName));
                        break;
                    case ToolExecutionProgressEvent toolExecutionProgressEvent when !string.IsNullOrWhiteSpace(toolExecutionProgressEvent.Data.ProgressMessage):
                        emittedStatus = true;
                        yield return CreateStatusUpdate(toolExecutionProgressEvent.Data.ProgressMessage.Trim());
                        break;
                    case AssistantMessageDeltaEvent assistantMessageDeltaEvent when !string.IsNullOrWhiteSpace(assistantMessageDeltaEvent.Data.DeltaContent):
                        streamedAssistantMessage.Append(assistantMessageDeltaEvent.Data.DeltaContent);
                        yield return CreateAssistantUpdate(streamedAssistantMessage.ToString());
                        break;
                    case AssistantMessageEvent assistantMessageEvent:
                    {
                        if (assistantMessageEvent.Data.ToolRequests is { Length: > 0 })
                        {
                            lastAssistantTurnIdThatRequestedTools = currentAssistantTurnId;
                        }

                        string fullContent = assistantMessageEvent.Data.Content?.Trim() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(fullContent)
                            && !string.Equals(streamedAssistantMessage.ToString(), fullContent, StringComparison.Ordinal))
                        {
                            streamedAssistantMessage.Clear();
                            streamedAssistantMessage.Append(fullContent);
                            yield return CreateAssistantUpdate(fullContent);
                        }

                        break;
                    }
                    case SessionErrorEvent sessionErrorEvent:
                        throw new InvalidOperationException(
                            $"Copilot session error ({sessionErrorEvent.Data.ErrorType}): {sessionErrorEvent.Data.Message}");
                    case AssistantTurnEndEvent assistantTurnEndEvent:
                        turnCompleted = !string.Equals(
                            assistantTurnEndEvent.Data.TurnId,
                            lastAssistantTurnIdThatRequestedTools,
                            StringComparison.Ordinal);
                        break;
                    case SessionIdleEvent:
                        turnCompleted = true;
                        break;
                }

                if (turnCompleted)
                {
                    break;
                }
            }
        }

        _ = await sendTask.ConfigureAwait(false);
        string finalAssistantMessage = await GetFinalAssistantMessageAsync(session, streamedAssistantMessage, cancellationToken).ConfigureAwait(false);
        logger?.CopilotTurnCompleted(
            session.SessionId,
            !string.IsNullOrWhiteSpace(finalAssistantMessage),
            streamedAssistantMessage.Length);
        if (string.IsNullOrWhiteSpace(finalAssistantMessage))
        {
            throw new InvalidOperationException("Copilot session completed without producing an assistant message.");
        }

        yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(finalAssistantMessage)])
        {
            FinishReason = ChatFinishReason.Stop,
        };

        async Task<string> SendPromptAsync()
        {
            try
            {
                logger?.StartingCopilotTurn(session.SessionId, prompt.Length);
                string messageId = await session
                    .SendAsync(new MessageOptions { Prompt = prompt }, cancellationToken)
                    .ConfigureAwait(false);
                return messageId;
            }
            catch (Exception e)
            {
                logger?.CopilotTurnFailed(session.SessionId, streamedAssistantMessage.Length, e);
                throw;
            }
        }
    }

    private static async Task<string> GetFinalAssistantMessageAsync(
        CopilotSession session,
        StringBuilder streamedAssistantMessage,
        CancellationToken cancellationToken)
    {
        string streamed = streamedAssistantMessage.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(streamed))
        {
            return streamed;
        }

        IReadOnlyList<SessionEvent> messages = await session.GetMessagesAsync(cancellationToken).ConfigureAwait(false);
        return messages
            .OfType<AssistantMessageEvent>()
            .Select(static message => message.Data.Content?.Trim())
            .LastOrDefault(static content => !string.IsNullOrWhiteSpace(content))
            ?? string.Empty;
    }

    private static AgentResponseUpdate CreateStatusUpdate(string message)
        => new(ChatRole.Assistant, [new TextContent($"{Conversation.UserStatusPrefix}{message}")])
        {
            FinishReason = ChatFinishReason.Stop,
        };

    private static AgentResponseUpdate CreateAssistantUpdate(string message)
        => new(ChatRole.Assistant, [new TextContent(message)]);

    private static string BuildToolStatusMessage(string? toolName)
        => toolName switch
        {
            FoundrySpecialistTool.ToolName => "Give me a sec - I am checking the official side of this now.",
            "fetch_meal_signup_info" => "Hang tight - I am pulling the signup board details now.",
            "statbotics_api" or "tba_api" or "tba_api_surface" or "tba_last_comp" => "Hang tight - I am pulling the numbers together now.",
            _ => "Give me a sec - I am lining up the grounded details now.",
        };
}
