namespace FunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

internal sealed class EmbedBuilderFactory(EmbeddingColorizer colorizer)
{
    private const string FooterText =
#if DEBUG
        "**DEVELOPMENT MODE** " +
#endif
        "Data provided by The Blue Alliance, FIRST, and Statbotics.io";

    public EmbedBuilder GetBuilder(string teamKey, bool footerRequired = true) => GetBuilder(teamKey.TeamKeyToTeamNumber(), footerRequired);

    public EmbedBuilder GetBuilder(ushort? teamNumber = null, bool footerRequired = true)
    {
        var r = new EmbedBuilder()
            .WithFooter(
#if !DEBUG
            footerRequired ? FooterText : string.Empty
#else
            FooterText
#endif
            );

        colorizer.SetEmbeddingColor(teamNumber, r);

        return r;
    }
}