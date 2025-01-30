namespace DiscordBotFunctionApp.StatboticsInterop.Models;

using Common.JsonConverters;

using System.Text.Json.Serialization;

internal sealed record Event
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("year")]
    public int Year { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("time"), JsonConverter(typeof(UnixEpochDateTimeConverter))]
    public DateTime Time { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("district")]
    public string? District { get; init; }

    [JsonPropertyName("start_date")]
    public DateTime? StartDate { get; init; }

    [JsonPropertyName("end_date")]
    public DateTime? EndDate { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("week")]
    public int Week { get; init; }

    [JsonPropertyName("offseason")]
    public bool Offseason { get; init; }

    [JsonPropertyName("video")]
    public string? Video { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("status_str")]
    public string? StatusStr { get; init; }

    [JsonPropertyName("num_teams")]
    public int NumTeams { get; init; }

    [JsonPropertyName("current_match")]
    public int CurrentMatch { get; init; }

    [JsonPropertyName("qual_matches")]
    public int QualMatches { get; init; }

    [JsonPropertyName("epa")]
    public Epa? EpaVal { get; init; }

    [JsonPropertyName("metrics")]
    public Metrics? MetricsVal { get; init; }

    public string GetEventStatusStr()
    {
        if (Status is not "Ongoing")
            return Status ?? string.Empty;

        if (QualMatches is 0)
            return "Scheduled Unreleased";
        else if (CurrentMatch is 0)
            return "Schedule Released";
        else if (CurrentMatch < QualMatches)
            return $"Qual {CurrentMatch}";
        else if (CurrentMatch == QualMatches)
            return "Quals Over";
        else
            return "Elims Ongoing";
    }

    internal sealed record Epa
    {
        [JsonPropertyName("max")]
        public float Max { get; init; }

        [JsonPropertyName("top_8")]
        public float Top8 { get; init; }

        [JsonPropertyName("top_24")]
        public float Top24 { get; init; }

        [JsonPropertyName("mean")]
        public float Mean { get; init; }

        [JsonPropertyName("sd")]
        public float Sd { get; init; }
    }

    internal sealed record Metrics
    {
        [JsonPropertyName("win_prob")]
        public WinProb? WinProb { get; init; }

        [JsonPropertyName("score_pred")]
        public ScorePred? ScorePred { get; init; }

        [JsonPropertyName("rp_pred")]
        public RpPred? RpPred { get; init; }
    }

    internal sealed record WinProb
    {
        [JsonPropertyName("count")]
        public int Count { get; init; }

        [JsonPropertyName("conf")]
        public float Conf { get; init; }

        [JsonPropertyName("acc")]
        public float Acc { get; init; }

        [JsonPropertyName("mse")]
        public float Mse { get; init; }
    }

    internal sealed record ScorePred
    {
        [JsonPropertyName("count")]
        public int Count { get; init; }

        [JsonPropertyName("rmse")]
        public float Rmse { get; init; }

        [JsonPropertyName("mae")]
        public float Mae { get; init; }

        [JsonPropertyName("error")]
        public float Error { get; init; }
    }

    internal sealed record RpPred
    {
        [JsonPropertyName("count")]
        public int Count { get; init; }

        [JsonPropertyName("melody_rp")]
        public MelodyRp? MelodyRp { get; init; }

        [JsonPropertyName("harmony_rp")]
        public HarmonyRp? HarmonyRp { get; init; }
    }

    internal sealed record MelodyRp
    {
        [JsonPropertyName("error")]
        public float Error { get; init; }

        [JsonPropertyName("acc")]
        public float Acc { get; init; }

        [JsonPropertyName("ll")]
        public float Ll { get; init; }

        [JsonPropertyName("f1")]
        public float F1 { get; init; }
    }

    internal sealed record HarmonyRp
    {
        [JsonPropertyName("error")]
        public float Error { get; init; }

        [JsonPropertyName("acc")]
        public float Acc { get; init; }

        [JsonPropertyName("ll")]
        public float Ll { get; init; }

        [JsonPropertyName("f1")]
        public float F1 { get; init; }
    }
}
