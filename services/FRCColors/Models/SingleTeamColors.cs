namespace FRCColors.Models;
using System.Text.Json.Serialization;

public sealed record SingleTeamColors
{
    [JsonPropertyName("teamNumber")]
    public int TeamNumber { get; init; }

    [JsonPropertyName("colors")]
    public ColorsObj? Colors { get; init; }

    public sealed record ColorsObj
    {
        [JsonPropertyName("primaryHex")]
        public string? PrimaryHex { get; init; }

        [JsonPropertyName("secondaryHex")]
        public string? SecondaryHex { get; init; }

        [JsonPropertyName("verified")]
        public bool Verified { get; init; }
    }
}

