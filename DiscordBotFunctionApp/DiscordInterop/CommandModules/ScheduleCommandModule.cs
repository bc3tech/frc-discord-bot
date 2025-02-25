namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;
using Discord.WebSocket;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using FIRST.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Text.Json;
using System.Threading.Tasks;

[Group("schedule", "Gets schedules details")]
public sealed class ScheduleCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<JsonElement> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<JsonElement>>(nameof(ScheduleDetail));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();
    private readonly FIRST.Api.IScheduleApi _scheduleApi = services.GetRequiredService<FIRST.Api.IScheduleApi>();
    private readonly DiscordSocketClient _discordClient = services.GetRequiredService<DiscordSocketClient>();

    [SlashCommand("get", "Gets the schedule for an event and, optionally, team")]
    public async Task ShowAsync([Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("match-type", "The type of match to show (defaults to all)"), Autocomplete(typeof(AutoCompleteHandlers.FrcMatchTypeAutoCompleteHandler))] string matchType,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string teamKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await this.DeferAsync(ephemeral: !post).ConfigureAwait(false);
        using var typing = this.Context.Channel.EnterTypingState();

        using var scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        var dataFetchTask = _scheduleApi.SeasonScheduleEventCodeGetAsync(eventKey, TimeProvider.System.GetLocalNow().Year.ToString(), teamNumber: teamKey.ToTeamNumber().ToString(), tournamentLevel: Enum.Parse<TournamentLevel>(matchType));
        int iteration = 0;
        while (true)
        {
            var waiter = Task.Delay(2000);
            var completedTask = await Task.WhenAny(waiter, dataFetchTask).ConfigureAwait(false);
            if (completedTask == waiter)
            {
                await this.ModifyOriginalResponseAsync(p =>
                {
                    p.Content = $"Still working on it! Standby...{new string('.', iteration++)}";
                });
                continue;
            }
            else if (completedTask is null)
            {
                await this.ModifyOriginalResponseAsync(p => p.Content = "No scheduled matches were found.").ConfigureAwait(false);
                return;
            }
            else if (completedTask == dataFetchTask)
            {
                var response = (JsonElement?)await dataFetchTask.ConfigureAwait(false);
                if (response is null || !response.HasValue || response.Value.GetProperty("Schedule").GetArrayLength() is 0)
                {
                    await this.ModifyOriginalResponseAsync(p => p.Content = "No scheduled matches were found.").ConfigureAwait(false);
                }
                else
                {
                    await GenerateResponseAsync(_embedCreator, response.Value).ConfigureAwait(false);
                }

                return;
            }
        }
    }
}
