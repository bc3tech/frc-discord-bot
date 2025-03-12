namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.TbaInterop;
using DiscordBotFunctionApp.TbaInterop.Models;
using DiscordBotFunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using TheBlueAlliance.Model;
using TheBlueAlliance.Model.MatchSimpleExtensions;

internal sealed partial class UpcomingMatch(TheBlueAlliance.Api.IEventApi eventInsights,
                                            TheBlueAlliance.Api.IMatchApi tbaApi,
                                            Statbotics.Api.IMatchApi matchStats,
                                            EventRepository events,
                                            TeamRepository teams,
                                            EmbedBuilderFactory builderFactory,
                                            ILogger<UpcomingMatch> logger) : INotificationEmbedCreator, IEmbedCreator<string>
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
            .WithTitle($"{events[detailedMatch.EventKey].GetLabel()}: {Translator.CompLevelToShortString(detailedMatch.CompLevel.ToInvariantString()!)} {detailedMatch.SetNumber} - Match {detailedMatch.MatchNumber}")
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
            # Next Match for {teams[highlightTeam!.Value.ToTeamKey()].GetLabel(includeLocation: false)} at {events[simpleMatch.EventKey].GetLabel(shortName: true)}
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

    private async Task<StringBuilder> BuildDescriptionAsync(StringBuilder descriptionBuilder, ushort? highlightTeam, MatchSimple matchDetails, CancellationToken cancellationToken, Action<StringBuilder>? beforeFooter = null)
    {
        var ranks = (await eventInsights.GetEventRankingsAsync(matchDetails.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);
        Debug.Assert(ranks is not null);
        logger.RankingsRankings(ranks is not null ? JsonSerializer.Serialize(ranks) : "[null]");

        var stats = await matchStats.ReadMatchV3MatchMatchGetAsync(matchDetails.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
        Debug.Assert(stats is not null);
        logger.MatchStatsMatchStats(stats is not null ? JsonSerializer.Serialize(stats) : "[null]");

        MatchSimple.WinningAllianceEnum? predictedWinner = stats?.Pred?.Winner is not null ? MatchSimple.WinningAllianceEnumFromStringOrDefault(stats.Pred.Winner).GetValueOrDefault(MatchSimple.WinningAllianceEnum.Empty) : null;
        var matchSimpleAlliances = matchDetails.Alliances.Blue.TeamKeys
            .Concat(matchDetails.Alliances.Red.TeamKeys)
            .Concat(matchDetails.Alliances.Blue.DqTeamKeys)
            .Concat(matchDetails.Alliances.Red.DqTeamKeys)
            .Concat(matchDetails.Alliances.Blue.SurrogateTeamKeys)
            .Concat(matchDetails.Alliances.Red.SurrogateTeamKeys)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.ToTeamNumber()!.Value);

        var allAlliancesInMatch = matchSimpleAlliances.Concat(stats?.Alliances?.Blue?.TeamKeys ?? [])
            .Concat(stats?.Alliances?.Blue?.DqTeamKeys ?? [])
            .Concat(stats?.Alliances?.Blue?.SurrogateTeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.TeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.DqTeamKeys ?? [])
            .Concat(stats?.Alliances?.Red?.SurrogateTeamKeys ?? [])
            .ToHashSet();
        var containsHighlightedTeam = highlightTeam.HasValue && allAlliancesInMatch.Contains(highlightTeam.Value);

        int[] allianceRanks = await GetAllianceRanksAsync(matchDetails, cancellationToken).ConfigureAwait(false);

        descriptionBuilder.AppendLine(
                $"""
                ### Alliances

                **Red Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red]})" : string.Empty)}**
                {string.Join("\n", matchDetails.Alliances.Red.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam)}{(ranks is not null ? $" (#{ranks[t]})" : string.Empty)}"))}

                """);
        descriptionBuilder.AppendLine(
            $"""
            **Blue Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue]})" : string.Empty)}**
            {string.Join("\n", matchDetails.Alliances.Blue.TeamKeys.OrderBy(k => k.ToTeamNumber()).Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam)}{(ranks is not null ? $" (#{ranks[t]})" : string.Empty)}"))}
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
            .Append($"View more match details [here](https://www.thebluealliance.com/match/{matchDetails.Key})");

#pragma warning disable EA0001 // Perform message formatting in the body of the logging method
        logger.EmbeddingNameBuiltEmbeddingDetail(nameof(UpcomingMatch), descriptionBuilder.ToString());
#pragma warning restore EA0001 // Perform message formatting in the body of the logging method

        return descriptionBuilder;
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex AllianceRankRegex();

    private async Task<int[]> GetAllianceRanksAsync(MatchSimple detailedMatch, CancellationToken cancellationToken)
    {
        var retVal = new int[] { 0, 0, 0 };
        if (detailedMatch.CompLevel is not MatchSimple.CompLevelEnum.Qm)
        {
            var eventAlliances = await eventInsights.GetEventAlliancesAsync(detailedMatch.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var eliminationAlliance in eventAlliances ?? [])
            {
                if (!eliminationAlliance.Picks.Except(detailedMatch.Alliances.Red.TeamKeys).Any())
                {
                    retVal[(int)MatchSimple.WinningAllianceEnum.Red] = int.Parse(AllianceRankRegex().Match(eliminationAlliance.Name ?? "0").Value);
                    continue;
                }
                else if (!eliminationAlliance.Picks.Except(detailedMatch.Alliances.Blue.TeamKeys).Any())
                {
                    retVal[(int)MatchSimple.WinningAllianceEnum.Blue] = int.Parse(AllianceRankRegex().Match(eliminationAlliance.Name ?? "0").Value);
                    continue;
                }
            }
        }

        return retVal;
    }
}
