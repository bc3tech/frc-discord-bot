namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

using Statbotics.Api;
using Statbotics.Model;

internal sealed class EmbedBuilderFactory(ITeamApi teamColorFetcher)
{
    public EmbedBuilder GetBuilder(string teamKey)
    {
        var n = teamKey.ToTeamNumber();
        return n.HasValue ? GetBuilder(n.Value) : GetBuilderInternal();
    }

    public EmbedBuilder GetBuilder(ushort? teamNumber)
    {
        if (teamNumber.HasValue)
        {
            var teamDetail = teamColorFetcher.ReadTeamV3TeamTeamGet(teamNumber.ToString()!);
            return GetBuilderInternal(teamDetail?.Colors);
        }

        return GetBuilderInternal(null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "DI-created singleton")]
    public EmbedBuilder GetBuilder(Colors? theme = null) => GetBuilderInternal(theme);

    private static readonly EmbedBuilder _defaultBuilder = new();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "DI-created singleton")]
    private static EmbedBuilder GetBuilderInternal(Colors? theme = null)
    {
        var r = _defaultBuilder
        .WithFooter(
#if DEBUG
        "**DEVELOPMENT MODE** " +
#endif
        "Data provided by The Blue Alliance, FIRST, and Statbotics.io");

        var themeColor = Utility.GetLightestColorOf(
                theme?.Primary is not null ? Color.Parse(theme.Primary) : null,
                theme?.Secondary is not null ? Color.Parse(theme.Secondary) : null);

        return themeColor is not null ? r.WithColor(themeColor.Value) : r.WithColor(255, 255, 01);
    }
}