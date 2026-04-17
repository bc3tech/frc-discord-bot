namespace FunctionApp.Tests;

using global::ChatBot;
using global::ChatBot.Copilot;

using Microsoft.Extensions.AI;

public sealed class CopilotChatStateTests
{
    [Fact]
    public void SerializeRoundTripsCompositeChatState()
    {
        CopilotChatState state = new()
        {
            CopilotSessionId = "session-123",
            FoundryThreadId = "thread-456",
            Transcript =
            [
                new("user", "How did we do at our last event?"),
                new("assistant", "You finished first in quals."),
            ],
        };

        string serialized = ConversationThreadState.Serialize(state);

        CopilotChatState parsed = ConversationThreadState.Parse(serialized);

        Assert.Equal(CopilotChatState.CurrentVersion, parsed.Version);
        Assert.Equal("session-123", parsed.CopilotSessionId);
        Assert.Equal("thread-456", parsed.FoundryThreadId);
        Assert.Collection(
            parsed.Transcript,
            entry =>
            {
                Assert.Equal("user", entry.Role);
                Assert.Equal("How did we do at our last event?", entry.Content);
            },
            entry =>
            {
                Assert.Equal("assistant", entry.Role);
                Assert.Equal("You finished first in quals.", entry.Content);
            });
    }

    [Fact]
    public void ParseWhenStateIsLegacyThreadIdUpgradesCleanly()
    {
        CopilotChatState parsed = ConversationThreadState.Parse("thread-legacy");

        Assert.Equal(CopilotChatState.CurrentVersion, parsed.Version);
        Assert.Equal("thread-legacy", parsed.FoundryThreadId);
        Assert.Null(parsed.CopilotSessionId);
        Assert.Empty(parsed.Transcript);
    }

    [Fact]
    public void ParseWhenStateIsMalformedReturnsEmptyEnvelope()
    {
        CopilotChatState parsed = ConversationThreadState.Parse("{ definitely-not-json");

        Assert.Equal(CopilotChatState.CurrentVersion, parsed.Version);
        Assert.Null(parsed.FoundryThreadId);
        Assert.Null(parsed.CopilotSessionId);
        Assert.Empty(parsed.Transcript);
    }

    [Fact]
    public void TranscriptWindowKeepsMostRecentMessages()
    {
        IReadOnlyList<CopilotTranscriptMessage> transcript =
            Enumerable.Range(0, 12)
                .Select(i => new CopilotTranscriptMessage(i % 2 is 0 ? "user" : "assistant", $"message-{i}"))
                .ToArray();

        IReadOnlyList<CopilotTranscriptMessage> updated = CopilotTranscriptWindow.AppendTurn(transcript, "new-user", "new-assistant");
        IReadOnlyList<ChatMessage> replay = CopilotTranscriptWindow.ToChatMessages(updated);

        Assert.Equal(12, updated.Count);
        Assert.Equal("message-2", updated[0].Content);
        Assert.Equal("new-user", updated[^2].Content);
        Assert.Equal("new-assistant", updated[^1].Content);
        Assert.Equal(ChatRole.User, replay[^2].Role);
        Assert.Equal(ChatRole.Assistant, replay[^1].Role);
    }
}
