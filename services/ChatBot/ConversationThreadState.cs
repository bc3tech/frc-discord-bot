namespace ChatBot;

using ChatBot.Copilot;

using Microsoft.Agents.AI;

using System.Text.Json;

internal static class ConversationThreadState
{
    private static readonly JsonSerializerOptions s_serializerOptions = new(JsonSerializerDefaults.Web);

    public static string GetThreadId(AgentSession session)
        => session is ChatClientAgentSession { ConversationId: { Length: > 0 } conversationId }
            ? conversationId
            : throw new InvalidOperationException("Agent session did not expose a Foundry thread id.");

    public static CopilotChatState Parse(string? storedConversationState)
    {
        if (string.IsNullOrWhiteSpace(storedConversationState))
        {
            return new();
        }

        string trimmed = storedConversationState.Trim();
        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
        {
            return new()
            {
                FoundryThreadId = trimmed,
            };
        }

        try
        {
            CopilotChatState? parsedState = JsonSerializer.Deserialize<CopilotChatState>(trimmed, s_serializerOptions);
            if (parsedState is not null)
            {
                return Normalize(parsedState);
            }
        }
        catch (JsonException)
        {
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(trimmed);
            string? threadId = TryExtractThreadId(document.RootElement);
            return string.IsNullOrWhiteSpace(threadId)
                ? new()
                : new()
                {
                    FoundryThreadId = threadId,
                };
        }
        catch (JsonException)
        {
            return new();
        }
    }

    public static string Serialize(CopilotChatState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return JsonSerializer.Serialize(Normalize(state), s_serializerOptions);
    }

    public static string? TryExtractThreadId(string? storedConversationState)
    {
        return Parse(storedConversationState).FoundryThreadId;
    }

    public static string? TryExtractThreadId(JsonElement element)
        => element.ValueKind switch
        {
            JsonValueKind.Object => TryExtractThreadIdFromObject(element),
            JsonValueKind.Array => TryExtractThreadIdFromArray(element),
            _ => null,
        };

    private static string? TryExtractThreadIdFromObject(JsonElement element)
    {
        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (property.Value.ValueKind is JsonValueKind.String
                && (property.NameEquals("threadId") || property.NameEquals("conversationId")))
            {
                return property.Value.GetString();
            }

            string? nested = TryExtractThreadId(property.Value);
            if (!string.IsNullOrWhiteSpace(nested))
            {
                return nested;
            }
        }

        return null;
    }

    private static string? TryExtractThreadIdFromArray(JsonElement element)
    {
        foreach (JsonElement child in element.EnumerateArray())
        {
            string? nested = TryExtractThreadId(child);
            if (!string.IsNullOrWhiteSpace(nested))
            {
                return nested;
            }
        }

        return null;
    }

    private static CopilotChatState Normalize(CopilotChatState state)
        => state with
        {
            Version = CopilotChatState.CurrentVersion,
            Transcript = CopilotTranscriptWindow.Normalize(state.Transcript),
        };
}
