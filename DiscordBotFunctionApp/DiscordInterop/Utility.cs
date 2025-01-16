namespace DiscordBotFunctionApp.DiscordInterop;

using Discord;

internal static class Utility
{
    public static RequestOptions CreateCancelRequestOptions(CancellationToken cancellationToken) => new() { CancelToken = cancellationToken };
}
