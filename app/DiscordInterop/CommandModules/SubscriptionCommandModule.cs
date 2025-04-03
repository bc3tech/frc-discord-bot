namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Net;

using FunctionApp;
using FunctionApp.DiscordInterop;
using FunctionApp.Extensions;
using FunctionApp.Subscription;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Linq;
using System.Threading;

using TheBlueAlliance.Caching;

[Group("subscription", "Manages subscriptions to FRC events and teams")]
public sealed class SubscriptionCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<SubscriptionCommandModule>>()), IHandleUserInteractions
{
    private readonly SubscriptionManager _subscriptionManager = services.GetRequiredService<SubscriptionManager>();
    private readonly EventCache _eventsRepo = services.GetRequiredService<EventCache>();
    private readonly TeamCache _teamsRepo = services.GetRequiredService<TeamCache>();

    [SlashCommand("show", "Shows the current subscriptions")]
    public async Task ShowAsync()
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        HashSet<(string, string)> currentSubs = [];
        await foreach (var sub in _subscriptionManager.GetSubscriptionsForGuildAsync(this.Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == this.Context.Interaction.ChannelId!.Value))
        {
            currentSubs.Add((sub.Event ?? CommonConstants.ALL, sub.Team ?? CommonConstants.ALL));
        }

        if (currentSubs.Count is 0)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
        }
        else
        {
            var groupedSubscriptions = currentSubs.GroupBy(i => i.Item1);
            // Create a string that starts with the group key then lists all the group values on subsequent lines
            // This is a bit more complex than it needs to be because we want to show the team number if it's not 'all'
            // and we want to show the event key if it's not 'all'
            var output = groupedSubscriptions.Select(i => $"""
                - **{(i.Key is not CommonConstants.ALL ? _eventsRepo[i.Key].GetLabel() : "All Events")}**:
                {string.Join('\n', i.Select(j => $"  - {(j.Item2 is not CommonConstants.ALL ? _teamsRepo[j.Item2].GetLabel() : "All Teams")}"))}
                """);
            await ModifyOriginalResponseAsync(p =>
            {
                p.Content = $"""
                Subscriptions for this channel include:
                {string.Join('\n', output)}
                """;
                p.Flags = MessageFlags.SuppressEmbeds;
            }).ConfigureAwait(false);
        }
    }

    [RequireUserPermission(GuildPermission.ManageChannels, Group = "Permission")]
    [RequireContext(ContextType.DM, Group = "Permission")]
    [SlashCommand("create", "Creates a subscription to a team/event for the current channel")]
    public async Task CreateAsync(
        [Summary("team", "Team to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string? teamKey = null,
        [Summary("event", "Event to subscribe to, 'all' if not specified."), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string? eventKey = null)
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey) && string.IsNullOrWhiteSpace(teamKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "At least one of Event or Team is required.").ConfigureAwait(false);
        }
        else
        {
            Debug.Assert(_subscriptionManager is not null);
            Debug.Assert(this.Context.Interaction.ChannelId.HasValue);

            try
            {
                await _subscriptionManager.SaveSubscriptionAsync(new NotificationSubscription(this.Context.Interaction.GuildId, this.Context.Interaction.ChannelId!.Value, eventKey, teamKey), default).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(eventKey) && !string.IsNullOrWhiteSpace(teamKey))
                {
                    await ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey].GetLabel(includeLocation: false)}** at the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else if (!string.IsNullOrWhiteSpace(eventKey))
                {
                    await ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else
                {
                    await ModifyOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey!].GetLabel(includeLocation: false)}**.").ConfigureAwait(false);
                }
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Debug.Fail(e.Message);
                await DeleteOriginalResponseAsync();
                await RespondAsync("An error occurred while creating the subscription. Please try again later.", ephemeral: true).ConfigureAwait(false);
            }
        }
    }

    private const string SubscriptionDeleteSelectionMenuId = "subscription-delete-selection";

    [RequireUserPermission(GuildPermission.ManageChannels, Group = "Permission")]
    [RequireContext(ContextType.DM, Group = "Permission")]
    [SlashCommand("delete", "Deletes a subscription to a team/event for the current channel")]
    public async Task DeleteAsync()
    {
        using var typing = await TryDeferAsync(ephemeral: this.Context.Channel is not IDMChannel).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        var activeSubsForChannel = await _subscriptionManager.GetSubscriptionsForGuildAsync(this.Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == this.Context.Interaction.ChannelId!.Value)
            .ToHashSetAsync();

        if (activeSubsForChannel.Count is 0)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
        }
        else
        {
            await ModifyOriginalResponseAsync(p =>
            {
                p.Components = new ComponentBuilder()
                    .WithSelectMenu(SubscriptionDeleteSelectionMenuId, [.. activeSubsForChannel.Select(buildOption)], placeholder: "Choose a subscription to remove from this channel")
                    .WithButton("Nevermind", Constants.InteractionElements.CancelButtonDeleteMessage, ButtonStyle.Secondary)
                    .Build();
            }).ConfigureAwait(false);

            SelectMenuOptionBuilder buildOption(NotificationSubscription i)
            {
                return new(
                    label: MakeLabelForSubscription(i),
                    value: $"{i.Event ?? "_"}|{i.Team?.ToString() ?? "_"}|{i.GuildId?.ToString() ?? "_"}|{i.ChannelId}");
            }
        }
    }

    private string MakeLabelForSubscription(NotificationSubscription i) => $"{(i.Event is null or CommonConstants.ALL ? "All Events" : _eventsRepo[i.Event].GetLabel(shortName: true, includeYear: true))} - {(i.Team is null or CommonConstants.ALL ? "All Teams" : _teamsRepo[i.Team].GetLabel(includeLocation: false, asMarkdownLink: false))}";

    public async Task<bool> HandleInteractionAsync(IServiceProvider services, IComponentInteraction component, CancellationToken cancellationToken)
    {
        if (component.Data.CustomId is SubscriptionDeleteSelectionMenuId)
        {
            var value = Common.Throws.IfNullOrWhiteSpace(component.Data.Values.FirstOrDefault());

            var pieces = new Range[4];
            string? evt = null, team = null, guild = null;
            ulong channel = 0;
            for (int i = 0; i < value.AsSpan().Split(pieces, '|'); i++)
            {
                var pieceValue = value[pieces[i]];
                switch (i)
                {
                    case 0: evt = pieceValue; break;
                    case 1: team = pieceValue; break;
                    case 2: guild = pieceValue; break;
                    case 3: channel = ulong.Parse(pieceValue); break;
                }
            }

            if (channel is 0)
            {
                throw new ArgumentException("Invalid channel number");
            }

            var subToDelete = new NotificationSubscription(ulong.TryParse(guild, out var g) ? g : null, channel, evt, team);
            await _subscriptionManager.RemoveSubscriptionAsync(subToDelete, cancellationToken).ConfigureAwait(false);

            // Create a copy of the existing components; when updating a message, components are only settable wholesale
            var newActionRows = new ComponentBuilder()
                .WithRows(component.Message.Components.OfType<ActionRowComponent>()
                    .Select(i => new ActionRowBuilder().WithComponents([.. i.Components])));
            var rowWithSelectMenuAndOption = newActionRows.ActionRows
                .First(i => i.Components.OfType<SelectMenuComponent>().Any(j => j.Options.Any(k => k.Value == value)));
            var oldSelectMenu = (SelectMenuComponent)rowWithSelectMenuAndOption.Components
                .First(i => i.CustomId is SubscriptionDeleteSelectionMenuId);
            var indexOfOldSelectMenu = rowWithSelectMenuAndOption.Components.IndexOf(oldSelectMenu);
            Debug.Assert(indexOfOldSelectMenu is not -1);

            // Create a new menu based on the old one
            var newMenu = oldSelectMenu.ToBuilder();
            // Remove the menu option that matches the one that was selected for this interaction
            var optionRemoved = newMenu.Options.RemoveAll(i => i.Value == value);
            Debug.Assert(optionRemoved is 1);
            if (newMenu.Options.Count is 0)
            {
                // If there are no more options, we can clear out all the interactive elements
                newActionRows = null;
            }
            else
            {
                // Otherwise, update the select menu with the new options
                rowWithSelectMenuAndOption.Components[indexOfOldSelectMenu] = newMenu.Build();
            }

            try
            {
                await component.UpdateAsync(p =>
                {
                    p.Content = $"**Subscription for `{MakeLabelForSubscription(subToDelete)}` deleted.**";
                    p.Components = newActionRows?.ActionRows.SelectMany(i => i.Components).Any() is true ? newActionRows.Build() : null;
                }, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
            catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged) { }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Debug.Fail(e.Message);
                this.Logger.ErrorUpdatingTheOriginalMessageForTheDeleteSubscriptionInteraction(e);
            }

            return true;
        }

        return false;
    }
}