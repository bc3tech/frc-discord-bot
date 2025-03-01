﻿namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
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

    public string GetLabelForEvent(string eventKey)
    {
        using var scope = logger.CreateMethodScope();
        if (eventKey is CommonConstants.ALL)
        {
            return "All Events";
        }

        if (_events.TryGetValue(eventKey, out var e) is true && e is not null)
        {
            return $"{(!string.IsNullOrWhiteSpace(e.Name) ? $"{e.Year} {e.Name}" : string.Empty)}{(!string.IsNullOrWhiteSpace(e.City) && !string.IsNullOrWhiteSpace(e.Country) ? $" - {e.City}, {e.Country}" : string.Empty)}";
        }

        logger.EventEventKeyNotFoundInCache(eventKey);

        return string.Empty;
    }
}
