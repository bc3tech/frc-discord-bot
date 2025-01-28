namespace TheBlueAlliance.Api.Notifications;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record MatchVideo(string event_key, string match_key, string? team_key, string event_name, Model.Match? match) : MatchScore(event_key, match_key, team_key, event_name, match);
