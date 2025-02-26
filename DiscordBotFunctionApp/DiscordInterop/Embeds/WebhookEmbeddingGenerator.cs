namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Runtime.CompilerServices;

internal sealed class WebhookEmbeddingGenerator(EmbedBuilderFactory embedBuilder, IServiceProvider services, ILogger<WebhookEmbeddingGenerator> logger)
{
    public async IAsyncEnumerable<Embed> CreateEmbeddingsAsync(WebhookMessage tbaWebhookMessage, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        var embedCreator = services.GetKeyedService<INotificationEmbedCreator>(tbaWebhookMessage.MessageType.ToInvariantString());
        if (embedCreator is null)
        {
            logger.LogWarning("No embedding creator registered for message type {MessageType}", tbaWebhookMessage.MessageType);
            yield break;
        }

        logger.LogTrace("Generating embeddings for webhook message...");
        await foreach (var i in embedCreator.CreateAsync(tbaWebhookMessage, highlightTeam, cancellationToken))
        {
            yield return i;
        }
    }
}
