namespace ChatBot.Copilot;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

internal interface ICopilotTurnSession : IAsyncDisposable
{
    string SessionId { get; }
}

internal interface ICopilotSessionRuntime
{
    Task<ICopilotTurnSession> StartSessionAsync(IReadOnlyList<AIFunction> sessionTools, CancellationToken cancellationToken);

    IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
        ICopilotTurnSession session,
        string prompt,
        ILogger? logger,
        CancellationToken cancellationToken);
}

internal sealed class CopilotSdkSessionRuntime(
    CopilotClientFactory clientFactory,
    CopilotAgentCatalog agentCatalog) : ICopilotSessionRuntime
{
    private readonly CopilotClientFactory _clientFactory = clientFactory;
    private readonly CopilotAgentCatalog _agentCatalog = agentCatalog;

    public async Task<ICopilotTurnSession> StartSessionAsync(IReadOnlyList<AIFunction> sessionTools, CancellationToken cancellationToken)
    {
        CopilotClient client = await _clientFactory.GetStartedClientAsync(cancellationToken).ConfigureAwait(false);
        CopilotSession session = await client
            .CreateSessionAsync(_agentCatalog.CreateSessionConfig(sessionTools, cancellationToken: cancellationToken), cancellationToken)
            .ConfigureAwait(false);
        return new CopilotSdkTurnSession(session);
    }

    public IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
        ICopilotTurnSession session,
        string prompt,
        ILogger? logger,
        CancellationToken cancellationToken)
        => session is CopilotSdkTurnSession sdkSession
            ? CopilotEventStreamAdapter.StreamTurnAsync(sdkSession.Session, prompt, logger, cancellationToken)
            : throw new InvalidOperationException($"Unsupported session type {session.GetType().FullName}.");

    private sealed class CopilotSdkTurnSession(CopilotSession session) : ICopilotTurnSession
    {
        public CopilotSession Session { get; } = session;

        public string SessionId => Session.SessionId;

        public ValueTask DisposeAsync()
            => Session.DisposeAsync();
    }
}
