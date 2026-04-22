namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using FunctionApp.DiscordInterop;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

[Group("matches", "Gets information about FRC matches")]
public class MatchesCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<EventsCommandModule>>())
{
    private readonly IEmbedCreator<(string eventKey, string teamKey)> _upcomingMatchEmbeddingCreator = services.GetRequiredKeyedService<IEmbedCreator<(string eventKey, string teamKey)>>(nameof(UpcomingMatch));
    private readonly IEmbedCreator<(string matchKey, bool summarize)> matchScoreEmbeddingGenerator = services.GetRequiredKeyedService<IEmbedCreator<(string, bool)>>(nameof(MatchScore));

    [SlashCommand("next", "Gets the next match for a team at an event")]
    public async Task ShowNextAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using IDisposable? typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using IDisposable scope = Logger.CreateMethodScope();
        try
        {
            teamKey = CommandInputNormalization.NormalizeTeamKey(teamKey);

            await GenerateResponseAsync(_upcomingMatchEmbeddingCreator, (eventKey, teamKey), teamKey.TeamKeyToTeamNumber()).ConfigureAwait(false);
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            HandleShowNextError(e, teamKey, eventKey);
            await UpdateOriginalResponseAsync(p => p.Content = "Sorry, I encountered an error processing your request. Maybe try again? Or contact your admin with this news so they can troubleshoot.").ConfigureAwait(false);
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
        using IDisposable? typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using IDisposable scope = Logger.CreateMethodScope();
        string matchKey = CommandInputNormalization.BuildMatchKey(eventKey, compLevel, matchNumber);
        await GenerateResponseAsync(matchScoreEmbeddingGenerator, (matchKey, summarize)).ConfigureAwait(false);
    }

    protected virtual void HandleShowNextError(Exception exception, string teamKey, string eventKey)
    {
        Debug.Fail(exception.Message);
        Logger.ErrorGettingNextMatchForTeamKeyAtEventKey(exception, teamKey, eventKey);
    }
}
