namespace ChatBot.Copilot;

using ChatBot.Tools;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

using System.Threading.Channels;

internal static class CopilotEventStreamAdapter
{
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

        Task sendTask = Task.Run(SendPromptAsync, cancellationToken);

        string? latestAssistantMessage = null;
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
                    case AssistantMessageEvent assistantMessageEvent when !string.IsNullOrWhiteSpace(assistantMessageEvent.Data.Content):
                        latestAssistantMessage = assistantMessageEvent.Data.Content.Trim();
                        break;
                    case SessionIdleEvent:
                        await sendTask.ConfigureAwait(false);

                        if (string.IsNullOrWhiteSpace(latestAssistantMessage))
                        {
                            throw new InvalidOperationException("Copilot session completed without producing an assistant message.");
                        }

                        yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(latestAssistantMessage)])
                        {
                            FinishReason = ChatFinishReason.Stop,
                        };
                        yield break;
                }
            }
        }

        await sendTask.ConfigureAwait(false);
        throw new InvalidOperationException("Copilot session completed without emitting a SessionIdle event.");

        async Task SendPromptAsync()
        {
            try
            {
                _ = await session.SendAsync(new MessageOptions { Prompt = prompt }, cancellationToken).ConfigureAwait(false);
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

    private static string BuildToolStatusMessage(string? toolName)
        => toolName switch
        {
            FoundrySpecialistTool.ToolName => "Give me a sec - I am checking the official side of this now.",
            "fetch_meal_signup_info" => "Hang tight - I am pulling the signup board details now.",
            "statbotics_api" or "tba_api" or "tba_api_surface" => "Hang tight - I am pulling the numbers together now.",
            _ => "Give me a sec - I am lining up the grounded details now.",
        };
}
