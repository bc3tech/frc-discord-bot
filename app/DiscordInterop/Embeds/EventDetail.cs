namespace FunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using FunctionApp.Apis;
using FunctionApp.DiscordInterop;

using Microsoft.Extensions.Logging;

using Statbotics.Model;

using System.Runtime.CompilerServices;
using System.Text;

using TheBlueAlliance.Caching;

internal sealed class EventDetail(IRESTCountries _countryCodeLookup,
                                  EmbedBuilderFactory builderFactory,
                                  EventCache _eventsRepo,
                                  Statbotics.Api.IEventApi eventStats,
                                  ILogger<EventDetail> logger) : IEmbedCreator<string>
{
    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string eventKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();

        var eventDetails = _eventsRepo[eventKey];
        var countryCode = (await _countryCodeLookup.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default).ConfigureAwait(false))!;

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.AppendLine($"{eventDetails.Year} Week {eventDetails.Week.GetValueOrDefault(-1) + 1} {eventDetails.EventTypeString} competition");

        #region Location URL section
        if (!string.IsNullOrWhiteSpace(eventDetails.GmapsUrl))
        {
            descriptionBuilder.Append('[');
        }

        descriptionBuilder.Append(!string.IsNullOrWhiteSpace(eventDetails.LocationName) ? $"{eventDetails.LocationName}, " : string.Empty)
            .Append(!string.IsNullOrWhiteSpace(eventDetails.City) ? $"{eventDetails.City}, " : string.Empty)
            .Append(!string.IsNullOrWhiteSpace(eventDetails.StateProv) ? $"{eventDetails.StateProv}, " : string.Empty)
            .Append(!string.IsNullOrWhiteSpace(eventDetails.Country) ? $"{eventDetails.Country}" : string.Empty);

        if (!string.IsNullOrWhiteSpace(eventDetails.GmapsUrl))
        {
            descriptionBuilder.Append($"]({eventDetails.GmapsUrl})");
        }

        descriptionBuilder.AppendLine()
            .AppendLine()
            .AppendLine($"Schedule [here]({eventDetails.ScheduleUrl})");
        #endregion

        var builder = builderFactory.GetBuilder()
            .WithTitle($"**{eventDetails.Name}**")
            .WithUrl(eventDetails.Website)
            .WithDescription(descriptionBuilder.ToString());

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            builder.ThumbnailUrl = Utility.CreateCountryFlagUrl(countryCode).ToString();
        }

        if (eventDetails.District is not null && !string.IsNullOrWhiteSpace(eventDetails.District.DisplayName))
        {
            builder.AddField("District", eventDetails.District.DisplayName);
        }

        builder.AddField("Dates", $"{eventDetails.StartDate:MMMM d} - {eventDetails.EndDate:MMMM d, yyyy}", inline: true);

        if (eventDetails.Webcasts is not null and { Count: > 0 })
        {
            builder.AddField("Where to watch", string.Join('\n', eventDetails.Webcasts.Select(i =>
            {
                var (webcastSource, webcastLink) = i.GetFullUrl(logger);
                return $"- [{webcastSource}]({webcastLink})";
            })));
        }

        builder
            .AddField("Event details on TBA", $"https://thebluealliance.com/event/{eventDetails.Key}")
            .AddField("Stats", "Checking for statistics...", inline: true);

        yield return new(builder.Build(), Transient: true);

        Event? stats = default;
        try
        {
            stats = await eventStats.ReadEventV3EventEventGetAsync(eventKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.ErrorGettingDataFromStatboticsForEventKey(e, eventKey);
        }

        if (stats is not null)
        {
            builder.Fields.Insert(0, new EmbedFieldBuilder().WithName("Status").WithValue(stats.GetEventStatusStr()));
            builder.Fields[^1].Value = stats.NumTeams is not null and not 0
                ? $"""
                    - {stats.NumTeams} teams
                    - Max EPA: {stats.EpaVal?.Max}*
                    - Avg EPA: {stats.EpaVal?.Mean}*
                    -# \* This is a prediction if the event has not yet started
                    """
                : "No stats available.";
        }
        else
        {
            builder.Fields[^1].Value = "No stats available.";
        }

        yield return new(builder.Build());
    }
}
