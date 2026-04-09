namespace FunctionApp.Tests;

using Azure.Data.Tables;

using Discord.WebSocket;

using FunctionApp.DiscordInterop;
using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.Storage.TableEntities;
using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Text.Json;

using TheBlueAlliance.Api;

[SuppressMessage("Performance", "EA0014:Add CancellationToken as the parameter of asynchronous method", Justification = "xUnit [Fact] methods do not support a CancellationToken parameter in this test setup.")]
public sealed class DiscordMessageDispatcherWebhookTests
{
    [Fact]
    public async Task ProcessWebhookMessageAsyncDispatchesNotificationsForMatchingTeamAndEventSubscriptions()
    {
        FakeWebhookSubscriptionStore<TeamSubscriptionEntity> teamStore = new("team-subscriptions");
        FakeWebhookSubscriptionStore<EventSubscriptionEntity> eventStore = new("event-subscriptions");

        teamStore.Add(new TeamSubscriptionEntity
        {
            PartitionKey = "frc2046",
            RowKey = "2026cabl",
            Subscribers = CreateSubscribers(guildId: 123, channelIds: [111])
        });

        eventStore.Add(new EventSubscriptionEntity
        {
            PartitionKey = "2026cabl",
            RowKey = Common.Constants.ALL,
            Subscribers = CreateSubscribers(guildId: 456, channelIds: [222])
        });

        using RecordingDiscordMessageDispatcher dispatcher = CreateDispatcher(teamStore, eventStore);
        WebhookMessage message = CreateMessage(
            NotificationType.upcoming_match,
            """{"team_key":"frc2046","event_key":"2026cabl"}""");

        bool processed = await dispatcher.ProcessWebhookMessageAsync(message, CancellationToken.None);

        Assert.True(processed);
        Assert.Equal(2, dispatcher.Dispatches.Count);

        NotificationDispatch teamDispatch = Assert.Single(dispatcher.Dispatches, i => i.HighlightTeam == 2046);
        Assert.Contains(111UL, teamDispatch.Subscribers.SubscriptionsForGuild(123));

        NotificationDispatch eventDispatch = Assert.Single(dispatcher.Dispatches, i => i.HighlightTeam is null);
        Assert.Contains(222UL, eventDispatch.Subscribers.SubscriptionsForGuild(456));
    }

    [Fact]
    public async Task ProcessWebhookMessageAsyncDispatchesForAllAndSpecificTeamSubscriptions()
    {
        FakeWebhookSubscriptionStore<TeamSubscriptionEntity> teamStore = new("team-subscriptions");
        FakeWebhookSubscriptionStore<EventSubscriptionEntity> eventStore = new("event-subscriptions");

        teamStore.Add(new TeamSubscriptionEntity
        {
            PartitionKey = "frc2046",
            RowKey = Common.Constants.ALL,
            Subscribers = CreateSubscribers(guildId: 123, channelIds: [111])
        });

        teamStore.Add(new TeamSubscriptionEntity
        {
            PartitionKey = "frc2046",
            RowKey = "2026cabl",
            Subscribers = CreateSubscribers(guildId: 123, channelIds: [222])
        });

        using RecordingDiscordMessageDispatcher dispatcher = CreateDispatcher(teamStore, eventStore);
        WebhookMessage message = CreateMessage(
            NotificationType.upcoming_match,
            """{"team_key":"frc2046","event_key":"2026cabl"}""");

        _ = await dispatcher.ProcessWebhookMessageAsync(message, CancellationToken.None);

        Assert.Equal(2, dispatcher.Dispatches.Count);
        Assert.All(dispatcher.Dispatches, dispatch => Assert.Equal((ushort)2046, dispatch.HighlightTeam));
        Assert.Contains(dispatcher.Dispatches, dispatch => dispatch.Subscribers.SubscriptionsForGuild(123).Contains(111));
        Assert.Contains(dispatcher.Dispatches, dispatch => dispatch.Subscribers.SubscriptionsForGuild(123).Contains(222));
    }

    [Fact]
    public async Task ProcessWebhookMessageAsyncSkipsRecordsWithoutSubscribers()
    {
        FakeWebhookSubscriptionStore<TeamSubscriptionEntity> teamStore = new("team-subscriptions");
        FakeWebhookSubscriptionStore<EventSubscriptionEntity> eventStore = new("event-subscriptions");

        teamStore.Add(new TeamSubscriptionEntity
        {
            PartitionKey = "frc2046",
            RowKey = "2026cabl",
            Subscribers = []
        });

        using RecordingDiscordMessageDispatcher dispatcher = CreateDispatcher(teamStore, eventStore);
        WebhookMessage message = CreateMessage(
            NotificationType.upcoming_match,
            """{"team_key":"frc2046","event_key":"2026cabl"}""");

        _ = await dispatcher.ProcessWebhookMessageAsync(message, CancellationToken.None);

        Assert.Empty(dispatcher.Dispatches);
    }

    private static RecordingDiscordMessageDispatcher CreateDispatcher(
        IWebhookSubscriptionStore<TeamSubscriptionEntity> teamStore,
        IWebhookSubscriptionStore<EventSubscriptionEntity> eventStore)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.ClearProviders());
        var services = new ServiceCollection().BuildServiceProvider();
        var embedGenerator = new WebhookEmbeddingGenerator(services, loggerFactory.CreateLogger<WebhookEmbeddingGenerator>());
        var discordClient = new DiscordSocketClient(new DiscordSocketConfig());
        var threadsTable = new TableClient("UseDevelopmentStorage=true", "threads");
        var meter = new Meter("FunctionApp.Tests.Webhooks");

        return new RecordingDiscordMessageDispatcher(
            teamStore,
            eventStore,
            threadsTable,
            new EventApi(),
            discordClient,
            embedGenerator,
            TimeProvider.System,
            meter,
            services,
            loggerFactory.CreateLogger<DiscordMessageDispatcher>());
    }

    private static WebhookMessage CreateMessage(NotificationType notificationType, string messageDataJson)
    {
        using var jsonDocument = JsonDocument.Parse(messageDataJson);
        return new WebhookMessage
        {
            MessageType = notificationType,
            MessageData = jsonDocument.RootElement.Clone()
        };
    }

    private static GuildSubscriptions CreateSubscribers(ulong? guildId, params ulong[] channelIds)
    {
        GuildSubscriptions subscribers = [];
        foreach (var channelId in channelIds)
        {
            subscribers.AddSubscription(guildId, channelId);
        }

        return subscribers;
    }

    private sealed class FakeWebhookSubscriptionStore<T>(string name) : IWebhookSubscriptionStore<T>
        where T : class, ISubscriptionEntity
    {
        private readonly Dictionary<(string PartitionKey, string RowKey), T> _entities = [];

        public string Name { get; } = name;

        public void Add(T entity) => _entities[(entity.PartitionKey, entity.RowKey)] = entity;

        public Task<T?> GetEntityIfExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_entities.GetValueOrDefault((partitionKey, rowKey)));
        }
    }

    private sealed class RecordingDiscordMessageDispatcher(
        IWebhookSubscriptionStore<TeamSubscriptionEntity> teamSubscriptionsTable,
        IWebhookSubscriptionStore<EventSubscriptionEntity> eventSubscriptionsTable,
        TableClient threadsTable,
        IEventApi eventApi,
        DiscordSocketClient discordClient,
        WebhookEmbeddingGenerator embedGenerator,
        TimeProvider time,
        Meter meter,
        IServiceProvider allServices,
        ILogger<DiscordMessageDispatcher> logger)
        : DiscordMessageDispatcher(
            teamSubscriptionsTable,
            eventSubscriptionsTable,
            threadsTable,
            eventApi,
            discordClient,
            embedGenerator,
            time,
            meter,
            allServices,
            logger), IDisposable
    {
        public List<NotificationDispatch> Dispatches { get; } = [];

        protected override Task ProcessSubscriptionAsync(WebhookMessage message, GuildSubscriptions subscribers, ushort? highlightTeam, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Dispatches.Add(new NotificationDispatch(Clone(subscribers), highlightTeam));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            discordClient.Dispose();
        }

        private static GuildSubscriptions Clone(GuildSubscriptions source)
        {
            GuildSubscriptions clone = [];
            foreach (var (guildId, channels) in source)
            {
                clone[guildId] = [.. channels];
            }

            return clone;
        }
    }

    private sealed record NotificationDispatch(GuildSubscriptions Subscribers, ushort? HighlightTeam);
}
