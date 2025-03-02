namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Group("teams", "Gets information about FRC teams")]
public class TeamsCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(TeamDetail));
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

        await GenerateResponseAsync(_embedCreator, teamKey).ConfigureAwait(false);
    }
}
