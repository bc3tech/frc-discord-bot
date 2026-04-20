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

        Channel<SessionEvent> events = Channel.CreateUnbounded<SessionEvent>();
        using IDisposable subscription = session.On(static @event =>
        {
            if (@event is null)
            {
                return;
            }
        });
        using IDisposable eventSubscription = session.On(@event => events.Writer.TryWrite(@event));

        _ = await session.SendAsync(new MessageOptions { Prompt = prompt }, cancellationToken).ConfigureAwait(false);

        string? latestAssistantMessage = null;
        bool emittedStatus = false;
        while (true)
        {
            SessionEvent @event = await events.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
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
