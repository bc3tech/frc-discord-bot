namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Subscription;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

[Group("subscription", "Manages subscriptions to FRC events and teams")]
public sealed class SubscriptionCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<SubscriptionCommandModule>>())
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

        using var scope = this.Logger.CreateMethodScope();
        HashSet<(string, ushort?)> currentSubs = [];
        await foreach (var sub in _subscriptionManager.GetSubscriptionsForGuildAsync(this.Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == this.Context.Interaction.ChannelId!.Value))
        {
            currentSubs.Add((sub.Event ?? CommonConstants.ALL, sub.Team));
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
            var output = groupedSubscriptions.Select(i => $"""
                - **{(i.Key is not CommonConstants.ALL ? _eventsRepo[i.Key].GetLabel() : "All Events")}**:
                {string.Join('\n', i.Select(j => $"  - {(j.Item2.HasValue ? _teamsRepo[j.Item2.Value].GetLabel() : "All Teams")}"))}
                """);
            await this.ModifyOriginalResponseAsync(p =>
            {
                p.Content = $"""
                Subscriptions for this channel include:
                {string.Join('\n', output)}
                """;
                p.Flags = MessageFlags.SuppressEmbeds;
            }).ConfigureAwait(false);
        }
    }

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
            await this.ModifyOriginalResponseAsync(p => p.Content = "At least one of Event or Team is required.").ConfigureAwait(false);
        }
        else
        {
            Debug.Assert(_subscriptionManager is not null);
            Debug.Assert(this.Context.Interaction.ChannelId.HasValue);

            var teamNumber = teamKey.TeamKeyToTeamNumber();

            try
            {
                await _subscriptionManager.SaveSubscriptionAsync(new NotificationSubscription(this.Context.Interaction.GuildId, this.Context.Interaction.ChannelId!.Value, eventKey, teamNumber), default).ConfigureAwait(false);
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

    private const string SubscriptionDeleteSelectionMenuId = "subscription-delete-selection";

    [SlashCommand("delete", "Deletes a subscription to a team/event for the current channel")]
    public async Task DeleteAsync()
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using var scope = this.Logger.CreateMethodScope();
        var activeSubsForChannel = await _subscriptionManager.GetSubscriptionsForGuildAsync(this.Context.Interaction.GuildId, default)
            .Where(i => i.ChannelId == this.Context.Interaction.ChannelId!.Value)
            .ToArrayAsync();

        if (activeSubsForChannel.Length is 0)
        {
            await this.ModifyOriginalResponseAsync(p => p.Content = "No subscriptions found for this channel.").ConfigureAwait(false);
        }
        else
        {
            await this.ModifyOriginalResponseAsync(p =>
            {
                p.Components = new ComponentBuilder()
                    .WithSelectMenu(SubscriptionDeleteSelectionMenuId, [.. activeSubsForChannel.Select(buildOption)], placeholder: "Choose a subscription to remove from this channel")
                    .WithButton("Nevermind", Constants.InteractionElements.CancelButtonDeleteMessage, ButtonStyle.Secondary)
                    .Build();
            }).ConfigureAwait(false);

            SelectMenuOptionBuilder buildOption(NotificationSubscription i)
            {
                return new(
                    label: MakeLabelForSubscription(i, _eventsRepo, _teamsRepo),
                    value: $"{i.Event ?? "_"}|{i.Team?.ToString() ?? "_"}|{i.GuildId?.ToString() ?? "_"}|{i.ChannelId}");
            }
        }
    }

    internal static string MakeLabelForSubscription(NotificationSubscription i, EventRepository events, TeamRepository teams) => $"{(i.Event is not null and not CommonConstants.ALL ? events[i.Event].GetLabel(shortName: true, includeYear: true) : "All Events")} - {(i.Team is null or 0 ? "All Teams" : teams[i.Team.Value].GetLabel(includeLocation: false, asMarkdownLink: false))}";

    internal static async Task<bool> HandleMenuSelectionAsync(IServiceProvider services, SocketMessageComponent menuSelection)
    {
        SubscriptionManager subscriptionManager = services.GetRequiredService<SubscriptionManager>();
        var value = Common.Throws.IfNullOrWhiteSpace(menuSelection.Data.Values.FirstOrDefault());

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

        var subToDelete = new NotificationSubscription(ulong.TryParse(guild, out var g) ? g : null, channel, evt, ushort.TryParse(team, out var t) ? t : null);
        await subscriptionManager.RemoveSubscriptionAsync(subToDelete, default);

        EventRepository events = services.GetRequiredService<EventRepository>();
        TeamRepository teams = services.GetRequiredService<TeamRepository>();

        // Create a copy of the existing components; when updating a message, components are only settable wholesale
        var newActionRows = new ComponentBuilder()
            .WithRows(menuSelection.Message.Components
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
        Debug.Assert(newMenu.Options.RemoveAll(i => i.Value == value) is 1);
        rowWithSelectMenuAndOption.Components[indexOfOldSelectMenu] = newMenu.Build();

        try
        {
            await menuSelection.UpdateAsync(p =>
            {
                p.Content = $"**Subscription for `{MakeLabelForSubscription(subToDelete, events, teams)}` deleted.**";
                p.Components = newActionRows.Build();
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
            services.GetService<ILogger<SubscriptionCommandModule>>().ErrorUpdatingTheOriginalMessageForTheDeleteSubscriptionInteraction(e);
        }

        return true;
    }
}