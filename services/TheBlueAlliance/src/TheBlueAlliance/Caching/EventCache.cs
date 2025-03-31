namespace TheBlueAlliance.Caching;

using Common;
using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

public class EventCache(IEventApi apiClient, TimeProvider time, Meter meter, ILogger<EventCache> logger)
{
    private static readonly ConcurrentDictionary<string, Event> _events = [];
    private static readonly ConcurrentQueue<Task> LogMetricTasks = [];

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        using var scope = logger.CreateMethodScope();
        for (int i = 0, currentYear = time.GetLocalNow().Year; i < 4; i++, currentYear--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogDebug("Loading Events from TBA for {EventYear}...", currentYear);
            try
            {
                var newEvents = await apiClient.GetEventsByYearAsync(currentYear, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (newEvents?.Count is null or 0)
                {
                    break;
                }

                logger.LogTrace("Retrieved {EventCount} events", newEvents.Count);

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
                logger.LogError(e, "An error occurred while loading events from the TBA API: {ErrorMessage}", e.Message);
                Debug.Fail(e.Message);
            }
        }

        logger.LogInformation("Cached {EventCount} event(s) from TBA", _events.Count);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed; we want this to run in the background and not block the rest of the app
        Task.Run(async () =>
        {
            await Task.WhenAll(LogMetricTasks);
            LogMetricTasks.Clear();
        }, cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    /// <summary>
    /// Gets the event associated with the specified event key.
    /// If the event is not found in the cache, it retrieves the event from the TBA API and adds it to the cache.
    /// </summary>
    /// <param name="eventKey">The key of the event to retrieve.</param>
    /// <returns>The event associated with the specified event key.</returns>
    /// <exception cref="EventNotFoundException">Thrown if the event key isn't found in the cache and is also not returned by the <see cref="IEventApi"/> service.</exception>
    public Event this[string eventKey]
    {
        get
        {
            if (_events.TryGetValue(eventKey, out var t) && t is not null)
            {
                return t;
            }

            logger.LogDebug("Event {EventKey} not found in cache, fetching...", eventKey);

            t = apiClient.GetEvent(eventKey);
            return t is not null ? _events.GetOrAdd(eventKey, t) : throw new EventNotFoundException(eventKey);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Not the API we're going for")]
    public IReadOnlyDictionary<string, Event> AllEvents => _events;

    /// <summary>
    /// Tries to get the event associated with the specified event key.
    /// </summary>
    /// <param name="eventKey">The key of the event to retrieve.</param>
    /// <param name="event">The event associated with the specified event key.</param>
    /// <returns>True if the event is found in the cache; otherwise, false.</returns>
    public bool TryGetValue(string eventKey, [NotNullWhen(true)] out Event? @event) => _events.TryGetValue(eventKey, out @event);
}

public class EventNotFoundException : KeyNotFoundException
{
    public EventNotFoundException(string eventKey) : base($"No event with key {eventKey} could be found")
    {
        this.Data["EventKey"] = eventKey;
    }
}
