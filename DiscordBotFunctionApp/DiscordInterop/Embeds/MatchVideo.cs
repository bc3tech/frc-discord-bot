namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model.MatchExtensions;

internal sealed class MatchVideo(IMatchApi matches,
                                 EventRepository eventRepo,
                                 EmbedBuilderFactory builderFactory,
                                 ILogger<MatchVideo> logger) : INotificationEmbedCreator, IEmbedCreator<string>
{
    public const NotificationType TargetType = NotificationType.match_video;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously; to conform to interface
    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam, footerRequired: false);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.MatchVideo>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        var videoUrls = notification.match?.GetVideoUrls();
        if (videoUrls is not null && videoUrls.Any())
        {
            var embedding = baseBuilder
                .WithTitle($"{notification.event_name} | {Translator.CompLevelToShortString(notification.match!.CompLevel.ToInvariantString())} {notification.match.SetNumber}.{notification.match.MatchNumber}")
                .WithDescription($"New video{(videoUrls.Count() > 1 ? 's' : string.Empty)} posted");
            IEnumerable<IMessageComponent> actions = videoUrls.Select(i => ButtonBuilder.CreateLinkButton($"🎞️{i.Name}", i.Link.OriginalString).Build());
            yield return new(embedding.Build(), actions);
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string matchKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);

        var match = await matches.GetMatchAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (match is not null)
        {
            var embedding = baseBuilder
                .WithTitle($"Videos for {eventRepo[match.EventKey].GetLabel()} | {Translator.CompLevelToShortString(match.CompLevel.ToInvariantString())} {match.SetNumber}.{match.MatchNumber}")
                .WithDescription(string.Join('\n', match.GetVideoUrls().Select(i => $"- [{i.Name}]({i.Link})")));

            yield return new(embedding.Build());
        }
        else
        {
            yield return new(baseBuilder.WithDescription("No videos found").Build());
        }
    }
}
