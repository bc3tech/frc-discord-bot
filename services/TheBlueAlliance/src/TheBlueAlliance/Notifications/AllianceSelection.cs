namespace TheBlueAlliance.Api.Notifications;

using System.Text.Json.Serialization;

using TheBlueAlliance.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AllianceSelection(string event_key, string? team_key, string event_name, [property: JsonPropertyName("event")] Event? Event);