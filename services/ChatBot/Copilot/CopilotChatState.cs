namespace ChatBot.Copilot;

internal sealed record CopilotChatState
{
    public const int CurrentVersion = 1;

    public int Version { get; init; } = CurrentVersion;

    public string? CopilotSessionId { get; init; }

    public string? FoundryThreadId { get; init; }

    public IReadOnlyList<CopilotTranscriptMessage> Transcript { get; init; } = [];
}

internal sealed record CopilotTranscriptMessage(string Role, string Content);
