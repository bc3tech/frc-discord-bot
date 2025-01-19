namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Tba;

using Discord;

using System.Runtime.CompilerServices;

internal interface IEmbedCreator
{
    IAsyncEnumerable<Embed> CreateAsync(WebhookMessage notification, ushort? highlightTeam = null, CancellationToken cancellationToken = default);
}