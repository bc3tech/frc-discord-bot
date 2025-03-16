namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;
using Discord.Net;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Group("teams", "Gets information about FRC teams")]
public sealed class TeamsCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _teamDetailEmbedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(TeamDetail));
    private readonly IEmbedCreator<(int? Year, string TeamKey, string? EventKey)> _teamRankEmbedCreator = services.GetRequiredKeyedService<IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>>(nameof(TeamRank));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<TeamsCommandModule>>();

    [SlashCommand("get-details", "Gets details about a team")]
    public async Task ShowAsync([Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey, [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(teamKey))
        {
            await this.RespondAsync("Team key is required.", ephemeral: true).ConfigureAwait(false);
            return;
        }

        // In case the user just gives us team number
        if (int.TryParse(teamKey, out var teamNumber))
        {
            teamKey = $"frc{teamNumber}";
        }

        await GenerateResponseAsync(_teamDetailEmbedCreator, teamKey).ConfigureAwait(false);
    }

    [SlashCommand("rank", "Gets ranking information about a team")]
    public async Task GetRankAsync(
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string? eventKey = null,
        [Summary("year", "Year to get rank, default: current year")] ushort? year = null,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        try
        {
            await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);
        }
        catch (HttpException e) when (e.DiscordCode is Discord.DiscordErrorCode.UnknownInteraction or Discord.DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
        {
            _logger.InteractionAlreadyAcknowledgedSkippingResponse();
            return;
        }

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(teamKey))
        {
            await this.RespondAsync("Team key is required.", ephemeral: true).ConfigureAwait(false);
            return;
        }

        // In case the user just gives us team number
        if (int.TryParse(teamKey, out var teamNumber))
        {
            teamKey = $"frc{teamNumber}";
        }

        await GenerateResponseAsync(_teamRankEmbedCreator, (year, teamKey, eventKey)).ConfigureAwait(false);
    }
}
