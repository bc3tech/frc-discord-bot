namespace Statbotics.Model;

using System;
using System.Text.Json.Serialization;

public sealed record TeamYear
{
    [JsonPropertyName("team")]
    public int Team { get; init; }

    [JsonPropertyName("year")]
    public int Year { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("district")]
    public string? District { get; init; }

    [JsonPropertyName("epa")]
    public EpaObj? Epa { get; init; }

    [JsonPropertyName("record")]
    public RecordObj? Record { get; init; }

    [JsonPropertyName("district_points")]
    public int DistrictPoints { get; init; }

    [JsonPropertyName("district_rank")]
    public int DistrictRank { get; init; }

    [JsonPropertyName("competing")]
    public CompetingObj? Competing { get; init; }

    public sealed record EpaObj
    {
        [JsonPropertyName("total_points")]
        public TotalPointsObj? TotalPoints { get; init; }

        [JsonPropertyName("unitless")]
        public float Unitless { get; init; }

        [JsonPropertyName("norm")]
        public float Norm { get; init; }

        [JsonPropertyName("conf")]
        public float[] Conf { get; init; } = Array.Empty<float>();

        [JsonPropertyName("breakdown")]
        public BreakdownObj? Breakdown { get; init; }

        [JsonPropertyName("stats")]
        public StatsObj? Stats { get; init; }

        [JsonPropertyName("ranks")]
        public RanksObj? Ranks { get; init; }
    }

    public sealed record TotalPointsObj
    {
        [JsonPropertyName("mean")]
        public float Mean { get; init; }

        [JsonPropertyName("sd")]
        public float Sd { get; init; }
    }

    public sealed record BreakdownObj
    {
        [JsonPropertyName("total_points")]
        public float TotalPoints { get; init; }

        [JsonPropertyName("auto_points")]
        public float AutoPoints { get; init; }

        [JsonPropertyName("teleop_points")]
        public float TeleopPoints { get; init; }

        [JsonPropertyName("endgame_points")]
        public float EndgamePoints { get; init; }

        [JsonPropertyName("auto_rp")]
        public float AutoRp { get; init; }

        [JsonPropertyName("coral_rp")]
        public float CoralRp { get; init; }

        [JsonPropertyName("barge_rp")]
        public float BargeRp { get; init; }

        [JsonPropertyName("tiebreaker_points")]
        public float TiebreakerPoints { get; init; }

        [JsonPropertyName("auto_leave_points")]
        public float AutoLeavePoints { get; init; }

        [JsonPropertyName("auto_coral")]
        public float AutoCoral { get; init; }

        [JsonPropertyName("auto_coral_points")]
        public float AutoCoralPoints { get; init; }

        [JsonPropertyName("teleop_coral")]
        public float TeleopCoral { get; init; }

        [JsonPropertyName("teleop_coral_points")]
        public float TeleopCoralPoints { get; init; }

        [JsonPropertyName("coral_l1")]
        public float CoralL1 { get; init; }

        [JsonPropertyName("coral_l2")]
        public float CoralL2 { get; init; }

        [JsonPropertyName("coral_l3")]
        public float CoralL3 { get; init; }

        [JsonPropertyName("coral_l4")]
        public float CoralL4 { get; init; }

        [JsonPropertyName("total_coral_points")]
        public float TotalCoralPoints { get; init; }

        [JsonPropertyName("processor_algae")]
        public float ProcessorAlgae { get; init; }

        [JsonPropertyName("processor_algae_points")]
        public float ProcessorAlgaePoints { get; init; }

        [JsonPropertyName("net_algae")]
        public float NetAlgae { get; init; }

        [JsonPropertyName("net_algae_points")]
        public float NetAlgaePoints { get; init; }

        [JsonPropertyName("total_algae_points")]
        public float TotalAlgaePoints { get; init; }

        [JsonPropertyName("total_game_pieces")]
        public float TotalGamePieces { get; init; }

        [JsonPropertyName("barge_points")]
        public float BargePoints { get; init; }

        [JsonPropertyName("rp_1")]
        public float Rp1 { get; init; }

        [JsonPropertyName("rp_2")]
        public float Rp2 { get; init; }

        [JsonPropertyName("rp_3")]
        public float Rp3 { get; init; }
    }

    public sealed record StatsObj
    {
        [JsonPropertyName("start")]
        public float Start { get; init; }

        [JsonPropertyName("pre_champs")]
        public float PreChamps { get; init; }

        [JsonPropertyName("max")]
        public float Max { get; init; }
    }

    public sealed record RanksObj
    {
        [JsonPropertyName("total")]
        public TotalObj? Total { get; init; }

        [JsonPropertyName("country")]
        public CountryObj? Country { get; init; }

        [JsonPropertyName("state")]
        public StateObj? State { get; init; }

        [JsonPropertyName("district")]
        public DistrictObj? District { get; init; }
    }

    public sealed record TotalObj
    {
        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("percentile")]
        public float Percentile { get; init; }

        [JsonPropertyName("team_count")]
        public int TeamCount { get; init; }
    }

    public sealed record CountryObj
    {
        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("percentile")]
        public float Percentile { get; init; }

        [JsonPropertyName("team_count")]
        public int TeamCount { get; init; }
    }

    public sealed record StateObj
    {
        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("percentile")]
        public float Percentile { get; init; }

        [JsonPropertyName("team_count")]
        public int TeamCount { get; init; }
    }

    public sealed record DistrictObj
    {
        [JsonPropertyName("rank")]
        public int Rank { get; init; }

        [JsonPropertyName("percentile")]
        public float Percentile { get; init; }

        [JsonPropertyName("team_count")]
        public int TeamCount { get; init; }
    }

    public sealed record RecordObj
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

    public sealed record CompetingObj
    {
        [JsonPropertyName("this_week")]
        public bool ThisWeek { get; init; }

        [JsonPropertyName("next_event_key")]
        public string? NextEventKey { get; init; }

        [JsonPropertyName("next_event_name")]
        public string? NextEventName { get; init; }

        [JsonPropertyName("next_event_week")]
        public int NextEventWeek { get; init; }
    }
}
