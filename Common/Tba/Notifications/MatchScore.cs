namespace Common.Tba.Notifications;

using System.Text.Json.Serialization;

using Match = Api.Models.Match;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record MatchScore(string? event_key, string? match_key, string? team_key, string? event_name) : IRequireCombinedSerialization<Match>
{
    public Match? match { get; set; }

    [JsonIgnore, JsonPropertyName(nameof(match))]
    public Match? Model { get => match; private set => match = value; }
}
