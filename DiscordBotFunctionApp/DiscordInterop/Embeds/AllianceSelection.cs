namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;
using Common.Tba;
using Common.Tba.Api;
using Common.Tba.Notifications;

using Discord;

using System.Linq;

internal sealed class AllianceSelection(ApiClient tbaClient, EmbedBuilder baseBuilder) : IEmbedCreator
{
    public static NotificationType TargetType { get; } = NotificationType.alliance_selection;

    public async Task<Embed> CreateAsync(WebhookMessage msg, ushort? highlightTeam = null, CancellationToken cancellationToken = default)
    {
        var notification = await msg.GetDataAsAsync<Common.Tba.Notifications.AllianceSelection>(cancellationToken).ConfigureAwait(false);
        if (notification is null)
        {
            return baseBuilder.Build();
        }

        var eventKey = !string.IsNullOrWhiteSpace(notification.event_key) ? notification.event_key : notification.Event?.Key;
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            return baseBuilder.Build();
        }

        var alliances = await tbaClient.Event[eventKey].Alliances.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (alliances is null or { Count: 0 })
        {
            return baseBuilder.Build();
        }

        return baseBuilder.WithTitle("Alliance Selection Complete")
            .WithDescription(string.Join("\n\n", alliances.Select((alliance, index) =>
$@"**Alliance {index + 1}**
{string.Join("\n", alliance.Picks!
.Select(t => t.ToTeamNumber())
.Select(team => $"- {(!highlightTeam.HasValue ? team : team == highlightTeam ? $"**{highlightTeam}**" : team)}"))}")))
            .Build();
    }
}
