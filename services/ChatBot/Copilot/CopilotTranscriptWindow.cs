namespace ChatBot.Copilot;

using Microsoft.Extensions.AI;

internal static class CopilotTranscriptWindow
{
    private const int MaxMessages = 12;

    public static IReadOnlyList<CopilotTranscriptMessage> Normalize(IReadOnlyList<CopilotTranscriptMessage>? transcript)
    {
        if (transcript is null || transcript.Count is 0)
        {
            return [];
        }

        List<CopilotTranscriptMessage> normalized = [];
        foreach (CopilotTranscriptMessage message in transcript)
        {
            if (string.IsNullOrWhiteSpace(message.Role) || string.IsNullOrWhiteSpace(message.Content))
            {
                continue;
            }

            normalized.Add(new(message.Role.Trim(), message.Content.Trim()));
        }

        return Trim(normalized);
    }

    public static IReadOnlyList<CopilotTranscriptMessage> AppendTurn(
        IReadOnlyList<CopilotTranscriptMessage>? transcript,
        string userMessage,
        string assistantMessage)
        => Normalize(
        [
            .. transcript ?? [],
            new("user", userMessage),
            new("assistant", assistantMessage),
        ]);

    public static IReadOnlyList<ChatMessage> ToChatMessages(IReadOnlyList<CopilotTranscriptMessage>? transcript)
    {
        IReadOnlyList<CopilotTranscriptMessage> normalized = Normalize(transcript);
        if (normalized.Count is 0)
        {
            return [];
        }

        return [.. normalized.Select(static message => new ChatMessage(ParseRole(message.Role), message.Content))];
    }

    private static IReadOnlyList<CopilotTranscriptMessage> Trim(IReadOnlyList<CopilotTranscriptMessage> transcript)
        => transcript.Count <= MaxMessages
            ? transcript
            : [.. transcript.Skip(transcript.Count - MaxMessages)];

    private static ChatRole ParseRole(string role)
        => role.ToLowerInvariant() switch
        {
            "assistant" => ChatRole.Assistant,
            "system" => ChatRole.System,
            _ => ChatRole.User,
        };
}
