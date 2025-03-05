namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Common.Extensions;

using Discord;

internal sealed class EmbedBuilderFactory(EmbeddingColorizer colorizer)
{
    private static readonly EmbedBuilder _defaultBuilder = new();

    public EmbedBuilder GetBuilder(string teamKey) => GetBuilder(teamKey.ToTeamNumber());

    public EmbedBuilder GetBuilder(ushort? teamNumber = null)
    {
        var r = _defaultBuilder
        .WithFooter(
#if DEBUG
        "**DEVELOPMENT MODE** " +
#endif
        "Data provided by The Blue Alliance, FIRST, and Statbotics.io");

        colorizer.SetEmbeddingColor(teamNumber, r);

        return r;
    }
}