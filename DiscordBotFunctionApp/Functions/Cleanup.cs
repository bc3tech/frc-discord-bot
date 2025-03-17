namespace DiscordBotFunctionApp.Functions;

using Azure.Data.Tables;

using DiscordBotFunctionApp.DiscordInterop;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Threading.Tasks;

internal sealed class Cleanup(ILogger<Cleanup> logger, [FromKeyedServices(Constants.ServiceKeys.TableClient_ProcessedMessages)] TableClient processedMessagesTable, [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient eventMessageThreads, TimeProvider time)
{
    [Function("Cleanup")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for Functions")]
    public async Task RunAsync([TimerTrigger("0 0 9 * * 3")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        logger.ExecutingCleanup();

        var startTime = time.GetTimestamp();
        await foreach (var oldMessage in processedMessagesTable.QueryAsync<TableEntity>(filter: $"Timestamp lt datetime'{DateTime.UtcNow.AddDays(-5):O}'", cancellationToken: cancellationToken))
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
        await foreach (var oldMessage in eventMessageThreads.QueryAsync<ThreadTableEntity>(filter: $"Timestamp lt datetime'{DateTime.UtcNow.AddDays(-7):O}'", cancellationToken: cancellationToken))
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
    }
}
