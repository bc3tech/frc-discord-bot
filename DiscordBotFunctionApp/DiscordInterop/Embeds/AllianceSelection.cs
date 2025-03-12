namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using TheBlueAlliance.Api;

internal sealed class AllianceSelection(IEventApi tbaClient,
                                        TeamRepository teams,
                                        EmbedBuilderFactory builderFactory,
                                        ILogger<AllianceSelection> logger) : INotificationEmbedCreator
{
    public const NotificationType TargetType = NotificationType.alliance_selection;

    private static readonly ConcurrentDictionary<string, bool> ProcessedEvents = new();

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        logger.CreatingAllianceSelectionEmbedWebhookMessage(msg);

        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.AllianceSelection>();
        if (notification != default)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        var eventKey = !string.IsNullOrWhiteSpace(notification.event_key) ? notification.event_key : notification.Event?.Key;
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            logger.EventKeyIsMissingFromNotificationData();
            yield return null;
            yield break;
        }

        if (!ProcessedEvents.TryAdd(eventKey, true))
        {
            logger.AlreadyProcessedAllianceSelectionForEventEventKey(eventKey);
            yield return null;
            yield break;
        }

        var alliances = await tbaClient.GetEventAlliancesAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (alliances?.Count is null or 0)
        {
            logger.FailedToRetrieveAllianceSelectionDataForEventKey(eventKey);
            yield return null;
            yield break;
        }

        var ranks = (await tbaClient.GetEventRankingsAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);

        // We have to build this with loops instead of interpolation because we don't want to output **anything** if Declines is empty (not even a line break)
        var descriptionBuilder = new StringBuilder($"## {notification.event_name}: Alliance Selection Complete\n");
        for (int i = 0; i < alliances.Count; i++)
        {
            var alliance = alliances[i];
            descriptionBuilder.AppendLine($"### Alliance {i + 1}\n");
            foreach (var teamKey in alliance.Picks)
            {
                descriptionBuilder.AppendLine($"- {teams[teamKey].GetLabelWithHighlight(highlightTeam)}{(ranks is not null ? $" (#{ranks[teamKey]})" : string.Empty)}");
            }

            if (alliance.Declines?.Count is not null and not 0)
            {
                descriptionBuilder.AppendLine($"__Declining Team{(alliance.Declines.Count > 1 ? "s" : string.Empty)}__");
                foreach (var team in alliance.Declines)
                {
                    descriptionBuilder.AppendLine($"- {teams[team].GetLabelWithHighlight(highlightTeam)}{(ranks is not null ? $" (#{ranks[team]})" : string.Empty)}");
                }
            }
        }

        descriptionBuilder.AppendLine($"\nYou can find more alliance details on the [Event Results](https://www.thebluealliance.com/event/{eventKey}#results) page");

#pragma warning disable EA0001 // Perform message formatting in the body of the logging method
        logger.EmbeddingNameBuiltEmbeddingDetail(nameof(AllianceSelection), descriptionBuilder.ToString());
#pragma warning restore EA0001 // Perform message formatting in the body of the logging method

        yield return new(baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build());
    }
}
