namespace Common.Tba.Notifications;

using Common.Tba.Api.Models;

using System.Text.Json.Serialization;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record UpcomingMatch(string event_key, string match_key, string? team_key, string event_name, string[] team_keys, int? scheduled_time, int? predicted_time) : IRequireCombinedSerialization<Webcast>
{
    public Webcast? webcast { get; set; }

    [JsonIgnore, JsonPropertyName("webcast")]
    public Webcast? Model { get => webcast; set => webcast = value; }
}