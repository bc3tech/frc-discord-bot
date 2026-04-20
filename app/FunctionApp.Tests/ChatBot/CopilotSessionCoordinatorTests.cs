namespace FunctionApp.Tests;

using global::ChatBot.Agents;
using global::ChatBot.Configuration;
using global::ChatBot.Copilot;
using global::ChatBot.Tools;

using Azure.AI.Projects;
using Azure.Core;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

public sealed class CopilotSessionCoordinatorTests
{
    [Fact]
    public async Task RunTurnStreamingAsyncDoesNotPersistSessionUntilAfterStreamingStarts()
    {
        RecordingSessionRuntime runtime = new(
            sessionId: "session-123",
            stream: SuccessStreamAsync);

        CopilotSessionCoordinator coordinator = CreateCoordinator(runtime);

        bool streamStarted = false;
        bool persistedBeforeStreaming = false;
        CopilotChatState chatState = new();
        ValueTask PersistStateAsync(CopilotChatState updatedState, CancellationToken cancellationToken)
        {
            _ = cancellationToken;
            if (!streamStarted)
            {
                persistedBeforeStreaming = true;
            }

            chatState = updatedState;
            return ValueTask.CompletedTask;
        }

        List<AgentResponseUpdate> updates = [];
        await foreach (AgentResponseUpdate update in coordinator.RunTurnStreamingAsync(
            chatState,
            "who's got meal duty this week",
            leadingMessages: null,
            PersistStateAsync,
            TestContext.Current.CancellationToken))
        {
            streamStarted = true;
            updates.Add(update);
        }

        Assert.False(persistedBeforeStreaming);
        Assert.Single(updates);
        Assert.Equal("session-123", chatState.CopilotSessionId);

        static async IAsyncEnumerable<AgentResponseUpdate> SuccessStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent("done")])
            {
                FinishReason = ChatFinishReason.Stop,
            };
        }
    }

    [Fact]
    public async Task RunTurnStreamingAsyncDoesNotPersistSessionWhenStreamingFailsImmediately()
    {
        RecordingSessionRuntime runtime = new(
            sessionId: "session-456",
            stream: ThrowingStreamAsync);

        CopilotSessionCoordinator coordinator = CreateCoordinator(runtime);

        int persistCalls = 0;
        CopilotChatState chatState = new();
        ValueTask PersistStateAsync(CopilotChatState updatedState, CancellationToken cancellationToken)
        {
            _ = updatedState;
            _ = cancellationToken;
            persistCalls++;
            return ValueTask.CompletedTask;
        }

        InvalidOperationException failure = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await foreach (AgentResponseUpdate _ in coordinator.RunTurnStreamingAsync(
                chatState,
                "who's got meal duty this week",
                leadingMessages: null,
                PersistStateAsync,
                TestContext.Current.CancellationToken))
            {
            }
        });

        Assert.Equal("boom", failure.Message);
        Assert.Equal(0, persistCalls);

        static async IAsyncEnumerable<AgentResponseUpdate> ThrowingStreamAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            throw new InvalidOperationException("boom");
#pragma warning disable CS0162
            yield break;
#pragma warning restore CS0162
        }
    }

    private static CopilotSessionCoordinator CreateCoordinator(ICopilotSessionRuntime runtime)
    {
        AiOptions options = new()
        {
            FoundryEndpoint = new Uri("https://example.services.ai.azure.com/api/projects/test"),
            AgentId = "discord-bot",
            MealSignupGeniusId = "signup-board",
            LocalAgentModel = "gpt-5.4-mini",
            OpenAIApiVersion = "2025-06-01",
            DefaultTeamNumber = 2046,
            Copilot =
            {
                Model = "gpt-5.4-mini",
            },
        };

        FoundrySpecialistTool foundryTool = new(
            new AIProjectClient(endpoint: options.FoundryEndpoint, tokenProvider: new StubTokenCredential("token-value")),
            Options.Create(options),
            new PromptCatalog(NullLogger<PromptCatalog>.Instance),
            NullLoggerFactory.Instance);

        return new CopilotSessionCoordinator(
            runtime,
            foundryTool,
            [],
            NullLogger<CopilotSessionCoordinator>.Instance);
    }

    private sealed class RecordingSessionRuntime(
        string sessionId,
        Func<CancellationToken, IAsyncEnumerable<AgentResponseUpdate>> stream) : ICopilotSessionRuntime
    {
        private readonly Func<CancellationToken, IAsyncEnumerable<AgentResponseUpdate>> _stream = stream;

        public Task<ICopilotTurnSession> StartSessionAsync(IReadOnlyList<AIFunction> sessionTools, CancellationToken cancellationToken)
        {
            _ = sessionTools;
            _ = cancellationToken;
            return Task.FromResult<ICopilotTurnSession>(new FakeTurnSession(sessionId));
        }

        public IAsyncEnumerable<AgentResponseUpdate> StreamTurnAsync(
            ICopilotTurnSession session,
            string prompt,
            Microsoft.Extensions.Logging.ILogger? logger,
            CancellationToken cancellationToken)
        {
            _ = session;
            _ = prompt;
            _ = logger;
            return _stream(cancellationToken);
        }

        private sealed class FakeTurnSession(string sessionId) : ICopilotTurnSession
        {
            public string SessionId { get; } = sessionId;

            public ValueTask DisposeAsync()
                => ValueTask.CompletedTask;
        }
    }

    private sealed class StubTokenCredential(string tokenValue) : TokenCredential
    {
        private readonly AccessToken _token = new(tokenValue, DateTimeOffset.UtcNow.AddHours(1));

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => _token;

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(_token);
    }
}
