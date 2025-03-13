namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Common.Extensions;

using Discord;
using Discord.Interactions;

using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Text;

[Group("events", "Gets information about FRC events")]
public sealed class EventsCommandModule(IServiceProvider services) : CommandModuleBase
{
    private readonly IEmbedCreator<string> _embedCreator = services.GetRequiredKeyedService<IEmbedCreator<string>>(nameof(EventDetail));
    private readonly ILogger _logger = services.GetRequiredService<ILogger<EventsCommandModule>>();

    [SlashCommand("get-details", "Gets details about an event")]
    public async Task ShowAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using IDisposable scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        await GenerateResponseAsync(_embedCreator, eventKey).ConfigureAwait(false);
    }

    [SlashCommand("add", "Adds an FRC event to this Discord team as an Event for people to subscribe to, etc.")]
    public async Task AddEventAsync(
        [Summary("event"), Autocomplete(typeof(AutoCompleteHandlers.EventsAutoCompleteHandler))] string eventKey,
        [Summary("title", "Title of the event, otherwise will match what's on FIRST website")] string? title = null,
        [Summary("description", "Description of the event, otherwise will be blank")] string? description = null,
        [Summary("channel", "The channel where users can chat about the event")] IMessageChannel? channel = null,
        [Summary("post", "`true` to post response publicly")] bool post = false)
    {
        await DeferAsync(ephemeral: !post).ConfigureAwait(false);

        using IDisposable scope = _logger.CreateMethodScope();
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            await ModifyOriginalResponseAsync(p => p.Content = "Event key is required.").ConfigureAwait(false);
            return;
        }

        if (Context.Guild is null)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "This command can only be used in a server.").ConfigureAwait(false);
            return;
        }

        IGuildUser invokingUser = await Context.Guild.GetUserAsync(Context.User.Id);
        if (!invokingUser.GuildPermissions.CreateEvents)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "You do not have permission to create events in this server.").ConfigureAwait(false);
            return;
        }

        IGuildUser botUserOnGuild = await Context.Guild.GetCurrentUserAsync().ConfigureAwait(false);
        if (!botUserOnGuild.GuildPermissions.ManageEvents)
        {
            await ModifyOriginalResponseAsync(p => p.Content = "I do not have permission to create events in this server. Talk to your admin about granting me this!").ConfigureAwait(false);
            return;
        }

        try
        {
            EventRepository eventRepo = services.GetRequiredService<EventRepository>();
            TheBlueAlliance.Model.Event targetEvent = eventRepo[eventKey];
            TimeZoneInfo eventTimezone = TimeZoneInfo.TryFindSystemTimeZoneById(targetEvent.Timezone, out TimeZoneInfo? z) && z is not null ? z : TimeZoneInfo.Utc;
            DateTime startDateTime = targetEvent.StartDate.ToDateTime(new(8, 0));
            TimeSpan eventTimezoneUtcOffset = eventTimezone.GetUtcOffset(startDateTime);
            DateTimeOffset startOffset = new(startDateTime, eventTimezoneUtcOffset);
            DateTimeOffset endOffset = new(targetEvent.EndDate, new TimeOnly(17, 0), eventTimezoneUtcOffset);

            string locationValue = channel is not null ? $"https://discord.com/channels/{Context.Guild.Id}/{channel.Id}" : targetEvent.LocationString;

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

            var guildEvent = await Context.Guild.CreateEventAsync(!string.IsNullOrWhiteSpace(title) ? title : targetEvent.Name, startOffset, GuildScheduledEventType.External, description: descriptionBuilder.ToString(), endTime: endOffset, location: locationValue);
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
        catch (Exception ex)
        {
            _logger.ThereWasAnErrorCreatingAGuildEventForEventKeyInGuildGuildNameGuildId(ex, eventKey, Context.Guild.Name, Context.Guild.Id);
            await ModifyOriginalResponseAsync(p => p.Content = "An error occurred while creating the event. Try again or contact your admin to investigate.").ConfigureAwait(false);
        }
    }
}
