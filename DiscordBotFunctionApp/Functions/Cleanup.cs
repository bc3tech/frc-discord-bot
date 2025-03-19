namespace DiscordBotFunctionApp.Functions;

using Azure.Data.Tables;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.Storage.TableEntities;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Threading.Tasks;

using TheBlueAlliance.Api;

internal sealed class Cleanup([FromKeyedServices(Constants.ServiceKeys.TableClient_ProcessedMessages)] TableClient processedMessagesTable,
                              [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient eventMessageThreads,
                              [FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
                              [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
                              IEventApi events,
                              TimeProvider time,
                              IConfiguration appConfig,
                              ILogger<Cleanup> logger)
{
    [Function("Cleanup")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for Functions")]
    public async Task RunAsync([TimerTrigger("0 0 9 * * 3", RunOnStartup = true)] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        if (!int.TryParse(appConfig["MaxDaysToKeepStateData"], out var maxDays))
        {
            maxDays = 5;
        }

        logger.ExecutingCleanup();

        var startTime = time.GetTimestamp();
        await foreach (var oldMessage in processedMessagesTable.QueryAsync<TableEntity>(filter: $"Timestamp lt datetime'{DateTime.UtcNow.AddDays(maxDays * -1):O}'", cancellationToken: cancellationToken))
        {
            logger.DeletingMessageFromRecordTimestamp(oldMessage.Timestamp);

            var r = await processedMessagesTable.DeleteEntityAsync(oldMessage.PartitionKey, oldMessage.RowKey, cancellationToken: cancellationToken);
            if (r.IsError)
            {
                logger.FailedToDeleteMessageFromRecordTimestampErrorMessage(oldMessage.Timestamp, r.ReasonPhrase);
                continue;
            }

            logger.LogMetric("ProcessedMessageCleanedUp", 1, new Dictionary<string, object> { { "Timestamp", oldMessage.Timestamp! } });
        }

        logger.LogMetric("ProcessedMessagesCleanupTimeSec", time.GetElapsedTime(startTime).TotalSeconds);

        startTime = time.GetTimestamp();
        await foreach (var oldMessage in eventMessageThreads.QueryAsync<ThreadTableEntity>(filter: $"Timestamp lt datetime'{DateTime.UtcNow.AddDays(maxDays * -1):O}'", cancellationToken: cancellationToken))
        {
            logger.DeletingOldThreadForThreadEventMessageFromRecordTimestamp(oldMessage.RowKey, oldMessage.Timestamp);

            var r = await eventMessageThreads.DeleteEntityAsync(oldMessage.PartitionKey, oldMessage.RowKey, cancellationToken: cancellationToken);
            if (r.IsError)
            {
                logger.FailedToDeleteThreadFromRecordTimestampErrorMessage(oldMessage.Timestamp, r.ReasonPhrase);
                continue;
            }

            logger.LogMetric("EventMessageThreadCleanedUp", 1, new Dictionary<string, object> { { "Thread", JsonSerializer.Serialize(oldMessage) } });
        }

        logger.LogMetric("EventMessageThreadsCleanupTimeSec", time.GetElapsedTime(startTime).TotalSeconds);

        startTime = time.GetTimestamp();
        await foreach (var s in teamSubscriptions.QueryAsync<TeamSubscriptionEntity>(e => e.RowKey != CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            logger.CleaningUpSubscriptionsForTeamTeam(s.Team);
            var tbaEvent = await events.GetEventSimpleAsync(s.Event, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (tbaEvent is null)
            {
                logger.FailedToRetrieveEventForTeamSubscriptionTeamEvent(s.Team, s.Event);
                continue;
            }

            if ((time.GetLocalNow().Date - tbaEvent.EndDate.ToDateTime(TimeOnly.MinValue)).TotalDays > maxDays)
            {
                logger.EventEventHasEnded5DaysAgoCleaningUpSubscriptionToItForTeamTeamKey(s.Event, s.Team);
                var r = await teamSubscriptions.DeleteEntityAsync(s.PartitionKey, s.RowKey, cancellationToken: cancellationToken);
                if (r.IsError)
                {
                    logger.FailedToDeleteSubscriptionForTeamTeamErrorMessage(s.Team, r.ReasonPhrase);
                    continue;
                }
            }
        }

        logger.LogMetric("TeamSubscriptionsCleanupTimeSec", time.GetElapsedTime(startTime).TotalSeconds);

        startTime = time.GetTimestamp();
        await foreach (var s in eventSubscriptions.QueryAsync<EventSubscriptionEntity>(e => e.PartitionKey != CommonConstants.ALL, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            logger.CleaningUpSubscriptionsForEventEvent(s.Event);
            var tbaEvent = await events.GetEventSimpleAsync(s.Event, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (tbaEvent is null)
            {
                logger.FailedToRetrieveEventForEventSubscriptionEvent(s.Event);
                continue;
            }

            if ((time.GetLocalNow().Date - tbaEvent.EndDate.ToDateTime(TimeOnly.MinValue)).TotalDays > maxDays)
            {
                logger.EventEventHasEnded5DaysAgoCleaningUpEventSubscription(s.Event);
                var r = await eventSubscriptions.DeleteEntityAsync(s.PartitionKey, s.RowKey, cancellationToken: cancellationToken);
                if (r.IsError)
                {
                    logger.FailedToDeleteSubscriptionForEventEventErrorMessage(s.Event, r.ReasonPhrase);
                    continue;
                }
            }
        }

        logger.LogMetric("EventSubscriptionsCleanupTimeSec", time.GetElapsedTime(startTime).TotalSeconds);
    }
}
