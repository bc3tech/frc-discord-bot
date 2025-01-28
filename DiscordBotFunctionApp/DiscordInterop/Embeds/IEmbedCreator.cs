namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

using DiscordBotFunctionApp.TbaInterop.Models;

internal interface IEmbedCreator
{
    IAsyncEnumerable<Embed> CreateAsync(WebhookMessage notification, ushort? highlightTeam = null, CancellationToken cancellationToken = default);
}