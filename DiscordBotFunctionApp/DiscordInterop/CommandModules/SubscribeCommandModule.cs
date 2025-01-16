namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord.Interactions;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Subscription;

using Microsoft.Extensions.DependencyInjection;

using System.Diagnostics;

[Group("subscription", "Manages subscriptions to FRC events and teams")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to work with Discord.Net")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Can't use CTs in command signatures as not compatible with injection model")]
public class SubscribeCommandModule(IServiceProvider services) : InteractionModuleBase
{
    private readonly SubscriptionManager? _subscriptionManager = services.GetRequiredService<SubscriptionManager>();
    private readonly EventRepository _eventsRepo = services.GetRequiredService<EventRepository>();
    private readonly TeamRepository _teamsRepo = services.GetRequiredService<TeamRepository>();

    [SlashCommand("show", "Shows the current subscriptions")]
    public async Task ShowAsync()
    {
        await this.RespondAsync("Not yet implemented").ConfigureAwait(false);
    }

    [SlashCommand("create", "Creates a subscription to a team/event for the current channel")]
    public async Task CreateAsync(
        [Summary("event", "Event to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))]
        string? eventKey=null,
        [Summary("team", "Team to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))]
        uint? teamNumber= null)
    {
        if (eventKey is null && teamNumber is null)
        {
            await this.RespondAsync("At least one of Event or Team is required.", ephemeral: true);
        }
        else
        {
            Debug.Assert(_subscriptionManager is not null);
            Debug.Assert(this.Context.Interaction.GuildId.HasValue);
            Debug.Assert(this.Context.Interaction.ChannelId.HasValue);

            try
            {
                await _subscriptionManager.SaveSubscriptionAsync(new SubscriptionRequest(this.Context.Interaction.GuildId!.Value, this.Context.Interaction.ChannelId!.Value, eventKey, teamNumber), default);
                if (!string.IsNullOrWhiteSpace(eventKey) && teamNumber is not null)
                {
                    await this.RespondAsync($"Channel subscription for {_teamsRepo.GetLabelForTeam(teamNumber)} at {_eventsRepo.GetLabelForEvent(eventKey)} created successfully.");
                }
                else if (!string.IsNullOrWhiteSpace(eventKey))
                {
                    await this.RespondAsync($"Channel subscription to {_eventsRepo.GetLabelForEvent(eventKey)} created successfully.");
                }
                else
                {
                    await this.RespondAsync($"Channel subscription to {_teamsRepo.GetLabelForTeam(teamNumber)} created successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
                await this.RespondAsync("An error occurred while creating the subscription. Please try again later.", ephemeral: true);
            }
        }
    }
}
