namespace TestCommon;

using Moq;

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit.Sdk;

public abstract class Test
{
    protected AutoMockerWithKeyedServiceSupport Mocker { get; } = new();

    protected Mock<TimeProvider> TimeMock { get; }

    protected Test()
    {
        this.TimeMock = new Mock<TimeProvider>();
        this.TimeMock.Setup(i => i.GetUtcNow())
            .Returns(TimeProvider.System.GetUtcNow);
        this.TimeMock.SetupGet(i => i.LocalTimeZone)
            .Returns(TimeProvider.System.LocalTimeZone);
        this.TimeMock.SetupGet(i => i.TimestampFrequency)
            .Returns(() => TimeProvider.System.TimestampFrequency);
        this.TimeMock.Setup(i => i.GetTimestamp())
            .Returns(TimeProvider.System.GetTimestamp);
        this.TimeMock.Setup(i => i.CreateTimer(It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Returns((TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period) =>
                TimeProvider.System.CreateTimer(callback, state, dueTime, period));

        this.Mocker.Use(this.TimeMock);
        this.Mocker.Use(new Meter("UnitTestMeter"));

        this.Mocker.With<EventCache>();
        this.Mocker.With<TeamCache>();
    }


    protected static IDisposable RequireClearedEventCache()
    {
        var o = new EventCacheMutex();
        ((ConcurrentDictionary<string, Event>)typeof(EventCache).GetField("_events", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!).Clear();
        return o;
    }

    protected static IDisposable BlockForEventCacheAccess() => new EventCacheMutex();

    private sealed class EventCacheMutex : IDisposable
    {
        private static readonly AutoResetEvent _mutex = new(true);

        public EventCacheMutex() => _mutex.WaitOne();

        public void Dispose() => _mutex.Set();
    }

    protected static IDisposable RequireClearedTeamCache()
    {
        var o = new TeamCacheMutex();
        ((ConcurrentDictionary<string, Team>)typeof(TeamCache).GetField("_teams", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!).Clear();
        return o;
    }

    protected static IDisposable BlockForTeamCacheAccess() => new TeamCacheMutex();

    private sealed class TeamCacheMutex : IDisposable
    {
        private static readonly AutoResetEvent _mutex = new(true);

        public TeamCacheMutex() => _mutex.WaitOne();

        public void Dispose() => _mutex.Set();
    }
}
