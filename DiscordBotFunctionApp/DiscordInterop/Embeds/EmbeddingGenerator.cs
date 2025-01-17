namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;
using Common.Tba;
using Common.Tba.Notifications;

using Discord;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;

internal sealed class EmbeddingGenerator(EmbedBuilderFactory embedBuilder, IServiceProvider services, ILogger<EmbeddingGenerator> logger)
{
    public async Task<Embed> CreateEmbeddingAsync(WebhookMessage tbaWebhookMessage, ushort? highlightTeam = null, CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        var embedCreator = services.GetKeyedService<IEmbedCreator>(tbaWebhookMessage.MessageType.ToInvariantString());
        if (embedCreator is null)
        {
            logger.LogWarning("No embedding creator registered for message type {MessageType}", tbaWebhookMessage.MessageType);
            return embedBuilder.GetBuilder().Build();
        }

        return await embedCreator.CreateAsync(tbaWebhookMessage, highlightTeam, cancellationToken).ConfigureAwait(false);
    }
}
