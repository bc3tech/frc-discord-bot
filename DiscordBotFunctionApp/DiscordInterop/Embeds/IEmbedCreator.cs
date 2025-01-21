namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

using TheBlueAlliance.Api;

internal interface IEmbedCreator
{
    IAsyncEnumerable<Embed> CreateAsync(WebhookMessage notification, ushort? highlightTeam = null, CancellationToken cancellationToken = default);
}