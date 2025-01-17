namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

internal sealed class EmbedBuilderFactory
{
    public EmbedBuilder GetBuilder() => new EmbedBuilder()
        .WithColor(255, 255, 01)
        .WithCurrentTimestamp();
}