namespace FunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Extensions;

using Discord;
using Discord.Net;
using Discord.WebSocket;

using FIRST.Api;
using FIRST.Model;

using FunctionApp.ChatBot;
using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.Extensions;
using FunctionApp.Storage;
using FunctionApp.TbaInterop;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.CompLevelExtensions;

using Match = TheBlueAlliance.Model.Match;

internal sealed partial class MatchScore(IEventApi eventApi,
                                         IMatchApi matchApi,
                                         IDistrictApi districtApi,
                                         IScheduleApi schedule,
                                         EventRepository events,
                                         TeamRepository teams,
                                         EmbedBuilderFactory builderFactory,
                                         TimeProvider time,
                                         Meter meter,
                                         ILogger<MatchScore> logger,
                                         ChatRunner? gpt = null) : INotificationEmbedCreator, IEmbedCreator<(string matchKey, bool summarize)>, IHandleUserInteractions
{
    public const NotificationType TargetType = NotificationType.match_score;
    public const string GetBreakdownButtonId = "get-score-breakdown";

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        logger.CreatingMatchScoreEmbed();
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<FunctionApp.TbaInterop.Models.Notifications.MatchScore>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        var tbaMatch = await matchApi.GetMatchAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (tbaMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(notification.match_key);
            yield return null;
            yield break;
        }

        var scores = GetActualScores(notification.match, tbaMatch);
        if (scores.Red is -1 || scores.Blue is -1)
        {
            yield return null;
            yield break;
        }

        logger.CreatingMatchScoreEmbedForMatch(JsonSerializer.Serialize(tbaMatch));

        StringBuilder descriptionBuilder = new();

        #region Header
        var postResultTime = notification.match?.PostResultTime.Or(tbaMatch.PostResultTime);
        descriptionBuilder.AppendLine($"# Scores are in!");
        #endregion

        await BuildDescriptionAsync(highlightTeam, notification.match, tbaMatch, descriptionBuilder, scores, false, cancellationToken).ConfigureAwait(false);

        var embedding = baseBuilder
            .WithTitle($"{notification.event_name}: {notification.match?.CompLevel.Or(tbaMatch.CompLevel).ToShortString()} {notification.match?.SetNumber.Or(tbaMatch.SetNumber)} - Match {notification.match?.MatchNumber.Or(tbaMatch.MatchNumber)}")
            .WithDescription(descriptionBuilder.ToString());

        yield return new(embedding.Build(), Actions: [ButtonBuilder.CreatePrimaryButton("Get breakdown", $"{GetBreakdownButtonId}_{notification.match_key}").Build()]);
    }

    private (int Red, int Blue) GetActualScores(Match? notificationMatch, Match tbaMatch)
    {
        using var scope = logger.CreateMethodScope();
        var match = notificationMatch ?? tbaMatch;
        var blueScore = match.Alliances.Blue.Score;
        if (blueScore is -1)
        {
            blueScore = GetTotalPoints(match.ScoreBreakdown, Match.WinningAllianceEnum.Blue).Or(GetTotalPoints(tbaMatch.ScoreBreakdown, Match.WinningAllianceEnum.Blue)) ?? -1;
        }

        var redScore = match.Alliances.Red.Score;
        if (redScore is -1)
        {
            redScore = GetTotalPoints(match.ScoreBreakdown, Match.WinningAllianceEnum.Red).Or(GetTotalPoints(tbaMatch.ScoreBreakdown, Match.WinningAllianceEnum.Red)) ?? -1;
        }

        if (redScore is -1 || blueScore is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(match.Key, JsonSerializer.Serialize(match));
        }

        return (redScore, blueScore);
    }

    private static int? GetTotalPoints(MatchScoreBreakdown? scoreBreakdown, Match.WinningAllianceEnum alliance)
    {
        var allianceBreakdown = GetAllianceBreakdown(scoreBreakdown, alliance);
        return allianceBreakdown?.GetType().GetProperty("TotalPoints")?.GetValue(allianceBreakdown) as int?;
    }

    private static object? GetAllianceBreakdown(MatchScoreBreakdown? scoreBreakdown, Match.WinningAllianceEnum alliance)
    {
        return scoreBreakdown?.ActualInstance?.GetType().GetProperty(alliance.ToInvariantString())?.GetValue(scoreBreakdown.ActualInstance);
    }

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync((string matchKey, bool summarize) input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();

        var matchKey = input.matchKey;
        var detailedMatch = await matchApi.GetMatchAsync(Throws.IfNullOrWhiteSpace(matchKey), cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(matchKey);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var scores = GetActualScores(detailedMatch, detailedMatch);
        if (scores.Red is -1 || scores.Blue is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(detailedMatch.Key, JsonSerializer.Serialize(detailedMatch));
            yield return null;
            yield break;
        }

        StringBuilder descriptionBuilder = new();

        #region Header
        var compLevelHeader = $"{detailedMatch.CompLevel.ToShortString()} {detailedMatch.SetNumber}";
        var matchHeader = $"Match {detailedMatch.MatchNumber}";
        descriptionBuilder.AppendLine(
            $"""
            # Match Result

            ## {events[detailedMatch.EventKey].GetLabel()}: {compLevelHeader} - {matchHeader}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, detailedMatch, detailedMatch, descriptionBuilder, scores, true, cancellationToken).ConfigureAwait(false);

        var embedding = baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build();

        yield return new(embedding);

        if (input.summarize && gpt is not null)
        {
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
    }

    private async Task BuildDescriptionAsync(ushort? highlightTeam, Match? notificationMatch, Match tbaMatch, StringBuilder descriptionBuilder, (int redScore, int blueScore) scores, bool includeBreakdown, CancellationToken cancellationToken)
    {
        var match = notificationMatch ?? tbaMatch;
        var winningAlliance = match.WinningAlliance;
        if (winningAlliance is Match.WinningAllianceEnum.Empty)
        {
            winningAlliance = scores.redScore > scores.blueScore ? Match.WinningAllianceEnum.Red : scores.redScore == scores.blueScore ? Match.WinningAllianceEnum.Empty : Match.WinningAllianceEnum.Blue;
        }

        await BuildBreakdownDetailAsync(descriptionBuilder, notificationMatch, tbaMatch, scores, highlightTeam, includeBreakdown, cancellationToken: cancellationToken).ConfigureAwait(false);

        await AddEventOrDayWrapupAsync(descriptionBuilder, notificationMatch, tbaMatch, winningAlliance, cancellationToken).ConfigureAwait(false);

        var videos = match.Videos.Or(tbaMatch.Videos)?.Where(v => v.Type is "youtube" && v.Key is not null).Select(v => $"- https://www.youtube.com/watch?v={v.Key}");
        if (videos?.Any() is true)
        {
            descriptionBuilder
                .AppendLine("### Videos")
                .AppendLine(string.Join("\n", videos));
        }

#pragma warning disable EA0001 // Perform message formatting in the body of the logging method
        logger.EmbeddingNameBuiltEmbeddingDetail(nameof(MatchScore), descriptionBuilder.ToString());
#pragma warning restore EA0001 // Perform message formatting in the body of the logging method
    }

    private async Task BuildBreakdownDetailAsync(StringBuilder builder, Match? notificationMatch, Match tbaMatch, (int red, int blue)? scores = null, ushort? highlightTeam = null, bool includeFullBreakdown = false, CancellationToken cancellationToken = default)
    {
        scores ??= GetActualScores(notificationMatch, tbaMatch);

        var match = notificationMatch ?? tbaMatch;
        var alliances = match.Alliances;
        var allTeamKeys = alliances.Red.TeamKeys.Concat(alliances.Blue.TeamKeys).ToHashSet();
        var winningAlliance = match.WinningAlliance.UnlessThen(i => i is Match.WinningAllianceEnum.Empty, tbaMatch.WinningAlliance);

        int[] allianceRanks = await GetAllianceRanksAsync(notificationMatch, tbaMatch, alliances, cancellationToken).ConfigureAwait(false);
        var districtPoints = await ComputeDistrictPointsForTeamsAsync(match.EventKey, allTeamKeys, cancellationToken).ConfigureAwait(false);

        bool isQuals = match.CompLevel is CompLevel.Qm;
        var scoreBreakdown = includeFullBreakdown ? await GetScoreBreakdownAsync(notificationMatch, tbaMatch, cancellationToken).ConfigureAwait(false) : null;

        var eventHighScore = await GetHighScoreForEventAsync(notificationMatch?.EventKey ?? tbaMatch.EventKey, cancellationToken).ConfigureAwait(false);
        #region Red Score Breakdown
        builder.Append($"### {(winningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)}Red Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red]})" : string.Empty)} - {scores.Value.red}{(scores.Value.red >= eventHighScore ? "⭐" : string.Empty)}");
        if (isQuals)
        {
            var rankingPointsValue = match.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red).Or(tbaMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red)).ToString();
            while (string.IsNullOrWhiteSpace(rankingPointsValue))
            {
                logger.RankingPointsWereEmptyForMatchKey1sPollUntilTheyGoLive(tbaMatch.Key);

                await Task.Delay(TimeSpan.FromSeconds(1), time, cancellationToken).ConfigureAwait(false);
                tbaMatch = await matchApi.GetMatchAsync(tbaMatch.Key, cancellationToken: cancellationToken).ConfigureAwait(false) ?? tbaMatch;
                rankingPointsValue = tbaMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red).ToString();
            }

            builder.AppendLine($" (+{(string.IsNullOrWhiteSpace(rankingPointsValue) ? "?" : rankingPointsValue)})");
        }
        else
        {
            builder.AppendLine();
        }

        var ranks = (await eventApi.GetEventRankingsAsync(match.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.Rankings
            .Where(i => allTeamKeys.Contains(i.TeamKey))
            .ToDictionary(i => i.TeamKey, i => i.Rank);

        builder.AppendLine($"{string.Join("\n", alliances.Red.TeamKeys.Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam, includeLocation: false)}{(ranks is not null ? $" (#{ranks[t]}{(districtPoints is not null ? $", +{districtPoints[t]}dp" : string.Empty)})" : string.Empty)}"))}");

        if (includeFullBreakdown)
        {
            var redBreakdown = GetAllianceBreakdown(scoreBreakdown, Match.WinningAllianceEnum.Red);
            if (redBreakdown is null)
            {
                builder.AppendLine("No score breakdown given");
                meter.LogMetric("NoRedScoreBreakdown", 1);
            }
            else
            {
                logger.ScoreBreakdownScoreBreakdown(JsonSerializer.Serialize(scoreBreakdown));
                AppendSeasonBreakdown(builder, redBreakdown, isQuals, Match.WinningAllianceEnum.Red, winningAlliance);
            }
        }
        #endregion

        cancellationToken.ThrowIfCancellationRequested();

        #region Blue Score Breakdown
        builder.Append($"### {(winningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)}Blue Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue]})" : string.Empty)} - {scores.Value.blue}{(scores.Value.blue >= eventHighScore ? "⭐" : string.Empty)}");
        if (isQuals)
        {
            var rankingPointsValue = match.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue).Or(tbaMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue)).ToString();
            Debug.Assert(!string.IsNullOrWhiteSpace(rankingPointsValue));
            builder.AppendLine($" (+{(string.IsNullOrWhiteSpace(rankingPointsValue) ? "?" : rankingPointsValue)})");
        }
        else
        {
            builder.AppendLine();
        }

        builder.AppendLine($"{string.Join("\n", alliances.Blue.TeamKeys.Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam, includeLocation: false)}{(ranks is not null ? $" (#{ranks[t]}{(districtPoints is not null ? $", +{districtPoints[t]}dp" : string.Empty)})" : string.Empty)}"))}");

        if (includeFullBreakdown)
        {
            var blueBreakdown = GetAllianceBreakdown(scoreBreakdown, Match.WinningAllianceEnum.Blue);
            if (blueBreakdown is null)
            {
                builder.AppendLine("No score breakdown given");
                meter.LogMetric("NoBlueScoreBreakdown", 1);
            }
            else
            {
                AppendSeasonBreakdown(builder, blueBreakdown, isQuals, Match.WinningAllianceEnum.Blue, winningAlliance);
            }
        }
        #endregion

        if (scores.Value.red >= eventHighScore || scores.Value.blue >= eventHighScore)
        {
            builder.AppendLine()
                .AppendLine("-# ⭐ Event high score");
        }
    }

    private async ValueTask<ushort> GetHighScoreForEventAsync(string eventKey, CancellationToken cancellationToken)
    {
        var matchesInEvent = await eventApi.GetEventMatchesSimpleAsync(eventKey, cancellationToken: cancellationToken).ConfigureAwait(false) ?? [];
        return (ushort)Math.Max(0, matchesInEvent.Max(i => Math.Max(i.Alliances.Blue.Score, i.Alliances.Red.Score)));
    }

    private async Task<MatchScoreBreakdown?> GetScoreBreakdownAsync(Match? notificationMatch, Match tbaMatch, CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        var breakdown = notificationMatch?.ScoreBreakdown.Or(tbaMatch.ScoreBreakdown);

        while (breakdown is null && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), time, cancellationToken).ConfigureAwait(false);
            try
            {
                breakdown = (await matchApi.GetMatchAsync(tbaMatch.Key, cancellationToken: cancellationToken).ConfigureAwait(false))?.ScoreBreakdown;

                if (time.GetElapsedTime(startTime).TotalMinutes >= 5)
                {
                    logger.ScoreBreakdownForMatchMatchKeyNotAvailableAfter5Minutes(tbaMatch.Key);
                    return breakdown;
                }

                if (breakdown is not null)
                {
                    var redRp = GetRankingPoints(breakdown, Match.WinningAllianceEnum.Red);
                    if (redRp is > 6 or < 0)
                    {
                        logger.InvalidRedRPValueForMatchMatchKeyRpValue(tbaMatch.Key, redRp.Value);
                        breakdown = null;
                    }
                    else
                    {
                        var blueRp = GetRankingPoints(breakdown, Match.WinningAllianceEnum.Blue);
                        if (blueRp is > 6 or < 0)
                        {
                            logger.InvalidBlueRPValueForMatchMatchKeyRpValue(tbaMatch.Key, blueRp.Value);
                            breakdown = null;
                        }
                    }
                }
                else
                {
                    logger.NoScoreBreakdownAvailableForMatchMatchKey(tbaMatch.Key);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorGettingMatchDataForMatchKeyContinuingToTry(ex, tbaMatch.Key);
            }
        }

        if (breakdown is not null)
        {
            meter.LogMetric("ScoreBreakdownAvailableTimeSec", time.GetElapsedTime(startTime).TotalSeconds, new Dictionary<string, object?> { { "MatchKey", tbaMatch.Key } });
        }

        return breakdown;
    }

    private static int? GetRankingPoints(MatchScoreBreakdown? scoreBreakdown, Match.WinningAllianceEnum alliance)
    {
        var allianceBreakdown = GetAllianceBreakdown(scoreBreakdown, alliance);
        return allianceBreakdown?.GetType().GetProperty("Rp")?.GetValue(allianceBreakdown) as int?;
    }

    private static void AppendSeasonBreakdown(StringBuilder builder, object allianceBreakdown, bool isQuals, Match.WinningAllianceEnum allianceColor, Match.WinningAllianceEnum winningAlliance)
    {
        switch (allianceBreakdown)
        {
            case MatchScoreBreakdown2025Alliance season2025:
                builder.AppendLine($"""
                                    - **Score Breakdown**
                                        - Auto: {season2025.AutoPoints}
                                    """);
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {season2025.AutoReef.TbaTopRowCount.ToStringHandleUnknown()}/{season2025.AutoReef.TbaMidRowCount.ToStringHandleUnknown()}/{season2025.AutoReef.TbaBotRowCount.ToStringHandleUnknown()}/{season2025.AutoReef.Trough.ToStringHandleUnknown()} - {season2025.AutoCoralPoints}pts");
                builder.AppendLine($"  - Teleop: {season2025.TeleopPoints}");
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {season2025.TeleopReef.TbaTopRowCount.ToStringHandleUnknown()}/{season2025.TeleopReef.TbaMidRowCount.ToStringHandleUnknown()}/{season2025.TeleopReef.TbaBotRowCount.ToStringHandleUnknown()}/{season2025.TeleopReef.Trough.ToStringHandleUnknown()} - {season2025.TeleopCoralPoints}pts");
                builder.AppendLine($"  - Endgame: ({season2025.EndGameRobot1.ToGlyph()}/{season2025.EndGameRobot2.ToGlyph()}/{season2025.EndGameRobot3.ToGlyph()}) {season2025.EndGameBargePoints}pts");
                builder.AppendLine($"  - Algae(net / wall): {season2025.NetAlgaeCount}/{season2025.WallAlgaeCount} - {season2025.AlgaePoints}pts");
                if (isQuals)
                {
                    builder.AppendLine($"  - {season2025.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                    builder.AppendLine($"  - {season2025.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({season2025.AutoLineRobot1.ToGlyph()}/{season2025.AutoLineRobot2.ToGlyph()}/{season2025.AutoLineRobot3.ToGlyph()})");
                    builder.AppendLine($"  - {season2025.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                    builder.AppendLine($"  - {season2025.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                    builder.AppendLine($"  - {winningAlliance.ToGlyph(allianceColor)} Win RP(3)");
                }

                break;
            case MatchScoreBreakdown2026Alliance season2026:
                builder.AppendLine($"""
                                    - **Score Breakdown**
                                        - Auto: {season2026.TotalAutoPoints}
                                    """);
                builder.AppendLine($"  - Auto tower: ({season2026.AutoTowerRobot1.ToGlyph()}/{season2026.AutoTowerRobot2.ToGlyph()}/{season2026.AutoTowerRobot3.ToGlyph()}) {season2026.AutoTowerPoints}pts");
                builder.AppendLine($"  - Teleop: {season2026.TotalTeleopPoints}");
                builder.AppendLine($"  - Hub( auto/transition/s1/s2/s3/s4/endgame ): {season2026.HubScore.AutoCount}/{season2026.HubScore.TransitionCount}/{season2026.HubScore.Shift1Count}/{season2026.HubScore.Shift2Count}/{season2026.HubScore.Shift3Count}/{season2026.HubScore.Shift4Count}/{season2026.HubScore.EndgameCount} - {season2026.HubScore.TotalPoints}pts");
                builder.AppendLine($"  - Endgame tower: ({season2026.EndGameTowerRobot1.ToGlyph()}/{season2026.EndGameTowerRobot2.ToGlyph()}/{season2026.EndGameTowerRobot3.ToGlyph()}) {season2026.EndGameTowerPoints}pts");
                builder.AppendLine($"  - Penalties: {season2026.MinorFoulCount} minor, {season2026.MajorFoulCount} major, G206={((bool?)season2026.G206Penalty).ToGlyph()} (+{season2026.FoulPoints}pts)");
                if (isQuals)
                {
                    builder.AppendLine($"  - {((bool?)season2026.EnergizedAchieved).ToGlyph()} Energized RP");
                    builder.AppendLine($"  - {((bool?)season2026.SuperchargedAchieved).ToGlyph()} Supercharged RP");
                    builder.AppendLine($"  - {((bool?)season2026.TraversalAchieved).ToGlyph()} Traversal RP");
                    builder.AppendLine($"  - {winningAlliance.ToGlyph(allianceColor)} Win RP");
                }

                break;
            default:
                builder.AppendLine("- **Score Breakdown**");
                builder.AppendLine($"  - Total points: {GetTotalPointsForAllianceBreakdown(allianceBreakdown)?.ToString() ?? "Unknown"}");
                break;
        }
    }

    private static int? GetTotalPointsForAllianceBreakdown(object allianceBreakdown)
    {
        return allianceBreakdown.GetType().GetProperty("TotalPoints")?.GetValue(allianceBreakdown) as int?;
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex AllianceRankRegex();

    private async Task<int[]> GetAllianceRanksAsync(Match? notificationMatch, Match tbaMatch, MatchAlliances alliances, CancellationToken cancellationToken)
    {
        var retVal = new int[] { 0, 0, 0 };
        var match = notificationMatch ?? tbaMatch;
        if (match.CompLevel is not CompLevel.Qm)
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

    private async Task AddEventOrDayWrapupAsync(StringBuilder descriptionBuilder, Match? notificationMatch, Match tbaMatch, Match.WinningAllianceEnum winningAlliance, CancellationToken cancellationToken)
    {
        var match = notificationMatch ?? tbaMatch;
        var matchNumber = match.MatchNumber;
        var isPossibleEventEnd = match.CompLevel is CompLevel.F && matchNumber > 1;
        string eventKey = match.EventKey;

        if (isPossibleEventEnd)
        {
            var winCounts = new int[3] { 0, 0, 0 };
            winCounts[(int)winningAlliance]++;

            var finalMatch = await matchApi.GetMatchAsync($"{eventKey}_f1m1", cancellationToken: cancellationToken).ConfigureAwait(false);
            Debug.Assert(finalMatch is not null);

            winCounts[(int)finalMatch.WinningAlliance]++;

            if (matchNumber is 3)
            {
                finalMatch = await matchApi.GetMatchAsync($"{eventKey}_f1m2", cancellationToken: cancellationToken).ConfigureAwait(false);
                Debug.Assert(finalMatch is not null);

                winCounts[(int)finalMatch.WinningAlliance]++;
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
            var eventPieces = EventPartsRegex().Match(eventKey);
            Debug.Assert(eventPieces.Success);
            var season = eventPieces.Groups[1].Value;
            var eventCode = eventPieces.Groups[2].Value;
            var tourneyLevel = (match.CompLevel is CompLevel.Qm) ? TournamentLevel.Qualification : TournamentLevel.Playoff;
            try
            {
                var eventSchedule = await schedule.SeasonScheduleEventCodeGetAsync(eventCode, season, tournamentLevel: tourneyLevel, cancellationToken: cancellationToken).ConfigureAwait(false);
                Debug.Assert(eventSchedule is not null, "We expect to find the event in the schedule using the event code & season");
                if (eventSchedule is not null)
                {
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
                        logger.WeCouldNotFindTheMatchInTheScheduleUsingTheCompLevelCompLevelTheMatchNumberMatchNumber(match.CompLevel.ToInvariantString(), matchNumber);
                    }
                }
                else
                {
                    logger.WeCouldNotFindTheEventInTheScheduleUsingTheEventCodeEventCodeSeasonSeason(eventCode, season);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
#pragma warning disable EA0011 // Consider removing unnecessary conditional access operator (?); except it's not known to be not null.
                logger.ErrorGettingEventScheduleForEventKey(ex, notificationMatch.EventKey);
#pragma warning restore EA0011 // Consider removing unnecessary conditional access operator (?)
            }
        }
    }

    [GeneratedRegex(@"(\d+)(\w+)", RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex EventPartsRegex();

    private async Task<IReadOnlyDictionary<string, int>?> ComputeDistrictPointsForTeamsAsync(string eventKey, IEnumerable<string> allTeamKeys, CancellationToken cancellationToken)
    {
        var eventDetail = events[eventKey];
        if (eventDetail.EventType is (int)EventType.District or (int)EventType.DistrictCmpDivision or (int)EventType.DistrictCmp)
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

        return null;
    }

    public async Task<bool> HandleInteractionAsync(IServiceProvider services, SocketMessageComponent component, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        if (component.Data.CustomId.StartsWith(GetBreakdownButtonId, StringComparison.Ordinal))
        {
            await SendBreakdownToUserAsync(component, cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    private async Task SendBreakdownToUserAsync(SocketMessageComponent arg, CancellationToken cancellationToken = default)
    {
        var canceledRequestOptions = cancellationToken.ToRequestOptions();
        var isDmInteraction = arg.Channel.GetChannelType() is ChannelType.DM;
        if (!isDmInteraction)
        {
            try
            {
                await arg.DeferAsync(ephemeral: true).ConfigureAwait(false);
            }
            catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged or DiscordErrorCode.UnknownInteraction)
            {
                return;
            }
        }

        using var typing = arg.Channel.EnterTypingState(canceledRequestOptions);

        var matchKey = arg.Data.CustomId[(arg.Data.CustomId.IndexOf('_') + 1)..];
        var matchData = await matchApi.GetMatchAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        Debug.Assert(matchData is not null);
        if (matchData is not null)
        {
            var content = new StringBuilder();

            // We pass the matchData as the notificationMatch because all the code biases toward using the notificationMatch first and we don't want EVERY call to have to fail then go to the detailed match data. In effect, this makes the 2nd param *NEVER* used but it's there for consistency.
            await BuildBreakdownDetailAsync(content, matchData, matchData, includeFullBreakdown: true, cancellationToken: cancellationToken).ConfigureAwait(false);

            Embed embed = builderFactory.GetBuilder()
                .WithTitle($"Breakdown for {events[matchData.EventKey].GetLabel(shortName: true)} {matchData.CompLevel.ToShortString()} {matchData.SetNumber}.{matchData.MatchNumber}")
                .WithDescription(content.ToString())
                .Build();

            try
            {
                if (isDmInteraction)
                {
                    await arg.UpdateAsync(p =>
                    {
                        p.Embeds = new([.. arg.Message.Embeds, embed]);
                        p.Components = null;
                    }).ConfigureAwait(false);
                }
                else
                {
                    await arg.RespondAsync(ephemeral: true, embed: embed, options: canceledRequestOptions).ConfigureAwait(false);
                }
            }
            catch (TimeoutException)
            {
                logger.TookTooLongToReplyToGetBreakdownButton();
            }
            catch (Exception e1)
            {
                logger.ErrorOnInitialActionForScoreBreakdown(e1);

                try
                {
                    await arg.FollowupAsync(ephemeral: true, embed: embed, options: canceledRequestOptions).ConfigureAwait(false);
                }
                catch (Exception e2)
                {
                    logger.ErrorOnFollowUpActionForScoreBreakdown(e2);
                    try
                    {
                        await arg.User.SendMessageAsync($"Here's the breakdown for {events[matchData.EventKey].GetLabel(shortName: true)} {matchData.CompLevel.ToShortString()} {matchData.SetNumber}.{matchData.MatchNumber} you asked for", embed: embed, options: canceledRequestOptions).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.CouldNotFigureOutHowToSendTheBreakdownToThisUserUserUserIdChannelNameChannelIdTypeChannelType(ex, arg.User.Username, arg.User.Id, arg.Channel.Name, arg.Channel.Id, arg.Channel.GetChannelType()?.ToInvariantString() ?? "[null]");
                    }
                }
            }
        }
        else
        {
            logger.DidnTFindAnyMatchDataForMatchKey(matchKey);
        }
    }
}

