namespace DiscordBotFunctionApp.Extensions;
using System.Drawing;

internal static class DiscordExtensions
{
    public static Discord.Color ToDiscordColor(this Color systemColor) => new(systemColor.R, systemColor.G, systemColor.B);
}
