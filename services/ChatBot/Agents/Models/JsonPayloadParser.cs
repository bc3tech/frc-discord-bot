namespace ChatBot.Agents.Models;

using System.Text;
using System.Text.Json;

internal static class JsonPayloadParser
{
    public static bool TryParse<T>(
        string payload,
        JsonSerializerOptions options,
        out T? result,
        out JsonException? exception,
        out bool recoveredMalformedJson)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            result = default;
            exception = new JsonException("The JSON payload was empty.");
            recoveredMalformedJson = false;
            return false;
        }

        if (TryDeserialize(payload, options, out result, out exception))
        {
            recoveredMalformedJson = false;
            return true;
        }

        string normalizedPayload = Normalize(payload);
        if (!string.Equals(normalizedPayload, payload, StringComparison.Ordinal)
            && TryDeserialize(normalizedPayload, options, out result, out exception))
        {
            recoveredMalformedJson = true;
            return true;
        }

        recoveredMalformedJson = false;
        return false;
    }

    private static bool TryDeserialize<T>(
        string payload,
        JsonSerializerOptions options,
        out T? result,
        out JsonException? exception)
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(payload, options);
            exception = null;
            return result is not null;
        }
        catch (JsonException ex)
        {
            result = default;
            exception = ex;
            return false;
        }
    }

    private static string Normalize(string payload)
    {
        string candidate = StripMarkdownCodeFence(payload).Trim();
        int firstBrace = candidate.IndexOf('{', StringComparison.Ordinal);
        int lastBrace = candidate.LastIndexOf('}');
        if (firstBrace >= 0 && lastBrace > firstBrace)
        {
            candidate = candidate[firstBrace..(lastBrace + 1)];
        }

        StringBuilder builder = new(candidate.Length);
        bool inString = false;
        bool isEscaped = false;

        foreach (char character in candidate)
        {
            if (inString)
            {
                if (isEscaped)
                {
                    builder.Append(character);
                    isEscaped = false;
                    continue;
                }

                switch (character)
                {
                    case '\\':
                        builder.Append(character);
                        isEscaped = true;
                        continue;
                    case '"':
                        builder.Append(character);
                        inString = false;
                        continue;
                    case '\r':
                        builder.Append("\\r");
                        continue;
                    case '\n':
                        builder.Append("\\n");
                        continue;
                    case '\t':
                        builder.Append("\\t");
                        continue;
                    default:
                        builder.Append(character);
                        continue;
                }
            }

            builder.Append(character);
            if (character == '"')
            {
                inString = true;
            }
        }

        return builder.ToString();
    }

    private static string StripMarkdownCodeFence(string payload)
    {
        string trimmed = payload.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        int firstNewLine = trimmed.IndexOf('\n');
        if (firstNewLine < 0)
        {
            return trimmed;
        }

        int lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        return lastFence <= firstNewLine ? trimmed : trimmed[(firstNewLine + 1)..lastFence];
    }
}
