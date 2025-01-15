namespace DiscordBotFunctionApp.Embeds;

using Common.Extensions;

internal static class Extensions
{
    public static string MarkdownHighlightIfIsTeam(this string text, uint? highlightTeam)
    {
        var t = text.ToTeamNumber();
        var highlight = highlightTeam.HasValue && t == highlightTeam.Value;

        return highlight ? $"**{t}**" : $"{t}";
    }
}
