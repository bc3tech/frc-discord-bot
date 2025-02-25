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

internal sealed record SubscriptionRequest([property: JsonPropertyName("guildId")] ulong GuildId, [property: JsonPropertyName("channelId")] ulong ChannelId, [property: JsonPropertyName("event")] string? Event, [property: JsonPropertyName("team")] uint? Team);

internal sealed class SubscriptionManager([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
    ILogger<SubscriptionManager> logger)
{
    public async IAsyncEnumerable<(ulong ChannelId, string EventKey, ushort? TeamNumber)> GetSubscriptionsForGuildAsync(ulong guildId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var e in teamSubscriptions.QueryAsync<TeamSubscriptionEntity>(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            foreach (var s in e.Subscribers.SubscriptionsForGuild(guildId))
            {
                yield return (s, e.Event, e.Team);
            }
        }

        await foreach (var e in eventSubscriptions.QueryAsync<EventSubscriptionEntity>(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            foreach (var s in e.Subscribers.SubscriptionsForGuild(guildId))
            {
                yield return (s, e.Event, e.Team);
            }
        }
    }

    public async Task SaveSubscriptionAsync(SubscriptionRequest sub, CancellationToken cancellationToken)
    {
        if (sub.Team is not null)
        {
            logger.LogTrace("Adding subscription for team {SubscriptionTeam}", sub.Team);
            var r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team.Value.ToString(CultureInfo.InvariantCulture), CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allEventSubscription = r.HasValue ? r.Value : null;
            if (allEventSubscription is null || !allEventSubscription.Subscribers.Exists(sub.GuildId, sub.ChannelId))
            {
                var eventString = sub.Event ?? CommonConstants.ALL;
                logger.LogInformation("Creating new subscription for team {SubscriptionTeam} and event {SubscriptionEvent}", sub.Team, eventString);

                r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team.Value.ToString(CultureInfo.InvariantCulture), eventString, cancellationToken: cancellationToken).ConfigureAwait(false);
                var eventSubscription = r.HasValue ? r.Value : null;
                eventSubscription ??= new TeamSubscriptionEntity { PartitionKey = sub.Team.Value.ToString(CultureInfo.InvariantCulture), RowKey = eventString };

                eventSubscription.Subscribers.AddSubscription(sub.GuildId, sub.ChannelId);
                var result = await teamSubscriptions.UpsertEntityAsync(eventSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.LogError("Failed to upsert subscription for team {SubscriptionTeam} and event {SubscriptionEvent} ({StatusCode}): {Reason}", sub.Team, eventString, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to upsert subscription for team {0} and event {1} ({2}): {3}", sub.Team, eventString, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.LogInformation("'All' subscription already exists for team {SubscriptionTeam}", sub.Team);
            }
        }
        else if (sub.Event is not null)
        {
            logger.LogTrace("Adding subscription for event {SubscriptionEvent}", sub.Event);
            var r = await eventSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event, CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allTeamSubscription = r.HasValue ? r.Value : null;
            if (allTeamSubscription is null || !allTeamSubscription.Subscribers.Exists(sub.GuildId, sub.ChannelId))
            {
                var teamString = sub.Team?.ToString(CultureInfo.InvariantCulture) ?? CommonConstants.ALL;
                logger.LogInformation("Creating new subscription for event {SubscriptionEvent} and team {SubscriptionTeam}", sub.Event, teamString);

                r = await teamSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event, teamString, cancellationToken: cancellationToken).ConfigureAwait(false);
                var teamSubscription = r.HasValue ? r.Value : null;
                teamSubscription ??= new EventSubscriptionEntity { PartitionKey = sub.Event, RowKey = teamString };

                teamSubscription.Subscribers.AddSubscription(sub.GuildId, sub.ChannelId);
                var result = await eventSubscriptions.UpsertEntityAsync(teamSubscription, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (result.IsError)
                {
                    logger.LogError("Failed to upsert subscription for event {SubscriptionEvent} and team {SubscriptionTeam} ({StatusCode}): {Reason}", sub.Event, teamString, result.Status, result.ReasonPhrase);
                    throw new HttpProtocolException(result.Status, string.Format(CultureInfo.InvariantCulture, "Failed to upsert subscription for event {0} and team {1} ({2}): {3}", sub.Event, teamString, result.Status, result.ReasonPhrase), null);
                }
            }
            else
            {
                logger.LogInformation("'All' subscription already exists for event {SubscriptionEvent}", sub.Event);
            }
        }
    }
}