namespace FunctionApp.TbaInterop.Models.Notifications;

using System.Text.Json.Serialization;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public readonly record struct AllianceSelection(string event_key, string? team_key, string event_name, [property: JsonPropertyName("event")] AllianceSelectionEvent? Event);

public readonly record struct AllianceSelectionEvent(
    [property: JsonPropertyName("key")] string? Key,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("short_name")] string? ShortName);
