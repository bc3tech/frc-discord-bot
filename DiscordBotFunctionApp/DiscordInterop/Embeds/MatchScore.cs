namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Extensions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Extensions;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchScoreBreakdown2025AllianceExtensions;

internal sealed class MatchScore(IMatchApi matchApi, IEventApi eventApi, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<MatchScore> logger) : INotificationEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.match_score;

    public async IAsyncEnumerable<SubscriptionEmbedding> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Creating match score embed for {msg}", msg);
        var baseBuilder = builderFactory.GetBuilder();
        var notification = JsonSerializer.Deserialize<TbaInterop.Models.Notifications.MatchScore>(msg.MessageData);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var detailedMatch = await matchApi.GetMatchAsync(notification.match_key ?? Throws.IfNullOrWhiteSpace(notification.match?.Key), cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.LogWarning("Failed to retrieve detailed match data for {MatchKey}", notification.match_key ?? notification.match?.Key);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var compLevelHeader = $"{Translator.CompLevelToShortString(notification.match!.CompLevel!.ToInvariantString()!)} {notification.match.SetNumber}";
        var matchHeader = $"Match {notification.match.MatchNumber}";
        var ranks = (await eventApi.GetEventRankingsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);
        var scoreBreakdown = detailedMatch.ScoreBreakdown.GetMatchScoreBreakdown2025()!;

        var videos = detailedMatch.Videos.Where(v => v.Type is "youtube" && v.Key is not null).Select(v => $"- https://www.youtube.com/watch?v={v.Key}");
        var videoSection = videos.Any() ? $"\n**Videos**\n{string.Join("\n", videos)}\n" : string.Empty;

        var embedding = baseBuilder
            .WithDescription(
$@"# Scores are in!
## {notification.event_name}: {compLevelHeader} - {matchHeader}
Predicted start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PredictedTime!.Value).ToPacificTime():t}
Actual start time: {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.ActualTime!.Value).ToPacificTime():t}
Results posted at {DateTimeOffset.FromUnixTimeSeconds(detailedMatch.PostResultTime!.Value).ToPacificTime():t}

### {(detailedMatch.WinningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)} Red Alliance - {detailedMatch.Alliances!.Red!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red)})
{string.Join("\n", detailedMatch.Alliances.Red.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}

**Score Breakdown**
- Auto: {scoreBreakdown.Red.AutoPoints}
  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.AutoReef?.TbaTopRowCount}/{scoreBreakdown.Red.AutoReef?.TbaMidRowCount}/{scoreBreakdown.Red.AutoReef?.TbaBotRowCount}/{scoreBreakdown.Red.AutoReef?.Trough} - {scoreBreakdown.Red.AutoCoralPoints}pts
- Teleop: {scoreBreakdown.Red.TeleopPoints}
  - Coral (top/mid/bottom/trough): {scoreBreakdown.Red.TeleopReef?.TbaTopRowCount}/{scoreBreakdown.Red.TeleopReef?.TbaMidRowCount}/{scoreBreakdown.Red.TeleopReef?.TbaBotRowCount}/{scoreBreakdown.Red.TeleopReef?.Trough} - {scoreBreakdown.Red.TeleopCoralPoints}pts
- Endgame: ({scoreBreakdown.Red.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Red.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Red.EndGameBargePoints}pts
- Algae (net/wall): {scoreBreakdown.Red.NetAlgaeCount}/{scoreBreakdown.Red.WallAlgaeCount} - {scoreBreakdown.Red.AlgaePoints}pts
- {scoreBreakdown.Red.CoopertitionCriteriaMet.ToGlyph()} Coopertition
- {scoreBreakdown.Red.AutoBonusAchieved.ToGlyph()} Auto Bonus ({scoreBreakdown.Red.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Red.AutoLineRobot3.ToGlyph()})
- {scoreBreakdown.Red.BargeBonusAchieved.ToGlyph()} Barge Bonus
- {scoreBreakdown.Red.CoralBonusAchieved.ToGlyph()} Coral Bonus

### {(detailedMatch.WinningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)} Blue Alliance - {detailedMatch.Alliances.Blue!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue)})
{string.Join("\n", detailedMatch.Alliances.Blue.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}

**Score Breakdown**
- Auto: {scoreBreakdown.Blue.AutoPoints}
  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.AutoReef?.TbaTopRowCount}/{scoreBreakdown.Blue.AutoReef?.TbaMidRowCount}/{scoreBreakdown.Blue.AutoReef?.TbaBotRowCount}/{scoreBreakdown.Blue.AutoReef?.Trough} - {scoreBreakdown.Blue.AutoCoralPoints}pts
- Teleop: {scoreBreakdown.Blue.TeleopPoints}
  - Coral (top/mid/bottom/trough): {scoreBreakdown.Blue.TeleopReef?.TbaTopRowCount}/{scoreBreakdown.Blue.TeleopReef?.TbaMidRowCount}/{scoreBreakdown.Blue.TeleopReef?.TbaBotRowCount}/{scoreBreakdown.Blue.TeleopReef?.Trough} - {scoreBreakdown.Blue.TeleopCoralPoints}pts
- Endgame: ({scoreBreakdown.Blue.EndGameRobot1.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot2.ToGlyph()}/{scoreBreakdown.Blue.EndGameRobot3.ToGlyph()}) {scoreBreakdown.Blue.EndGameBargePoints}pts
- Algae (net/wall): {scoreBreakdown.Blue.NetAlgaeCount}/{scoreBreakdown.Blue.WallAlgaeCount} - {scoreBreakdown.Blue.AlgaePoints}pts
- {scoreBreakdown.Blue.CoopertitionCriteriaMet.ToGlyph()} Coopertition
- {scoreBreakdown.Blue.AutoBonusAchieved.ToGlyph()} Auto Bonus ({scoreBreakdown.Blue.AutoLineRobot1.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot2.ToGlyph()}/{scoreBreakdown.Blue.AutoLineRobot3.ToGlyph()})
- {scoreBreakdown.Blue.BargeBonusAchieved.ToGlyph()} Barge Bonus
- {scoreBreakdown.Blue.CoralBonusAchieved.ToGlyph()} Coral Bonus
{videoSection}
View more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})")
                    .Build();

        yield return new(embedding);
    }
}
