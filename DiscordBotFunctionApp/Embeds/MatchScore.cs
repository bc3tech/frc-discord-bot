namespace DiscordBotFunctionApp.Embeds;

using Common.Tba;

using Discord;
using Discord.Rest;

internal static class MatchScore
{
    private static readonly EmbedBuilder _builder = new();
    public static Embed Create(Common.Tba.Notifications.MatchScore notification, uint? highlightTeam = null)
    {
        var embedding = _builder
            .WithDescription(
$@"# Scores are in!
## {Translator.CompLevelToShortString(notification.match.CompLevel.ToString())} {notification.match.SetNumber} 
### Match {notification.match.MatchNumber}

**{(notification.match.Alliances.Blue.Score < notification.match.Alliances.Red.Score ? "🏅" : string.Empty)}Red Alliance - {notification.match.Alliances.Red.Score}**
{string.Join("\n", notification.match.Alliances.Red.TeamKeys
.Select(t => $"- {t.MarkdownHighlightIfIsTeam(highlightTeam)}"))}

        **{(notification.match.Alliances.Blue.Score > notification.match.Alliances.Red.Score ? "🏅" : string.Empty)}
        Blue Alliance - {notification.match.Alliances.Blue.Score}
        **
{string.Join("\n", notification.match.Alliances.Blue.TeamKeys
.Select(t => $"- {t.MarkdownHighlightIfIsTeam(highlightTeam)}"))}
        ")
            .WithUrl($"https://www.thebluealliance.com/match/{notification.match.Key}")
            .WithColor(255, 255, 1) // Bear Metal Yellow
            .Build();

        var embeddingJson = embedding.ToJsonString();
        return embedding;
    }
}
