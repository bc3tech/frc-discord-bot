namespace DiscordBotFunctionApp.Subscription;

using Azure.Data.Tables;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.Storage.TableEntities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

internal sealed record NotificationSubscription([property: JsonPropertyName("guildId")] ulong? GuildId,
                                           [property: JsonPropertyName("channelId")] ulong ChannelId,
                                           [property: JsonPropertyName("event")] string? Event,
                                           [property: JsonPropertyName("team")] string? Team);

internal sealed class SubscriptionManager(
    [FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
    ILogger<SubscriptionManager> logger)
{
    public async IAsyncEnumerable<NotificationSubscription> GetSubscriptionsForGuildAsync(ulong? guildId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var e in teamSubscriptions.QueryAsync<TeamSubscriptionEntity>(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            foreach (var s in e.Subscribers.SubscriptionsForGuild(guildId))
            {
                yield return new(guildId, s, e.Event, e.Team);
            }
        }

        await foreach (var e in eventSubscriptions.QueryAsync<EventSubscriptionEntity>(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            foreach (var s in e.Subscribers.SubscriptionsForGuild(guildId))
            {
                yield return new(guildId, s, e.Event, e.Team);
            }
        }
    }

    public async Task SaveSubscriptionAsync(NotificationSubscription sub, CancellationToken cancellationToken)
    {
        if (sub.Team is not null)
        {
            logger.AddingSubscriptionForTeamSubscriptionTeam(sub.Team);
            var r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team, CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allEventSubscription = r.HasValue ? r.Value : null;
            if (allEventSubscription is null || !allEventSubscription.Subscribers.Exists(sub.GuildId, sub.ChannelId))
            {
                var eventString = sub.Event ?? CommonConstants.ALL;
                logger.CreatingNewSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEvent(sub.Team, eventString);

                r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team, eventString, cancellationToken: cancellationToken).ConfigureAwait(false);
                var eventSubscription = r.HasValue ? r.Value : null;
                eventSubscription ??= new TeamSubscriptionEntity { PartitionKey = sub.Team, RowKey = eventString };

                eventSubscription.Subscribers.AddSubscription(sub.GuildId, sub.ChannelId);
                var result = await teamSubscriptions.UpsertEntityAsync(eventSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.FailedToUpsertSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEventStatusCodeReason(sub.Team, eventString, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to upsert subscription for team {0} and event {1} ({2}): {3}", sub.Team, eventString, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.AllSubscriptionAlreadyExistsForTeamSubscriptionTeam(sub.Team);
            }
        }
        else if (sub.Event is not null)
        {
            logger.AddingSubscriptionForEventSubscriptionEvent(sub.Event);
            var r = await eventSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event, CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allTeamSubscription = r.HasValue ? r.Value : null;
            if (allTeamSubscription is null || !allTeamSubscription.Subscribers.Exists(sub.GuildId, sub.ChannelId))
            {
                logger.CreatingNewSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeam(sub.Event);

                r = await eventSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event, CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
                var eventSubscription = r.HasValue ? r.Value : null;
                eventSubscription ??= new EventSubscriptionEntity { PartitionKey = sub.Event };

                eventSubscription.Subscribers.AddSubscription(sub.GuildId, sub.ChannelId);
                var result = await eventSubscriptions.UpsertEntityAsync(eventSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.FailedToUpsertSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeamStatusCodeReason(sub.Event, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to upsert subscription for event {0} and team ({1}): {2}", sub.Event, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.AllSubscriptionAlreadyExistsForEventSubscriptionEvent(sub.Event);
            }
        }
    }

    public async Task RemoveSubscriptionAsync(NotificationSubscription sub, CancellationToken cancellationToken)
    {
        if (sub.Team is not null)
        {
            logger.RemovingSubscriptionForTeamSubscriptionTeamTeam(sub.Team);
            var r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team, sub.Event ?? CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var eventSubscription = r.HasValue ? r.Value : null;
            if (eventSubscription is not null)
            {
                eventSubscription.Subscribers.RemoveSubscription(sub.GuildId, sub.ChannelId);
                var result = await teamSubscriptions.UpsertEntityAsync(eventSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.FailedToRemoveSubscriptionForTeamTeamStatusReason(sub.Team, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to remove subscription for team {0} ({1}): {2}", sub.Team, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.NoSubscriptionsFoundForSubscription(sub);
            }
        }
        else
        {
            logger.RemovingSubscriptionForEventSubscriptionEventEvent(sub.Event);
            var r = await eventSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event ?? CommonConstants.ALL, sub.Team?.ToString(CultureInfo.InvariantCulture) ?? CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var eventSubscription = r.HasValue ? r.Value : null;
            if (eventSubscription is not null)
            {
                eventSubscription.Subscribers.RemoveSubscription(sub.GuildId, sub.ChannelId);
                var result = await eventSubscriptions.UpsertEntityAsync(eventSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.FailedToRemoveSubscriptionForEventEventStatusReason(sub.Event, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to remove subscription for event {0} ({1}): {2}", sub.Event, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.NoSubscriptionsFoundForSubscription(sub);
            }
        }
    }
}
