namespace FunctionApp.TbaInterop.Models.Notifications;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Reduce boilerplate by just matching exact JSON body")]
internal readonly record struct CompLevelStarting(string event_name, string comp_level, string event_key, int? scheduled_time);
