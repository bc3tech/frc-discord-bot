namespace FIRST.Model;
using System.Text.Json.Serialization;

public sealed record SeasonRankingsEvent
{
    [JsonPropertyName("Rankings")]
    public Ranking[] Rankings { get; init; } = [];

    public sealed record Ranking
    {
        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("teamNumber")]
        public int TeamNumber { get; init; }

        [JsonPropertyName("sortOrder1")]
        public float SortOrder1 { get; init; }

        [JsonPropertyName("sortOrder2")]
        public float SortOrder2 { get; init; }

        [JsonPropertyName("sortOrder3")]
        public float SortOrder3 { get; init; }

        [JsonPropertyName("sortOrder4")]
        public float SortOrder4 { get; init; }

        [JsonPropertyName("sortOrder5")]
        public float SortOrder5 { get; init; }

        [JsonPropertyName("sortOrder6")]
        public float SortOrder6 { get; init; }

        [JsonPropertyName("wins")]
        public int Wins { get; init; }

        [JsonPropertyName("losses")]
        public int Losses { get; init; }

        [JsonPropertyName("ties")]
        public int Ties { get; init; }

        [JsonPropertyName("qualAverage")]
        public float QualAverage { get; init; }

        [JsonPropertyName("dq")]
        public int Dq { get; init; }

        [JsonPropertyName("matchesPlayed")]
        public int MatchesPlayed { get; init; }
    }
}

