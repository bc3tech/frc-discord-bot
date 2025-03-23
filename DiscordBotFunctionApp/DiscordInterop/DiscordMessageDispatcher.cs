namespace DiscordBotFunctionApp.DiscordInterop;

using Azure;
using Azure.Data.Tables;

using Common.Extensions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Extensions;
using DiscordBotFunctionApp.Storage.TableEntities;
using DiscordBotFunctionApp.TbaInterop.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using TheBlueAlliance.Api;

internal sealed partial class DiscordMessageDispatcher([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptionsTable,
                                                       [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptionsTable,
                                                       [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient threadsTable,
                                                       IEventApi eventApi,
                                                       IDiscordClient discordClient,
                                                       WebhookEmbeddingGenerator _embedGenerator,
                                                       TimeProvider time,
                                                       Meter meter,
                                                       IServiceProvider allServices,
                                                       ILogger<DiscordMessageDispatcher> logger)
{
    private readonly DiscordSocketClient _discordClient = (discordClient as DiscordSocketClient) ?? throw new ArgumentException(nameof(discordClient));

    public async Task<bool> ProcessWebhookMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        using var scope = logger.CreateMethodScope();
        (var teams, var events) = GetTeamsAndEventsInMessage(message.MessageData);
        logger.TeamsTeamsInMessageEventsEventsInMessage(teams.Count, events.Count);

        if (message.IsBroadcast)
        {
            logger.LogDebug("Broadcast message detected. Fetching all teams at the event.");
            // If we're broadcasting the message to everybody at the event, then we need to include every team at the event as a possible subscriber.
            foreach (var e in events)
            {
                var eventTeams = await eventApi.GetEventTeamsKeysAsync(e, cancellationToken: cancellationToken).ConfigureAwait(false);
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
    private async Task SendNotificationsAsync<T>(WebhookMessage message, TableClient sourceTable, IEnumerable<(string p, string r)> records, Func<(string, string), ushort?> teamFinder, ILogger<DiscordMessageDispatcher> logger, CancellationToken cancellationToken) where T : class, ISubscriptionEntity
    {
        foreach (var i in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var subscriptionScope = _notificationLoggingScope(logger, i.p, i.r);
            logger.CheckingTargetTable(sourceTable.Name);

            var sub = await sourceTable.GetEntityIfExistsAsync<T>(i.p, i.r, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (sub.HasValue && sub.Value?.Subscribers.SelectMany(i => i.Value).Any() is true)
            {
                logger.FoundRecord();
                await ProcessSubscriptionAsync(message, sub.Value.Subscribers, teamFinder(i), cancellationToken);
            }
        }
    }

    private const ushort MAX_EMBEDS_PER_MESSAGE = 10;

    private async Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, ushort? highlightTeam, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        var chunksOfEmbeddingsToSend = (await _embedGenerator.CreateEmbeddingsAsync(message, highlightTeam, cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false))
            .Chunk(MAX_EMBEDS_PER_MESSAGE);
        var discordRequestOptions = cancellationToken.ToRequestOptions();
        if (chunksOfEmbeddingsToSend.Any(i => i.Length is not 0))
        {
            // check to see if there are any threads already created for this message
            var threadLocator = message.GetThreadDetails(allServices);
            List<ulong?> channelsWhereWeAlreadyPostedIntoThreads = [];
            if (threadLocator is not null)
            {
                var (pk, rk, _) = threadLocator.Value;
                var entity = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(pk, rk, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (entity.HasValue && entity.Value is not null)
                {
                    foreach (var t in entity.Value.ThreadIdList)
                    {
                        var chanId = t.ChannelId;
                        var threadId = t.ThreadId;
                        IMessageChannel? rawChan = (await _discordClient.GetChannelAsync(threadId, discordRequestOptions).ConfigureAwait(false)) as IMessageChannel ?? await _discordClient.GetDMChannelAsync(threadId).ConfigureAwait(false);
                        var guildId = (rawChan as IGuildChannel)?.GuildId;
                        var guildSubscriptions = subscribers.SubscriptionsForGuild(guildId);
                        if (guildSubscriptions.Any() && rawChan is not null)
                        {
                            MessageReference? replyToMessage;
                            try
                            {
                                replyToMessage = t.MessageId is not null
                                    && (await rawChan.GetMessageAsync(t.MessageId.Value, options: discordRequestOptions).ConfigureAwait(false)) is not null
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
                var targetChannel = await _discordClient.GetChannelAsync(subscriberChannelId, discordRequestOptions).ConfigureAwait(false);
                Debug.Assert(targetChannel is not null);
                logger.RetrievedChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);

                if (targetChannel is IMessageChannel msgChan)
                {
                    logger.SendingNotificationToChannelChannelIdChannelName(subscriberChannelId, targetChannel.Name);
                    var threadForMessage = await CreateThreadForMessageAsync(message, msgChan, cancellationToken);
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
            foreach (var i in chunksOfEmbeddingsToSend)
            {
                var m = await chan.SendMessageAsync(embeds: [.. i.Select(i => i.Content)], components: ComponentBuilder.FromComponents([.. i.SelectMany(j => j.Actions ?? [])]).Build(), messageReference: replyToMessage, options: discordRequestOptions).ConfigureAwait(false);
                id ??= m.Id;
            }

            return id.GetValueOrDefault(0);
        }
    }

    private async Task StoreReplyToMessageAsync(WebhookMessage msg, IMessageChannel channel, IMessageChannel thread, ulong? replyToMessageId, CancellationToken cancellationToken)
    {
        var threadDetails = msg.GetThreadDetails(allServices);
        if (threadDetails is not null)
        {
            var entity = (await threadsTable.GetEntityAsync<ThreadTableEntity>(threadDetails.Value.PartitionKey, threadDetails.Value.RowKey, cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
            var i = entity.ThreadIdList.First(i => i.ChannelId == channel.Id && i.ThreadId == thread.Id);
            i.MessageId = replyToMessageId;

            await threadsTable.UpdateEntityAsync(entity, ETag.All, mode: TableUpdateMode.Replace, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IMessageChannel> CreateThreadForMessageAsync(WebhookMessage message, IMessageChannel msgChan, CancellationToken cancellationToken)
    {
        var threadDetails = message.GetThreadDetails(allServices);
        if (threadDetails is null || !threadDetails.HasValue)
        {
            return msgChan;
        }

        IMessageChannel thread;
        try
        {
            thread = (msgChan is ITextChannel threadableChannel) ? await threadableChannel.CreateThreadAsync(threadDetails.Value.Title) : msgChan;
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger.TriedToCreateThreadOnAnITextChannelButItFailed(e);
            thread = msgChan;
        }

        try
        {
            var tableResponse = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(threadDetails.Value.PartitionKey, threadDetails.Value.RowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
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

    private static (ImmutableHashSet<string> Teams, ImmutableHashSet<string> Events) GetTeamsAndEventsInMessage(JsonElement messageData)
    {
        HashSet<string> teams = [], events = [];

        foreach (var propertyName in new[] { "team", "team_key", "teams", "team_keys", "picks", "declines", "team_number" })
        {
            if (messageData.TryGetPropertyAnywhere(propertyName, out var properties) && properties is not null)
            {
                foreach (var propertyValue in properties)
                {
                    addElement(propertyValue, teams);
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

        return (teams.ToImmutableHashSet(), events.ToImmutableHashSet());

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
sealed record ThreadTableEntity : ITableEntity
{
    [JsonConstructor]
    public ThreadTableEntity() : this(TimeProvider.System) { }

    internal ThreadTableEntity(TimeProvider time)
    {
        this.Timestamp = time.GetUtcNow();
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
            this.Channel = channel;
            this.Thread = thread;
            this.Message = message;
        }

        public ThreadDetail(ulong channel, ulong thread)
        {
            this.ChannelId = channel;
            this.ThreadId = thread;
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
