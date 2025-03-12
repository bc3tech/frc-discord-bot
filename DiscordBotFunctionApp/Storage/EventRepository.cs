namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class EventRepository(IEventApi apiClient, TimeProvider time, ILogger<EventRepository> logger)
{
    private static readonly ConcurrentDictionary<string, Event> _events = [];

    public async ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
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
                    if (_events.TryAdd(e.Key!, e))
                    {
                        logger.LogMetric("EventAdded", 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                logger.AnErrorOccurredWhileLoadingEventsFromTheTBAAPIErrorMessage(ex, ex.Message);
            }
        }

        logger.CachedEventCountTeamsFromTBA(_events.Count);
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

    public string GetLabelForEvent(string eventKey, bool shortName = false, bool includeYear = false, bool includeCity = false, bool includeStateProv = false, bool includeCountry = false)
    {
        using var scope = logger.CreateMethodScope();
        if (eventKey is CommonConstants.ALL)
        {
            return "All Events";
        }

        if (_events.TryGetValue(eventKey, out var e) is not true || e is null)
        {
            logger.EventEventKeyNotFoundInCache(eventKey);
            var liveEvent = apiClient.GetEvent(eventKey);
            if (liveEvent is not null)
            {
                e = _events.GetOrAdd(eventKey, liveEvent);
            }
        }

        if (e is not null)
        {
            var location = new StringBuilder();
            if (includeCity && !string.IsNullOrEmpty(e.City))
            {
                location.Append(e.City);
            }

            if (includeStateProv && !string.IsNullOrEmpty(e.StateProv))
            {
                if (location.Length > 0)
                {
                    location.Append(", ");
                }

                location.Append(e.StateProv);
            }

            if (includeCountry && !string.IsNullOrEmpty(e.Country))
            {
                if (location.Length > 0)
                {
                    location.Append(", ");
                }

                location.Append(e.Country);
            }

            return $"{(includeYear ? $"{e.Year} " : string.Empty)}{(shortName ? e.ShortName : e.Name)}{(location.Length > 0 ? $" - {location}" : string.Empty)}";
        }

        logger.EventEventKeyNotKnownAtAll(eventKey);

        return string.Empty;
    }
}
