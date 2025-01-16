﻿namespace DiscordBotFunctionApp.Storage;

using Common.Extensions;
using Common.Tba.Api;
using Common.Tba.Api.Models;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

internal sealed class EventRepository(ApiClient apiClient, ILogger<EventRepository> logger)
{
    private IReadOnlyDictionary<string, Event>? _events;

    public async ValueTask<IReadOnlyDictionary<string, Event>> GetEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        if (_events is null)
        {
            var currentYear = TimeProvider.System.GetLocalNow().Year;
            logger.LogDebug("Loading Events from TBA...");
            try
            {
                var newEvents = await apiClient.Events[currentYear].GetAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                logger.LogInformation("Loaded {EventCount} events", newEvents?.Count ?? 0);
                _events = newEvents?.ToDictionary(t => t.Key!) ?? [];
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                logger.LogError(ex, "An error occurred while loading events from the TBA API: {ErrorMessage}", ex.Message);
                _events = new Dictionary<string, Event>();
            }
        }

        return _events;
    }

    public string GetLabelForEvent(string eventKey)
    {
        using var scope = logger.CreateMethodScope();
        if (_events?.TryGetValue(eventKey, out var e) is true && e is not null)
        {
            return $"{(!string.IsNullOrWhiteSpace(e.Name) ? $"{e.Year} {e.Name}" : string.Empty)}{(!string.IsNullOrWhiteSpace(e.City) && !string.IsNullOrWhiteSpace(e.Country) ? $" - {e.City}, {e.Country}" : string.Empty)}";
        }

        logger.LogWarning("Event {EventKey} not found in cache", eventKey);

        return string.Empty;
    }
}
