namespace FunctionApp.TbaInterop.Models.Notifications;

using System.Text.Json.Serialization;

using TheBlueAlliance.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
internal sealed record MatchVideo([property: JsonRequired] string event_key, [property: JsonRequired] string match_key, string? team_key = null, string? event_name = null, Match? match = null) : MatchScore(event_key, match_key, team_key, event_name, match);
