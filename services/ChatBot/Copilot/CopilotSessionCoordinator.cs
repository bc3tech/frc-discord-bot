namespace ChatBot.Copilot;

using ChatBot.Tools;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Text;

internal sealed class CopilotSessionCoordinator(
    CopilotClientFactory clientFactory,
    CopilotAgentCatalog agentCatalog,
    CopilotEventStreamAdapter eventStreamAdapter,
    FoundrySpecialistTool foundrySpecialistTool,
    IEnumerable<IProvideFunctionTools> toolProviders,
    ILogger<CopilotSessionCoordinator> logger)
{
    private readonly IReadOnlyList<AIFunction> _localToolFunctions = toolProviders.CombineFunctions(FunctionToolScope.LocalFrcData);
    private readonly CopilotClientFactory _clientFactory = clientFactory;
    private readonly CopilotAgentCatalog _agentCatalog = agentCatalog;
    private readonly CopilotEventStreamAdapter _eventStreamAdapter = eventStreamAdapter;
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

        CopilotClient client = await _clientFactory.GetStartedClientAsync(cancellationToken).ConfigureAwait(false);
        (CopilotSession Session, bool Resumed) sessionInfo = await GetOrCreateSessionAsync(client, currentState, sessionTools, PersistStateAsync, cancellationToken).ConfigureAwait(false);
        await using CopilotSession session = sessionInfo.Session;

        string prompt = BuildPrompt(message, leadingMessages, currentState.Transcript, includeTranscriptReplay: !sessionInfo.Resumed);
        await foreach (AgentResponseUpdate update in CopilotEventStreamAdapter.StreamTurnAsync(session, prompt, cancellationToken).ConfigureAwait(false))
        {
            yield return update;
        }
    }

    private async Task<(CopilotSession Session, bool Resumed)> GetOrCreateSessionAsync(
        CopilotClient client,
        CopilotChatState chatState,
        IReadOnlyList<AIFunction> sessionTools,
        Func<CopilotChatState, CancellationToken, ValueTask> persistState,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(chatState.CopilotSessionId))
        {
            try
            {
                CopilotSession resumedSession = await client
                    .ResumeSessionAsync(chatState.CopilotSessionId, _agentCatalog.CreateResumeSessionConfig(sessionTools), cancellationToken)
                    .ConfigureAwait(false);

                if (!string.Equals(chatState.CopilotSessionId, resumedSession.SessionId, StringComparison.Ordinal))
                {
                    await persistState(chatState with { CopilotSessionId = resumedSession.SessionId }, cancellationToken).ConfigureAwait(false);
                }

                return (resumedSession, true);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Log.UnableToResumeCopilotSessionFallingBackToAFreshSession(_logger, e, chatState.CopilotSessionId);
                await persistState(chatState with { CopilotSessionId = null }, cancellationToken).ConfigureAwait(false);
                chatState = chatState with { CopilotSessionId = null };
            }
        }

        CopilotSession session = await client
            .CreateSessionAsync(_agentCatalog.CreateSessionConfig(sessionTools), cancellationToken)
            .ConfigureAwait(false);

        await persistState(chatState with { CopilotSessionId = session.SessionId }, cancellationToken).ConfigureAwait(false);
        return (session, false);
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
