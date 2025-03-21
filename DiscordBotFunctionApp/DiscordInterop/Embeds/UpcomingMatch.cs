namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

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

using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model.MatchExtensions;
using TheBlueAlliance.Model.MatchSimpleExtensions;
using TheBlueAlliance.Model;

internal sealed partial class UpcomingMatch(TheBlueAlliance.Api.IEventApi eventInsights,
                                            TheBlueAlliance.Api.IMatchApi matchApi,
                                            Statbotics.Api.IMatchApi matchStats,
                                            EventRepository events,
                                            TeamRepository teams,
                                            EmbedBuilderFactory builderFactory,
                                            TimeProvider time,
                                            ILogger<UpcomingMatch> logger) : INotificationEmbedCreator, IEmbedCreator<(string eventKey, string teamKey)>
{
    public const NotificationType TargetType = NotificationType.upcoming_match;

    public async IAsyncEnumerable<SubscriptionEmbedding?> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);
        var notification = msg.GetDataAs<TbaInterop.Models.Notifications.UpcomingMatch>();
        if (notification is null)
        {
            logger.FailedToDeserializeNotificationDataAsNotificationType(TargetType);
            yield return null;
            yield break;
        }

        if (string.IsNullOrWhiteSpace(notification.match_key))
        {
            logger.MatchKeyIsMissingFromNotificationData();
            yield return null;
            yield break;
        }

        var detailedMatch = await matchApi.GetMatchSimpleAsync(notification.match_key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (detailedMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(notification.match_key);
            yield return null;
            yield break;
        }

        logger.CreatingUpcomingMatchEmbeddingForWebhookMessageNotification(JsonSerializer.Serialize(msg), JsonSerializer.Serialize(notification));

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.AppendLine(
            $"""
            # Match starting soon!

            {(notification.scheduled_time.HasValue ? $"Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.scheduled_time!).ToLocalTime(time):t}" : string.Empty)}
            {(notification.predicted_time.HasValue ? $"**Predicted start time: {DateTimeOffset.FromUnixTimeSeconds((long)notification.predicted_time!).ToLocalTime(time):t}**" : string.Empty)}            
            """);

        await BuildDescriptionAsync(descriptionBuilder, highlightTeam, detailedMatch, cancellationToken).ConfigureAwait(false);

        var embedding = baseBuilder
            .WithTitle($"{events[detailedMatch.EventKey].GetLabel()}: {detailedMatch.CompLevel.ToShortString()} {detailedMatch.SetNumber} - Match {detailedMatch.MatchNumber}")
            .WithUrl($"https://www.thebluealliance.com/match/{notification.match_key}")
            .WithDescription(descriptionBuilder.ToString());

        if (notification.webcast is not null)
        {
            var (source, url) = notification.webcast.GetFullUrl(logger);
            if (url is not null)
            {
                var webcastButton = ButtonBuilder
                    .CreateLinkButton($"{source} ({notification.webcast.ViewerCount} viewer{(notification.webcast.ViewerCount is not 1 ? "s" : string.Empty)})", url.OriginalString)
                    .WithEmote(Emoji.Parse("📺"));
                yield return new(embedding.Build(), [webcastButton.Build()]);
                yield break;
            }
        }

        yield return new(embedding.Build());
    }

    public async IAsyncEnumerable<ResponseEmbedding?> CreateAsync((string eventKey, string teamKey) input, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder(highlightTeam);

        var (eventKey, teamKey) = input;
        var matches = await matchApi.GetTeamEventMatchesAsync(eventKey, teamKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (matches is null)
        {
            yield return new(baseBuilder.WithDescription("No upcoming matches found.").Build());
            yield break;
        }

        var nextMatch = matches.OrderBy(i => i.MatchNumber).First(i => i.ActualTime is null);
        var matchKey = nextMatch.Key;
        var simpleMatch = await matchApi.GetMatchSimpleAsync(matchKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (simpleMatch is null)
        {
            logger.FailedToRetrieveDetailedMatchDataForMatchKey(matchKey);
            yield return null;
            yield break;
        }

        var compLevelHeader = $"{simpleMatch.CompLevel.ToShortString()} {simpleMatch.SetNumber}";

        StringBuilder descriptionBuilder = new();
        descriptionBuilder.AppendLine(
            $"""
            # Next Match for {(teams[highlightTeam!.Value].GetLabel(includeLocation: false, includeNumber: false, asMarkdownLink: false))} at {(events[simpleMatch.EventKey].GetLabel(shortName: true))}
            ## {compLevelHeader} - Match {simpleMatch.MatchNumber}

            Scheduled start time: {DateTimeOffset.FromUnixTimeSeconds(simpleMatch.Time.GetValueOrDefault(0)!).ToLocalTime(time):t}
            **Predicted start time: {DateTimeOffset.FromUnixTimeSeconds(simpleMatch.PredictedTime.GetValueOrDefault(0)).ToLocalTime(time):t}**
            """);

        var matchVideoData = await matchApi.GetMatchAsync(simpleMatch.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
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

        var embedding = baseBuilder.WithDescription(descriptionBuilder.ToString())
            .WithUrl($"https://www.thebluealliance.com/match/{simpleMatch.Key}")
            .Build();

        yield return new(embedding);
    }

    private async Task<StringBuilder> BuildDescriptionAsync(StringBuilder descriptionBuilder, ushort? highlightTeam, MatchSimple matchDetails, CancellationToken cancellationToken, Action<StringBuilder>? beforeFooter = null)
    {
        var ranks = (await eventInsights.GetEventRankingsAsync(matchDetails.EventKey, cancellationToken: cancellationToken).ConfigureAwait(false))!.Rankings.ToDictionary(i => i.TeamKey, i => i.Rank);
        Debug.Assert(ranks is not null);
        logger.RankingsRankings(ranks is not null ? JsonSerializer.Serialize(ranks) : "[null]");

        var stats = await matchStats.ReadMatchV3MatchMatchGetAsync(matchDetails.Key, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (stats is null)
        {
            logger.NoStatsGivenFromStatboticsForMatchMatchKey(matchDetails.Key);
        }

        logger.MatchStatsMatchStats(stats is not null ? JsonSerializer.Serialize(stats) : "[null]");

        MatchSimple.WinningAllianceEnum? predictedWinner = stats?.Pred?.Winner is not null ? MatchSimple.WinningAllianceEnumFromStringOrDefault(stats.Pred.Winner).GetValueOrDefault(MatchSimple.WinningAllianceEnum.Empty) : null;
        var matchSimpleAlliances = matchDetails.Alliances.Blue.TeamKeys
            .Concat(matchDetails.Alliances.Red.TeamKeys)
            .Concat(matchDetails.Alliances.Blue.DqTeamKeys)
            .Concat(matchDetails.Alliances.Red.DqTeamKeys)
            .Concat(matchDetails.Alliances.Blue.SurrogateTeamKeys)
            .Concat(matchDetails.Alliances.Red.SurrogateTeamKeys)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.TeamKeyToTeamNumber()!.Value);

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
                ### Red Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Red]})" : string.Empty)}
                {string.Join("\n", matchDetails.Alliances.Red.TeamKeys.OrderBy(k => k.TeamKeyToTeamNumber()).Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam)}{(ranks is not null && ranks.TryGetValue(t, out var rk) ? $" (#{rk})" : string.Empty)}"))}

                """);
        descriptionBuilder.AppendLine(
            $"""
            ### Blue Alliance{(allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue] is not 0 ? $" (#{allianceRanks[(int)MatchSimple.WinningAllianceEnum.Blue]})" : string.Empty)}
            {string.Join("\n", matchDetails.Alliances.Blue.TeamKeys.OrderBy(k => k.TeamKeyToTeamNumber()).Select(t => $"- {teams[t].GetLabelWithHighlight(highlightTeam)}{(ranks is not null && ranks.TryGetValue(t, out var rk) ? $" (#{rk})" : string.Empty)}"))}
            """);

        if (predictedWinner is not null and not MatchSimple.WinningAllianceEnum.Empty)
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
