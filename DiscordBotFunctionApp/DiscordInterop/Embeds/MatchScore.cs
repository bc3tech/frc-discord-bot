namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common;
using Common.Tba;
using Common.Tba.Api;
using Common.Tba.Api.Models;
using Common.Tba.Extensions;
using Common.Tba.Notifications;

using Discord;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

internal sealed class MatchScore(ApiClient tbaApi, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<MatchScore> logger) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.match_score;

    public async Task<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = await msg.GetDataAsAsync<Common.Tba.Notifications.MatchScore, Match>(cancellationToken).ConfigureAwait(false);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            return baseBuilder.Build();
        }

        var compLevelHeader = $"{Translator.CompLevelToShortString(notification.match!.CompLevel!.ToString()!)} {notification.match.SetNumber}";
        var matchheader = $"Match {notification.match.MatchNumber}";

        var detailedMatch = await tbaApi.Match[notification.match_key ?? Throws.IfNullOrWhiteSpace(notification.match?.Key)].GetAsync(cancellationToken: cancellationToken);
        if (detailedMatch is null)
        {
            logger.LogWarning("Failed to retrieve detailed match data for {MatchKey}", notification.match_key ?? notification.match?.Key);
            return baseBuilder.Build();
        }

        var embedding = baseBuilder
            .WithDescription(
$@"# Scores are in!
## {compLevelHeader} - {matchheader}

### {(detailedMatch.WinningAlliance is Match_winning_alliance.Red ? "🏅" : string.Empty)} Red Alliance - {detailedMatch.Alliances!.Red!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match_winning_alliance.Red)})
{string.Join("\n", detailedMatch.Alliances.Red.TeamKeys!.Select(t => $"- {highlightIfMatches(t, highlightTeam)}"))}

### {(detailedMatch.WinningAlliance is Match_winning_alliance.Blue ? "🏅" : string.Empty)} Blue Alliance - {detailedMatch.Alliances.Blue!.Score} (+{detailedMatch.GetAllianceRankingPoints(Match_winning_alliance.Blue)})
{string.Join("\n", detailedMatch.Alliances.Blue.TeamKeys!.Select(t => $"- {highlightIfMatches(t, highlightTeam)}"))}

View more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})
")
            .Build();

        string highlightIfMatches(string teamKey, ushort? highlightTeam)
        {
            var teamLabel = teams.GetLabelForTeam(teamKey);
            if (highlightTeam is not null && teamLabel.StartsWith(highlightTeam.ToString()!, StringComparison.Ordinal))
            {
                return $"**{teamLabel}**";
            }

            return teamLabel;
        }

        return embedding;
    }
}
