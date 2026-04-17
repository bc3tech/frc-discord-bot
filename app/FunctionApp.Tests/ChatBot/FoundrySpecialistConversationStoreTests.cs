namespace FunctionApp.Tests;

using global::ChatBot.Copilot;

public sealed class FoundrySpecialistConversationStoreTests
{
    [Fact]
    public void GetThreadIdReturnsStoredFoundryThread()
    {
        CopilotChatState state = new()
        {
            FoundryThreadId = "thread-123",
        };

        Assert.Equal("thread-123", FoundrySpecialistConversationStore.GetThreadId(state));
    }

    [Fact]
    public void SetThreadIdUpdatesOnlyFoundryThreadId()
    {
        CopilotChatState state = new()
        {
            CopilotSessionId = "session-456",
            Transcript = [new("user", "hello")],
        };

        CopilotChatState updated = FoundrySpecialistConversationStore.SetThreadId(state, "thread-123");

        Assert.Equal("thread-123", updated.FoundryThreadId);
        Assert.Equal("session-456", updated.CopilotSessionId);
        Assert.Collection(
            updated.Transcript,
            entry =>
            {
                Assert.Equal("user", entry.Role);
                Assert.Equal("hello", entry.Content);
            });
    }
}
