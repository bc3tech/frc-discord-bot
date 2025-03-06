namespace DiscordBotFunctionApp.Extensions;

using Discord;

internal static class DiscordExtensions
{
    public static Color ToDiscordColor(this System.Drawing.Color systemColor) => new(systemColor.R, systemColor.G, systemColor.B);

    public static RequestOptions ToRequestOptions(this CancellationToken cancellationToken) => new() { CancelToken = cancellationToken };
}
