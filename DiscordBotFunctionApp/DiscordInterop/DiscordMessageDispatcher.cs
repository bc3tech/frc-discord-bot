namespace DiscordBotFunctionApp.DiscordInterop;

using Azure.Data.Tables;

using Common.Extensions;
using Common.Tba;
using Common.Tba.Notifications;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.Storage.TableEntities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;

using MatchScoreEmbed = Embeds.MatchScore;

internal sealed partial class DiscordMessageDispatcher([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
                                        [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
                                        DiscordSocketClient _discordClient,
                                        ILogger<DiscordMessageDispatcher> logger)
{
    public async Task<bool> ProcessWebhookMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
    {
        (var teams, var events) = GetTeamsAndEventsInMessage(message.MessageData);
        logger.LogDebug("Teams: {TeamsInMessage}, Events: {EventsInMessage}", teams.Count, events.Count);

        var teamRecordsToFind = teams.Permutate([CommonConstants.ALL, .. events], (p, r) => (p, r)).ToArray().AsReadOnly();
        var eventRecordsToFind = events.Permutate([CommonConstants.ALL, .. teams], (p, r) => (p, r)).ToArray().AsReadOnly();

        logger.LogTrace("Permutated results into {TeamRecordsCount} team records and {EventRecordsCount} event records", teamRecordsToFind.Count, eventRecordsToFind.Count);

        List<Task> notifications = [];

        await sendNotificationsAsync<TeamSubscriptionEntity>(teamSubscriptions, teamRecordsToFind, notifications, cancellationToken);
        await sendNotificationsAsync<EventSubscriptionEntity>(eventSubscriptions, eventRecordsToFind, notifications, cancellationToken);

        logger.LogInformation("Waiting for notifications...");

        await Task.WhenAll(notifications).WithCancellation(cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Notifications completed");

        return true;

        async Task sendNotificationsAsync<T>(TableClient sourceTable, IReadOnlyList<(string p, string r)> records, List<Task> notifications, CancellationToken ct) where T : class, ITableEntity, ISubscriptionEntity
        {
            foreach (var i in records)
            {
                ct.ThrowIfCancellationRequested();
                logger.LogTrace("Checking {TargetTable} for {PartitionKey} / {RowKey} ...", sourceTable.Name, i.p, i.r);
                var sub = await getSubscriptionForAsync(sourceTable, i.p, i.r, cancellationToken);
                if (sub is not null)
                {
                    logger.LogTrace("Found record for {TargetTable} for {PartitionKey} / {RowKey}", sourceTable.Name, i.p, i.r);
                    notifications.Add(ProcessSubscriptionAsync(message, sub.Subscribers, cancellationToken));
                }
            }

            static async Task<T?> getSubscriptionForAsync(TableClient sourceTable, string partitionKey, string rowKey, CancellationToken ct)
            {
                var subscription = await sourceTable.GetEntityIfExistsAsync<T>(partitionKey, rowKey, cancellationToken: ct).ConfigureAwait(false);
                return subscription.HasValue ? subscription.Value : null;
            }
        }
    }

    private async Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, CancellationToken cancellationToken)
    {
        var embed = MatchScoreEmbed.Create(message.GetDataAs<MatchScore>()!);

        foreach (var i in subscribers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var targetGuild = _discordClient!.GetGuild(GuildSubscriptions.DiscordGuildId(i.Key));
            Debug.Assert(targetGuild is not null);
            logger.LogTrace("Retrieved guild {GuildId} from Discord.", i.Key);
            foreach (var c in i.Value)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var targetChannel = targetGuild.GetTextChannel(c);
                Debug.Assert(targetChannel is not null);
                logger.LogTrace("Retrieved channel {ChannelId} from guild {GuildId}", c, i.Key);

                await targetChannel.SendMessageAsync(embed: embed, options: new RequestOptions { CancelToken = cancellationToken }).ConfigureAwait(false);
            }
        }

        await Task.CompletedTask;
    }

    private static (IReadOnlySet<string> Teams, IReadOnlySet<string> Events) GetTeamsAndEventsInMessage(JsonElement messageData)
    {
        HashSet<string> teams = [], events = [];

        foreach (var propertyName in new[] { "team_key", "teams", "team_keys" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out var properties) && properties is not null)
            {
                foreach (var propertyValue in properties)
                {
                    addElement(propertyValue, teams, s => s.ToTeamNumber().ToString());
                }
            }
        }

        foreach (var propertyName in new[] { "event_key", "events", "event_keys" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out var properties) && properties is not null)
            {
                foreach (var propertyValue in properties)
                {
                    addElement(propertyValue, events);
                }
            }
        }

        return (teams, events);

        static void addElement(JsonElement? maybeNullElt, HashSet<string> toHashSet, Func<string, string>? transform = null)
        {
            if (maybeNullElt.HasValue)
            {
                var elt = maybeNullElt.Value;
                string? s;
                if (elt.ValueKind is JsonValueKind.Array)
                {
                    foreach (var i in elt.EnumerateArray())
                    {
                        s = i.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            toHashSet.Add(transform?.Invoke(s) ?? s!);
                        }
                    }
                }
                else if ((s = elt.GetString()) is not null)
                {
                    toHashSet.Add(transform?.Invoke(s) ?? s!);
                }
            }
        }
    }
}

