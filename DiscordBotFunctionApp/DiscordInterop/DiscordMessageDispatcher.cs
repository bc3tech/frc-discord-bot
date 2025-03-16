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
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

internal sealed partial class DiscordMessageDispatcher([FromKeyedServices(Constants.ServiceKeys.TableClient_TeamSubscriptions)] TableClient teamSubscriptionsTable,
                                                       [FromKeyedServices(Constants.ServiceKeys.TableClient_EventSubscriptions)] TableClient eventSubscriptionsTable,
                                                       [FromKeyedServices(Constants.ServiceKeys.TableClient_Threads)] TableClient threadsTable,
                                                       IDiscordClient discordClient,
                                                       WebhookEmbeddingGenerator _embedGenerator,
                                                       TimeProvider time,
                                                       ILogger<DiscordMessageDispatcher> logger)
{
    private readonly DiscordSocketClient _discordClient = (discordClient as DiscordSocketClient) ?? throw new ArgumentException(nameof(discordClient));

    public async Task<bool> ProcessWebhookMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
    {
        using var scope = logger.CreateMethodScope();
        (var teams, var events) = GetTeamsAndEventsInMessage(message.MessageData);
        logger.TeamsTeamsInMessageEventsEventsInMessage(teams.Count, events.Count);

        var teamRecordsToFind = teams.Permutate([CommonConstants.ALL, .. events], (p, r) => (p, r)).ToArray().AsReadOnly();
        var eventRecordsToFind = events.Permutate([CommonConstants.ALL, .. teams], (p, r) => (p, r)).ToArray().AsReadOnly();

        logger.PermutatedResultsIntoTeamRecordsCountTeamRecordsAndEventRecordsCountEventRecords(teamRecordsToFind.Count, eventRecordsToFind.Count);

        List<Task> notifications = [];

        await SendNotificationsAsync<TeamSubscriptionEntity>(message, teamSubscriptionsTable, teamRecordsToFind, i => i.Item1 is not CommonConstants.ALL ? i.Item1.TeamKeyToTeamNumber() : null, logger, cancellationToken).ConfigureAwait(false);
        await SendNotificationsAsync<EventSubscriptionEntity>(message, eventSubscriptionsTable, eventRecordsToFind, i => i.Item2 is not CommonConstants.ALL ? i.Item2.TeamKeyToTeamNumber() : null, logger, cancellationToken).ConfigureAwait(false);

        logger.WaitingForNotificationsToBeSent();

        await Task.WhenAll(notifications).ConfigureAwait(false);

        return true;
    }

    private async Task SendNotificationsAsync<T>(WebhookMessage message, TableClient sourceTable, IReadOnlyList<(string p, string r)> records, Func<(string, string), ushort?> teamFinder, ILogger<DiscordMessageDispatcher> logger, CancellationToken cancellationToken) where T : class, ITableEntity, ISubscriptionEntity
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
        var discordRequestOptions = cancellationToken.ToRequestOptions();
        if (await embeds.AnyAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            // check to see if there are any threads already created for this message
            var threadLocator = message.GetThreadDetails();
            List<ulong?> channelsWhereWeAlreadyPostedIntoThreads = [];
            if (threadLocator is not null)
            {
                var (pk, rk, _) = threadLocator.Value;
                var entity = await threadsTable.GetEntityIfExistsAsync<ThreadTableEntity>(pk, rk, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (entity.HasValue && entity.Value is not null)
                {
                    var embedsToSend = await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false);
                    foreach (var t in entity.Value.ThreadIdList)
                    {
                        var chanId = t.ChannelId;
                        var threadId = t.ThreadId;
                        IMessageChannel? rawChan = _discordClient.GetChannel(threadId) as IMessageChannel ?? await _discordClient.GetDMChannelAsync(threadId).ConfigureAwait(false);
                        var guildId = (rawChan as IGuildChannel)?.GuildId;
                        if (rawChan is not null)
                        {
                            MessageReference? replyToMessage;
                            try
                            {
                                replyToMessage = t.MessageId is not null
                                    && (await rawChan.GetMessageAsync(t.MessageId.Value, options: discordRequestOptions).ConfigureAwait(false)) is not null
                                        ? new MessageReference(t.MessageId)
                                        : null;
                            }
                            catch
                            {
                                replyToMessage = null;
                            }

                            try
                            {
                                await rawChan.SendMessageAsync(embeds: embedsToSend, messageReference: replyToMessage, options: discordRequestOptions).ConfigureAwait(false);

                                logger.LogMetric("NotificationSent", 1,
                                    new Dictionary<string, object>() {
                                    { "ChannelId", chanId },
                                    { "ChannelName", rawChan.Name },
                                    { "Threaded", true }
                                    });

                                subscribers.RemoveSubscription(guildId, rawChan.Id);
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Error while trying to send threaded notification to channel {ChannelId} ({ChannelName})", chanId, rawChan.Name, e.Message);
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
                    var embedChunks = (await embeds.ToArrayAsync(cancellationToken).ConfigureAwait(false)).Chunk(MAX_EMBEDS_PER_MESSAGE);
                    var threadForMessage = await CreateThreadForMessageAsync(message, msgChan, cancellationToken);
                    ulong? replyToMessageId = null;
                    foreach (var i in embedChunks)
                    {
                        replyToMessageId ??= (await threadForMessage.SendMessageAsync(embeds: i, options: discordRequestOptions).ConfigureAwait(false)).Id;
                    }

                    await StoreReplyToMessageAsync(message, msgChan, threadForMessage, replyToMessageId, cancellationToken);

                    logger.LogMetric("NotificationSent", 1,
                        new Dictionary<string, object>() {
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
    }

    private async Task StoreReplyToMessageAsync(WebhookMessage msg, IMessageChannel channel, IMessageChannel thread, ulong? replyToMessageId, CancellationToken cancellationToken)
    {
        var threadDetails = msg.GetThreadDetails();
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
        var threadDetails = message.GetThreadDetails();
        if (threadDetails is null || !threadDetails.HasValue)
        {
            return msgChan;
        }

        IMessageChannel thread;
        try
        {
            thread = (msgChan is ITextChannel threadableChannel) ? await threadableChannel.CreateThreadAsync(threadDetails.Value.Title) : msgChan;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tried to create thread on an `ITextChannel` but it failed");
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
        catch (Exception ex)
        {
            logger.ErrorWhileTryingToCreateThreadInChannelChannelIdChannelNameMessage(ex, msgChan.Id, msgChan.Name, ex.Message);
            Debug.Fail(ex.Message);
        }

        return thread;
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
                    addElement(propertyValue, teams, s => s.TeamKeyToTeamNumber().ToString()!);
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
