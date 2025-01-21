namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.Json;

using TheBlueAlliance.Api;
using TheBlueAlliance.Api.Api;
using TheBlueAlliance.Api.Model;
using TheBlueAlliance.Api.Notifications;

internal sealed class UpcomingMatch(IMatchApi tbaApi, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<UpcomingMatch> logger) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.upcoming_match;

    public async IAsyncEnumerable<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = JsonSerializer.Deserialize<TheBlueAlliance.Api.Notifications.UpcomingMatch>(msg.MessageData);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return baseBuilder.Build();
            yield break;
        }

        if (string.IsNullOrWhiteSpace(notification.match_key))
        {
            logger.LogWarning("Match key is missing from notification data");
            yield return baseBuilder.Build();
            yield break;
        }

        var detailedMatch = await tbaApi.GetMatchSimpleAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.LogWarning("Failed to retrieve detailed match data for {MatchKey}", notification.match_key);
            yield return baseBuilder.Build();
            yield break;
        }

        var compLevelHeader = $"{Translator.CompLevelToShortString(detailedMatch.CompLevel.ToInvariantString()!)} {detailedMatch.SetNumber}";
        var matchHeader = $"Match {detailedMatch.MatchNumber}";

        var embedding = baseBuilder
            .WithDescription(
$@"# Match starting soon!
## {compLevelHeader} - {matchHeader}
Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.scheduled_time!).ToLocalTime():t}
**Predicted start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.predicted_time!).ToLocalTime():t}**
### Alliances
**Red Alliance**
{string.Join("\n", detailedMatch.Alliances!.Red!.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}"))}

**Blue Alliance**
{string.Join("\n", detailedMatch.Alliances.Blue!.TeamKeys!.Order().Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}"))}

View more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})
")
            .Build();

        yield return embedding;
    }
}
