namespace TheBlueAlliance.Api.Notifications;

using TheBlueAlliance.Api.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
public record AwardsPosted(string event_key, string? team_key, string event_name, Award[]? awards);