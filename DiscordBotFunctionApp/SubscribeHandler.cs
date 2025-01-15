namespace DiscordBotFunctionApp;

using Azure.Data.Tables;

using DiscordBotFunctionApp.TableEntities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

internal sealed class SubscribeHandler(ILogger<SubscribeHandler> logger)
{
    [Function("subscribe")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken)
    {
        logger.LogDebug("Processing subscription request");

        var content = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        logger.LogTrace("{SubscriptionRequestBody}", content);

        var sub = JsonSerializer.Deserialize<SubscriptionRequest>(content);
        logger.LogTrace("Deserialized request value: {SubscriptionRequest}", sub?.ToString() ?? "NULL");

        if (sub is null)
        {
            return new BadRequestObjectResult("Invalid request body.");
        }

        if (sub.Team is null && sub.Event is null)
        {
            return new BadRequestObjectResult("Request must specify either a team or an event.");
        }

        return new AcceptedResult();
    }
}

public record SubscriptionRequest([property: JsonPropertyName("guildId")] ulong GuildId, [property: JsonPropertyName("channelId")] ulong ChannelId, [property: JsonPropertyName("event")] string? Event, [property: JsonPropertyName("team")] uint? Team);

internal class SubscriptionManager([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
    ILogger<SubscriptionManager> logger)
{
    public async Task SaveSubscriptionAsync(SubscriptionRequest sub, CancellationToken cancellationToken)
    {
        if (sub.Team is not null)
        {
            logger.LogTrace("Adding subscription for team {SubscriptionTeam}", sub.Team);
            var r = await teamSubscriptions.GetEntityIfExistsAsync<TeamSubscriptionEntity>(sub.Team.Value.ToString(CultureInfo.InvariantCulture), CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allEventSubscription = r.HasValue ? r.Value : null;
            if (allEventSubscription is null)
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

        if (sub.Event is not null)
        {
            logger.LogTrace("Adding subscription for event {SubscriptionEvent}", sub.Event);
            var r = await eventSubscriptions.GetEntityIfExistsAsync<EventSubscriptionEntity>(sub.Event, CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false);
            var allTeamSubscription = r.HasValue ? r.Value : null;
            if (allTeamSubscription is null)
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