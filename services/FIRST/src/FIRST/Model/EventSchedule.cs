namespace FIRST.Model;

using System.Text.Json.Serialization;

public sealed record EventSchedule
{
    [JsonPropertyName("Schedule")]
    public ScheduleObj[] Schedule { get; init; } = [];

    public sealed record ScheduleObj
    {
        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset? StartTime { get; init; }

        [JsonPropertyName("matchNumber")]
        public int MatchNumber { get; init; }

        [JsonPropertyName("field")]
        public string? Field { get; init; }

        [JsonPropertyName("tournamentLevel"), JsonConverter(typeof(JsonStringEnumConverter<TournamentLevel>))]
        public TournamentLevel? TournamentLevel { get; init; }

        [JsonPropertyName("teams")]
        public TeamObj[] Teams { get; init; } = [];
    }

    public sealed record TeamObj
    {
        [JsonPropertyName("teamNumber")]
        public int TeamNumber { get; init; }

        [JsonPropertyName("station")]
        public string? Station { get; init; }

        [JsonPropertyName("surrogate")]
        public bool Surrogate { get; init; }
    }
}
