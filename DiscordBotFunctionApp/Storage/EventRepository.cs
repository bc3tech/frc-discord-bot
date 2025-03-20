namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class EventRepository(IEventApi apiClient, TimeProvider time, Meter meter, ILogger<EventRepository> logger)
{
    private static readonly ConcurrentDictionary<string, Event> _events = [];
    private static readonly ConcurrentQueue<Task> LogMetricTasks = [];

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        using var scope = logger.CreateMethodScope();
        for (int i = 0, currentYear = time.GetLocalNow().Year; i < 4; i++, currentYear--)
        {
            logger.LoadingEventsFromTBAForEventYear(currentYear);
            try
            {
                var newEvents = await apiClient.GetEventsByYearAsync(currentYear, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (newEvents?.Count is null or 0)
                {
                    break;
                }

                logger.RetrievedEventCountEvents(newEvents.Count);

                foreach (var e in newEvents)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_events.TryAdd(e.Key, e))
                    {
                        LogMetricTasks.Enqueue(Task.Run(() => meter.LogMetric("EventAdded", 1), cancellationToken));
                    }
                }
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Debug.Fail(e.Message);
                logger.AnErrorOccurredWhileLoadingEventsFromTheTBAAPIErrorMessage(e, e.Message);
            }
        }

        logger.CachedEventCountTeamsFromTBA(_events.Count);

        Task.Run(async () =>
        {
            await Task.WhenAll(LogMetricTasks);
            LogMetricTasks.Clear();
        }, cancellationToken);
    }

    /// <summary>
    /// Gets the event associated with the specified event key.
    /// If the event is not found in the cache, it retrieves the event from the TBA API and adds it to the cache.
    /// </summary>
    /// <param name="eventKey">The key of the event to retrieve.</param>
    /// <returns>The event associated with the specified event key.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the event is not found in the cache or the TBA API.</exception>
    public Event this[string eventKey]
    {
        get
        {
            if (_events.TryGetValue(eventKey, out var t) && t is not null)
            {
                return t;
            }

            logger.EventEventKeyNotFoundInCache(eventKey);
            t = apiClient.GetEvent(eventKey);
            return t is not null ? _events.GetOrAdd(eventKey, t) : throw new KeyNotFoundException();
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not the API we're going for")]
    public IReadOnlyDictionary<string, Event> AllEvents => _events;
}
