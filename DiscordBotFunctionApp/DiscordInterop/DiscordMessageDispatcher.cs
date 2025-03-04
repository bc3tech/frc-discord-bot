namespace DiscordBotFunctionApp.DiscordInterop;

using Azure;
using Azure.Data.Tables;

using Common.Extensions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Storage.TableEntities;
using DiscordBotFunctionApp.TbaInterop.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

internal sealed partial class DiscordMessageDispatcher(
    EventRepository events,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptionsTable,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptionsTable,
    [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient threadsTable,
    DiscordSocketClient _discordClient, WebhookEmbeddingGenerator _embedGenerator,
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
        var discordRequestOptions = Utility.CreateCancelRequestOptions(cancellationToken);
        if (await embeds.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            Embed[]? embedsToSend = null;
            // check to see if there are any threads already created for this message
            var threadLocator = message.GetThreadDetails();
            List<ulong?> channelsWhereWeAlreadyPostedIntoThreads = [];
            if (threadLocator is not null)
            {
                var (pk, rk, title) = threadLocator.Value;
                var entity = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(pk, rk, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (entity.HasValue && entity.Value is not null)
                {
                    embedsToSend = await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                    foreach (var (chanId, threadId) in entity.Value.ThreadIdList)
                    {
                        await ((IMessageChannel)_discordClient.GetChannel(threadId))
                            .SendMessageAsync(embeds: embedsToSend, options: discordRequestOptions).ConfigureAwait(false);
                        channelsWhereWeAlreadyPostedIntoThreads.Add(chanId);
                    }
                }
            }

            foreach (var subscriberChannelId in subscribers.SelectMany(i => i.Value))
            {
                if (channelsWhereWeAlreadyPostedIntoThreads.Contains(subscriberChannelId))
                {
                    logger.SkippingPostingToSubscriberChannelIdBecauseWeAlreadyPostedToAThreadInThatChannel(subscriberChannelId);
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();
                var targetChannel = await _discordClient.GetChannelAsync(subscriberChannelId, discordRequestOptions).ConfigureAwait(false);
                Debug.Assert(targetChannel is not null);
                logger.RetrievedChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);

                if (targetChannel is IMessageChannel msgChan)
                {
                    logger.SendingNotificationToChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);
                    var embedChunks = (await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false)).Chunk(MAX_EMBEDS_PER_MESSAGE);
                    var threadForMessage = await createThreadForMessageAsync();
                    foreach (var i in embedChunks)
                    {
                        await threadForMessage.SendMessageAsync(embeds: i, options: discordRequestOptions).ConfigureAwait(false);
                    }

                    logger.LogMetric("NotificationSent", 1, new Dictionary<string, object>() { { "ChannelId", subscriberChannelId }, { "ChannelName", targetChannel.Name } });

                    async Task<IMessageChannel> createThreadForMessageAsync()
                    {
                        var threadDetails = message.GetThreadDetails();
                        if (threadDetails is null || !threadDetails.HasValue)
                        {
                            return msgChan;
                        }

                        if (_discordClient.GetChannel(msgChan.Id) is ITextChannel threadableChannel)
                        {
                            var newThread = await threadableChannel.CreateThreadAsync(threadDetails.Value.Title);
                            var tableResponse = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(threadDetails.Value.PartitionKey, threadDetails.Value.RowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
                            ThreadTableEntity tableEntity = tableResponse is not null && tableResponse.HasValue
                                ? tableResponse.Value!
                                : new()
                                {
                                    PartitionKey = threadDetails.Value.PartitionKey,
                                    RowKey = threadDetails.Value.RowKey,
                                };
                            tableEntity.ThreadIdList.Add(new(msgChan.Id, newThread.Id));
                            await threadsTable.UpsertEntityAsync(tableEntity, mode: TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
                            return newThread;
                        }

                        return msgChan;
                    }
                }
                else
                {
                    logger.ChannelChannelIdIsNotAMessageChannel(subscriberChannelId);
                }
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

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
record ThreadTableEntity() : ITableEntity
{
    internal ThreadTableEntity(IEnumerable<ThreadDetail> threadIds) : this() => this.ThreadIdList = [.. threadIds];

    public DateTimeOffset? Timestamp { get; set; } = TimeProvider.System.GetUtcNow();
    public ETag ETag { get; set; } = ETag.All;

    required public string PartitionKey { get; set; }
    required public string RowKey { get; set; }

    public string ThreadIds
    {
        get => JsonSerializer.Serialize(ThreadIdList);
        set => ThreadIdList = JsonSerializer.Deserialize<List<ThreadDetail>>(value) ?? [];
    }

    [JsonIgnore]
    internal List<ThreadDetail> ThreadIdList { get; private set; } = [];

    public record struct ThreadDetail([property: JsonIgnore] ulong ChannelId, [property: JsonIgnore] ulong ThreadId)
    {
        // We have to store these as strings because Table SDK doesn't support ulong types
        public string Channel
        {
            readonly get => ChannelId.ToString();
            set => ChannelId = ulong.Parse(value);
        }

        public string Thread
        {
            readonly get => ThreadId.ToString();
            set => ThreadId = ulong.Parse(value);
        }
    }
}
