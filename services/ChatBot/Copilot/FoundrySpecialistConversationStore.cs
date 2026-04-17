namespace ChatBot.Copilot;

internal static class FoundrySpecialistConversationStore
{
    public static string? GetThreadId(CopilotChatState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return state.FoundryThreadId;
    }

    public static CopilotChatState SetThreadId(CopilotChatState state, string threadId)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(threadId);

        return state with
        {
            FoundryThreadId = threadId,
        };
    }
}
