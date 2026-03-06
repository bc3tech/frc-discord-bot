namespace FunctionApp.Tests.Subscription;

using CommonConstants = Common.Constants;

using FunctionApp.Storage.TableEntities;
using FunctionApp.Subscription;

using Microsoft.Extensions.Logging.Abstractions;

public sealed class SubscriptionManagerTests
{
    [Fact]
    public async Task SaveSubscriptionAsync_TeamSubscription_CreatesExpectedEntity()
    {
        var teamTable = new InMemorySubscriptionTableClient();
        var eventTable = new InMemorySubscriptionTableClient();
        var sut = new SubscriptionManager(teamTable, eventTable, NullLogger<SubscriptionManager>.Instance);

        await sut.SaveSubscriptionAsync(new NotificationSubscription(123, 456, "2026miket", "frc33"), CancellationToken.None);

        var saved = await teamTable.GetEntityIfExistsAsync<TeamSubscriptionEntity>("frc33", "2026miket", CancellationToken.None);
        Assert.NotNull(saved);
        Assert.True(saved!.Subscribers.Exists(123, 456));
        Assert.Equal(1, teamTable.UpsertCount);
    }

    [Fact]
    public async Task SaveSubscriptionAsync_TeamSubscription_WhenAllEventExists_DoesNotDuplicateSpecificEvent()
    {
        var teamTable = new InMemorySubscriptionTableClient();
        var eventTable = new InMemorySubscriptionTableClient();
        var existingAllEvent = new TeamSubscriptionEntity
        {
            PartitionKey = "frc33",
            RowKey = CommonConstants.ALL
        };
        existingAllEvent.Subscribers.AddSubscription(123, 456);
        teamTable.Seed(existingAllEvent);

        var sut = new SubscriptionManager(teamTable, eventTable, NullLogger<SubscriptionManager>.Instance);

        await sut.SaveSubscriptionAsync(new NotificationSubscription(123, 456, "2026miket", "frc33"), CancellationToken.None);

        var specificEvent = await teamTable.GetEntityIfExistsAsync<TeamSubscriptionEntity>("frc33", "2026miket", CancellationToken.None);
        Assert.Null(specificEvent);
        Assert.Equal(0, teamTable.UpsertCount);
    }

    [Fact]
    public async Task RemoveSubscriptionAsync_EventSubscription_RemovesChannelFromEntity()
    {
        var teamTable = new InMemorySubscriptionTableClient();
        var eventTable = new InMemorySubscriptionTableClient();
        var existingEvent = new EventSubscriptionEntity { PartitionKey = "2026miket", RowKey = CommonConstants.ALL };
        existingEvent.Subscribers.AddSubscription(123, 456);
        eventTable.Seed(existingEvent);

        var sut = new SubscriptionManager(teamTable, eventTable, NullLogger<SubscriptionManager>.Instance);

        await sut.RemoveSubscriptionAsync(new NotificationSubscription(123, 456, "2026miket", null), CancellationToken.None);

        var updated = await eventTable.GetEntityIfExistsAsync<EventSubscriptionEntity>("2026miket", CommonConstants.ALL, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.False(updated!.Subscribers.Exists(123, 456));
    }

    [Fact]
    public async Task GetSubscriptionsForGuildAsync_ReturnsTeamAndEventRows()
    {
        var teamTable = new InMemorySubscriptionTableClient();
        var eventTable = new InMemorySubscriptionTableClient();
        var teamEntity = new TeamSubscriptionEntity { PartitionKey = "frc33", RowKey = CommonConstants.ALL };
        var eventEntity = new EventSubscriptionEntity { PartitionKey = "2026miket", RowKey = CommonConstants.ALL };
        teamEntity.Subscribers.AddSubscription(123, 456);
        eventEntity.Subscribers.AddSubscription(123, 789);
        teamTable.Seed(teamEntity);
        eventTable.Seed(eventEntity);

        var sut = new SubscriptionManager(teamTable, eventTable, NullLogger<SubscriptionManager>.Instance);

        var results = new List<NotificationSubscription>();
        await foreach (var subscription in sut.GetSubscriptionsForGuildAsync(123, CancellationToken.None))
        {
            results.Add(subscription);
        }

        Assert.Contains(results, i => i.Team == "frc33" && i.Event == CommonConstants.ALL && i.ChannelId == 456);
        Assert.Contains(results, i => i.Event == "2026miket" && i.Team == CommonConstants.ALL && i.ChannelId == 789);
    }
}
