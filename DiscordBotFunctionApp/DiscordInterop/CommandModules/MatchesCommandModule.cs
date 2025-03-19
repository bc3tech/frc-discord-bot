namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

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
public sealed class MatchesCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<EventsCommandModule>>())
{
    private readonly IEmbedCreator<(string eventKey, string teamKey)> _upcomingMatchEmbeddingCreator = services.GetRequiredKeyedService<IEmbedCreator<(string eventKey, string teamKey)>>(nameof(UpcomingMatch));
    private readonly IEmbedCreator<(string matchKey, bool summarize)> matchScoreEmbeddingGenerator = services.GetRequiredKeyedService<IEmbedCreator<(string, bool)>>(nameof(MatchScore));
    private readonly IMatchApi _matchApi = services.GetRequiredService<IMatchApi>();

    [SlashCommand("next", "Gets the next match for a team at an event")]
    public async Task ShowNextAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        try
        {
            // In case the user just gives us team number
            if (int.TryParse(teamKey, out var teamNumber))
            {
                teamKey = $"frc{teamNumber}";
            }

            await GenerateResponseAsync(_upcomingMatchEmbeddingCreator, (eventKey, teamKey), teamKey.TeamKeyToTeamNumber()).ConfigureAwait(false);
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            Debug.Fail(e.Message);
            this.Logger.ErrorGettingNextMatchForTeamKeyAtEventKey(e, teamKey, eventKey);
            await this.ModifyOriginalResponseAsync(p => p.Content = "Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
        }
    }

    [SlashCommand("score", "Gets the score of a match")]
    public Task GetScoreAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("stage", "The stage of the competition"), Autocomplete(typeof(AutoCompleteHandlers.CompStageAutocompleteHandler))] int compLevel,
        [Summary("match", "Match number")] uint matchNumber,
        [Summary("summarize", "Create a 'ChatGPT' style summary?")] bool summarize = false,
        [Summary("post", "`true` to post response publicly")] bool post = false) => GetResultAsync(eventKey, compLevel, matchNumber, summarize, post);

    [SlashCommand("result", "Gets the result for a match")]
    public async Task GetResultAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("stage", "The stage of the competition"), Autocomplete(typeof(AutoCompleteHandlers.CompStageAutocompleteHandler))] int compLevel,
        [Summary("match", "Match number")] uint matchNumber,
        [Summary("summarize", "Create a 'ChatGPT' style summary?")] bool summarize = false,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        string matchKey = (Match.CompLevelEnum)compLevel switch
        {
            Match.CompLevelEnum.Qm => $"qm{matchNumber}",
            Match.CompLevelEnum.Sf => $"sf{matchNumber}m1",
            Match.CompLevelEnum.F => $"f1m{matchNumber}",
            _ => throw new ArgumentOutOfRangeException(nameof(compLevel), compLevel, null)
        };

        await GenerateResponseAsync(matchScoreEmbeddingGenerator, ($"{eventKey}_{matchKey}", summarize)).ConfigureAwait(false);
    }
}
