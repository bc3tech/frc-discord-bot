namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;
using Common.Tba;
using Common.Tba.Api;
using Common.Tba.Notifications;

using Discord;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

internal sealed class Award(ApiClient tbaApi, EmbedBuilderFactory builderFactory, TeamRepository teams, ILogger<Award> logger) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.awards_posted;

    public async IAsyncEnumerable<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = await msg.GetManyDataAsAsync<AwardsPosted, Common.Tba.Api.Models.Award>(cancellationToken).ConfigureAwait(false);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return baseBuilder.Build();
            yield break;
        }

        var eventAwards = await tbaApi.Event[notification.event_key].Awards.GetAsync(cancellationToken: cancellationToken);
        if (eventAwards is null)
        {
            logger.LogWarning("Failed to retrieve detailed awards data for {EventKey}", notification.event_key);
            yield return baseBuilder.Build();
            yield break;
        }

        if (eventAwards.Count is 0)
        {
            logger.LogWarning("No awards found for {EventKey}", notification.event_key);
            yield return baseBuilder.Build();
            yield break;
        }

        if (highlightTeam is not null)
        {
            eventAwards = [.. eventAwards.Where(i => i.RecipientList?.Any(j => j.TeamKey.ToTeamNumber() == highlightTeam) is true)];
        }

        foreach (var latestAward in eventAwards)
        {
            var embedding = baseBuilder
                .WithDescription(
    $@"# Award!
## {latestAward.Name}
### {(latestAward.RecipientList?.Count > 1 ? "Recipients" : "Recipient")}
{string.Join("\n", latestAward.RecipientList!
        .Select(t => $"- {teams.GetTeamLabelWithHighlight(t.TeamKey ?? string.Empty, highlightTeam)}{(!string.IsNullOrWhiteSpace(t.Awardee) ? $" [{t.Awardee}]" : string.Empty)}"))}

View more event awards [here](https://www.thebluealliance.com/event/{notification.event_key}#awards)
")
                .Build();

            yield return embedding;
        }
    }
}
