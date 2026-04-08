namespace ChatBot;

using Microsoft.Agents.AI;

using System.Text.Json;

internal static class ConversationThreadState
{

    public static string GetThreadId(AgentSession session)
        => session is ChatClientAgentSession { ConversationId: { Length: > 0 } conversationId }
            ? conversationId
            : throw new InvalidOperationException("Agent session did not expose a Foundry thread id.");

    public static string? TryExtractThreadId(string? storedConversationState)
    {
        if (string.IsNullOrWhiteSpace(storedConversationState))
        {
            return null;
        }

        string trimmed = storedConversationState.Trim();
        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
        {
            return trimmed;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(trimmed);
            return TryExtractThreadId(document.RootElement);
        }
        catch (JsonException)
        {
            return null;
        }
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
}
