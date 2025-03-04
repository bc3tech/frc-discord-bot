namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Extensions;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model.MatchExtensions;

internal sealed class MatchVideo(EmbedBuilderFactory builderFactory, IMatchApi matches, EventRepository eventRepo, ILogger<MatchVideo> logger) : INotificationEmbedCreator, IEmbedCreator<string>
{
    public static NotificationType TargetType { get; } = NotificationType.match_video;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously; to conform to interface
    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.MatchVideo>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var embedding = builderFactory.GetBuilder()
            .WithTitle($"New video for {notification.event_name} | {Translator.CompLevelToShortString(notification.match!.CompLevel.ToInvariantString())} {notification.match.SetNumber}-{notification.match.MatchNumber}")
            .WithDescription(string.Join('\n', notification.match.GetVideoUrls().Select(i => $"- {i}")));

        yield return new(embedding.Build());
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string matchKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);

        var match = await matches.GetMatchAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (match is not null)
        {
            var embedding = builderFactory.GetBuilder()
                .WithTitle($"Videos for {eventRepo.GetLabelForEvent(match.EventKey)} | {Translator.CompLevelToShortString(match.CompLevel.ToInvariantString())} {match.SetNumber}-{match.MatchNumber}")
                .WithDescription(string.Join('\n', match.GetVideoUrls().Select(i => $"- {i}")));

            yield return new(embedding.Build());
        }
        else
        {
            yield return new(baseBuilder.WithDescription("No videos found").Build());
        }
    }
}
