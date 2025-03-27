namespace FunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;

using FunctionApp.DiscordInterop;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text;

using TheBlueAlliance.Interfaces.Caching;

[Group("events", "Gets information about FRC events")]
public sealed class EventsCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<EventsCommandModule>>())
{
    private readonly IEmbedCreator<string> _detailEmbedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(EventDetail));
    private readonly IEmbedCreator<(string?, ushort)> _scheduleEmbedCreator = services.GetRequiredKeyedService<IEmbedCreator<(string?, ushort)>>(nameof(Schedule));

    [SlashCommand("get-details", "Gets details about an event")]
    public async Task GetDetailsAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using IDisposable scope = this.Logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        await GenerateResponseAsync(_detailEmbedCreator, eventKey).ConfigureAwait(false);
    }

    [CommandContextType(InteractionContextType.Guild)]
    [RequireUserPermission(GuildPermission.CreateEvents)]
    [SlashCommand("add", "Adds an FRC event to this Discord team as an Event for people to subscribe to, etc.")]
    public async Task AddEventAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("title", "Title of the event, otherwise will match what's on FIRST website")] string? title = null,
        [Summary("description", "Description of the event, otherwise will be blank")] string? description = null,
        [Summary("channel", "The channel where users can chat about the event")] IMessageChannel? channel = null,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using IDisposable scope = this.Logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        if (this.Context.Guild is null)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "This command can only be used in a server.").ConfigureAwait(false);
            return;
        }

        IGuildUser invokingUser = await this.Context.Guild.GetUserAsync(this.Context.User.Id);
        if (!invokingUser.GuildPermissions.CreateEvents)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "You do not have permission to create events in this server.").ConfigureAwait(false);
            return;
        }

        IGuildUser botUserOnGuild = await this.Context.Guild.GetCurrentUserAsync().ConfigureAwait(false);
        if (!botUserOnGuild.GuildPermissions.ManageEvents)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "I do not have permission to create events in this server. Talk to your admin about granting me this!").ConfigureAwait(false);
            return;
        }

        try
        {
            var eventRepo = services.GetRequiredService<IEventCache>();
            TheBlueAlliance.Model.Event targetEvent = eventRepo[eventKey];
            TimeZoneInfo eventTimezone = TimeZoneInfo.TryFindSystemTimeZoneById(targetEvent.Timezone, out TimeZoneInfo? z) && z is not null ? z : TimeZoneInfo.Utc;
            DateTime startDateTime = targetEvent.StartDate.ToDateTime(new(8, 0));
            TimeSpan eventTimezoneUtcOffset = eventTimezone.GetUtcOffset(startDateTime);
            DateTimeOffset startOffset = new(startDateTime, eventTimezoneUtcOffset);
            DateTimeOffset endOffset = new(targetEvent.EndDate, new TimeOnly(17, 0), eventTimezoneUtcOffset);

            string locationValue = channel is not null ? $"https://discord.com/channels/{this.Context.Guild.Id}/{channel.Id}" : targetEvent.LocationString;

            StringBuilder descriptionBuilder = new(description is not null ? description : $"{targetEvent.Year} Season - Week {targetEvent.Week.GetValueOrDefault(-1) + 1}");
            descriptionBuilder.AppendLine().AppendLine()
                .Append("Event location: ");
            if (!string.IsNullOrWhiteSpace(targetEvent.GmapsUrl))
            {
                descriptionBuilder.AppendLine($"[{targetEvent.LocationString}]({targetEvent.GmapsUrl})");
            }
            else
            {
                descriptionBuilder.AppendLine(targetEvent.LocationString);
            }

            descriptionBuilder.AppendLine($"[Schedule]({targetEvent.ScheduleUrl})");
            descriptionBuilder.AppendLine($"Results and more at: {targetEvent.TbaUrl}");

            var guildEvent = await this.Context.Guild.CreateEventAsync(!string.IsNullOrWhiteSpace(title) ? title : targetEvent.Name, startOffset, GuildScheduledEventType.External, description: descriptionBuilder.ToString(), endTime: endOffset, location: locationValue);
            var eventLink = $"https://discord.com/events/{guildEvent.GuildId}/{guildEvent.Id}";

            await ModifyOriginalResponseAsync(p => p.Content = $@"[Event created]({eventLink}){(post ? " and link posted" : string.Empty)}!").ConfigureAwait(false);
            if (post)
            {
                if (channel is not null)
                {
                    await channel.SendMessageAsync(eventLink).ConfigureAwait(false);
                }
                else
                {
                    await this.Context.Channel.SendMessageAsync(eventLink).ConfigureAwait(false);
                }
            }
        }
        catch (KeyNotFoundException)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event not found.").ConfigureAwait(false);
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            this.Logger.ThereWasAnErrorCreatingAGuildEventForEventKeyInGuildGuildNameGuildId(e, eventKey, this.Context.Guild.Name, this.Context.Guild.Id);
            await ModifyOriginalResponseAsync(p => p.Content = "An error occurred while creating the event. Try again or contact your admin to investigate.").ConfigureAwait(false);
        }
    }

    [SlashCommand("schedule", "Gets the schedule for an event and, optionally, a team at the event")]
    public async Task GetScheduleAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("team"), Autocomplete(typeof(AutoCompleteHandlers.TeamsAutoCompleteHandler))] string? teamKey = null,
        [Summary("num-matches")] ushort numMatches = 6,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        using var typing = await TryDeferAsync(!post).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        using IDisposable scope = this.Logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        await GenerateResponseAsync(_scheduleEmbedCreator, (eventKey, numMatches), teamKey.TeamKeyToTeamNumber()).ConfigureAwait(false);
    }
}
