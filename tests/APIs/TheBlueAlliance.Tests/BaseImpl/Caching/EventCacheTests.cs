namespace TheBlueAlliance.Tests.BaseImpl.Caching;
using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net;
using System.Reflection;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.BaseImpl.Caching;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

public class EventCacheTests : TestWithLogger
{
    private static readonly Event _utEvent = new Event(
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
    public EventCacheTests() : base(typeof(EventCache))
    {
        this.Mocker.WithSelfMock<IEventApi>();
        this.Mocker.Use(new Meter(nameof(EventCacheTests)));

        this.Mocker.With<IEventCache, EventCache>();

        ((ConcurrentDictionary<string, Event>)typeof(EventCache).GetField("_events", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null)).Clear();
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadEventsFromApi()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventsByYearAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_utEvent]);

        // Act
        var cache = this.Mocker.Get<IEventCache>();
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

        // Act
        var cache = this.Mocker.Get<IEventCache>();
        await cache.InitializeAsync(CancellationToken.None).ConfigureAwait(false);

        var result = cache[_utEvent.Key];

        // Assert
        Assert.Equal(_utEvent, result);
    }

    [Fact]
    public void Indexer_ShouldFetchEventFromApiIfNotInCache()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEvent(_utEvent.Key, It.IsAny<string>()))
            .Returns(_utEvent);

        var cache = this.Mocker.Get<IEventCache>();

        // Act
        Assert.Empty(cache.AllEvents);
        var result = cache[_utEvent.Key];

        // Assert
        Assert.Equal(_utEvent, result);
        Assert.Contains(_utEvent.Key, cache.AllEvents.Keys);
    }

    [Fact]
    public void Indexer_ShouldThrowKeyNotFoundExceptionIfEventNotFound()
    {
        // Arrange
        var eventKey = "nonexistent";
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEvent(eventKey, It.IsAny<string>()))
            .Returns((Event)null);

        var cache = this.Mocker.Get<IEventCache>();
        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => cache[eventKey]);
    }

    [Fact]
    public async Task InitializeAsync_ShouldHandleApiExceptionsAsync()
    {
        // Arrange
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventsByYearAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API error"));

        var cache = this.Mocker.Get<IEventCache>();

        // Act & Assert
        await AssertDebugExceptionAsync(cache.InitializeAsync(CancellationToken.None).AsTask());
        this.Logger.Verify(LogLevel.Error);
    }
}
