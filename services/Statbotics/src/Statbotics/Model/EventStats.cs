namespace Statbotics.Model;

using System.Text.Json.Serialization;

public sealed record EventStats
{
    [JsonPropertyName("key")]
    public string Key { get; init; }

    [JsonPropertyName("year")]
    public int Year { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("time")]
    public int Time { get; init; }

    [JsonPropertyName("country")]
    public string Country { get; init; }

    [JsonPropertyName("state")]
    public object State { get; init; }

    [JsonPropertyName("district")]
    public string District { get; init; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; init; }

    [JsonPropertyName("end_date")]
    public string EndDate { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("week")]
    public int Week { get; init; }

    [JsonPropertyName("video")]
    public string Video { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("status_str")]
    public string StatusStr { get; init; }

    [JsonPropertyName("num_teams")]
    public int NumTeams { get; init; }

    [JsonPropertyName("current_match")]
    public int CurrentMatch { get; init; }

    [JsonPropertyName("qual_matches")]
    public int QualMatches { get; init; }

    [JsonPropertyName("epa")]
    public EpaObj Epa { get; init; }

    [JsonPropertyName("metrics")]
    public MetricsObj Metrics { get; init; }

    public sealed record EpaObj
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

    public sealed record MetricsObj
    {
        [JsonPropertyName("win_prob")]
        public WinProbObj WinProb { get; init; }

        [JsonPropertyName("score_pred")]
        public ScorePredObj ScorePred { get; init; }

        [JsonPropertyName("rp_pred")]
        public RpPredObj RpPred { get; init; }

        public sealed record WinProbObj
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

        public sealed record ScorePredObj
        {
            [JsonPropertyName("count")]
            public int Count { get; init; }

            [JsonPropertyName("rmse")]
            public float Rmse { get; init; }

            [JsonPropertyName("error")]
            public float Error { get; init; }
        }

        public sealed record RpPredObj
        {
            [JsonPropertyName("count")]
            public int Count { get; init; }

            [JsonPropertyName("auto_rp")]
            public AutoRpObj AutoRp { get; init; }

            [JsonPropertyName("coral_rp")]
            public CoralRpObj CoralRp { get; init; }

            [JsonPropertyName("barge_rp")]
            public BargeRpObj[] BargeRp { get; init; }

            public sealed record AutoRpObj
            {
                [JsonPropertyName("error")]
                public float Error { get; init; }

                [JsonPropertyName("acc")]
                public float Acc { get; init; }
            }

            public sealed record CoralRpObj
            {
                [JsonPropertyName("error")]
                public float Error { get; init; }

                [JsonPropertyName("acc")]
                public float Acc { get; init; }
            }

            public sealed record BargeRpObj
            {
                [JsonPropertyName("error")]
                public float Error { get; init; }

                [JsonPropertyName("acc")]
                public float Acc { get; init; }
            }
        }
    }
}
