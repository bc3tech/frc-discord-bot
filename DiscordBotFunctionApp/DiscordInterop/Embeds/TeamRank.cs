namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using DiscordBotFunctionApp.Storage;

using FIRST.Api;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using TheBlueAlliance.Api;

internal sealed class TeamRank(EmbedBuilderFactory builderFactory,
                        TeamRepository teams,
                        IEventApi tbaEventData,
                        IDistrictApi tbaDistrictData,
                        EventRepository events,
                        Statbotics.Api.ITeamYearApi teamStats,
                        IRankingsApi rankings,
                        ILogger<TeamRank> logger) : IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>
{
    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync((int? Year, string TeamKey, string? EventKey) input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var teamKey = input.TeamKey;
        var targetYear = input.Year ?? TimeProvider.System.GetLocalNow().Year;
        var inputTeamNum = teamKey.ToTeamNumber();
        if (inputTeamNum is null || !inputTeamNum.HasValue)
        {
            logger.UnableToGetTeamNumberFromKeyTeamKey(teamKey);
            yield return new(new EmbedBuilder().WithTitle($"Invalid team key: {teamKey}").WithDescription("Please provide a valid team key.").Build());
            yield break;
        }

        Debug.Assert(!highlightTeam.HasValue || inputTeamNum == highlightTeam.Value, "Given highlightTeam should match the input Team");
        highlightTeam ??= inputTeamNum;

        var teamNumberStr = highlightTeam.Value.ToString();
        yield return new(new EmbedBuilder().WithTitle($"{targetYear} Ranking detail for {teams.GetLabelForTeam(teamKey)}").WithUrl($"https://www.thebluealliance.com/team/{highlightTeam}").Build());

        var embedding = builderFactory.GetBuilder(teamKey);
        var description = new StringBuilder();

        var frcTeamRankings = await rankings.SeasonRankingsDistrictGetAsync(targetYear.ToString(), teamNumber: teamNumberStr, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (frcTeamRankings is null or { DistrictRanks: null or { Length: 0 } })
        {
            description.AppendLine($"No district ranking data found for {teams.GetLabelForTeam(teamKey)}");
        }
        else
        {
            var teamYearStats = await teamStats.ReadTeamYearV3TeamYearTeamYearGetAsync(teamNumberStr, targetYear, cancellationToken).ConfigureAwait(false);
            if (teamYearStats?.Epa?.Ranks is not null)
            {
                if (teamYearStats.Epa.TotalPoints?.Mean is not null)
                {
                    description.AppendLine($"## EPA ({teamYearStats.Epa.TotalPoints.Mean:0.##}) rank");
                }
                else
                {
                    description.AppendLine($"## EPA rank");
                }

                if (teamYearStats.Epa.Ranks.State is not null)
                {
                    description.AppendLine($"State{(!string.IsNullOrWhiteSpace(teamYearStats.State) ? $" ({teamYearStats.State})" : string.Empty)}: {teamYearStats.Epa.Ranks.State.Rank}/{teamYearStats.Epa.Ranks.State.TeamCount} ({teamYearStats.Epa.Ranks.State.Percentile:P2}ile)");
                }

                if (teamYearStats.Epa.Ranks.District is not null)
                {
                    var districtDetail = tbaDistrictData.GetDistrictsByYear(targetYear)?.FirstOrDefault(d => d.Abbreviation.Equals(teamYearStats.District, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(districtDetail?.DisplayName))
                    {
                        description.AppendLine($"District ({districtDetail.DisplayName}): {teamYearStats.Epa.Ranks.District.Rank} / {teamYearStats.Epa.Ranks.District.TeamCount} ({teamYearStats.Epa.Ranks.District.Percentile:P2}ile)");
                    }
                    else
                    {
                        description.AppendLine($"District: {teamYearStats.Epa.Ranks.District.Rank} / {teamYearStats.Epa.Ranks.District.TeamCount} ({teamYearStats.Epa.Ranks.District.Percentile:P2}ile)");
                    }
                }

                if (teamYearStats.Epa.Ranks.Country is not null)
                {
                    description.AppendLine($"Country ({teamYearStats.Country}): {teamYearStats.Epa.Ranks.Country.Rank} / {teamYearStats.Epa.Ranks.Country.TeamCount} ({teamYearStats.Epa.Ranks.Country.Percentile:P2}ile)");
                }

                if (teamYearStats.Epa.Ranks.Total is not null)
                {
                    description.AppendLine($"World: {teamYearStats.Epa.Ranks.Total.Rank} / {teamYearStats.Epa.Ranks.Total.TeamCount} ({teamYearStats.Epa.Ranks.Total.Percentile:P2}ile)");
                }
            }

            foreach (var d in frcTeamRankings.DistrictRanks)
            {
                var districtDetail = await tbaDistrictData.GetDistrictsByYearAsync(targetYear, cancellationToken: cancellationToken);
                var district = districtDetail?.FirstOrDefault(i => i.Abbreviation.Equals(d.DistrictCode, StringComparison.OrdinalIgnoreCase));
                description.AppendLine($"## {(district is not null ? district.DisplayName : d.DistrictCode)} District rank");
                description.AppendLine($"Current Rank: {d.Rank}");
                description.AppendLine($"District Points: {d.TotalPoints} (Adjusted - {d.AdjustmentPoints})");
                yield return new(embedding.WithDescription(description.ToString()).Build());

                description.Clear();
                if (district is not null)
                {
                    var tbaRanks = await tbaDistrictData.GetDistrictRankingsAsync(district.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
                    var tbaTeamRank = tbaRanks?.FirstOrDefault(tbaRanks => tbaRanks.TeamKey == teamKey);
                    if (tbaTeamRank is not null and { EventPoints: not null and { Count: > 0 } })
                    {
                        description.AppendLine($"### District Point breakdown by Event");
                        foreach (var e in tbaTeamRank.EventPoints)
                        {
                            description.AppendLine($"**{events.GetLabelForEvent(e.EventKey)}**");
                            description.AppendLine($"- Alliance: {e.AlliancePoints}");
                            description.AppendLine($"- Quals: {e.QualPoints}");
                            description.AppendLine($"- Elims: {e.ElimPoints}");
                            description.AppendLine($"- Awards: {e.AwardPoints}");

                            yield return new(embedding.WithDescription(description.ToString()).Build());
                            description.Clear();
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(input.EventKey))
        {
            description.AppendLine($"## {events.GetLabelForEvent(input.EventKey)}");
            var eventDetail = await tbaEventData.GetEventSimpleAsync(input.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
            Debug.Assert(eventDetail is not null);
            if (eventDetail is null)
            {
                description.AppendLine($"No data found for {teams.GetLabelForTeam(teamKey)} at {events.GetLabelForEvent(input.EventKey)}");
            }
            else
            {
                var eventRankings = await rankings.SeasonRankingsEventCodeGetAsync(eventDetail.EventCode.ToUpperInvariant(), targetYear.ToString(), teamNumberStr, cancellationToken: cancellationToken).ConfigureAwait(false);
                Debug.Assert(eventRankings is not null);
                if (eventRankings is null)
                {
                    description.AppendLine($"No data found for {teams.GetLabelForTeam(teamKey)} at {events.GetLabelForEvent(input.EventKey)}");
                }
                else
                {
                    var teamRanking = eventRankings.Rankings.FirstOrDefault(r => r.TeamNumber == highlightTeam);
                    Debug.Assert(teamRanking is not null);
                    if (teamRanking is null)
                    {
                        description.AppendLine($"No data found for {teams.GetLabelForTeam(teamKey)} at {events.GetLabelForEvent(input.EventKey)}");
                    }
                    else
                    {
                        description.AppendLine($"Rank: {teamRanking.Rank}");
                        description.AppendLine($"Record: {teamRanking.Wins}-{teamRanking.Losses}-{teamRanking.Ties}");
                        description.AppendLine($"Avg Qual Match Score: {teamRanking.QualAverage}");
                        description.AppendLine($"DQ count: {teamRanking.Dq}");
                    }
                }
            }

            yield return new(embedding.WithDescription(description.ToString()).Build());
        }
    }
}
