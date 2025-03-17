namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;
using Discord.Interactions.Builders;

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
                               EventRepository events,
                               IDistrictApi tbaDistrictData,
                               Statbotics.Api.ITeamYearApi teamStats,
                               IRankingsApi rankings,
                               TimeProvider time,
                               ILogger<TeamRank> logger) : IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>
{
    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync((int? Year, string TeamKey, string? EventKey) input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var teamKey = input.TeamKey;
        var targetYear = input.Year ?? time.GetLocalNow().Year;
        var inputTeamNum = teamKey.TeamKeyToTeamNumber();
        if (inputTeamNum is null || !inputTeamNum.HasValue)
        {
            logger.UnableToGetTeamNumberFromKeyTeamKey(teamKey);
            yield return new(new EmbedBuilder().WithTitle($"Invalid team key: {teamKey}").WithDescription("Please provide a valid team key.").Build());
            yield break;
        }

        Debug.Assert(!highlightTeam.HasValue || inputTeamNum == highlightTeam.Value, "Given highlightTeam should match the input Team");
        highlightTeam ??= inputTeamNum;

        var teamNumberStr = highlightTeam.Value.ToString();
        yield return new(new EmbedBuilder().WithTitle($"{targetYear} Ranking detail for {teams[teamKey].GetLabel(includeLocation: false, asMarkdownLink: false)}").WithUrl($"https://frc.link/tba/{highlightTeam}").Build());

        var embedding = builderFactory.GetBuilder(teamKey);
        var descriptionBuilder = new StringBuilder();

        var frcTeamRankings = await rankings.SeasonRankingsDistrictGetAsync(targetYear.ToString(), teamNumber: teamNumberStr, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (frcTeamRankings is null or { DistrictRanks: null or { Length: 0 } })
        {
            descriptionBuilder.AppendLine($"No district ranking data found for {teams[teamKey].GetLabel()}");
        }
        else
        {
            var teamYearStats = await teamStats.ReadTeamYearV3TeamYearTeamYearGetAsync(teamNumberStr, targetYear, cancellationToken).ConfigureAwait(false);
            if (teamYearStats?.Epa?.Ranks is not null)
            {
                if (teamYearStats.Epa.TotalPoints?.Mean is not null)
                {
                    descriptionBuilder.AppendLine($"## EPA ({teamYearStats.Epa.TotalPoints.Mean:0.##}) rank\n");
                }
                else
                {
                    descriptionBuilder.AppendLine($"## EPA rank\n");
                }

                if (teamYearStats.Epa.Ranks.State is not null)
                {
                    descriptionBuilder.AppendLine($"State{(!string.IsNullOrWhiteSpace(teamYearStats.State) ? $" ({teamYearStats.State})" : string.Empty)}: {teamYearStats.Epa.Ranks.State.Rank}/{teamYearStats.Epa.Ranks.State.TeamCount} ({teamYearStats.Epa.Ranks.State.Percentile:P2}ile)");
                }

                if (teamYearStats.Epa.Ranks.District is not null)
                {
                    var districtDetail = tbaDistrictData.GetDistrictsByYear(targetYear)?.FirstOrDefault(d => d.Abbreviation.Equals(teamYearStats.District, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(districtDetail?.DisplayName))
                    {
                        descriptionBuilder.AppendLine($"District ({districtDetail.DisplayName}): {teamYearStats.Epa.Ranks.District.Rank} / {teamYearStats.Epa.Ranks.District.TeamCount} ({teamYearStats.Epa.Ranks.District.Percentile:P2}ile)");
                    }
                    else
                    {
                        descriptionBuilder.AppendLine($"District: {teamYearStats.Epa.Ranks.District.Rank} / {teamYearStats.Epa.Ranks.District.TeamCount} ({teamYearStats.Epa.Ranks.District.Percentile:P2}ile)");
                    }
                }

                if (teamYearStats.Epa.Ranks.Country is not null)
                {
                    descriptionBuilder.AppendLine($"Country ({teamYearStats.Country}): {teamYearStats.Epa.Ranks.Country.Rank} / {teamYearStats.Epa.Ranks.Country.TeamCount} ({teamYearStats.Epa.Ranks.Country.Percentile:P2}ile)");
                }

                if (teamYearStats.Epa.Ranks.Total is not null)
                {
                    descriptionBuilder.AppendLine($"World: {teamYearStats.Epa.Ranks.Total.Rank} / {teamYearStats.Epa.Ranks.Total.TeamCount} ({teamYearStats.Epa.Ranks.Total.Percentile:P2}ile)");
                }
            }

            foreach (var d in frcTeamRankings.DistrictRanks)
            {
                var districtDetail = await tbaDistrictData.GetDistrictsByYearAsync(targetYear, cancellationToken: cancellationToken);
                var district = districtDetail?.FirstOrDefault(i => i.Abbreviation.Equals(d.DistrictCode, StringComparison.OrdinalIgnoreCase));
                descriptionBuilder.AppendLine($"## {(district is not null ? district.DisplayName : d.DistrictCode)} District rank\n");
                descriptionBuilder.AppendLine($"Current Rank: {d.Rank}");
                descriptionBuilder.AppendLine($"District Points: {d.TotalPoints} (Adjusted - {d.AdjustmentPoints})");
                yield return new(embedding.WithDescription(descriptionBuilder.ToString()).Build());

                descriptionBuilder.Clear();
                if (district is not null)
                {
                    var tbaRanks = await tbaDistrictData.GetDistrictRankingsAsync(district.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
                    var tbaTeamRank = tbaRanks?.FirstOrDefault(tbaRanks => tbaRanks.TeamKey == teamKey);
                    if (tbaTeamRank is not null and { EventPoints: not null and { Count: > 0 } }
                        && string.IsNullOrWhiteSpace(input.EventKey))
                    {
                        descriptionBuilder.AppendLine($"### District Point breakdown by Event\n");
                        foreach (var e in tbaTeamRank.EventPoints)
                        {
                            descriptionBuilder.AppendLine($"**{events[e.EventKey].GetLabel()}**");
                            addEventDistrictPointsForEvent(descriptionBuilder, e);

                            yield return new(embedding.WithDescription(descriptionBuilder.ToString()).Build());
                            descriptionBuilder.Clear();
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(input.EventKey))
        {
            descriptionBuilder.AppendLine($"## {events[input.EventKey].GetLabel()}\n");
            var eventDetail = events[input.EventKey];
            Debug.Assert(eventDetail is not null);
            if (eventDetail is null)
            {
                descriptionBuilder.AppendLine($"No data found for {teams[teamKey].GetLabel()} at {events[input.EventKey].GetLabel()}");
            }
            else
            {
                var eventRankings = await rankings.SeasonRankingsEventCodeGetAsync(eventDetail.EventCode.ToUpperInvariant(), targetYear.ToString(), teamNumberStr, cancellationToken: cancellationToken).ConfigureAwait(false);
                Debug.Assert(eventRankings is not null);
                if (eventRankings is null)
                {
                    descriptionBuilder.AppendLine($"No data found for {teams[teamKey].GetLabel()} at {events[input.EventKey].GetLabel()}");
                }
                else
                {
                    var teamRanking = eventRankings.Rankings.FirstOrDefault(r => r.TeamNumber == highlightTeam);
                    Debug.Assert(teamRanking is not null);
                    if (teamRanking is null)
                    {
                        descriptionBuilder.AppendLine($"No data found for {teams[teamKey].GetLabel()} at {events[input.EventKey].GetLabel()}");
                    }
                    else
                    {
                        descriptionBuilder.AppendLine($"Rank: {teamRanking.Rank}");
                        descriptionBuilder.AppendLine($"Record: {teamRanking.Wins}-{teamRanking.Losses}-{teamRanking.Ties}");
                        descriptionBuilder.AppendLine($"Avg Qual Match Score: {teamRanking.QualAverage}");
                        descriptionBuilder.AppendLine($"DQ count: {teamRanking.Dq}");

                        var teamDistrict = await tbaDistrictData.GetTeamDistrictsAsync(teamKey, cancellationToken: cancellationToken).ConfigureAwait(false);
                        if (teamDistrict is not null and { Count: > 0 })
                        {
                            var tbaRanks = await tbaDistrictData.GetDistrictRankingsAsync(teamDistrict.First().Key, cancellationToken: cancellationToken).ConfigureAwait(false);
                            var tbaTeamRank = tbaRanks?.FirstOrDefault(tbaRanks => tbaRanks.TeamKey == teamKey);
                            var eventRank = tbaTeamRank?.EventPoints?.FirstOrDefault(i => i.EventKey == input.EventKey);
                            if (eventRank is not null)
                            {
                                descriptionBuilder.AppendLine($"District Points: {teamRanking.MatchesPlayed}");
                                addEventDistrictPointsForEvent(descriptionBuilder, eventRank);
                            }
                            else
                            {
                                logger.NoDistrictPointsDataFoundForTeamTeamKeyAtEventEventKey(teamKey, input.EventKey);
                            }
                        }
                        else
                        {
                            logger.NoDistrictDataFoundForTeamTeamKey(teamKey);
                        }
                    }
                }
            }

            yield return new(embedding.WithDescription(descriptionBuilder.ToString()).Build());
        }

        static void addEventDistrictPointsForEvent(StringBuilder descriptionBuilder, TheBlueAlliance.Model.DistrictRankingEventPointsInner e)
        {
            descriptionBuilder.AppendLine($"- Alliance: {e.AlliancePoints}");
            descriptionBuilder.AppendLine($"- Quals: {e.QualPoints}");
            descriptionBuilder.AppendLine($"- Elims: {e.ElimPoints}");
            descriptionBuilder.AppendLine($"- Awards: {e.AwardPoints}");
        }
    }
}
