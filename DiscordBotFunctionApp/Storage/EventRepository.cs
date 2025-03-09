namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

internal sealed class EventRepository(IEventApi apiClient, ILogger<EventRepository> logger)
{
    private Dictionary<string, Event> _events = [];

    public async ValueTask<IReadOnlyDictionary<string, Event>> GetEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        if (_events.Count is 0)
        {
            for (int i = 0, currentYear = TimeProvider.System.GetLocalNow().Year; i < 4; i++, currentYear--)
            {
                logger.LoadingEventsFromTBAForEventYear(currentYear);
                try
                {
                    var newEvents = await apiClient.GetEventsByYearAsync(currentYear, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (newEvents?.Count is null or 0)
                    {
                        return _events = [];
                    }

                    logger.LoadedEventCountEvents(newEvents.Count);
                    logger.LogMetric("EventCount", newEvents.Count, new Dictionary<string, object>() { { "Year", currentYear } });
                    _events = new Dictionary<string, Event>([.. _events, .. newEvents.ToDictionary(t => t.Key!)]);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                    logger.AnErrorOccurredWhileLoadingEventsFromTheTBAAPIErrorMessage(ex, ex.Message);
                    _events = [];
                }
            }
        }

        return _events;
    }

    public string GetLabelForEvent(string eventKey, bool shortName = false, bool includeYear = false, bool includeCity = false, bool includeStateProv=false, bool includeCountry = false)
    {
        using var scope = logger.CreateMethodScope();
        if (eventKey is CommonConstants.ALL)
        {
            return "All Events";
        }

        if (_events.TryGetValue(eventKey, out var e) is true && e is not null)
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

        logger.EventEventKeyNotFoundInCache(eventKey);

        return string.Empty;
    }
}
