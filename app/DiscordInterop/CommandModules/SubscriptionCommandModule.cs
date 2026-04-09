namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Discord;
using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;

using FunctionApp;
using FunctionApp.DiscordInterop;
using FunctionApp.Storage;
using FunctionApp.Subscription;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Linq;
using System.Threading;

[Group("subscription", "Manages subscriptions to FRC events and teams")]
public sealed class SubscriptionCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<SubscriptionCommandModule>>()), IHandleUserInteractions
{
    private readonly SubscriptionManager _subscriptionManager = services.GetRequiredService<SubscriptionManager>();
    private readonly EventRepository _eventsRepo = services.GetRequiredService<EventRepository>();
    private readonly TeamRepository _teamsRepo = services.GetRequiredService<TeamRepository>();

    [SlashCommand("show", "Shows the current subscriptions")]
    public async Task ShowAsync()
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = Logger.CreateMethodScope();
        var currentSubs = await _subscriptionManager.GetSubscriptionsForGuildAsync(Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == Context.Interaction.ChannelId!.Value)
            .Select(i => (i.Event ?? CommonConstants.ALL, i.Team ?? CommonConstants.ALL))
            .ToHashSetAsync()
            .ConfigureAwait(false);

        if (currentSubs.Count is 0)
        {
            await UpdateOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
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
            await UpdateOriginalResponseAsync(p =>
                p.WithNoEmbeds($"""
                    Subscriptions for this channel include:
                    {string.Join('\n', output)}
                    """)
            ).ConfigureAwait(false);
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

        using var scope = Logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey) && string.IsNullOrWhiteSpace(teamKey))
        {
            await UpdateOriginalResponseAsync(p => p.Content = "At least one of Event or Team is required.").ConfigureAwait(false);
        }
        else
        {
            Debug.Assert(_subscriptionManager is not null);
            Debug.Assert(Context.Interaction.ChannelId.HasValue);

            try
            {
                await _subscriptionManager.SaveSubscriptionAsync(new NotificationSubscription(Context.Interaction.GuildId, Context.Interaction.ChannelId!.Value, eventKey, teamKey), default).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(eventKey) && !string.IsNullOrWhiteSpace(teamKey))
                {
                    await UpdateOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey].GetLabel(includeLocation: false)}** at the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else if (!string.IsNullOrWhiteSpace(eventKey))
                {
                    await UpdateOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to the **{_eventsRepo[eventKey].GetLabel(includeYear: true)}** event.").ConfigureAwait(false);
                }
                else
                {
                    await UpdateOriginalResponseAsync(p => p.Content = $"This channel is now subscribed to team **{_teamsRepo[teamKey!].GetLabel(includeLocation: false)}**.").ConfigureAwait(false);
                }
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Debug.Fail(e.Message);
                await DeleteResponseAsync().ConfigureAwait(false);
                await SendResponseAsync("An error occurred while creating the subscription. Please try again later.", ephemeral: true).ConfigureAwait(false);
            }
        }
    }

    private const string SubscriptionDeleteSelectionMenuId = "subscription-delete-selection";

    [RequireUserPermission(GuildPermission.ManageChannels, Group = "Permission")]
    [RequireContext(ContextType.DM, Group = "Permission")]
    [SlashCommand("delete", "Deletes a subscription to a team/event for the current channel")]
    public async Task DeleteAsync()
    {
        using var typing = await TryDeferAsync(ephemeral: Context.Channel is not IDMChannel).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = Logger.CreateMethodScope();
        var activeSubsForChannel = await _subscriptionManager.GetSubscriptionsForGuildAsync(Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == Context.Interaction.ChannelId!.Value)
            .ToArrayAsync()
            .ConfigureAwait(false);

        if (activeSubsForChannel.Length is 0)
        {
            await UpdateOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
        }
        else
        {
            await UpdateOriginalResponseAsync(p =>
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

    public async Task<bool> HandleInteractionAsync(IServiceProvider services, SocketMessageComponent component, CancellationToken cancellationToken)
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
            await _subscriptionManager.RemoveSubscriptionAsync(subToDelete, default).ConfigureAwait(false);

            // Create a copy of the existing components; when updating a message, components are only settable wholesale
            var newActionRows = new ComponentBuilder()
                .WithRows(component.Message.Components
                    .OfType<ActionRowComponent>()
                    .Select(i => new ActionRowBuilder().WithComponents([.. i.Components.Select(static j => j.ToBuilder())])));
            var rowWithSelectMenuAndOption = newActionRows.ActionRows
                .First(i => i.Components.OfType<SelectMenuBuilder>().Any(j => j.Options.Any(k => k.Value == value)));
            var oldSelectMenu = rowWithSelectMenuAndOption.Components
                .OfType<SelectMenuBuilder>()
                .First(i => i.CustomId is SubscriptionDeleteSelectionMenuId);
            var indexOfOldSelectMenu = rowWithSelectMenuAndOption.Components.IndexOf(oldSelectMenu);
            Debug.Assert(indexOfOldSelectMenu is not -1);

            // Remove the menu option that matches the one that was selected for this interaction
            var optionRemoved = oldSelectMenu.Options.RemoveAll(i => i.Value == value);
            Debug.Assert(optionRemoved is 1);
            if (oldSelectMenu.Options.Count is 0)
            {
                // If there are no more options, we can clear out all the interactive elements
                newActionRows = null;
            }

            try
            {
                await component.UpdateAsync(p =>
                {
                    p.Content = $"**Subscription for `{MakeLabelForSubscription(subToDelete)}` deleted.**";
                    p.Components = newActionRows?.ActionRows.SelectMany(i => i.Components).Any() is true ? newActionRows.Build() : null;
                }).ConfigureAwait(false);
            }
            catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged) { }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                Debug.Fail(e.Message);
                Logger.ErrorUpdatingTheOriginalMessageForTheDeleteSubscriptionInteraction(e);
            }

            return true;
        }

        return false;
    }
}
