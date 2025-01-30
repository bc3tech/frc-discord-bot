namespace DiscordBotFunctionApp.DiscordInterop;

using Azure.Data.Tables;

using Common.Extensions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Storage.TableEntities;
using DiscordBotFunctionApp.TbaInterop.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;

internal sealed partial class DiscordMessageDispatcher([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptions,
                                        [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptions,
                                        DiscordSocketClient _discordClient,
                                        WebhookEmbeddingGenerator _embedGenerator,
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

        await sendNotificationsAsync<TeamSubscriptionEntity>(teamSubscriptions, teamRecordsToFind, notifications, i => i.Item1 != CommonConstants.ALL ? i.Item1.ToTeamNumber() : null, cancellationToken).ConfigureAwait(false);
        await sendNotificationsAsync<EventSubscriptionEntity>(eventSubscriptions, eventRecordsToFind, notifications, i => i.Item2 != CommonConstants.ALL ? i.Item2.ToTeamNumber() : null, cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Waiting for notifications...");

        await Task.WhenAll(notifications).ConfigureAwait(false);

        logger.LogInformation("Notifications completed");

        return true;

        async Task sendNotificationsAsync<T>(TableClient sourceTable, IReadOnlyList<(string p, string r)> records, List<Task> notifications, Func<(string, string), ushort?> teamFinder, CancellationToken ct) where T : class, ITableEntity, ISubscriptionEntity
        {
            foreach (var i in records)
            {
                ct.ThrowIfCancellationRequested();
                logger.LogTrace("Checking {TargetTable} for {PartitionKey} / {RowKey} ...", sourceTable.Name, i.p, i.r);
                var sub = await getSubscriptionForAsync(sourceTable, i.p, i.r, cancellationToken).ConfigureAwait(false);
                if (sub is not null)
                {
                    logger.LogTrace("Found record for {TargetTable} for {PartitionKey} / {RowKey}", sourceTable.Name, i.p, i.r);
                    notifications.Add(ProcessSubscriptionAsync(message, sub.Subscribers, teamFinder(i), cancellationToken));
                }
            }

            static async Task<T?> getSubscriptionForAsync(TableClient sourceTable, string partitionKey, string rowKey, CancellationToken ct)
            {
                var subscription = await sourceTable.GetEntityIfExistsAsync<T>(partitionKey, rowKey, cancellationToken: ct).ConfigureAwait(false);
                return subscription.HasValue ? subscription.Value : null;
            }
        }
    }

    private async Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, ushort? highlightTeam, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        var embeds = _embedGenerator.CreateEmbeddingsAsync(message, highlightTeam, cancellationToken: cancellationToken);

        foreach (var c in subscribers.SelectMany(i => i.Value))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var targetChannel = await _discordClient.GetChannelAsync(c, Utility.CreateCancelRequestOptions(cancellationToken)).ConfigureAwait(false);
            Debug.Assert(targetChannel is not null);
            logger.LogTrace("Retrieved channel {ChannelId} - '{ChannelName}'", c, targetChannel.Name);

            if (targetChannel is IMessageChannel msgChan)
            {
                await msgChan.SendMessageAsync(embeds: await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false), options: new RequestOptions { CancelToken = cancellationToken }).ConfigureAwait(false);
            }
            else
            {
                logger.LogWarning("Channel {ChannelId} is not a message channel", c);
            }
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private static (IReadOnlySet<string> Teams, IReadOnlySet<string> Events) GetTeamsAndEventsInMessage(JsonElement messageData)
    {
        HashSet<string> teams = [], events = [];

        foreach (var propertyName in new[] { "team", "team_key", "teams", "team_keys", "picks", "declines", "team_number" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out var properties) && properties is not null)
            {
                foreach (var propertyValue in properties)
                {
                    addElement(propertyValue, teams, s => s.ToTeamNumber().ToString()!);
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
                else if ((s = elt.ToString()) is not null)
                {
                    toHashSet.Add(transform?.Invoke(s) ?? s!);
                }
            }
        }
    }
}

