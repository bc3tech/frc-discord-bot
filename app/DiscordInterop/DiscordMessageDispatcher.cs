namespace FunctionApp.DiscordInterop;

using Azure;
using Azure.Data.Tables;

using Common.Discord;
using Common.Extensions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using FunctionApp;
using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.Storage.TableEntities;
using FunctionApp.TbaInterop.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using TheBlueAlliance.Api;

internal partial class DiscordMessageDispatcher(IWebhookSubscriptionStore<TeamSubscriptionEntity> teamSubscriptionsTable,
                                                       IWebhookSubscriptionStore<EventSubscriptionEntity> eventSubscriptionsTable,
                                                       [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient threadsTable,
                                                       IEventApi eventApi,
                                                       IDiscordClient discordClient,
                                                       WebhookEmbeddingGenerator _embedGenerator,
                                                       TimeProvider time,
                                                       Meter meter,
                                                       IServiceProvider allServices,
                                                       ILogger<DiscordMessageDispatcher> logger)
{
    private readonly DiscordSocketClient _discordClient = discordClient as DiscordSocketClient ?? throw new ArgumentException(nameof(discordClient));

    public async Task<bool> ProcessWebhookMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        using IDisposable scope = logger.CreateMethodScope();
        (ImmutableHashSet<string>? teams, ImmutableHashSet<string>? events) = GetTeamsAndEventsInMessage(message.MessageData);
        logger.TeamsTeamsInMessageEventsEventsInMessage(teams.Count, events.Count);

        if (message.IsBroadcast)
        {
            logger.BroadcastMessageDetectedFetchingAllTeamsAtTheEvent();
            // If we're broadcasting the message to everybody at the event, then we need to include every team at the event as a possible subscriber.
            foreach (var e in events)
            {
                Collection<string>? eventTeams = await eventApi.GetEventTeamsKeysAsync(e, cancellationToken: cancellationToken).ConfigureAwait(false);
                teams = [.. teams, .. eventTeams ?? []];
            }
        }

        var teamRecordsToFind = teams.Permutate([CommonConstants.ALL, .. events], (p, r) => (p, r)).ToImmutableHashSet();
        var eventRecordsToFind = events.Select(i => (i, CommonConstants.ALL)).ToImmutableHashSet();

        logger.PermutatedResultsIntoTeamRecordsCountTeamRecordsAndEventRecordsCountEventRecords(teamRecordsToFind.Count, eventRecordsToFind.Count);

        List<Task> notifications = [
            SendNotificationsAsync<TeamSubscriptionEntity>(message, teamSubscriptionsTable, teamRecordsToFind, i => i.Item1 is not CommonConstants.ALL ? i.Item1.TeamKeyToTeamNumber() : null, logger, cancellationToken),
            SendNotificationsAsync<EventSubscriptionEntity>(message, eventSubscriptionsTable, eventRecordsToFind, i => i.Item2 is not CommonConstants.ALL ? i.Item2.TeamKeyToTeamNumber() : null, logger, cancellationToken)
        ];

        logger.WaitingForNotificationsToBeSent();

        await Task.WhenAll(notifications).ConfigureAwait(false);

        logger.AllNotificationsDispatched();
        meter.LogMetric("NotificationDispatchTimeSec", time.GetElapsedTime(startTime).TotalSeconds);

        return true;
    }

    private static readonly Func<ILogger, string, string, IDisposable?> _notificationLoggingScope = LoggerMessage.DefineScope<string, string>("Subscription: {PartitionKey}/{RowKey}");
    private async Task SendNotificationsAsync<T>(WebhookMessage message, IWebhookSubscriptionStore<T> sourceTable, IEnumerable<(string p, string r)> records, Func<(string, string), ushort?> teamFinder, ILogger<DiscordMessageDispatcher> logger, CancellationToken cancellationToken)
        where T : class, ISubscriptionEntity
    {
        foreach ((string p, string r) i in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using IDisposable? subscriptionScope = _notificationLoggingScope(logger, i.p, i.r);
            logger.CheckingTargetTable(sourceTable.Name);

            T? sub = await sourceTable.GetEntityIfExistsAsync(i.p, i.r, cancellationToken).ConfigureAwait(false);
            if (sub?.Subscribers.SelectMany(i => i.Value).Any() is true)
            {
                logger.FoundRecord();
                await ProcessSubscriptionAsync(message, sub.Subscribers, teamFinder(i), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private const ushort MAX_EMBEDS_PER_MESSAGE = 10;

    protected virtual async Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, ushort? highlightTeam, CancellationToken cancellationToken)
    {
        using IDisposable scope = logger.CreateMethodScope();
        IEnumerable<SubscriptionEmbedding[]> chunksOfEmbeddingsToSend = (await _embedGenerator.CreateEmbeddingsAsync(message, highlightTeam, cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false))
            .Chunk(MAX_EMBEDS_PER_MESSAGE);
        var discordRequestOptions = cancellationToken.ToRequestOptions();
        if (chunksOfEmbeddingsToSend.Any(i => i.Length is not 0))
        {
            (string PartitionKey, string RowKey, string Title)? threadLocator = message.GetThreadDetails(allServices);
            if (threadLocator is not null)
            {
                (string? pk, string? rk, string _) = threadLocator.Value;
                NullableResponse<ThreadTableEntity> entity = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(pk, rk, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (entity.HasValue && entity.Value is not null)
                {
                    foreach (ThreadTableEntity.ThreadDetail t in entity.Value.ThreadIdList)
                    {
                        var chanId = t.ChannelId;
                        var threadId = t.ThreadId;
                        IMessageChannel? rawChan = await _discordClient.GetChannelAsync(threadId, discordRequestOptions).ConfigureAwait(false) as IMessageChannel ?? await _discordClient.GetDMChannelAsync(threadId).ConfigureAwait(false);
                        var guildId = (rawChan as IGuildChannel)?.GuildId;
                        IReadOnlySet<ulong> guildSubscriptions = subscribers.SubscriptionsForGuild(guildId);
                        if (guildSubscriptions.Any() && rawChan is not null)
                        {
                            MessageReference? replyToMessage;
                            try
                            {
                                replyToMessage = t.MessageId is not null
                                    && await rawChan.GetMessageAsync(t.MessageId.Value, options: discordRequestOptions).ConfigureAwait(false) is not null
                                        ? new MessageReference(t.MessageId)
                                        : null;
                            }
                            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                            {
                                replyToMessage = null;
                            }

                            try
                            {
                                await sendEmbeddingsAsync(chunksOfEmbeddingsToSend, discordRequestOptions, rawChan, replyToMessage).ConfigureAwait(false);

                                meter.LogMetric("NotificationSent", 1,
                                    new Dictionary<string, object?>() {
                                    { "ChannelId", chanId },
                                    { "ChannelName", rawChan.Name },
                                    { "Threaded", true }
                                    });

                                subscribers.RemoveSubscription(guildId, chanId, logger);
                            }
                            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                            {
                                logger.ErrorWhileTryingToSendThreadedNotificationToChannelChannelIdChannelName(e, chanId, rawChan.Name);
                            }
                        }
                        else
                        {
                            logger.DidnTGetAMessageChannelFromThreadThreadId(threadId);
                        }
                    }
                }
            }

            foreach (var subscriberChannelId in subscribers.SelectMany(i => i.Value))
            {
                cancellationToken.ThrowIfCancellationRequested();
                IChannel? targetChannel = await _discordClient.GetChannelAsync(subscriberChannelId, discordRequestOptions).ConfigureAwait(false);
                Debug.Assert(targetChannel is not null);
                logger.RetrievedChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);

                if (targetChannel is IMessageChannel msgChan)
                {
                    logger.SendingNotificationToChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);
                    IMessageChannel threadForMessage = await CreateThreadForMessageAsync(message, msgChan, cancellationToken);
                    var replyToMessageId = await sendEmbeddingsAsync(chunksOfEmbeddingsToSend, discordRequestOptions, threadForMessage).ConfigureAwait(false);

                    await StoreReplyToMessageAsync(message, msgChan, threadForMessage, replyToMessageId, cancellationToken);

                    meter.LogMetric("NotificationSent", 1,
                        new Dictionary<string, object?>() {
                            { "ChannelId", subscriberChannelId },
                            { "ChannelName", targetChannel.Name },
                            { "Threaded", false }
                        });
                }
                else
                {
                    logger.ChannelChannelIdIsNotAMessageChannel(subscriberChannelId);
                }
            }
        }

        static async Task<ulong> sendEmbeddingsAsync(IEnumerable<SubscriptionEmbedding[]> chunksOfEmbeddingsToSend, RequestOptions discordRequestOptions, IMessageChannel chan, MessageReference? replyToMessage = null)
        {
            ulong? id = null;
            foreach (SubscriptionEmbedding[] i in chunksOfEmbeddingsToSend)
            {
                IUserMessage m = await chan.SendMessageAsync(embeds: [.. i.Select(i => i.Content)], components: ComponentBuilder.FromComponents([.. i.SelectMany(j => j.Actions ?? [])]).Build(), messageReference: replyToMessage, options: discordRequestOptions).ConfigureAwait(false);
                id ??= m.Id;
            }

            return id.GetValueOrDefault(0);
        }
    }

    private async Task StoreReplyToMessageAsync(WebhookMessage msg, IMessageChannel channel, IMessageChannel thread, ulong? replyToMessageId, CancellationToken cancellationToken)
    {
        (string PartitionKey, string RowKey, string Title)? threadDetails = msg.GetThreadDetails(allServices);
        if (threadDetails is not null)
        {
            ThreadTableEntity entity = (await threadsTable.GetEntityAsync<ThreadTableEntity>(threadDetails.Value.PartitionKey, threadDetails.Value.RowKey, cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
            ThreadTableEntity.ThreadDetail i = entity.ThreadIdList.First(i => i.ChannelId == channel.Id && i.ThreadId == thread.Id);
            i.MessageId = replyToMessageId;

            await threadsTable.UpdateEntityAsync(entity, ETag.All, mode: TableUpdateMode.Replace, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IMessageChannel> CreateThreadForMessageAsync(WebhookMessage message, IMessageChannel msgChan, CancellationToken cancellationToken)
    {
        (string PartitionKey, string RowKey, string Title)? threadDetails = message.GetThreadDetails(allServices);
        if (threadDetails is null)
        {
            return msgChan;
        }

        IMessageChannel thread;
        try
        {
            thread = msgChan is ITextChannel threadableChannel ? await threadableChannel.CreateThreadAsync(threadDetails.Value.Title) : msgChan;
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.TriedToCreateThreadOnAnITextChannelButItFailed(e);
            thread = msgChan;
        }

        try
        {
            NullableResponse<ThreadTableEntity>? tableResponse = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(threadDetails.Value.PartitionKey, threadDetails.Value.RowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
            ThreadTableEntity tableEntity = tableResponse is not null && tableResponse.HasValue
                ? tableResponse.Value!
                : new(time)
                {
                    PartitionKey = threadDetails.Value.PartitionKey,
                    RowKey = threadDetails.Value.RowKey,
                };
            tableEntity.ThreadIdList.Add(new(msgChan.Id, thread.Id));
            await threadsTable.UpsertEntityAsync(tableEntity, mode: TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
            return thread;
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.ErrorWhileTryingToCreateThreadInChannelChannelIdChannelNameMessage(e, msgChan.Id, msgChan.Name, e.Message);
            Debug.Fail(e.Message);
        }

        return thread;
    }

    public async Task CleanupDeletedThreadAsync(ulong threadId, CancellationToken cancellationToken)
    {
        using IDisposable scope = logger.CreateMethodScope();
        logger.CleaningUpTrackedStateForDeletedDiscordThreadThreadId(threadId);

        bool foundTrackedThread = false;
        await foreach (ThreadTableEntity? entity in threadsTable.QueryAsync<ThreadTableEntity>(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var removedCount = entity.ThreadIdList.RemoveAll(i => i.ThreadId == threadId);
            if (removedCount is 0)
            {
                continue;
            }

            foundTrackedThread = true;
            if (entity.ThreadIdList.Count is 0)
            {
                await threadsTable.DeleteEntityAsync(entity, ETag.All, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await threadsTable.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            logger.RemovedTrackedStateForDeletedDiscordThreadThreadIdFromPartitionKeyRowKeyRemovedEntriesRemovedEntries(threadId, entity.PartitionKey, entity.RowKey, removedCount);
            meter.LogMetric("DeletedDiscordThreadStateCleanedUp", removedCount,
                new Dictionary<string, object?>
                {
                    { "ThreadId", threadId },
                    { "PartitionKey", entity.PartitionKey },
                    { "RowKey", entity.RowKey }
                });
        }

        if (!foundTrackedThread)
        {
            logger.NoTrackedStateFoundForDeletedDiscordThreadThreadId(threadId);
        }
    }

    private static (ImmutableHashSet<string> Teams, ImmutableHashSet<string> Events) GetTeamsAndEventsInMessage(JsonElement messageData)
    {
        HashSet<string> teams = [], events = [];

        foreach (var propertyName in new[] { "team", "team_key", "teams", "team_keys", "picks", "declines", "team_number" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out IReadOnlyCollection<JsonElement>? properties) && properties is not null)
            {
                foreach (JsonElement propertyValue in properties)
                {
                    addElement(propertyValue, teams);
                }
            }
        }

        foreach (var propertyName in new[] { "event_key", "events", "event_keys" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out IReadOnlyCollection<JsonElement>? properties) && properties is not null)
            {
                foreach (JsonElement propertyValue in properties)
                {
                    addElement(propertyValue, events);
                }
            }
        }

        return (teams.ToImmutableHashSet(), events.ToImmutableHashSet());

        static void addElement(JsonElement elt, HashSet<string> toHashSet)
        {
            string? s;
            if (elt.ValueKind is JsonValueKind.Array)
            {
                foreach (JsonElement i in elt.EnumerateArray())
                {
                    s = i.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        toHashSet.Add(s);
                    }
                }
            }
            else if ((s = elt.ToString()) is not null)
            {
                toHashSet.Add(s);
            }
        }
    }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
sealed record ThreadTableEntity : ITableEntity
{
    [JsonConstructor]
    public ThreadTableEntity() : this(TimeProvider.System) { }

    internal ThreadTableEntity(TimeProvider time)
    {
        Timestamp = time.GetUtcNow();
    }

    public DateTimeOffset? Timestamp { get; set; }
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

    public sealed record ThreadDetail
    {
        [JsonConstructor]
        public ThreadDetail(string channel, string thread, string? message)
        {
            Channel = channel;
            Thread = thread;
            Message = message;
        }

        public ThreadDetail(ulong channel, ulong thread)
        {
            ChannelId = channel;
            ThreadId = thread;
        }

        [JsonIgnore]
        public ulong ChannelId { get; set; }

        [JsonIgnore]
        public ulong ThreadId { get; set; }

        [JsonIgnore]
        public ulong? MessageId { get; set; }

        // We have to store these as strings because Table SDK doesn't support ulong types
        public string Channel
        {
            get => ChannelId.ToString();
            set => ChannelId = ulong.Parse(value);
        }

        public string Thread
        {
            get => ThreadId.ToString();
            set => ThreadId = ulong.Parse(value);
        }

        public string? Message
        {
            get => MessageId?.ToString();
            set => MessageId = value is not null ? ulong.Parse(value) : null;
        }
    }
}
