namespace DiscordBotFunctionApp.TbaInterop.Models.Notifications;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Reduce boilerplate by just matching exact JSON body")]
internal readonly record struct Ping(string title, string desc);
