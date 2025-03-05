namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using TheBlueAlliance.Api;

internal sealed class AllianceSelection(IEventApi tbaClient, TeamRepository teams, EmbedBuilderFactory builderFactory, ILogger<AllianceSelection> logger) : INotificationEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.alliance_selection;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.AllianceSelection>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var eventKey = !string.IsNullOrWhiteSpace(notification.event_key) ? notification.event_key : notification.Event?.Key;
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            logger.EventKeyIsMissingFromNotificationData();
            yield return new(baseBuilder.Build());
            yield break;
        }

        var alliances = await tbaClient.GetEventAlliancesAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (alliances?.Count is null or 0)
        {
            logger.FailedToRetrieveAllianceSelectionDataForEventKey(eventKey);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var ranks = (await tbaClient.GetEventRankingsAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);

        // We have to build this with loops instead of interpolation because we don't want to output **anything** if Declines is empty (not even a line break)
        var descriptionBuilder = new StringBuilder($"## {notification.event_name}: Alliance Selection Complete\n");
        for (int i = 0; i < alliances.Count; i++)
        {
            var alliance = alliances[i];
            descriptionBuilder.AppendLine($"### Alliance {i + 1}");
            foreach (var team in alliance.Picks!.OrderBy(t => t.ToTeamNumber()))
            {
                descriptionBuilder.AppendLine($"- {teams.GetTeamLabelWithHighlight(team, highlightTeam)} (#{ranks[team]})");
            }

            if (alliance.Declines?.Count is not null and not 0)
            {
                descriptionBuilder.AppendLine($"__Declining Team{(alliance.Declines!.Count > 1 ? "s" : string.Empty)}__");
                foreach (var team in alliance.Declines!.OrderBy(t => t.ToTeamNumber()))
                {
                    descriptionBuilder.AppendLine($"- {teams.GetTeamLabelWithHighlight(team, highlightTeam)} (#{ranks[team]})");
                }
            }
        }

        descriptionBuilder.AppendLine($"\nYou can find more alliance details on the [Event Results](https://www.thebluealliance.com/event/{eventKey}#results) page");

        yield return new(baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build());
    }
}
