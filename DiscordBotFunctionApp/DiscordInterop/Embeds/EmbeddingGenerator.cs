namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Runtime.CompilerServices;

using TheBlueAlliance.Api;
using TheBlueAlliance.Api.Notifications;

internal sealed class EmbeddingGenerator(EmbedBuilderFactory embedBuilder, IServiceProvider services, ILogger<EmbeddingGenerator> logger)
{
    public async IAsyncEnumerable<Embed> CreateEmbeddingsAsync(WebhookMessage tbaWebhookMessage, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        var embedCreator = services.GetKeyedService<IEmbedCreator>(tbaWebhookMessage.MessageType.ToInvariantString());
        if (embedCreator is null)
        {
            logger.LogWarning("No embedding creator registered for message type {MessageType}", tbaWebhookMessage.MessageType);
            yield return embedBuilder.GetBuilder().Build();
            yield break;
        }

        await foreach (var i in embedCreator.CreateAsync(tbaWebhookMessage, highlightTeam, cancellationToken))
        {
            yield return i;
        }
    }
}
