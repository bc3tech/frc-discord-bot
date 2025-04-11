namespace TheBlueAlliance.Tests.Caching;
using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Reflection;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit.Abstractions;

public class EventCacheTests : TestWithLogger
{
    private static readonly Event _utEvent = new(
                address: "123 Main St",
                city: "Test City",
                country: "Test Country",
                district: new DistrictList("test", "Test District", "2025test", 2025),
                divisionKeys: ["division1"],
                endDate: new DateOnly(2025, 12, 31),
                eventCode: "test",
                eventType: 1,
                eventTypeString: "Regional",
                firstEventCode: "TEST",
                firstEventId: "12345",
                gmapsPlaceId: "place123",
                gmapsUrl: "http://maps.google.com/place123",
                key: "2025test",
                lat: 40.7128,
                lng: -74.0060,
                locationName: "Test Location",
                name: "Test Event",
                parentEventKey: "parent2025",
                playoffType: 1,
                playoffTypeString: "Elimination",
                postalCode: "12345",
                shortName: "Test Event",
                startDate: new DateOnly(2025, 1, 1),
                stateProv: "Test State",
                timezone: "UTC",
                webcasts: [new Webcast("channel", Webcast.TypeEnum.Youtube)],
                website: "http://testevent.com",
                week: 1,
                year: 2025
            );
    public EventCacheTests(ITestOutputHelper outputHelper) : base(typeof(EventCache), outputHelper)
    {
        this.Mocker.WithSelfMock<IEventApi>();
        this.Mocker.Use(new Meter(nameof(EventCacheTests)));

        this.Mocker.With<EventCache>();

        ((ConcurrentDictionary<string, Event>)typeof(EventCache).GetField("_events", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Clear();
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadEventsFromApi()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventsByYearAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_utEvent]);

        // Act
        var cache = this.Mocker.Get<EventCache>();
        await cache.InitializeAsync(CancellationToken.None);

        // Assert
        Assert.Contains("2025test", cache.AllEvents.Keys);
        this.Logger.Verify(LogLevel.Information, "Cached 1 event(s) from TBA");
    }

    [Fact]
    public async Task Indexer_ShouldReturnEventFromCacheAsync()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventsByYearAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_utEvent]);
        var cache = this.Mocker.Get<EventCache>();

        // Act & Assert
        await cache.InitializeAsync(CancellationToken.None).ConfigureAwait(true);
        Assert.Contains(_utEvent.Key, cache.AllEvents.Keys);

        var result = cache[_utEvent.Key];
        Assert.Equal(_utEvent, result);
        this.Mocker.GetMock<IEventApi>()
            .Verify(api => api.GetEvent(_utEvent.Key, It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Indexer_ShouldFetchEventFromApiIfNotInCache()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEvent(_utEvent.Key, It.IsAny<string>()))
            .Returns(_utEvent);

        var cache = this.Mocker.Get<EventCache>();

        // Act & Assert
        Assert.Empty(cache.AllEvents);

        var result = cache[_utEvent.Key];
        Assert.Equal(_utEvent, result);

        Assert.Contains(_utEvent.Key, cache.AllEvents.Keys);
        this.Mocker.GetMock<IEventApi>()
            .Verify(api => api.GetEvent(_utEvent.Key, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Indexer_ShouldThrowIfEventNotFound()
    {
        // Arrange
        var eventKey = "nonexistent";
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Event?)null);
        var cache = this.Mocker.Get<EventCache>();

        // Act & Assert
        var ex = Assert.Throws<EventNotFoundException>(() => cache[eventKey]);
        Assert.Equal($"No event with key {eventKey} could be found", ex.Message);
        Assert.Equal(eventKey, ex.Data["EventKey"]);
    }

    [Fact]
    public void InitializeAsync_ShouldHandleApiExceptions()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventsByYearAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<HttpRequestException>();

        var cache = this.Mocker.Get<EventCache>();

        // Act & Assert
        DebugHelper.AssertDebugException(cache.InitializeAsync(CancellationToken.None).AsTask());
        this.Logger.Verify(LogLevel.Error);
        Assert.Empty(cache.AllEvents);
    }
}
