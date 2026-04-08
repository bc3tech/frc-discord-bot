namespace ChatBot.Diagnostics;

using System.Text.Json;

internal static class LogSerialization
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
    };

    public static string Serialize(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is string text)
        {
            return text;
        }

        try
        {
            return JsonSerializer.Serialize(value, JsonOptions);
        }
        catch (NotSupportedException ex)
        {
            return JsonSerializer.Serialize(
                new UnsupportedPayloadSummary(
                    value.GetType().FullName ?? value.GetType().Name,
                    ex.Message),
                JsonOptions);
        }
    }

    private sealed record UnsupportedPayloadSummary(string Type, string Error);
}
