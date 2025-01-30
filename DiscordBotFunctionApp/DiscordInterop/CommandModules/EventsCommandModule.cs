namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Group("events", "Gets information about FRC events")]
public class EventsCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(EventDetail));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();

    [SlashCommand("get-details", "Gets details about an event")]
    public async Task ShowAsync([Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey, [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await this.RespondAsync("Event key is required.", ephemeral: true).ConfigureAwait(false);
            return;
        }

        await GenerateResponseAsync(_embedCreator, eventKey, e => this.ModifyOriginalResponseAsync(p => p.Embeds = e.ToArray())).ConfigureAwait(false);
    }
}
