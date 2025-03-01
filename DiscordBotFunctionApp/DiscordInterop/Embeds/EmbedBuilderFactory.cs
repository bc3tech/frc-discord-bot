﻿namespace DiscordBotFunctionApp.DiscordInterop.Embeds;

using Discord;

internal sealed class EmbedBuilderFactory
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "DI-created singleton")]
    public EmbedBuilder GetBuilder() => new EmbedBuilder()
        .WithColor(255, 255, 01)
        .WithFooter(
#if DEBUG
        "**DEVELOPMENT MODE** " +
#endif
        "Data provided by The Blue Alliance, FIRST, and Statbotics.io");
}