namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

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

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchSimpleExtensions;

internal sealed class UpcomingMatch(TheBlueAlliance.Api.IEventApi eventInsights, TheBlueAlliance.Api.IMatchApi tbaApi, Statbotics.Api.IMatchApi matchStats, EventRepository events, TeamRepository teams, EmbedBuilderFactory builderFactory, ILogger<UpcomingMatch> logger) : INotificationEmbedCreator, IEmbedCreator<string>
{
    public const NotificationType TargetType = NotificationType.upcoming_match;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.UpcomingMatch>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return new(baseBuilder.Build());
            yield break;
        }

        if (string.IsNullOrWhiteSpace(notification.match_key))
        {
            logger.MatchKeyIsMissingFromNotificationData();
            yield return new(baseBuilder.Build());
            yield break;
        }

        var detailedMatch = await tbaApi.GetMatchSimpleAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(notification.match_key);
            yield return new(baseBuilder.Build());
            yield break;
        }

        logger.CreatingUpcomingMatchEmbeddingForWebhookMessageNotification(JsonSerializer.Serialize(msg), JsonSerializer.Serialize(notification));

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.AppendLine(
            $"""
            # Match starting soon!

            Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.scheduled_time!).ToPacificTime():t}
            **Predicted start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.predicted_time!).ToPacificTime():t}**
            """);

        await BuildDescriptionAsync(descriptionBuilder, highlightTeam, detailedMatch, cancellationToken, beforeFooter: addWebcastDetail).ConfigureAwait(false);

        void addWebcastDetail(StringBuilder sb)
        {
            if (notification.webcast is not null)
            {
                var (source, url) = notification.webcast.GetFullUrl(logger);
                var link = !string.IsNullOrWhiteSpace(notification.webcast.StreamTitle)
                    ? $"[{notification.webcast.StreamTitle}]({url})"
                    : $"[{source}]({url})";

                descriptionBuilder.AppendLine(
                    $"""

                    ### Watch live

                    - {link} ({notification.webcast.ViewerCount} current viewer{(notification.webcast.ViewerCount is not 1 ? "s" : string.Empty)})
                    """);
            }
        }

        var embedding = baseBuilder
            .WithTitle($"{events.GetLabelForEvent(detailedMatch.EventKey)}: {Translator.CompLevelToShortString(detailedMatch.CompLevel.ToInvariantString()!)} {detailedMatch.SetNumber} - Match {detailedMatch.MatchNumber}")
            .WithDescription(descriptionBuilder.ToString());

        yield return new(embedding.Build());
    }

    public IAsyncEnumerable<ResponseEmbedding?> CreateNextMatchEmbeddingsAsync(string matchKey, ushort? highlightTeam = null, CancellationToken cancellationToken = default) => CreateAsync(matchKey, highlightTeam, cancellationToken);

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync(string matchKey, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);

        if (string.IsNullOrWhiteSpace(matchKey))
        {
            logger.MatchKeyIsMissingFromNotificationData();
            yield return new(baseBuilder.Build());
            yield break;
        }

        var simpleMatch = await tbaApi.GetMatchSimpleAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (simpleMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(matchKey);
            yield return new(baseBuilder.Build());
            yield break;
        }

        var compLevelHeader = $"{Translator.CompLevelToShortString(simpleMatch.CompLevel.ToInvariantString()!)} {simpleMatch.SetNumber}";

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.AppendLine(
            $"""
            # Next Match for {teams.GetLabelForTeam(highlightTeam, includeLocation: false)} at {events.GetLabelForEvent(simpleMatch.EventKey, shortName: true)}
            ## {compLevelHeader} - Match {simpleMatch.MatchNumber}

            Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds(simpleMatch.Time.GetValueOrDefault(0)!).ToPacificTime():t}
            **Predicted start time: {DateTimeOffset.FromUnixTimeSeconds(simpleMatch.PredictedTime.GetValueOrDefault(0)).ToPacificTime():t}**
            """);

        var matchVideoData = await tbaApi.GetMatchAsync(simpleMatch.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
        await BuildDescriptionAsync(descriptionBuilder, highlightTeam, simpleMatch, cancellationToken, beforeFooter: addMatchVideos);

        void addMatchVideos(StringBuilder builder)
        {
            var videoUrls = matchVideoData?.GetVideoUrls();
            if (videoUrls?.Any() is true)
            {
                builder.AppendLine(
                    $"""
                    ### Match videos

                    {string.Join("\n", videoUrls.Select(i => $"- [{i.Name}]({i.Link})"))}
                    """);
            }
        }

        var embedding = baseBuilder.WithDescription(descriptionBuilder.ToString()).Build();

        yield return new(embedding);
    }

    private async Task<StringBuilder> BuildDescriptionAsync(StringBuilder descriptionBuilder, ushort? highlightTeam, MatchSimple detailedMatch, CancellationToken cancellationToken, Action<StringBuilder>? beforeFooter = null)
    {
        var ranks = (await eventInsights.GetEventRankingsAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);
        Debug.Assert(ranks is not null);
        logger.RankingsRankings(ranks is not null ? JsonSerializer.Serialize(ranks) : "[null]");

        var stats = await matchStats.ReadMatchV3MatchMatchGetAsync(detailedMatch.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
        Debug.Assert(stats is not null);
        logger.MatchStatsMatchStats(stats is not null ? JsonSerializer.Serialize(stats) : "[null]");

        MatchSimple.WinningAllianceEnum? predictedWinner = stats?.Pred?.Winner is not null ? MatchSimple.WinningAllianceEnumFromStringOrDefault(stats.Pred.Winner).GetValueOrDefault(MatchSimple.WinningAllianceEnum.Empty) : null;
        var allAlliancesInMatch = (stats?.Alliances?.Blue?.TeamKeys ?? [])
            .Concat(stats?.Alliances?.Blue?.DqTeamKeys ?? [])
            .Concat(stats?.Alliances?.Blue?.SurrogateTeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.TeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.DqTeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.SurrogateTeamKeys ?? []);
        var containsHighlightedTeam = highlightTeam.HasValue && allAlliancesInMatch.Contains(highlightTeam.Value);

        descriptionBuilder.AppendLine(
                $"""
                ### Alliances

                **Red Alliance**
                {string.Join("\n", detailedMatch.Alliances.Red.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]})" : string.Empty)}"))}

                """);
        descriptionBuilder.AppendLine(
            $"""
            **Blue Alliance**
            {string.Join("\n", detailedMatch.Alliances.Blue.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams.GetTeamLabelWithHighlight(t, highlightTeam)}{(ranks is not null ? $" (#{ranks[t]})" : string.Empty)}"))}
            """);

        if (predictedWinner is not null and not MatchSimple.WinningAllianceEnum.Empty && predictedWinner.HasValue)
        {
            var certainty = predictedWinner is MatchSimple.WinningAllianceEnum.Red ? stats!.Pred!.RedWinProb : (1 - stats!.Pred!.RedWinProb);
            bool redWins = predictedWinner is MatchSimple.WinningAllianceEnum.Red;
            descriptionBuilder.Append(
                $"""
                ## Prediction

                - Winner: {predictedWinner.Value.ToInvariantString()} Alliance{(certainty.HasValue ? $" ({certainty:P2})" : string.Empty)}
                """);
            if (containsHighlightedTeam)
            {
                var highlightShouldWin = (predictedWinner is MatchSimple.WinningAllianceEnum.Red && stats.Alliances!.Red?.TeamKeys?.Contains(highlightTeam!.Value) is true)
                        || (predictedWinner is MatchSimple.WinningAllianceEnum.Blue && stats.Alliances!.Blue?.TeamKeys?.Contains(highlightTeam!.Value) is true);
                var nailBiter = certainty.HasValue && certainty >= .45f && certainty <= .55f;
                descriptionBuilder
                    .Append(highlightShouldWin ? $" 🤞{(nailBiter ? "😬" : "🤞")}" : $" 💪{(nailBiter ? "😬" : "💪")}");
            }

            descriptionBuilder
                .AppendLine()
                .AppendLine(
                $"""
                - Score:
                  - {(redWins ? "**" : string.Empty)}Red: {stats.Pred!.RedScore}{(redWins ? "**" : string.Empty)}
                  - {(!redWins ? "**" : string.Empty)}Blue: {stats.Pred.BlueScore}{(!redWins ? "**" : string.Empty)}
                """);
        }

        beforeFooter?.Invoke(descriptionBuilder);

        descriptionBuilder.AppendLine()
            .Append($"View more match details [here](https://www.thebluealliance.com/match/{detailedMatch.Key})");

        return descriptionBuilder;
    }
}
