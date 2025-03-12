namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Extensions;

using DiscordBotFunctionApp.ChatBot;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Extensions;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using FIRST.Api;
using FIRST.Model;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchScoreBreakdown2025AllianceExtensions;

using Match = TheBlueAlliance.Model.Match;

internal sealed partial class MatchScore(IEventApi eventApi,
                                         IMatchApi matchApi,
                                         IDistrictApi districtApi,
                                         IScheduleApi schedule,
                                         EventRepository events,
                                         TeamRepository teams,
                                         EmbedBuilderFactory builderFactory,
                                         ChatRunner gpt,
                                         ILogger<MatchScore> logger) : INotificationEmbedCreator, IEmbedCreator<string>
{
    public const NotificationType TargetType = NotificationType.match_score;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        logger.CreatingMatchScoreEmbed(msg);
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.MatchScore>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var detailedMatch = await matchApi.GetMatchAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(notification.match_key);
            yield return new(baseBuilder.Build());
            yield break;
        }

        (int blueScore, int redScore) = GetActualScores(notification.match, detailedMatch);

        if (redScore is -1 || blueScore is -1)
        {
            yield return null;
            yield break;
        }

        logger.CreatingMatchScoreEmbedForMatch(JsonSerializer.Serialize(detailedMatch));

        StringBuilder descriptionBuilder = new();

        #region Header
        descriptionBuilder.AppendLine(
            $"""
            # Scores are in!
            
            Actual start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.ActualTime.GetValueOrDefault(0)).ToPacificTime():t}{(detailedMatch.PostResultTime.HasValue ? $"\nResults posted at {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PostResultTime.Value).ToPacificTime():t}" : string.Empty)}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, notification.match, detailedMatch, descriptionBuilder, (redScore, blueScore), cancellationToken);

        var embedding = baseBuilder
            .WithTitle($"{notification.event_name}: {Translator.CompLevelToShortString(notification.match!.CompLevel!.ToInvariantString()!)} {notification.match.SetNumber} - Match {notification.match.MatchNumber}")
            .WithDescription(descriptionBuilder.ToString());

        yield return new(embedding.Build());
    }

    private (int, int) GetActualScores(Match? notificationMatch, Match detailedMatch)
    {
        var blueScore = notificationMatch?.Alliances.Blue.Score ?? detailedMatch.Alliances.Blue.Score;
        if (blueScore is -1)
        {
            blueScore = notificationMatch?.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Blue.TotalPoints ?? detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Blue.TotalPoints ?? -1;
        }

        var redScore = notificationMatch?.Alliances.Red.Score ?? detailedMatch.Alliances.Red.Score;
        if (redScore is -1)
        {
            redScore = notificationMatch?.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Red.TotalPoints ?? detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Red.TotalPoints ?? -1;
        }

        if (redScore is -1 || blueScore is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(detailedMatch.Key, JsonSerializer.Serialize(detailedMatch));
        }

        return (redScore, blueScore);
    }

    public IAsyncEnumerable<ResponseEmbedding?> GetMatchScoreAsync(string matchKey, ushort? highlightTeam = null, CancellationToken cancellationToken = default) => CreateAsync(matchKey, highlightTeam, cancellationToken);

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string matchKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();

        var detailedMatch = await matchApi.GetMatchAsync(Throws.IfNullOrWhiteSpace(matchKey), cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(matchKey);
            yield return new(baseBuilder.Build());
            yield break;
        }

        (int redScore, int blueScore) = GetActualScores(null, detailedMatch);

        if (redScore is -1 || blueScore is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(detailedMatch.Key, JsonSerializer.Serialize(detailedMatch));
            yield return null;
            yield break;
        }

        StringBuilder descriptionBuilder = new();

        #region Header
        var compLevelHeader = $"{Translator.CompLevelToShortString(detailedMatch.CompLevel.ToInvariantString()!)} {detailedMatch.SetNumber}";
        var matchHeader = $"Match {detailedMatch.MatchNumber}";
        descriptionBuilder.AppendLine(
            $"""
            # Match Result

            ## {events.GetLabelForEvent(detailedMatch.EventKey)}: {compLevelHeader} - {matchHeader}
            Predicted start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PredictedTime.GetValueOrDefault(0)).ToPacificTime():t}
            Actual start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.ActualTime.GetValueOrDefault(0)).ToPacificTime():t}{(detailedMatch.PostResultTime.HasValue ? $"\nResults posted at {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PostResultTime.Value).ToPacificTime():t}" : string.Empty)}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, null, detailedMatch, descriptionBuilder, (redScore, blueScore), cancellationToken);

        var embedding = baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build();

        yield return new(embedding);

        bool first = true;
        var prompt = $"Create a narrative for this match:\n\n```json{JsonSerializer.Serialize(detailedMatch)}\n```";
        yield return new ResponseEmbedding(baseBuilder.WithDescription("Generating match summary... 🤖").Build(), Transient: true);
        await foreach (var completion in gpt.GetCompletionsAsync(prompt, cancellationToken))
        {
            var builder = builderFactory.GetBuilder(highlightTeam);
            if (first)
            {
                builder.Title = "AI Match Summary";
                first = false;
            }

            foreach (var descriptionChunk in completion.Chunk(4096))
            {
                yield return new(builder.WithDescription(new(descriptionChunk)).Build());
            }
        }

        if (first)
        {
            yield return new(baseBuilder.WithDescription("No summary generated. 🥺").Build());
        }
    }

    private async Task BuildDescriptionAsync(ushort? highlightTeam, Match? notificationMatch, Match detailedMatch, StringBuilder descriptionBuilder, (int redScore, int blueScore) scores, CancellationToken cancellationToken)
    {
        var preferredMatch = notificationMatch ?? detailedMatch;
        var winningAlliance = notificationMatch?.WinningAlliance ?? detailedMatch.WinningAlliance;
        if (winningAlliance is Match.WinningAllianceEnum.Empty)
        {
            winningAlliance = scores.redScore > scores.blueScore ? Match.WinningAllianceEnum.Red : scores.redScore == scores.blueScore ? Match.WinningAllianceEnum.Empty : Match.WinningAllianceEnum.Blue;
        }

        var alliances = notificationMatch?.Alliances ?? detailedMatch.Alliances;
        var allTeamKeys = alliances.Red.TeamKeys.Concat(alliances.Blue.TeamKeys).ToHashSet();

        int[] allianceRanks = await GetAllianceRanksAsync(detailedMatch, alliances, cancellationToken).ConfigureAwait(false);

        var scoreBreakdown = notificationMatch?.ScoreBreakdown?.GetMatchScoreBreakdown2025() ?? detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025();
        var ranks = (await eventApi.GetEventRankingsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.Rankings
            .Where(i => allTeamKeys.Contains(i.TeamKey))
            .ToDictionary(i => i.TeamKey, i => i.Rank);
        var districtPoints = await ComputeDistrictPointsForTeamsAsync(notificationMatch?.EventKey ?? detailedMatch.EventKey, allTeamKeys, cancellationToken);

        #region Red Score Breakdown
        string? rankingPointsValue = preferredMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red)?.ToString();
        string rankingPointsStr = $" (+{(string.IsNullOrWhiteSpace(rankingPointsValue) ? "?" : rankingPointsValue)})";
        descriptionBuilder.AppendLine($"### {(winningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)}Red Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red]})" : string.Empty)} - {alliances.Red.Score}{(preferredMatch.CompLevel is Match.CompLevelEnum.Qm ? rankingPointsStr : string.Empty)}");
        descriptionBuilder.AppendLine($"{string.Join("\n", alliances.Red.TeamKeys.Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp) " : string.Empty)}"))}");
        if (scoreBreakdown?.Red is null)
        {
            descriptionBuilder.AppendLine("No score breakdown given");
            logger.LogMetric("NoRedScoreBreakdown", 1);
        }
        else
        {
            logger.ScoreBreakdownScoreBreakdown(JsonSerializer.Serialize(scoreBreakdown));
            descriptionBuilder.AppendLine(
                                       $"""
                                       - **Score Breakdown**
                                         - Auto: {scoreBreakdown.Red.AutoPoints}
                                       """);
            descriptionBuilder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Red.AutoCoralPoints}pts");
            descriptionBuilder.AppendLine($"  - Teleop: {scoreBreakdown.Red.TeleopPoints}");
            descriptionBuilder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Red.TeleopCoralPoints}pts");
            descriptionBuilder.AppendLine($"  - Endgame: ({scoreBreakdown.Red.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Red.EndGameBargePoints}pts");
            descriptionBuilder.AppendLine($"  - Algae(net / wall): {scoreBreakdown.Red.NetAlgaeCount}/{scoreBreakdown.Red.WallAlgaeCount} - {scoreBreakdown.Red.AlgaePoints}pts");
            if (detailedMatch.CompLevel is Match.CompLevelEnum.Qm)
            {
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Red.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Red.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Red.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot3.ToGlyph()})");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Red.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Red.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                descriptionBuilder.AppendLine($"  - {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Red)} Win RP(3)");
            }
        }
        #endregion

        cancellationToken.ThrowIfCancellationRequested();

        #region Blue Score Breakdown
        rankingPointsValue = preferredMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue)?.ToString();
        rankingPointsStr = $" (+{(string.IsNullOrWhiteSpace(rankingPointsValue) ? "?" : rankingPointsValue)})";
        descriptionBuilder.AppendLine($"### {(winningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)}Blue Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue]})" : string.Empty)} - {alliances.Blue.Score}{(preferredMatch.CompLevel is Match.CompLevelEnum.Qm ? rankingPointsStr : string.Empty)}");
        descriptionBuilder.AppendLine($"{string.Join("\n", alliances.Blue.TeamKeys.Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp)" : string.Empty)}"))}");
        if (scoreBreakdown?.Red is null)
        {
            descriptionBuilder.AppendLine("No score breakdown given");
            logger.LogMetric("NoBlueScoreBreakdown", 1);
        }
        else
        {
            descriptionBuilder.AppendLine(
                                       $"""
                                       - **Score Breakdown**
                                         - Auto: {scoreBreakdown.Blue.AutoPoints}
                                       """);
            descriptionBuilder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Blue.AutoCoralPoints}pts");
            descriptionBuilder.AppendLine($"  - Teleop: {scoreBreakdown.Blue.TeleopPoints}");
            descriptionBuilder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Blue.TeleopCoralPoints}pts");
            descriptionBuilder.AppendLine($"  - Endgame: ({scoreBreakdown.Blue.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Blue.EndGameBargePoints}pts");
            descriptionBuilder.AppendLine($"  - Algae(net / wall): {scoreBreakdown.Blue.NetAlgaeCount}/{scoreBreakdown.Blue.WallAlgaeCount} - {scoreBreakdown.Blue.AlgaePoints}pts");
            if (detailedMatch.CompLevel is Match.CompLevelEnum.Qm)
            {
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Blue.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Blue.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Blue.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot3.ToGlyph()})");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Blue.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                descriptionBuilder.AppendLine($"  - {scoreBreakdown.Blue.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                descriptionBuilder.AppendLine($"  - {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Blue)} Win RP(3)");
            }
        }
        #endregion

        await AddEventOrDayWrapupAsync(descriptionBuilder, notificationMatch, detailedMatch, winningAlliance, cancellationToken).ConfigureAwait(false);

        var videos = (notificationMatch?.Videos ?? detailedMatch.Videos)?.Where(v => v.Type is "youtube" && v.Key is not null).Select(v => $"- https://www.youtube.com/watch?v={v.Key}");
        if (videos?.Any() is true)
        {
            descriptionBuilder
                .AppendLine("### Videos")
                .AppendLine(string.Join("\n", videos));
        }

        descriptionBuilder.AppendLine($"\nView more match details [here](https://www.thebluealliance.com/match/{notificationMatch?.Key ?? detailedMatch.Key})");
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex AllianceRankRegex();

    private async Task<int[]> GetAllianceRanksAsync(Match match, MatchSimpleAlliances alliances, CancellationToken cancellationToken)
    {
        var retVal = new int[] { 0, 0, 0 };
        if (match.CompLevel is not Match.CompLevelEnum.Qm)
        {
            var eventAlliances = await eventApi.GetEventAlliancesAsync(match.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var eliminationAlliance in eventAlliances ?? [])
            {
                if (!eliminationAlliance.Picks.Except(alliances.Red.TeamKeys).Any())
                {
                    retVal[(int)MatchSimple.WinningAllianceEnum.Red] = int.Parse(AllianceRankRegex().Match(eliminationAlliance.Name ?? "0").Value);
                    continue;
                }
                else if (!eliminationAlliance.Picks.Except(alliances.Blue.TeamKeys).Any())
                {
                    retVal[(int)MatchSimple.WinningAllianceEnum.Blue] = int.Parse(AllianceRankRegex().Match(eliminationAlliance.Name ?? "0").Value);
                    continue;
                }
            }
        }

        return retVal;
    }

    private async Task AddEventOrDayWrapupAsync(StringBuilder descriptionBuilder, Match? notificationMatch, Match detailedMatch, Match.WinningAllianceEnum winningAlliance, CancellationToken cancellationToken)
    {
        var currentMatchNumber = notificationMatch?.MatchNumber ?? detailedMatch.MatchNumber;
        var isPossibleEventEnd = (notificationMatch?.CompLevel ?? detailedMatch.CompLevel) is Match.CompLevelEnum.F && currentMatchNumber > 1;

        if (isPossibleEventEnd)
        {
            var winCounts = new int[3] { 0, 0, 0 };
            winCounts[(int)winningAlliance]++;

            var match = await matchApi.GetMatchAsync($"{detailedMatch.EventKey}_f1m1", cancellationToken: cancellationToken).ConfigureAwait(false);
            Debug.Assert(match is not null);

            winCounts[(int)match.WinningAlliance]++;

            if (currentMatchNumber is 3)
            {
                match = await matchApi.GetMatchAsync($"{detailedMatch.EventKey}_f1m1", cancellationToken: cancellationToken).ConfigureAwait(false);
                Debug.Assert(match is not null);

                winCounts[(int)match.WinningAlliance]++;
            }

            // check if blue or red has a count of 2
            // if so, then the event is over
            if (winCounts[1] is 2 || winCounts[2] is 2)
            {
                descriptionBuilder.AppendLine($"### 🎉{(winCounts[1] is 2 ? "Red" : "Blue")} Alliance wins the event! Congratulations!!🎉");
            }
        }
        else
        {
            var eventPieces = EventPartsRegex().Match(notificationMatch?.EventKey ?? detailedMatch.EventKey);
            Debug.Assert(eventPieces.Success);
            var eventCode = eventPieces.Groups[2].Value;
            var season = eventPieces.Groups[1].Value;

            var firstTourneyLevel = ((notificationMatch?.CompLevel ?? detailedMatch.CompLevel) is Match.CompLevelEnum.Qm) ? TournamentLevel.Qualification : TournamentLevel.Playoff;
            var eventSchedule = await schedule.SeasonScheduleEventCodeGetAsync(eventCode, season, tournamentLevel: firstTourneyLevel, cancellationToken: cancellationToken);
            Debug.Assert(eventSchedule is not null, "We expect to find the event in the schedule using the event code & season");
            if (eventSchedule is not null)
            {
                string compLevel = notificationMatch?.CompLevel.ToInvariantString() ?? detailedMatch.CompLevel.ToInvariantString();
                var matchNumber = notificationMatch?.MatchNumber ?? detailedMatch.MatchNumber;
                var thisMatch = eventSchedule.Schedule.FirstOrDefault(i => i.MatchNumber == matchNumber);
                Debug.Assert(thisMatch is not null, "We expect to find the match in the schedule using the comp level & the match number");
                if (thisMatch is not null)
                {
                    var lastEventOfEachDay = eventSchedule.Schedule
                        .GroupBy(i => i.StartTime.GetValueOrDefault(DateTimeOffset.MinValue).Date)
                        .Select(i => i.MaxBy(j => j.StartTime.GetValueOrDefault(DateTimeOffset.MinValue))!);
                    if (lastEventOfEachDay.Any(i => i.MatchNumber == thisMatch.MatchNumber))
                    {
                        descriptionBuilder.AppendLine("### That's it for today! Matches will continue tomorrow.");
                    }
                }
                else
                {
                    logger.WeCouldNotFindTheMatchInTheScheduleUsingTheCompLevelCompLevelTheMatchNumberMatchNumber(compLevel, matchNumber);
                }
            }
            else
            {
                logger.WeCouldNotFindTheEventInTheScheduleUsingTheEventCodeEventCodeSeasonSeason(eventCode, season);
            }
        }
    }

    [GeneratedRegex(@"(\d+)(\w+)", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex EventPartsRegex();

    private async Task<IReadOnlyDictionary<string, int>> ComputeDistrictPointsForTeamsAsync(string eventKey, IEnumerable<string> allTeamKeys, CancellationToken cancellationToken)
    {
        var districtPoints = await districtApi.GetEventDistrictPointsAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        Dictionary<string, int> retVal = [];
        if (districtPoints is null)
        {
            return retVal;
        }

        foreach (var teamKey in allTeamKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!districtPoints.Points.TryGetValue(teamKey, out var dpEntry))
            {
                retVal.Add(teamKey, 0);
            }
            else
            {
                retVal.Add(teamKey, dpEntry.Total);
            }
        }

        return retVal.AsReadOnly();
    }
}
