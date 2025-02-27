namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.Apis;
using DiscordBotFunctionApp.StatboticsInterop.Models;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop.Extensions;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

internal sealed class EventDetail(RESTCountries _countryCodeLookup, EmbedBuilderFactory builderFactory, EventRepository _eventsRepo, Statbotics.Api.IEventApi eventStats, ILogger<EventDetail> logger) : IEmbedCreator<string>
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async IAsyncEnumerable<ResponseEmbedding> CreateAsync(string eventKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();

        if ((await _eventsRepo.GetEventsAsync(default).ConfigureAwait(false)).TryGetValue(eventKey, out var eventDetails) && eventDetails is not null)
        {
            var countryCode = (await _countryCodeLookup.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default).ConfigureAwait(false))!;

            var builder = builderFactory.GetBuilder()
                .WithTitle($"**{eventDetails.Name}**")
                .WithUrl(eventDetails.Website)
                .WithDescription($@"{eventDetails.Year} Week {eventDetails.Week.GetValueOrDefault(-1) + 1} {eventDetails.EventTypeString} competition
[{eventDetails.LocationName}, {eventDetails.City}, {eventDetails.StateProv}, {eventDetails.Country}]({eventDetails.GmapsUrl})");

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                builder.ThumbnailUrl = Utility.CreateCountryFlagUrl(countryCode).ToString();
            }

            if (eventDetails.District is not null && !string.IsNullOrWhiteSpace(eventDetails.District.DisplayName))
            {
                builder.AddField("District", eventDetails.District.DisplayName);
            }

            builder
                .AddField("Dates", $"{eventDetails.StartDate:MMMM d} - {eventDetails.EndDate:MMMM d, yyyy}", inline: true);

            if (eventDetails.Webcasts is not null and { Count: > 0 })
            {
                builder.AddField("Where to watch", string.Join('\n', eventDetails.GetWebcastFullUrls().Where(i => !string.IsNullOrWhiteSpace(i.Url)).Select(i => $"- {i.Url}")));
            }

            builder
                .AddField("Event details on TBA", $"https://thebluealliance.com/event/{eventDetails.Key}")
                .AddField("-- Stats --", "Checking for statistics...", inline: true);

            yield return new(builder.Build(), Transient: true);

            Event? stats = default;
            try
            {
                var responseObj = await eventStats.ReadEventV3EventEventGetAsync(eventKey, cancellationToken).ConfigureAwait(false);
                if (responseObj is not null)
                {
                    stats = JsonSerializer.Deserialize<Event>(JsonSerializer.Serialize(responseObj, _jsonOptions), _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting data from Statbotics for {EventKey}", eventKey);
            }

            if (stats is not null)
            {
                builder.Fields.Insert(0, new EmbedFieldBuilder().WithName("Status").WithValue(stats.GetEventStatusStr()));
                builder.Fields[^1].Value = $@"{stats.NumTeams} teams
Max EPA: {stats.EpaVal?.Max}
Avg EPA: {stats.EpaVal?.Mean}";
                builder.Fields.Add(new EmbedFieldBuilder().WithName("Additional video").WithValue(stats.Video));
            }
            else
            {
                builder.Fields[^1].Value = "No stats available.";
            }

            yield return new(builder.Build());
        }
    }
}
