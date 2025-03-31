namespace Statbotics.Model;
using System.Text.Json.Serialization;

public sealed record Match
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("year")]
    public int? Year { get; init; }

    [JsonPropertyName("event")]
    public string? Event { get; init; }

    [JsonPropertyName("week")]
    public int? Week { get; init; }

    [JsonPropertyName("elim")]
    public bool Elim { get; init; }

    [JsonPropertyName("comp_level")]
    public string? CompLevel { get; init; }

    [JsonPropertyName("set_number")]
    public int? SetNumber { get; init; }

    [JsonPropertyName("match_number")]
    public int? MatchNumber { get; init; }

    [JsonPropertyName("match_name")]
    public string? MatchName { get; init; }

    [JsonPropertyName("time")]
    public int? Time { get; init; }

    [JsonPropertyName("predicted_time")]
    public int? PredictedTime { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("video")]
    public object? Video { get; init; }

    [JsonPropertyName("alliances")]
    public AlliancesObj? Alliances { get; init; }

    [JsonPropertyName("pred")]
    public PredObj? Pred { get; init; }

    [JsonPropertyName("result")]
    public ResultObj? Result { get; init; }

    public sealed record AlliancesObj
    {
        [JsonPropertyName("red")]
        public Alliance? Red { get; init; }

        [JsonPropertyName("blue")]
        public Alliance? Blue { get; init; }

        public sealed record Alliance
        {
            [JsonPropertyName("team_keys")]
            public ushort[]? TeamKeys { get; init; }

            [JsonPropertyName("surrogate_team_keys")]
            public ushort[]? SurrogateTeamKeys { get; init; }

            [JsonPropertyName("dq_team_keys")]
            public ushort[]? DqTeamKeys { get; init; }
        }
    }

    public sealed record PredObj
    {
        [JsonPropertyName("winner")]
        public string? Winner { get; init; }

        [JsonPropertyName("red_win_prob")]
        public float? RedWinProb { get; init; }

        [JsonPropertyName("red_score")]
        public float? RedScore { get; init; }

        [JsonPropertyName("blue_score")]
        public float? BlueScore { get; init; }

        [JsonPropertyName("red_auto_rp")]
        public float? RedAutoRp { get; init; }

        [JsonPropertyName("blue_auto_rp")]
        public float? BlueAutoRp { get; init; }

        [JsonPropertyName("red_coral_rp")]
        public float? RedCoralRp { get; init; }

        [JsonPropertyName("blue_coral_rp")]
        public float? BlueCoralRp { get; init; }

        [JsonPropertyName("red_barge_rp")]
        public float? RedBargeRp { get; init; }

        [JsonPropertyName("blue_barge_rp")]
        public float? BlueBargeRp { get; init; }
    }

    public sealed record ResultObj
    {
        [JsonPropertyName("winner")]
        public string? Winner { get; init; }

        [JsonPropertyName("red_score")]
        public int? RedScore { get; init; }

        [JsonPropertyName("blue_score")]
        public int? BlueScore { get; init; }

        [JsonPropertyName("red_no_foul")]
        public int? RedNoFoul { get; init; }

        [JsonPropertyName("blue_no_foul")]
        public int? BlueNoFoul { get; init; }

        [JsonPropertyName("red_auto_points")]
        public int? RedAutoPoints { get; init; }

        [JsonPropertyName("blue_auto_points")]
        public int? BlueAutoPoints { get; init; }

        [JsonPropertyName("red_teleop_points")]
        public int? RedTeleopPoints { get; init; }

        [JsonPropertyName("blue_teleop_points")]
        public int? BlueTeleopPoints { get; init; }

        [JsonPropertyName("red_endgame_points")]
        public int? RedEndgamePoints { get; init; }

        [JsonPropertyName("blue_endgame_points")]
        public int? BlueEndgamePoints { get; init; }

        [JsonPropertyName("red_auto_rp")]
        public bool RedAutoRp { get; init; }

        [JsonPropertyName("blue_auto_rp")]
        public bool BlueAutoRp { get; init; }

        [JsonPropertyName("red_coral_rp")]
        public bool RedCoralRp { get; init; }

        [JsonPropertyName("blue_coral_rp")]
        public bool BlueCoralRp { get; init; }

        [JsonPropertyName("red_barge_rp")]
        public bool RedBargeRp { get; init; }

        [JsonPropertyName("blue_barge_rp")]
        public bool BlueBargeRp { get; init; }

        [JsonPropertyName("red_tiebreaker_points")]
        public int? RedTiebreakerPoints { get; init; }

        [JsonPropertyName("blue_tiebreaker_points")]
        public int? BlueTiebreakerPoints { get; init; }

        [JsonPropertyName("red_auto_leave_points")]
        public float? RedAutoLeavePoints { get; init; }

        [JsonPropertyName("blue_auto_leave_points")]
        public float? BlueAutoLeavePoints { get; init; }

        [JsonPropertyName("red_auto_coral")]
        public float? RedAutoCoral { get; init; }

        [JsonPropertyName("blue_auto_coral")]
        public float? BlueAutoCoral { get; init; }

        [JsonPropertyName("red_auto_coral_points")]
        public float? RedAutoCoralPoints { get; init; }

        [JsonPropertyName("blue_auto_coral_points")]
        public float? BlueAutoCoralPoints { get; init; }

        [JsonPropertyName("red_teleop_coral")]
        public float? RedTeleopCoral { get; init; }

        [JsonPropertyName("blue_teleop_coral")]
        public float? BlueTeleopCoral { get; init; }

        [JsonPropertyName("red_teleop_coral_points")]
        public float? RedTeleopCoralPoints { get; init; }

        [JsonPropertyName("blue_teleop_coral_points")]
        public float? BlueTeleopCoralPoints { get; init; }

        [JsonPropertyName("red_coral_l1")]
        public float? RedCoralL1 { get; init; }

        [JsonPropertyName("blue_coral_l1")]
        public float? BlueCoralL1 { get; init; }

        [JsonPropertyName("red_coral_l2")]
        public float? RedCoralL2 { get; init; }

        [JsonPropertyName("blue_coral_l2")]
        public float? BlueCoralL2 { get; init; }

        [JsonPropertyName("red_coral_l3")]
        public float? RedCoralL3 { get; init; }

        [JsonPropertyName("blue_coral_l3")]
        public float? BlueCoralL3 { get; init; }

        [JsonPropertyName("red_coral_l4")]
        public float? RedCoralL4 { get; init; }

        [JsonPropertyName("blue_coral_l4")]
        public float? BlueCoralL4 { get; init; }

        [JsonPropertyName("red_total_coral_points")]
        public float? RedTotalCoralPoints { get; init; }

        [JsonPropertyName("blue_total_coral_points")]
        public float? BlueTotalCoralPoints { get; init; }

        [JsonPropertyName("red_processor_algae")]
        public float? RedProcessorAlgae { get; init; }

        [JsonPropertyName("blue_processor_algae")]
        public float? BlueProcessorAlgae { get; init; }

        [JsonPropertyName("red_processor_algae_points")]
        public float? RedProcessorAlgaePoints { get; init; }

        [JsonPropertyName("blue_processor_algae_points")]
        public float? BlueProcessorAlgaePoints { get; init; }

        [JsonPropertyName("red_net_algae")]
        public float? RedNetAlgae { get; init; }

        [JsonPropertyName("blue_net_algae")]
        public float? BlueNetAlgae { get; init; }

        [JsonPropertyName("red_net_algae_points")]
        public float? RedNetAlgaePoints { get; init; }

        [JsonPropertyName("blue_net_algae_points")]
        public float? BlueNetAlgaePoints { get; init; }

        [JsonPropertyName("red_total_algae_points")]
        public float? RedTotalAlgaePoints { get; init; }

        [JsonPropertyName("blue_total_algae_points")]
        public float? BlueTotalAlgaePoints { get; init; }

        [JsonPropertyName("red_total_game_pieces")]
        public float? RedTotalGamePieces { get; init; }

        [JsonPropertyName("blue_total_game_pieces")]
        public float? BlueTotalGamePieces { get; init; }

        [JsonPropertyName("red_barge_points")]
        public float? RedBargePoints { get; init; }

        [JsonPropertyName("blue_barge_points")]
        public float? BlueBargePoints { get; init; }

        [JsonPropertyName("red_rp_1")]
        public bool RedRp1 { get; init; }

        [JsonPropertyName("blue_rp_1")]
        public bool BlueRp1 { get; init; }

        [JsonPropertyName("red_rp_2")]
        public bool RedRp2 { get; init; }

        [JsonPropertyName("blue_rp_2")]
        public bool BlueRp2 { get; init; }

        [JsonPropertyName("red_rp_3")]
        public bool RedRp3 { get; init; }

        [JsonPropertyName("blue_rp_3")]
        public bool BlueRp3 { get; init; }
    }
}
