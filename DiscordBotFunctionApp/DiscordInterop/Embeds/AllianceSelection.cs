namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using System.Linq;

internal static class AllianceSelection
{
    private static readonly EmbedBuilder _builder = new();
    public static Embed Create(Common.Tba.Notifications.AllianceSelection notification, uint? highlightTeam = null)
    {
        return _builder.WithTitle("Alliance Selection Complete")
            .WithDescription(string.Join("\n\n", notification._event.alliances.Select((alliance, index) =>
$@"**Alliance {index + 1}**
{string.Join("\n", alliance.picks
.Select(t => t.ToTeamNumber())
.Select(team => $"- {(!highlightTeam.HasValue ? team : team == highlightTeam ? $"**{highlightTeam}**" : team)}"))}")))
            .Build();
    }
}
