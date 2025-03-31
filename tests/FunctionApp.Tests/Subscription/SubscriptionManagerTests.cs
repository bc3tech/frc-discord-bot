namespace FunctionApp.Tests.Subscription;
using Azure;
using Azure.Data.Tables;

using FunctionApp.Storage.TableEntities;
using FunctionApp.Subscription;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using Xunit;
using Xunit.Abstractions;

public class SubscriptionManagerTests : TestWithLogger
{
    private const ulong GuildId = 12345UL;
    private const ulong ChannelId = 67890UL;
    private const string EventName = "2025wabon";
    private const string TeamKey = "frc2046";

    private readonly Mock<TableClient> _mockTeamSubscriptions;
    private readonly Mock<TableClient> _mockEventSubscriptions;
    private readonly SubscriptionManager _subscriptionManager;

    public SubscriptionManagerTests(ITestOutputHelper outputHelper) : base(typeof(SubscriptionManager), outputHelper)
    {
        _mockTeamSubscriptions = new Mock<TableClient>();
        _mockEventSubscriptions = new Mock<TableClient>();
        _subscriptionManager = new SubscriptionManager(_mockTeamSubscriptions.Object, _mockEventSubscriptions.Object, this.Mocker.Get<ILogger<SubscriptionManager>>());
    }

    [Fact]
    public async Task GetSubscriptionsForGuildAsync_ShouldReturnSubscriptions()
    {
        // Arrange
        var teamSubscriptionEntity = new TeamSubscriptionEntity { PartitionKey = TeamKey, RowKey = EventName, Subscribers = new() { { GuildId.ToString(), [ChannelId] } } };
        var eventSubscriptionEntity = new EventSubscriptionEntity { PartitionKey = EventName, RowKey = TeamKey, Subscribers = new() { { GuildId.ToString(), [ChannelId] } } };

        _mockTeamSubscriptions
            .Setup(t => t.QueryAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<TeamSubscriptionEntity>.FromPages([Page<TeamSubscriptionEntity>.FromValues([teamSubscriptionEntity], null, Mock.Of<Response>())]));

        _mockEventSubscriptions
            .Setup(e => e.QueryAsync<EventSubscriptionEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncPageable<EventSubscriptionEntity>.FromPages([Page<EventSubscriptionEntity>.FromValues([eventSubscriptionEntity], null, Mock.Of<Response>())]));

        // Act
        var result = new List<NotificationSubscription>();
        await foreach (var subscription in _subscriptionManager.GetSubscriptionsForGuildAsync(GuildId, CancellationToken.None))
        {
            result.Add(subscription);
        }

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveSubscriptionAsync_ShouldSaveSubscription()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<TeamSubscriptionEntity>>());

        _mockTeamSubscriptions.Setup(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response>());

        // Act
        await _subscriptionManager.SaveSubscriptionAsync(subscription, CancellationToken.None);

        // Assert
        _mockTeamSubscriptions.Verify(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveSubscriptionAsync_ShouldRemoveSubscription()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(new TeamSubscriptionEntity { PartitionKey = TeamKey, RowKey = EventName, Subscribers = new() { { subscription.GuildId!.Value.ToString(), [subscription.ChannelId] } } }, Mock.Of<Response>()));

        _mockTeamSubscriptions.Setup(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response>());

        // Act
        await _subscriptionManager.RemoveSubscriptionAsync(subscription, CancellationToken.None);

        // Assert
        _mockTeamSubscriptions.Verify(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveSubscriptionAsync_ShouldLogAndThrowException_WhenUpsertFails()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<TeamSubscriptionEntity>>());

        var errorResponse = new Mock<Response>();
        errorResponse.SetupGet(r => r.IsError).Returns(true);
        errorResponse.SetupGet(r => r.Status).Returns(500);
        errorResponse.SetupGet(r => r.ReasonPhrase).Returns("Internal Server Error");

        _mockTeamSubscriptions.Setup(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResponse.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpProtocolException>(() => _subscriptionManager.SaveSubscriptionAsync(subscription, CancellationToken.None));
        Assert.Equal(500, exception.ErrorCode);
        this.Logger.Verify(LogLevel.Error);
    }

    [Fact]
    public async Task RemoveSubscriptionAsync_ShouldLogAndThrowException_WhenUpsertFails()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue<TeamSubscriptionEntity>(new() { PartitionKey = TeamKey, RowKey = EventName, Subscribers = new() { { GuildId.ToString(), [ChannelId] } } }, Mock.Of<Response>()));

        var errorResponse = new Mock<Response>();
        errorResponse.SetupGet(r => r.IsError).Returns(true);
        errorResponse.SetupGet(r => r.Status).Returns(500);
        errorResponse.SetupGet(r => r.ReasonPhrase).Returns("Internal Server Error");

        _mockTeamSubscriptions.Setup(t => t.UpsertEntityAsync(It.IsAny<TeamSubscriptionEntity>(), TableUpdateMode.Replace, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResponse.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpProtocolException>(() => _subscriptionManager.RemoveSubscriptionAsync(subscription, CancellationToken.None));
        Assert.Equal(500, exception.ErrorCode);
        this.Logger.Verify(LogLevel.Error);
    }

    [Fact]
    public async Task SaveSubscriptionAsync_ShouldLogWhenSubscriptionAlreadyExists()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        var existingEntity = new TeamSubscriptionEntity() { PartitionKey = TeamKey, RowKey = EventName };
        existingEntity.Subscribers.AddSubscription(GuildId, ChannelId);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(existingEntity, Mock.Of<Response>()));

        // Act
        await _subscriptionManager.SaveSubscriptionAsync(subscription, CancellationToken.None);

        // Assert
        this.Logger.Verify(LogLevel.Warning);
    }

    [Fact]
    public async Task RemoveSubscriptionAsync_ShouldLogWhenNoSubscriptionsFound()
    {
        // Arrange
        var subscription = new NotificationSubscription(GuildId, ChannelId, EventName, TeamKey);

        _mockTeamSubscriptions.Setup(t => t.GetEntityIfExistsAsync<TeamSubscriptionEntity>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<TeamSubscriptionEntity>>());

        // Act
        await _subscriptionManager.RemoveSubscriptionAsync(subscription, CancellationToken.None);

        // Assert
        this.Logger.Verify(LogLevel.Debug);
    }
}
