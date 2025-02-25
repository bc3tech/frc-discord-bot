namespace Statbotics.Model;

using System.Text.Json.Serialization;

public sealed record TeamEventStats
{
    [JsonPropertyName("team")]
    public int Team { get; init; }

    [JsonPropertyName("year")]
    public int Year { get; init; }

    [JsonPropertyName("event")]
    public string Event { get; init; }

    [JsonPropertyName("time")]
    public int Time { get; init; }

    [JsonPropertyName("team_name")]
    public string TeamName { get; init; }

    [JsonPropertyName("event_name")]
    public string EventName { get; init; }

    [JsonPropertyName("country")]
    public string Country { get; init; }

    [JsonPropertyName("state")]
    public object State { get; init; }

    [JsonPropertyName("district")]
    public string District { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("week")]
    public int Week { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("first_event")]
    public bool FirstEvent { get; init; }

    [JsonPropertyName("epa")]
    public EpaObj Epa { get; init; }

    [JsonPropertyName("record")]
    public RecordObj Record { get; init; }

    [JsonPropertyName("district_points")]
    public int DistrictPoints { get; init; }

    public sealed record EpaObj
    {
        [JsonPropertyName("total_points")]
        public TotalPointsObj TotalPoints { get; init; }

        [JsonPropertyName("unitless")]
        public float Unitless { get; init; }

        [JsonPropertyName("norm")]
        public float Norm { get; init; }

        [JsonPropertyName("conf")]
        public float[] Conf { get; init; }

        [JsonPropertyName("breakdown")]
        public BreakdownObj Breakdown { get; init; }

        [JsonPropertyName("stats")]
        public StatsObj Stats { get; init; }

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

            [JsonPropertyName("pre_elim")]
            public float PreElim { get; init; }

            [JsonPropertyName("mean")]
            public float Mean { get; init; }

            [JsonPropertyName("max")]
            public float Max { get; init; }
        }
    }

    public sealed record RecordObj
    {
        [JsonPropertyName("qual")]
        public QualRecord Qual { get; init; }

        [JsonPropertyName("elim")]
        public ElimRecord Elim { get; init; }

        [JsonPropertyName("total")]
        public TotalRecord Total { get; init; }

        public sealed record QualRecord
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

            [JsonPropertyName("rps")]
            public int Rps { get; init; }

            [JsonPropertyName("rps_per_match")]
            public float RpsPerMatch { get; init; }

            [JsonPropertyName("rank")]
            public int Rank { get; init; }

            [JsonPropertyName("num_teams")]
            public int NumTeams { get; init; }
        }

        public sealed record ElimRecord
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

            [JsonPropertyName("alliance")]
            public object Alliance { get; init; }

            [JsonPropertyName("is_captain")]
            public object IsCaptain { get; init; }
        }

        public sealed record TotalRecord
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
    }
}
