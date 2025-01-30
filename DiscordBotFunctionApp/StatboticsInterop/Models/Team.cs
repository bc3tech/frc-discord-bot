namespace DiscordBotFunctionApp.StatboticsInterop.Models;

using System.Text.Json.Serialization;

internal sealed record Team
{
    [JsonPropertyName("team")]
    public string Number { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("country")]
    public string Country { get; init; }

    [JsonPropertyName("state")]
    public string State { get; init; }

    [JsonPropertyName("district")]
    public string District { get; init; }

    [JsonPropertyName("rookie_year")]
    public int RookieYear { get; init; }

    [JsonPropertyName("offseason")]
    public bool Offseason { get; init; }

    [JsonPropertyName("active")]
    public bool Active { get; init; }

    [JsonPropertyName("colors")]
    public Colors Colors { get; init; }

    [JsonPropertyName("record")]
    public Records Records { get; init; }

    [JsonPropertyName("norm_epa")]
    public NormEpa NormEpa { get; init; }
}

internal sealed record Colors
{
    [JsonPropertyName("primary")]
    public string Primary { get; init; }

    [JsonPropertyName("secondary")]
    public string Secondary { get; init; }
}

internal sealed record Records
{
    [JsonPropertyName("season")]
    public Record Season { get; init; }

    [JsonPropertyName("full")]
    public Record Full { get; init; }
}

internal sealed record Record
{
    [JsonPropertyName("wins")]
    public int Wins { get; init; }

    [JsonPropertyName("losses")]
    public int Losses { get; init; }

    [JsonPropertyName("ties")]
    public int Ties { get; init; }

    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("winrate")]
    public float Winrate { get; init; }
}

internal sealed record NormEpa
{
    [JsonPropertyName("current")]
    public float Current { get; init; }

    [JsonPropertyName("recent")]
    public float Recent { get; init; }

    [JsonPropertyName("mean")]
    public float Mean { get; init; }

    [JsonPropertyName("max")]
    public float Max { get; init; }
}
