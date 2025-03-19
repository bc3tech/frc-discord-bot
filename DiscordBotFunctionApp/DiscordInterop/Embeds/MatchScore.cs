namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Extensions;

using Discord;
using Discord.WebSocket;

using DiscordBotFunctionApp.ChatBot;
using DiscordBotFunctionApp.DiscordInterop.CommandModules;
using DiscordBotFunctionApp.Extensions;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using FIRST.Api;
using FIRST.Model;

using Microsoft.Extensions.DependencyInjection;
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
                                         TimeProvider time,
                                         ILogger<MatchScore> logger) : INotificationEmbedCreator, IEmbedCreator<(string matchKey, bool summarize)>, IHandleUserInteractions
{
    public const NotificationType TargetType = NotificationType.match_score;
    public const string GetBreakdownButtonId = "get-score-breakdown";

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = logger.CreateMethodScope();
        logger.CreatingMatchScoreEmbed(msg);
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.MatchScore>();
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

        (int blueScore, int redScore) = GetActualScores(notification.match, tbaMatch);

        if (redScore is -1 || blueScore is -1)
        {
            yield return null;
            yield break;
        }

        logger.CreatingMatchScoreEmbedForMatch(JsonSerializer.Serialize(tbaMatch));

        StringBuilder descriptionBuilder = new();

        #region Header
        var postResultTime = notification.match?.PostResultTime.Or(tbaMatch.PostResultTime);
        descriptionBuilder.AppendLine(
            $"""
            # Scores are in!
            
            Actual start time: {DateTimeOffset.FromUnixTimeSeconds(notification.match?.ActualTime.Or(tbaMatch.ActualTime) ?? 0).ToPacificTime():t}{(postResultTime.HasValue ? $"\nResults posted at {DateTimeOffset.FromUnixTimeSeconds(postResultTime.Value).ToPacificTime():t}" : string.Empty)}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, notification.match, tbaMatch, descriptionBuilder, (redScore, blueScore), false, cancellationToken);

        var embedding = baseBuilder
            .WithTitle($"{notification.event_name}: {Translator.CompLevelToShortString(notification.match?.CompLevel.Or(tbaMatch.CompLevel).ToInvariantString()!)} {notification.match?.SetNumber.Or(tbaMatch.SetNumber)} - Match {notification.match?.MatchNumber.Or(tbaMatch.MatchNumber)}")
            .WithDescription(descriptionBuilder.ToString());

        yield return new(embedding.Build(), Actions: [ButtonBuilder.CreatePrimaryButton("Get breakdown", $"{GetBreakdownButtonId}_{notification.match_key}").Build()]);
    }

    private (int, int) GetActualScores(Match? notificationMatch, Match tbaMatch)
    {
        using var scope = logger.CreateMethodScope();
        var match = notificationMatch ?? tbaMatch;
        var blueScore = match.Alliances.Blue.Score ?? -1;
        if (blueScore is -1)
        {
            blueScore = match.ScoreBreakdown?.GetMatchScoreBreakdown2025().Or(tbaMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025())?.Blue.TotalPoints ?? -1;
        }

        var redScore = match.Alliances.Red.Score ?? -1;
        if (redScore is -1)
        {
            redScore = match.ScoreBreakdown?.GetMatchScoreBreakdown2025().Or(tbaMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025())?.Red.TotalPoints ?? -1;
        }

        if (redScore is -1 || blueScore is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(match.Key, JsonSerializer.Serialize(match));
        }

        return (redScore, blueScore);
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

            ## {events[detailedMatch.EventKey].GetLabel()}: {compLevelHeader} - {matchHeader}
            Predicted start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PredictedTime.GetValueOrDefault(0)).ToPacificTime():t}
            Actual start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.ActualTime.GetValueOrDefault(0)).ToPacificTime():t}{(detailedMatch.PostResultTime.HasValue ? $"\nResults posted at {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PostResultTime.Value).ToPacificTime():t}" : string.Empty)}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, detailedMatch, detailedMatch, descriptionBuilder, (redScore, blueScore), true, cancellationToken);

        var embedding = baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build();

        yield return new(embedding);

        if (input.summarize)
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

        await BuildBreakdownDetailAsync(descriptionBuilder, notificationMatch, tbaMatch, highlightTeam, includeBreakdown, cancellationToken: cancellationToken);

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

    private async Task BuildBreakdownDetailAsync(StringBuilder builder, Match? notificationMatch, Match tbaMatch, ushort? highlightTeam = null, bool includeFullBreakdown = false, CancellationToken cancellationToken = default)
    {
        var match = notificationMatch ?? tbaMatch;
        var alliances = match.Alliances;
        var allTeamKeys = alliances.Red.TeamKeys.Concat(alliances.Blue.TeamKeys).ToHashSet();
        var winningAlliance = match.WinningAlliance.UnlessThen(i => i is Match.WinningAllianceEnum.Empty, tbaMatch.WinningAlliance);

        int[] allianceRanks = await GetAllianceRanksAsync(notificationMatch, tbaMatch, alliances, cancellationToken).ConfigureAwait(false);

        var ranks = (await eventApi.GetEventRankingsAsync(match.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.Rankings
            .Where(i => allTeamKeys.Contains(i.TeamKey))
            .ToDictionary(i => i.TeamKey, i => i.Rank);
        var districtPoints = await ComputeDistrictPointsForTeamsAsync(match.EventKey, allTeamKeys, cancellationToken);

        bool isQuals = match.CompLevel is Match.CompLevelEnum.Qm;
        var scoreBreakdown = includeFullBreakdown ? await GetScoreBreakdownAsync(notificationMatch, tbaMatch, cancellationToken).ConfigureAwait(false) : null;

        #region Red Score Breakdown
        builder.Append($"### {(winningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)}Red Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red]})" : string.Empty)} - {alliances.Red.Score}");
        if (isQuals)
        {
            var rankingPointsValue = match.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red).Or(tbaMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red)).ToString();
            Debug.Assert(!string.IsNullOrWhiteSpace(rankingPointsValue));
            builder.AppendLine($" (+{(string.IsNullOrWhiteSpace(rankingPointsValue) ? "?" : rankingPointsValue)})");
        }
        else
        {
            builder.AppendLine();
        }

        builder.AppendLine($"{string.Join("\n", alliances.Red.TeamKeys.Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam, includeLocation: false)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp) " : string.Empty)}"))}");

        if (includeFullBreakdown)
        {
            if (scoreBreakdown?.Red is null)
            {
                builder.AppendLine("No score breakdown given");
                logger.LogMetric("NoRedScoreBreakdown", 1);
            }
            else
            {
                logger.ScoreBreakdownScoreBreakdown(JsonSerializer.Serialize(scoreBreakdown));
                builder.AppendLine($"""
                                    - **Score Breakdown**
                                        - Auto: {scoreBreakdown.Red.AutoPoints}
                                    """);
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Red.AutoCoralPoints}pts");
                builder.AppendLine($"  - Teleop: {scoreBreakdown.Red.TeleopPoints}");
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Red.TeleopCoralPoints}pts");
                builder.AppendLine($"  - Endgame: ({scoreBreakdown.Red.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Red.EndGameBargePoints}pts");
                builder.AppendLine($"  - Algae(net / wall): {scoreBreakdown.Red.NetAlgaeCount}/{scoreBreakdown.Red.WallAlgaeCount} - {scoreBreakdown.Red.AlgaePoints}pts");
                if (isQuals)
                {
                    builder.AppendLine($"  - {scoreBreakdown.Red.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                    builder.AppendLine($"  - {scoreBreakdown.Red.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Red.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot3.ToGlyph()})");
                    builder.AppendLine($"  - {scoreBreakdown.Red.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                    builder.AppendLine($"  - {scoreBreakdown.Red.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                    builder.AppendLine($"  - {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Red)} Win RP(3)");
                }
            }
        }
        #endregion

        cancellationToken.ThrowIfCancellationRequested();

        #region Blue Score Breakdown
        builder.Append($"### {(winningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)}Blue Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue]})" : string.Empty)} - {alliances.Blue.Score}");
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

        builder.AppendLine($"{string.Join("\n", alliances.Blue.TeamKeys.Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam, includeLocation: false)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp)" : string.Empty)}"))}");

        if (includeFullBreakdown)
        {
            if (scoreBreakdown?.Blue is null)
            {
                builder.AppendLine("No score breakdown given");
                logger.LogMetric("NoBlueScoreBreakdown", 1);
            }
            else
            {
                builder.AppendLine($"""
                                    - **Score Breakdown**
                                        - Auto: {scoreBreakdown.Blue.AutoPoints}
                                    """);
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Blue.AutoCoralPoints}pts");
                builder.AppendLine($"  - Teleop: {scoreBreakdown.Blue.TeleopPoints}");
                builder.AppendLine($"  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Blue.TeleopCoralPoints}pts");
                builder.AppendLine($"  - Endgame: ({scoreBreakdown.Blue.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Blue.EndGameBargePoints}pts");
                builder.AppendLine($"  - Algae(net / wall): {scoreBreakdown.Blue.NetAlgaeCount}/{scoreBreakdown.Blue.WallAlgaeCount} - {scoreBreakdown.Blue.AlgaePoints}pts");
                if (isQuals)
                {
                    builder.AppendLine($"  - {scoreBreakdown.Blue.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                    builder.AppendLine($"  - {scoreBreakdown.Blue.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Blue.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot3.ToGlyph()})");
                    builder.AppendLine($"  - {scoreBreakdown.Blue.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                    builder.AppendLine($"  - {scoreBreakdown.Blue.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                    builder.AppendLine($"  - {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Blue)} Win RP(3)");
                }
            }
        }
        #endregion
    }

    private async Task<MatchScoreBreakdown2025?> GetScoreBreakdownAsync(Match? notificationMatch, Match tbaMatch, CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        var breakdown = notificationMatch?.ScoreBreakdown.Or(tbaMatch.ScoreBreakdown)?.GetMatchScoreBreakdown2025();

        while (breakdown is null && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), time, cancellationToken).ConfigureAwait(false);
            try
            {
                breakdown = (await matchApi.GetMatchAsync(tbaMatch.Key, cancellationToken: cancellationToken))?.ScoreBreakdown?.GetMatchScoreBreakdown2025();

                if (time.GetElapsedTime(startTime).TotalMinutes >= 5)
                {
                    logger.ScoreBreakdownForMatchMatchKeyNotAvailableAfter5Minutes(tbaMatch.Key);
                    return breakdown;
                }

                if (breakdown is not null)
                {
                    if (breakdown.Red.Rp is null or > 6 or < 0)
                    {
                        logger.InvalidRedRPValueForMatchMatchKeyRpValue(tbaMatch.Key, breakdown.Red.Rp);
                        breakdown = null;
                    }
                    else if (breakdown.Blue.Rp is null or > 6 or < 0)
                    {
                        logger.InvalidBlueRPValueForMatchMatchKeyRpValue(tbaMatch.Key, breakdown.Blue.Rp);
                        breakdown = null;
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
            logger.LogMetric("ScoreBreakdownAvailableTimeSec", time.GetElapsedTime(startTime).TotalSeconds, new Dictionary<string, object> { { "MatchKey", tbaMatch.Key } });
        }

        return breakdown;
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex AllianceRankRegex();

    private async Task<int[]> GetAllianceRanksAsync(Match? notificationMatch, Match tbaMatch, MatchSimpleAlliances alliances, CancellationToken cancellationToken)
    {
        var retVal = new int[] { 0, 0, 0 };
        var match = notificationMatch ?? tbaMatch;
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

    private async Task AddEventOrDayWrapupAsync(StringBuilder descriptionBuilder, Match? notificationMatch, Match tbaMatch, Match.WinningAllianceEnum winningAlliance, CancellationToken cancellationToken)
    {
        var match = notificationMatch ?? tbaMatch;
        var matchNumber = match.MatchNumber;
        var isPossibleEventEnd = match.CompLevel is Match.CompLevelEnum.F && matchNumber > 1;
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
            var tourneyLevel = (match.CompLevel is Match.CompLevelEnum.Qm) ? TournamentLevel.Qualification : TournamentLevel.Playoff;
            try
            {
                var eventSchedule = await schedule.SeasonScheduleEventCodeGetAsync(eventCode, season, tournamentLevel: tourneyLevel, cancellationToken: cancellationToken);
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
        await arg.DeferAsync(ephemeral: true, cancellationToken.ToRequestOptions());

        var matchKey = arg.Data.CustomId[(arg.Data.CustomId.IndexOf('_') + 1)..];
        var matchData = await matchApi.GetMatchAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        Debug.Assert(matchData is not null);
        if (matchData is not null)
        {
            var content = new StringBuilder();

            // We pass the matchData as the notificationMatch because all the code biases toward using the notificationMatch first and we don't want EVERY call to have to fail then go to the detailed match data. In effect, this makes the 2nd param *NEVER* used but it's there for consistency.
            await BuildBreakdownDetailAsync(content, matchData, matchData, includeFullBreakdown: true, cancellationToken: cancellationToken);
            await arg.FollowupAsync(embed: new EmbedBuilder().WithTitle("Here's the breakdown").WithDescription(content.ToString()).Build(), ephemeral: true, options: cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        }
        else
        {
            logger.DidnTFindAnyMatchDataForMatchKey(matchKey);
        }
    }
}

