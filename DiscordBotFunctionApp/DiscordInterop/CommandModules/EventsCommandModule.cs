namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Group("events", "Gets information about FRC events")]
public class EventsCommandModule(IServiceProvider services) : InteractionModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(EventDetail));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();

    [SlashCommand("get-details", "Gets details about an event")]
    public async Task ShowAsync([Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey, [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await this.RespondAsync("Event key is required.", ephemeral: true).ConfigureAwait(false);
            return;
        }

        ResponseEmbedding[] embeds = [];
        var modify = false;
        await foreach (var m in _embedCreator.CreateAsync(eventKey))
        {
            embeds = [.. embeds, m];
            var discordEmbeds = embeds.Select(i => i.Content).ToArray();
            if (!m.Transient)
            {
                discordEmbeds = [.. embeds.Where(i => !i.Transient).Select(i => i.Content)];
            }

            if (modify)
            {
                await this.ModifyOriginalResponseAsync(p => p.Embeds = discordEmbeds);
            }
            else
            {
                await this.RespondAsync(embeds: discordEmbeds, ephemeral: !post);
                modify = true;
            }
        }
    }
}
