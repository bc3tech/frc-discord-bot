namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord.Interactions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Subscription;

using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics;

[Group("subscription", "Manages subscriptions to FRC events and teams")]
public sealed class SubscriptionCommandModule(IServiceProvider services) : InteractionModuleBase
{
    private readonly SubscriptionManager _subscriptionManager = services.GetRequiredService<SubscriptionManager>();
    private readonly EventRepository _eventsRepo = services.GetRequiredService<EventRepository>();
    private readonly TeamRepository _teamsRepo = services.GetRequiredService<TeamRepository>();

    [SlashCommand("show", "Shows the current subscriptions")]
    public async Task ShowAsync()
    {
        await this.DeferAsync(ephemeral: true).ConfigureAwait(false);

        HashSet<(string, ushort?)> currentSubs = [];
        await foreach (var (channelId, eventKey, teamNumber) in _subscriptionManager.GetSubscriptionsForGuildAsync(this.Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == this.Context.Interaction.ChannelId!.Value))
        {
            currentSubs.Add((eventKey, teamNumber));
        }

        if (currentSubs.Count is 0)
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
        }
        else
        {
            var groupedSubscriptions = currentSubs.GroupBy(i => i.Item1);
            // Create a string that starts withe the group key then lists all the group values on subsequent lines
            // This is a bit more complex than it needs to be because we want to show the team number if it's not 'all'
            // and we want to show the event key if it's not 'all'
            var output = groupedSubscriptions.Select(i => $@"- **{(i.Key is not CommonConstants.ALL ? _eventsRepo[i.Key].GetLabel() : "All Events")}**:
{string.Join('\n', i.Select(j => $"  - {(j.Item2.HasValue ? _teamsRepo[j.Item2.Value.ToTeamKey()].GetLabel() : "All Teams")}"))}");
            await this.ModifyOriginalResponseAsync(p => p.Content = $@"Subscriptions for this channel include:
{string.Join('\n', output)}").ConfigureAwait(false);
        }
    }

    [SlashCommand("create", "Creates a subscription to a team/event for the current channel")]
    public async Task CreateAsync(
        [Summary("team", "Team to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string? teamKey = null,
        [Summary("event", "Event to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string? eventKey = null)
    {
        await this.DeferAsync(ephemeral: true).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(eventKey) && string.IsNullOrWhiteSpace(teamKey))
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "At least one of Event or Team is required.").ConfigureAwait(false);
        }
        else
        {
            Debug.Assert(_subscriptionManager is not null);
            Debug.Assert(this.Context.Interaction.ChannelId.HasValue);

            var teamNumber = teamKey.ToTeamNumber();

            try
            {
                await _subscriptionManager.SaveSubscriptionAsync(new SubscriptionRequest(this.Context.Interaction.GuildId, this.Context.Interaction.ChannelId!.Value, eventKey, teamNumber), default).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(eventKey) && !string.IsNullOrWhiteSpace(teamKey))
                {
                    await this.ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey].GetLabel(includeLocation: false)}** at the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else if (!string.IsNullOrWhiteSpace(eventKey))
                {
                    await this.ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else
                {
                    await this.ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey!].GetLabel(includeLocation: false)}**.").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                await this.DeleteOriginalResponseAsync();
                await this.RespondAsync("An error occurred while creating the subscription. Please try again later.", ephemeral: true).ConfigureAwait(false);
            }
        }
    }
}
