namespace ChatBot.Copilot;

using ChatBot.Tools;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Text;

internal sealed class CopilotSessionCoordinator(
    ICopilotSessionRuntime sessionRuntime,
    FoundrySpecialistTool foundrySpecialistTool,
    IEnumerable<IProvideFunctionTools> toolProviders,
    ILogger<CopilotSessionCoordinator> logger)
{
    private readonly IReadOnlyList<AIFunction> _localToolFunctions = toolProviders.CombineFunctions(FunctionToolScope.LocalFrcData);
    private readonly ICopilotSessionRuntime _sessionRuntime = sessionRuntime;
    private readonly FoundrySpecialistTool _foundrySpecialistTool = foundrySpecialistTool;
    private readonly ILogger<CopilotSessionCoordinator> _logger = logger;

    public async IAsyncEnumerable<AgentResponseUpdate> RunTurnStreamingAsync(
        CopilotChatState chatState,
        string message,
        IEnumerable<ChatMessage>? leadingMessages,
        Func<CopilotChatState, CancellationToken, ValueTask>? persistChatState,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(chatState);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        CopilotChatState currentState = chatState;

        async ValueTask PersistStateAsync(CopilotChatState updatedState, CancellationToken ct)
        {
            currentState = updatedState;
            if (persistChatState is not null)
            {
                await persistChatState(updatedState, ct).ConfigureAwait(false);
            }
        }

        AIFunction foundryTool = _foundrySpecialistTool.CreateFunction(
            () => currentState,
            PersistStateAsync);
        List<AIFunction> sessionTools = [.. _localToolFunctions, foundryTool];

        ICopilotTurnSession session = await _sessionRuntime.StartSessionAsync(sessionTools, cancellationToken).ConfigureAwait(false);
        currentState = currentState with { CopilotSessionId = session.SessionId };
        await using (session.ConfigureAwait(false))
        {
            string prompt = BuildPrompt(message, leadingMessages, currentState.Transcript, includeTranscriptReplay: true);
            await foreach (AgentResponseUpdate update in _sessionRuntime.StreamTurnAsync(session, prompt, _logger, cancellationToken).ConfigureAwait(false))
            {
                yield return update;
            }
        }

        if (persistChatState is not null)
        {
            await persistChatState(currentState, cancellationToken).ConfigureAwait(false);
        }
    }

    private static string BuildPrompt(
        string currentUserMessage,
        IEnumerable<ChatMessage>? leadingMessages,
        IReadOnlyList<CopilotTranscriptMessage> transcript,
        bool includeTranscriptReplay)
    {
        StringBuilder prompt = new();
        AppendMessages(prompt, "Conversation context", leadingMessages);

        if (includeTranscriptReplay)
        {
            AppendTranscript(prompt, transcript);
        }

        if (prompt.Length > 0)
        {
            prompt.AppendLine();
            prompt.AppendLine("Current user request:");
        }

        prompt.Append(currentUserMessage.Trim());
        return prompt.ToString();
    }

    private static void AppendMessages(StringBuilder prompt, string heading, IEnumerable<ChatMessage>? messages)
    {
        if (messages is null)
        {
            return;
        }

        List<string> lines = [];
        foreach (ChatMessage message in messages)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
            {
                continue;
            }

            lines.Add($"{message.Role.Value}: {message.Text.Trim()}");
        }

        if (lines.Count is 0)
        {
            return;
        }

        prompt.AppendLine($"{heading}:");
        foreach (string line in lines)
        {
            prompt.AppendLine(line);
        }
    }

    private static void AppendTranscript(StringBuilder prompt, IReadOnlyList<CopilotTranscriptMessage> transcript)
    {
        if (transcript.Count is 0)
        {
            return;
        }

        prompt.AppendLine("Recent transcript:");
        foreach (CopilotTranscriptMessage message in transcript)
        {
            prompt.AppendLine($"{message.Role}: {message.Content}");
        }
    }
}
