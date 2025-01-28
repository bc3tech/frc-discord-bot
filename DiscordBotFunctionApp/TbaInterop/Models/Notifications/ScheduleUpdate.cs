namespace DiscordBotFunctionApp.TbaInterop.Models.Notifications;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
internal sealed record ScheduleUpdate(string event_key, string event_name, int? first_match_time);
