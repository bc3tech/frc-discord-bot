namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Extensions;

using DiscordBotFunctionApp.ChatBot;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Extensions;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;

using TheBlueAlliance.Api;
using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchScoreBreakdown2025AllianceExtensions;

internal sealed class MatchScore(IEventApi eventApi, IMatchApi matchApi, IDistrictApi districtApi, EventRepository events, TeamRepository teams, EmbedBuilderFactory builderFactory, ChatRunner gpt, ILogger<MatchScore> logger) : INotificationEmbedCreator, IEmbedCreator<string>
{
    public static NotificationType TargetType { get; } = NotificationType.match_score;

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

        var blueScore = detailedMatch.Alliances.Blue.Score;
        if (blueScore is -1)
        {
            if (detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Blue.TotalPoints.HasValue is true)
            {
                blueScore = detailedMatch.ScoreBreakdown.GetMatchScoreBreakdown2025()!.Blue.TotalPoints!.Value;
            }
        }

        var redScore = detailedMatch.Alliances.Red.Score;
        if (redScore is -1)
        {
            if (detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Red.TotalPoints.HasValue is true)
            {
                redScore = detailedMatch.ScoreBreakdown.GetMatchScoreBreakdown2025()!.Red.TotalPoints!.Value;
            }
        }

        if (redScore is -1 || blueScore is -1)
        {
            logger.BadDataForMatchMatchKeyMatchData(detailedMatch.Key, JsonSerializer.Serialize(detailedMatch));
            yield return null;
            yield break;
        }

        logger.CreatingMatchScoreEmbedForMatch(JsonSerializer.Serialize(detailedMatch));

        StringBuilder descriptionBuilder = new();

        #region Header
        var compLevelHeader = $"{Translator.CompLevelToShortString(notification.match!.CompLevel!.ToInvariantString()!)} {notification.match.SetNumber}";
        var matchHeader = $"Match {notification.match.MatchNumber}";
        descriptionBuilder.AppendLine(
            $"""
            # Scores are in!
            
            Actual start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.ActualTime.GetValueOrDefault(0)).ToPacificTime():t}{(detailedMatch.PostResultTime.HasValue ? $"\nResults posted at {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PostResultTime.Value).ToPacificTime():t}" : string.Empty)}
            """);
        #endregion

        await BuildDescriptionAsync(highlightTeam, detailedMatch, descriptionBuilder, (redScore, blueScore), cancellationToken);

        var embedding = baseBuilder
            .WithTitle($"{notification.event_name}: {Translator.CompLevelToShortString(notification.match!.CompLevel!.ToInvariantString()!)} {notification.match.SetNumber} - Match {notification.match.MatchNumber}")
            .WithDescription(descriptionBuilder.ToString());

        yield return new(embedding.Build());
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

        var blueScore = detailedMatch.Alliances.Blue.Score;
        if (blueScore is -1)
        {
            if (detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Blue.TotalPoints is not null and not -1)
            {
                blueScore = detailedMatch.ScoreBreakdown.GetMatchScoreBreakdown2025()!.Blue.TotalPoints!.Value;
            }
        }

        var redScore = detailedMatch.Alliances.Red.Score;
        if (redScore is -1)
        {
            if (detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025()?.Red.TotalPoints is not null and not -1)
            {
                redScore = detailedMatch.ScoreBreakdown.GetMatchScoreBreakdown2025()!.Red.TotalPoints!.Value;
            }
        }

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

        await BuildDescriptionAsync(highlightTeam, detailedMatch, descriptionBuilder, (redScore, blueScore), cancellationToken);

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

    private async Task BuildDescriptionAsync(ushort? highlightTeam, Match detailedMatch, StringBuilder descriptionBuilder, (int redScore, int blueScore) scores, CancellationToken cancellationToken)
    {
        var winningAlliance = detailedMatch.WinningAlliance;
        if (winningAlliance is Match.WinningAllianceEnum.Empty)
        {
            winningAlliance = scores.redScore > scores.blueScore ? Match.WinningAllianceEnum.Red : scores.redScore == scores.blueScore ? Match.WinningAllianceEnum.Empty : Match.WinningAllianceEnum.Blue;
        }

        var scoreBreakdown = detailedMatch.ScoreBreakdown?.GetMatchScoreBreakdown2025();
        var ranks = (await eventApi.GetEventRankingsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))?.Rankings
            .Where(i => detailedMatch.Alliances.Red.TeamKeys.Concat(detailedMatch.Alliances.Blue.TeamKeys).Contains(i.TeamKey))
            .ToDictionary(i => i.TeamKey, i => i.Rank);
        var districtPoints = await ComputeDistrictPointsForTeamsAsync(detailedMatch, cancellationToken);

        #region Red Score Breakdown
        descriptionBuilder.AppendLine($"### {(winningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)}Red Alliance - {detailedMatch.Alliances.Red.Score}{(detailedMatch.CompLevel is Match.CompLevelEnum.Qm ? $" (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red).ToString() ?? "?"})" : string.Empty)}");
        descriptionBuilder.AppendLine($"{string.Join("\n", detailedMatch.Alliances.Red.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp) " : string.Empty)}"))}");
        if (scoreBreakdown?.Red is null)
        {
            descriptionBuilder.AppendLine("No score breakdown given");
        }
        else
        {
            logger.ScoreBreakdownScoreBreakdown(JsonSerializer.Serialize(scoreBreakdown));
            descriptionBuilder.AppendLine(
                                       $"""

                                       **Score Breakdown**
                                       - Auto: {scoreBreakdown.Red.AutoPoints}
                                       """);
            descriptionBuilder.AppendLine($"- Coral (top/mid/bottom/trough): {scoreBreakdown.Red.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Red.AutoCoralPoints}pts");
            descriptionBuilder.AppendLine($"- Teleop: {scoreBreakdown.Red.TeleopPoints}");
            descriptionBuilder.AppendLine($"- Coral (top/mid/bottom/trough): {scoreBreakdown.Red.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Red.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Red.TeleopCoralPoints}pts");
            descriptionBuilder.AppendLine($"- Endgame: ({scoreBreakdown.Red.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Red.EndGameBargePoints}pts");
            descriptionBuilder.AppendLine($"- Algae(net / wall): {scoreBreakdown.Red.NetAlgaeCount}/{scoreBreakdown.Red.WallAlgaeCount} - {scoreBreakdown.Red.AlgaePoints}pts");
            if (detailedMatch.CompLevel is Match.CompLevelEnum.Qm)
            {
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Red.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Red.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Red.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot3.ToGlyph()})");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Red.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Red.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                descriptionBuilder.AppendLine($"- {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Red)} Win RP(3)");
            }
        }
        #endregion

        cancellationToken.ThrowIfCancellationRequested();

        #region Blue Score Breakdown
        descriptionBuilder.AppendLine($"### {(winningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)}Blue Alliance - {detailedMatch.Alliances.Blue.Score}{(detailedMatch.CompLevel is Match.CompLevelEnum.Qm ? $" (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue).ToString() ?? "?"})" : string.Empty)}");
        descriptionBuilder.AppendLine($"{string.Join("\n", detailedMatch.Alliances.Blue.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]}, +{districtPoints[t]}dp)" : string.Empty)}"))}");
        if (scoreBreakdown?.Red is null)
        {
            descriptionBuilder.AppendLine("No score breakdown given");
        }
        else
        {
            descriptionBuilder.AppendLine(
                                       $"""
                                       **Score Breakdown**
                                       - Auto: {scoreBreakdown.Blue.AutoPoints}
                                       """);
            descriptionBuilder.AppendLine($"- Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.AutoReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.AutoReef?.Trough ?? '?'} - {scoreBreakdown.Blue.AutoCoralPoints}pts");
            descriptionBuilder.AppendLine($"- Teleop: {scoreBreakdown.Blue.TeleopPoints}");
            descriptionBuilder.AppendLine($"- Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.TeleopReef?.TbaTopRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaMidRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.TbaBotRowCount ?? '?'}/{scoreBreakdown.Blue.TeleopReef?.Trough ?? '?'} - {scoreBreakdown.Blue.TeleopCoralPoints}pts");
            descriptionBuilder.AppendLine($"- Endgame: ({scoreBreakdown.Blue.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Blue.EndGameBargePoints}pts");
            descriptionBuilder.AppendLine($"- Algae(net / wall): {scoreBreakdown.Blue.NetAlgaeCount}/{scoreBreakdown.Blue.WallAlgaeCount} - {scoreBreakdown.Blue.AlgaePoints}pts");
            if (detailedMatch.CompLevel is Match.CompLevelEnum.Qm)
            {
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Blue.CoopertitionCriteriaMet.ToGlyph()} Coopertition");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Blue.AutoBonusAchieved.ToGlyph()} Auto RP(1) ({scoreBreakdown.Blue.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot3.ToGlyph()})");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Blue.BargeBonusAchieved.ToGlyph()} Barge RP(1)");
                descriptionBuilder.AppendLine($"- {scoreBreakdown.Blue.CoralBonusAchieved.ToGlyph()} Coral RP(1)");
                descriptionBuilder.AppendLine($"- {winningAlliance.ToGlyph(Match.WinningAllianceEnum.Blue)} Win RP(3)");
            }
        }
        #endregion

        var videos = detailedMatch.Videos?.Where(v => v.Type is "youtube" && v.Key is not null).Select(v => $"- https://www.youtube.com/watch?v={v.Key}");
        if (videos?.Any() is true)
        {
            descriptionBuilder
                .AppendLine("\n**Videos**")
                .AppendLine(string.Join("\n", videos));
        }

        descriptionBuilder.AppendLine($"\nView more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})");
    }

    private async Task<IReadOnlyDictionary<string, int>> ComputeDistrictPointsForTeamsAsync(Match detailedMatch, CancellationToken cancellationToken)
    {
        var districtPoints = await districtApi.GetEventDistrictPointsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        Dictionary<string, int> retVal = [];
        if (districtPoints is null)
        {
            return retVal;
        }

        foreach (var teamKey in detailedMatch.Alliances.Red.TeamKeys.Concat(detailedMatch.Alliances.Blue.TeamKeys))
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
