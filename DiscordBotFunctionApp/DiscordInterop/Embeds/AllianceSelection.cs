namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;
using Common.Tba;
using Common.Tba.Api;
using Common.Tba.Notifications;

using Discord;

using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.Logging;

using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

internal sealed class AllianceSelection(TeamRepository teams, ApiClient tbaClient, EmbedBuilderFactory builderFactory, ILogger<AllianceSelection> logger) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.alliance_selection;

    public async IAsyncEnumerable<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseBuilder = builderFactory.GetBuilder();
        var notification = await msg.GetDataAsAsync<Common.Tba.Notifications.AllianceSelection>(cancellationToken).ConfigureAwait(false);
        if (notification is null)
        {
            logger.LogWarning("Failed to deserialize notification data as {NotificationType}", TargetType);
            yield return baseBuilder.Build();
            yield break;
        }

        var eventKey = !string.IsNullOrWhiteSpace(notification.event_key) ? notification.event_key : notification.Event?.Key;
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            logger.LogWarning("Event key is missing from notification data");
            yield return baseBuilder.Build();
            yield break;
        }

        var alliances = await tbaClient.Event[eventKey].Alliances.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (alliances?.Count is null or 0)
        {
            logger.LogWarning("Failed to retrieve alliance selection data for {EventKey}", eventKey);
            yield return baseBuilder.Build();
            yield break;
        }

        if (alliances[0].Declines is null)
        {
            alliances[0].Declines = ["frc2910", "frc360"];
        }
        else
        {
            alliances[0].Declines!.AddRange(["frc2910", "frc360"]);
        }

        // We have to build this with loops instead of interpolation because we don't want to output **anything** if Declines is empty (not even a line break)
        var descriptionBuilder = new StringBuilder("# Alliance Selection Complete\n");
        for (int i = 0; i < alliances.Count; i++)
        {
            var alliance = alliances[i];
            descriptionBuilder.AppendLine($"## Alliance {i + 1}");
            foreach (var team in alliance.Picks!.OrderBy(t => t.ToTeamNumber()))
            {
                descriptionBuilder.AppendLine($"- {teams.GetTeamLabelWithHighlight(team, highlightTeam)}");
            }

            if (alliance.Declines?.Count is not null and not 0)
            {
                descriptionBuilder.AppendLine($"__Declining Team{(alliance.Declines!.Count > 1 ? "s" : string.Empty)}__");
                foreach (var team in alliance.Declines!.OrderBy(t => t.ToTeamNumber()))
                {
                    descriptionBuilder.AppendLine($"- {teams.GetTeamLabelWithHighlight(team, highlightTeam)}");
                }
            }
        }

        yield return baseBuilder
            .WithDescription(descriptionBuilder.ToString())
            .Build();
    }
}
