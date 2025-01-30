namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;

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

internal sealed class MatchScore(IMatchApi matchApi, IEventApi eventApi, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<MatchScore> logger) : INotificationEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.match_score;

    public async IAsyncEnumerable<SubscriptionEmbedding> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
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

        var embedding = baseBuilder
            .WithDescription(
$@"# Scores are in!
## {compLevelHeader} - {matchHeader}
### {(detailedMatch.WinningAlliance is Match.WinningAllianceEnum.Red ? "🏅" : string.Empty)} Red Alliance - {detailedMatch.Alliances!.Red!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Red)})
{string.Join("\n", detailedMatch.Alliances.Red.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}
### {(detailedMatch.WinningAlliance is Match.WinningAllianceEnum.Blue ? "🏅" : string.Empty)} Blue Alliance - {detailedMatch.Alliances.Blue!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match.WinningAllianceEnum.Blue)})
{string.Join("\n", detailedMatch.Alliances.Blue.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)} (#{ranks[t]})"))}

        View more match details[here](https://www.thebluealliance.com/match/{detailedMatch.Key})
        ")
                    .Build();

        yield return new(embedding);
    }
}
