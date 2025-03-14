﻿namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;
using Discord.Net;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

[Group("matches", "Gets information about FRC matches")]
public sealed class MatchesCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly UpcomingMatch embeddingCreator = (UpcomingMatch)services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(UpcomingMatch));
    private readonly MatchScore matchScoreEmbeddingGenerator = (MatchScore)services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(MatchScore));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();
    private readonly IMatchApi _matchApi = services.GetRequiredService<IMatchApi>();

    [SlashCommand("next", "Gets the next match for a team at an event")]
    public async Task ShowNextAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);
        using var scope = _logger.CreateMethodScope();
        try
        {
            // In case the user just gives us team number
            if (int.TryParse(teamKey, out var teamNumber))
            {
                teamKey = $"frc{teamNumber}";
            }

            var matches = await _matchApi.GetTeamEventMatchesAsync(eventKey, teamKey);
            if (matches is not null)
            {
                var nextMatch = matches.OrderBy(i => i.MatchNumber).First(i => i.ActualTime is null);

                ResponseEmbedding[] embeds = [];
                await foreach (var m in embeddingCreator.CreateNextMatchEmbeddingsAsync(nextMatch.Key, highlightTeam: teamKey.TeamKeyToTeamNumber()).ConfigureAwait(false))
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
            Debug.Fail(ex.Message);
            _logger.ErrorGettingNextMatchForTeamKeyAtEventKey(ex, teamKey, eventKey);
            await this.ModifyOriginalResponseAsync(p => p.Content = "Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
        }
    }

    [SlashCommand("score", "Gets the score of a match")]
    public Task GetScoreAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("stage", "The stage of the competition"), Autocomplete(typeof(AutoCompleteHandlers.CompStageAutocompleteHandler))] int compLevel,
        [Summary("match", "Match number")] uint matchNumber,
        [Summary("post", "`true` to post response publicly")] bool post = false) => GetResultAsync(eventKey, compLevel, matchNumber, post);

    [SlashCommand("result", "Gets the result for a match")]
    public async Task GetResultAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("stage", "The stage of the competition"), Autocomplete(typeof(AutoCompleteHandlers.CompStageAutocompleteHandler))] int compLevel,
        [Summary("match", "Match number")] uint matchNumber,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);
        using var scope = _logger.CreateMethodScope();
        string matchKey = (Match.CompLevelEnum)compLevel switch
        {
            Match.CompLevelEnum.Qm => $"qm{matchNumber}",
            Match.CompLevelEnum.Sf => $"sf{matchNumber}m1",
            Match.CompLevelEnum.F => $"f1m{matchNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(compLevel), compLevel, null)
        };
        try
        {
            ResponseEmbedding[] embeds = [];

            await foreach (var m in matchScoreEmbeddingGenerator.GetMatchScoreAsync($"{eventKey}_{matchKey}").ConfigureAwait(false))
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
        catch (Exception ex) when (ex is HttpException { DiscordCode: Discord.DiscordErrorCode.UnknownInteraction or Discord.DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged } or InteractionException)
        {
            _logger.InteractionAlreadyAcknowledgedSkippingResponse();
        }
        catch (Exception ex)
        {
            _logger.ErrorRespondingWithMatchDataForEventEventKeyMatchKeyMatchKey(ex, eventKey, matchKey);
            await this.ModifyOriginalResponseAsync(p => p.Content = "Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
        }
    }
}
