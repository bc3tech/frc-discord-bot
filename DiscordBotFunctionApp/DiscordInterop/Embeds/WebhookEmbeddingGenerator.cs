﻿namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

internal sealed class WebhookEmbeddingGenerator(IServiceProvider services, ILogger<WebhookEmbeddingGenerator> logger)
{
    [return: NotNull]
    public async IAsyncEnumerable<Embed> CreateEmbeddingsAsync(WebhookMessage tbaWebhookMessage, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        var embedCreator = services.GetKeyedService<INotificationEmbedCreator>(tbaWebhookMessage.MessageType.ToInvariantString());
        if (embedCreator is null)
        {
            logger.NoEmbeddingCreatorRegisteredForMessageTypeMessageType(tbaWebhookMessage.MessageType);
        }
        else
        {
            logger.GeneratingEmbeddingsForWebhookMessageTypeWebhookMessageType(tbaWebhookMessage.MessageType);
            await foreach (var i in embedCreator.CreateAsync(tbaWebhookMessage, highlightTeam, cancellationToken))
            {
                if (i is not null)
                {
                    yield return i;
                }
            }
        }
    }
}
