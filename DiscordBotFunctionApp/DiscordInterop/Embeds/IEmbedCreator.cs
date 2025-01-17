namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Tba;

using Discord;

internal interface IEmbedCreator
{
    Task<Embed> CreateAsync(WebhookMessage notification, ushort? highlightTeam = null, CancellationToken cancellationToken = default);
}