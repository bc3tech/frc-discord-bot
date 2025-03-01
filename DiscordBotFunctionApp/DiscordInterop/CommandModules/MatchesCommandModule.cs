namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using TheBlueAlliance.Api;

[Group("matches", "Gets information about FRC matches")]
public sealed class MatchesCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(UpcomingMatch));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();
    private readonly IMatchApi _matchApi = services.GetRequiredService<IMatchApi>();

    [SlashCommand("next", "Gets the next match for a team at an event")]
    public async Task ShowAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);
        using var scope = _logger.CreateMethodScope();
        var matches = await _matchApi.GetTeamEventMatchesAsync(eventKey, teamKey);
        if (matches is not null)
        {
            var nextMatch = matches.OrderBy(i => i.MatchNumber).First(i => i.ActualTime is null);
            await GenerateResponseAsync(_embedCreator, nextMatch.Key).ConfigureAwait(false);
        }
    }
}
