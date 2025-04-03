namespace FIRST.Model;
using System.Text.Json.Serialization;

public sealed record SeasonRankingsDistrict
{
    [JsonPropertyName("districtRanks")]
    public DistrictRank[] DistrictRanks { get; init; } = [];

    [JsonPropertyName("rankingCountTotal")]
    public int RankingCountTotal { get; init; }

    [JsonPropertyName("rankingCountPage")]
    public int RankingCountPage { get; init; }

    [JsonPropertyName("pageCurrent")]
    public int PageCurrent { get; init; }

    [JsonPropertyName("pageTotal")]
    public int PageTotal { get; init; }

    public sealed record DistrictRank
    {
        [JsonPropertyName("districtCode")]
        public string? DistrictCode { get; init; }

        [JsonPropertyName("teamNumber")]
        public int TeamNumber { get; init; }

        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("totalPoints")]
        public int TotalPoints { get; init; }

        [JsonPropertyName("event1Code")]
        public string? Event1Code { get; init; }

        [JsonPropertyName("event1Points")]
        public float Event1Points { get; init; }

        [JsonPropertyName("event2Code")]
        public string? Event2Code { get; init; }

        [JsonPropertyName("event2Points")]
        public float Event2Points { get; init; }

        [JsonPropertyName("districtCmpCode")]
        public object? DistrictCmpCode { get; init; }

        [JsonPropertyName("districtCmpPoints")]
        public float DistrictCmpPoints { get; init; }

        [JsonPropertyName("teamAgePoints")]
        public int TeamAgePoints { get; init; }

        [JsonPropertyName("adjustmentPoints")]
        public int AdjustmentPoints { get; init; }

        [JsonPropertyName("qualifiedDistrictCmp")]
        public bool QualifiedDistrictCmp { get; init; }

        [JsonPropertyName("qualifiedFirstCmp")]
        public bool QualifiedFirstCmp { get; init; }
    }
}
