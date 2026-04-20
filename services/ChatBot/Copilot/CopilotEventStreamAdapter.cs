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

    public static async IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
        CopilotSession session,
        string prompt,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

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

        Task<AssistantMessageEvent?> sendTask = Task.Run(SendPromptAsync, cancellationToken);

        StringBuilder streamedAssistantMessage = new();
        bool emittedStatus = false;
        while (await events.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            while (events.Reader.TryRead(out SessionEvent? @event))
            {
                switch (@event)
                {
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
                    case AssistantMessageEvent assistantMessageEvent when !string.IsNullOrWhiteSpace(assistantMessageEvent.Data.Content):
                    {
                        string fullContent = assistantMessageEvent.Data.Content.Trim();
                        if (!string.Equals(streamedAssistantMessage.ToString(), fullContent, StringComparison.Ordinal))
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
                }
            }
        }

        AssistantMessageEvent? finalAssistantMessageEvent = await sendTask.ConfigureAwait(false);
        string finalAssistantMessage = finalAssistantMessageEvent is { Data.Content: { } content }
            ? content.Trim()
            : streamedAssistantMessage.ToString().Trim();
        if (string.IsNullOrWhiteSpace(finalAssistantMessage))
        {
            throw new InvalidOperationException("Copilot session completed without producing an assistant message.");
        }

        yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(finalAssistantMessage)])
        {
            FinishReason = ChatFinishReason.Stop,
        };

        async Task<AssistantMessageEvent?> SendPromptAsync()
        {
            try
            {
                AssistantMessageEvent? response = await session
                    .SendAndWaitAsync(new MessageOptions { Prompt = prompt }, TurnCompletionTimeout, cancellationToken)
                    .ConfigureAwait(false);
                events.Writer.TryComplete();
                return response;
            }
            catch (Exception e)
            {
                events.Writer.TryComplete(e);
                throw;
            }
        }
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
            "statbotics_api" or "tba_api" or "tba_api_surface" => "Hang tight - I am pulling the numbers together now.",
            _ => "Give me a sec - I am lining up the grounded details now.",
        };
}
