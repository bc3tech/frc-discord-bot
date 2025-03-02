namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;
using Discord.Net;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Api;

[Group("matches", "Gets information about FRC matches")]
public sealed class MatchesCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly UpcomingMatch embeddingCreator = (UpcomingMatch)services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(UpcomingMatch));
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
        try
        {
            var matches = await _matchApi.GetTeamEventMatchesAsync(eventKey, teamKey);
            if (matches is not null)
            {
                var nextMatch = matches.OrderBy(i => i.MatchNumber).First(i => i.ActualTime is null);

                ResponseEmbedding[] embeds = [];
                await foreach (var m in embeddingCreator.CreateNextMatchEmbeddingsAsync(nextMatch.Key, highlightTeam: teamKey.ToTeamNumber()).ConfigureAwait(false))
                {
                    if (m is null)
                    {
                        continue;
                    }

                    embeds = [.. embeds, m];
                    var discordEmbeds = embeds.Select(i => i.Content).ToArray();
                    if (!m.Transient)
                    {
                        discordEmbeds = [.. embeds.Where(i => !i.Transient).Select(i => i.Content)];
                    }

                    await this.ModifyOriginalResponseAsync(p => p.Embeds = discordEmbeds).ConfigureAwait(false);
                }
            }
            else
            {
                await this.ModifyOriginalResponseAsync(p => p.Content = "No upcoming matches found.").ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is HttpException { DiscordCode: Discord.DiscordErrorCode.UnknownInteraction or Discord.DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged } or InteractionException)
        {
            _logger.InteractionAlreadyAcknowledgedSkippingResponse();
        }
        catch (Exception ex)
        {
            _logger.ErrorGettingNextMatchForTeamKeyAtEventKey(ex, teamKey, eventKey);
            await this.ModifyOriginalResponseAsync(p => p.Content = "Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
        }
    }
}
