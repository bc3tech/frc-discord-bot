namespace FunctionApp.DiscordInterop.Embeds;

using Common;

using Discord;

using FunctionApp;

using FunctionApp.DiscordInterop;
using FunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;

internal sealed class Schedule(EmbedBuilderFactory builderFactory, EventRepository events, TeamRepository teams, IMatchApi matchApi, TimeProvider time, ILogger<Schedule> logger) : IEmbedCreator<(string? eventKey, ushort numMatches)>
{
    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync((string? eventKey, ushort numMatches) input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (input.numMatches is 0)
        {
            logger.InvalidNumberOfMatchesRequestedNumMatches(input.numMatches);
            yield break;
        }

        var embedding = builderFactory.GetBuilder(highlightTeam, footerRequired: false)
                    .WithTitle("Schedule");

        IEnumerable<Match>? matches = [];
        if (highlightTeam.HasValue)
        {
            if (string.IsNullOrWhiteSpace(input.eventKey))
            {
                embedding.Description = $"Schedule for {teams[highlightTeam.Value].GetLabel(includeNumber: false, includeLocation: false, asMarkdownLink: false)}";

                matches = await matchApi.GetTeamMatchesByYearAsync($"frc{highlightTeam.Value}", time.GetLocalNow().Year, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                embedding.Description = $"Schedule for {teams[highlightTeam.Value].GetLabel(includeNumber: false, includeLocation: false)} at {events[input.eventKey].GetLabel(shortName: true, asMarkdownLink: true)}";

                matches = await matchApi.GetTeamEventMatchesAsync(input.eventKey, $"frc{highlightTeam.Value}", cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            embedding.Description = $"Schedule for {events[input.eventKey!].GetLabel(shortName: true, asMarkdownLink: true)}";

            matches = await matchApi.GetEventMatchesAsync(input.eventKey!, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        var matchesToPost = matches?.OrderBy(m => (int)m.CompLevel)
                .ThenBy(m => m.SetNumber)
                .ThenBy(m => m.MatchNumber)
                .Where(m => !m.ActualTime.HasValue && !m.PostResultTime.HasValue
                    && (!m.PredictedTime.HasValue
                        || DateTimeOffset.FromUnixTimeSeconds(m.PredictedTime.Value) >= time.GetUtcNow().AddMinutes(-15)))
                .Take(input.numMatches);
        if (matchesToPost?.Any() is not true)
        {
            embedding.Description = "No matches scheduled yet.";
            yield return new(embedding.Build());
        }
        else
        {
            embedding.Description += " (All times Pacific, estimated)";

            var groupedMatches = matchesToPost.GroupBy(m => m.EventKey);

            yield return new(embedding.Build());

            foreach (var g in groupedMatches)
            {
                embedding = builderFactory.GetBuilder();
                if (string.IsNullOrEmpty(input.eventKey))
                {
                    embedding.Title = events[g.Key].GetLabel();
                    embedding.Url = events[g.Key].TbaUrl;
                }

                foreach (var m in g)
                {
                    var matchField = new EmbedFieldBuilder()
                        .WithName($"{m.CompLevel.ToShortString()} {m.SetNumber}.{m.MatchNumber}")
                        .WithIsInline(true);

                    DateTimeOffset? predictedTime = m.PredictedTime.HasValue
                        ? DateTimeOffset.FromUnixTimeSeconds(m.PredictedTime.Value).ToLocalTime(time) : null;
                    matchField.Value = predictedTime.HasValue
                        ? $"- {predictedTime.Value:M.d} @ {predictedTime.Value.ToString("h:mmt").ToLowerInvariant()}"
                        : "- Unknown Start Time";

                    embedding.AddField(matchField);
                }

                yield return new(embedding.Build());
            }
        }
    }
}
