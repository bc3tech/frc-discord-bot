namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchSimpleExtensions;

internal sealed class UpcomingMatch(TheBlueAlliance.Api.IMatchApi tbaApi, TheBlueAlliance.Api.IEventApi eventInsights, Statbotics.Api.IMatchApi matchStats, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<UpcomingMatch> logger) : INotificationEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.upcoming_match;

    public async IAsyncEnumerable<SubscriptionEmbedding> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = JsonSerializer.Deserialize<TbaInterop.Models.Notifications.UpcomingMatch>(msg.MessageData);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        if (string.IsNullOrWhiteSpace(notification.match_key))
        {
            logger.LogWarning("Match key is missing from notification data");
            yield return new(baseBuilder.Build());
            yield break;
        }

        var detailedMatch = await tbaApi.GetMatchSimpleAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.LogWarning("Failed to retrieve detailed match data for {MatchKey}", notification.match_key);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var compLevelHeader = $"{Translator.CompLevelToShortString(detailedMatch.CompLevel.ToInvariantString()!)} {detailedMatch.SetNumber}";
        var matchHeader = $"Match {detailedMatch.MatchNumber}";
        var ranks = (await eventInsights.GetEventRankingsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);
        var stats = await matchStats.ReadMatchV3MatchMatchGetAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        var predictedWinner = MatchSimple.WinningAllianceEnumFromString(stats!.Pred.Winner!);

        var embedding = baseBuilder
            .WithDescription(
$@"# Match starting soon!
## {compLevelHeader} - {matchHeader}
Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.scheduled_time!).ToPacificTime():t}
**Predicted start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.predicted_time!).ToPacificTime():t}**
### Alliances
**Red Alliance**
{string.Join("\n", detailedMatch.Alliances!.Red!.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}

**Blue Alliance**
{string.Join("\n", detailedMatch.Alliances.Blue!.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}

## Prediction

- Winner: {predictedWinner.ToInvariantString()} Alliance ({(predictedWinner is MatchSimple.WinningAllianceEnum.Red ? stats.Pred.RedWinProb : (1 - stats.Pred.RedWinProb)):P2}) {(highlightTeam.HasValue && ((predictedWinner is MatchSimple.WinningAllianceEnum.Red && stats.Alliances.Red.TeamKeys.Contains(highlightTeam.Value)) || (predictedWinner is MatchSimple.WinningAllianceEnum.Blue && stats.Alliances.Blue.TeamKeys.Contains(highlightTeam.Value))) ? "🤞🤞" : "💪💪")}
- Score: [Red] {stats.Pred.RedScore} - [Blue] {stats.Pred.BlueScore}

View more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})")
            .Build();

        yield return new(embedding);
    }
}
