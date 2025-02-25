namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Group("events", "Gets information about FRC events")]
public partial class EventsCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(EventDetail));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();
    private readonly Statbotics.Api.IEventApi _eventApi = services.GetRequiredService<Statbotics.Api.IEventApi>();
    private readonly Statbotics.Api.ITeamEventApi _teamEventApi = services.GetRequiredService<Statbotics.Api.ITeamEventApi>();

    [SlashCommand("get-details", "Gets details about an event")]
    public async Task ShowAsync([Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey, [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        await GenerateResponseAsync(_embedCreator, eventKey).ConfigureAwait(false);
    }

    [SlashCommand("get-stats", "Gets stats about an event")]
    public async Task GetStatsAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string? teamKey = null,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        if (!string.IsNullOrWhiteSpace(teamKey))
        {
            var teamEventStatus = await _teamEventApi.ReadTeamEventV3TeamEventTeamEventGetAsync(teamKey, eventKey).ConfigureAwait(false);
        }
        else
        {
            var eventStats = await _eventApi.ReadEventV3EventEventGetAsync(eventKey).ConfigureAwait(false);
        }

        await GenerateResponseAsync(_embedCreator, eventKey).ConfigureAwait(false);
    }
}
