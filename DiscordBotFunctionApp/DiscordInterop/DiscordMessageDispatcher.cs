namespace DiscordBotFunctionApp.DiscordInterop;

using Azure;
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

internal sealed partial class DiscordMessageDispatcher([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptionsTable,
                                        [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptionsTable,
                                        [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient threadsTable,
                                        DiscordSocketClient _discordClient,
                                        WebhookEmbeddingGenerator _embedGenerator,
                                        ILogger<DiscordMessageDispatcher> logger)
{
    public async Task<bool> ProcessWebhookMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        (var teams, var events) = GetTeamsAndEventsInMessage(message.MessageData);
        logger.TeamsTeamsInMessageEventsEventsInMessage(teams.Count, events.Count);

        var teamRecordsToFind = teams.Permutate([CommonConstants.ALL, .. events], (p, r) => (p, r)).ToArray().AsReadOnly();
        var eventRecordsToFind = events.Permutate([CommonConstants.ALL, .. teams], (p, r) => (p, r)).ToArray().AsReadOnly();

        logger.PermutatedResultsIntoTeamRecordsCountTeamRecordsAndEventRecordsCountEventRecords(teamRecordsToFind.Count, eventRecordsToFind.Count);

        List<Task> notifications = [];

        await SendNotificationsAsync<TeamSubscriptionEntity>(teamSubscriptionsTable, teamRecordsToFind, i => i.Item1 is not CommonConstants.ALL ? i.Item1.ToTeamNumber() : null, logger, message, cancellationToken).ConfigureAwait(false);
        await SendNotificationsAsync<EventSubscriptionEntity>(eventSubscriptionsTable, eventRecordsToFind, i => i.Item2 is not CommonConstants.ALL ? i.Item2.ToTeamNumber() : null, logger, message, cancellationToken).ConfigureAwait(false);

        logger.WaitingForNotificationsToBeSent();

        await Task.WhenAll(notifications).ConfigureAwait(false);

        return true;
    }

    private async Task SendNotificationsAsync<T>(TableClient sourceTable, IReadOnlyList<(string p, string r)> records, Func<(string, string), ushort?> teamFinder, ILogger<DiscordMessageDispatcher> logger, WebhookMessage message, CancellationToken cancellationToken) where T : class, ITableEntity, ISubscriptionEntity
    {
        using var scope = logger.CreateMethodScope();
        foreach (var i in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.CheckingTargetTableForPartitionKeyRowKey(sourceTable.Name, i.p, i.r);
            var sub = await getSubscriptionForAsync(sourceTable, i.p, i.r, cancellationToken).ConfigureAwait(false);
            if (sub is not null)
            {
                logger.FoundRecordForTargetTableForPartitionKeyRowKey(sourceTable.Name, i.p, i.r);
                await ProcessSubscriptionAsync(message, sub.Subscribers, teamFinder(i), cancellationToken);
            }
        }

        static async Task<T?> getSubscriptionForAsync(TableClient sourceTable, string partitionKey, string rowKey, CancellationToken ct)
        {
            var subscription = await sourceTable.GetEntityIfExistsAsync<T>(partitionKey, rowKey, cancellationToken: ct).ConfigureAwait(false);
            return subscription.HasValue ? subscription.Value : null;
        }
    }

    private const ushort MAX_EMBEDS_PER_MESSAGE = 10;

    private async Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, ushort? highlightTeam, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        var embeds = _embedGenerator.CreateEmbeddingsAsync(message, highlightTeam, cancellationToken: cancellationToken);
        if (await embeds.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            foreach (var c in subscribers.SelectMany(i => i.Value))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var targetChannel = await _discordClient.GetChannelAsync(c, Utility.CreateCancelRequestOptions(cancellationToken)).ConfigureAwait(false);
                Debug.Assert(targetChannel is not null);
                logger.RetrievedChannelChannelIdChannelName(c, targetChannel.Name);

                if (targetChannel is IMessageChannel msgChan)
                {
                    logger.SendingNotificationToChannelChannelIdChannelName(c, targetChannel.Name);
                    var embedChunks = (await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false)).Chunk(MAX_EMBEDS_PER_MESSAGE);
                    MessageReference? initialMessageForThread = message.ThreadReplies() ? await getThreadIdForMessageAsync() : null;
                    foreach (var i in embedChunks)
                    {
                        var createdMessage = await msgChan.SendMessageAsync(embeds: i, options: new RequestOptions { CancelToken = cancellationToken }, messageReference: initialMessageForThread).ConfigureAwait(false);
                        if (initialMessageForThread is null && message.ThreadReplies())
                        {
                            initialMessageForThread = new MessageReference(createdMessage.Id);
                            await saveMessageIdAsync(initialMessageForThread);
                        }
                    }

                    logger.LogMetric("NotificationSent", 1, new Dictionary<string, object>() { { "ChannelId", c }, { "ChannelName", targetChannel.Name } });
                }
                else
                {
                    logger.ChannelChannelIdIsNotAMessageChannel(c);
                }
            }
        }

        async Task<MessageReference?> getThreadIdForMessageAsync()
        {
            var threadLocator = message.GetThreadLocator();
            if (threadLocator is null)
            {
                return null;
            }

            var (pk, rk) = threadLocator.Value;
            var entity = await threadsTable.GetEntityIfExistsAsync<TableEntity>(pk, rk, cancellationToken: cancellationToken).ConfigureAwait(false);
            var messageId = entity.HasValue is true ? entity.Value!.GetInt64("MessageId") : null;
            return messageId is null ? null : new MessageReference((ulong)messageId);
        }

        async Task saveMessageIdAsync(MessageReference threadId)
        {
            var threadLocator = message.GetThreadLocator();
            if (threadLocator is null)
            {
                logger.LogWarning("Attempted to save thread ID for message that didn't return a locator value.");
                return;
            }

            var (pk, rk) = threadLocator.Value;
            var entity = new TableEntity(pk, rk) { { "MessageId", threadId.MessageId.Value } };
            try
            {
                await threadsTable.AddEntityAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex) when (ex.Status is 409)
            {
                logger.LogWarning("Thread ID for message already existed in table. Locator: {ThreadLocator}", threadLocator);
            }
        }
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

