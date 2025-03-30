namespace TestCommon;

using Moq;

using System.Diagnostics.Metrics;

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
    }

    protected static void AssertDebugException(Action action, string? messageContents = null)
    {
        try
        {
            action();
            Assert.Fail("DebugAssertException exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }
    }

    protected static T AssertDebugException<T>(Func<T> func, string? messageContents = null)
    {
        try
        {
            func();
            Assert.Fail("DebugAssertException exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }

        DebugHelper.IgnoreDebugAsserts();
        return func();
    }

    protected static async Task AssertDebugExceptionAsync(Task task, string? messageContents = null)
    {
        try
        {
            await task.ConfigureAwait(false);
            if (task.Exception is not null)
            {
                throw task.Exception;
            }
            Assert.Fail("DebugAssertException exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }
    }

    protected static async Task<T> AssertDebugExceptionAsync<T>(Task<T> task, string? messageContents = null)
    {
        try
        {
            await task.ConfigureAwait(false);
            if (task.Exception is not null)
            {
                throw task.Exception;
            }
            Assert.Fail("DebugAssertException exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }

        DebugHelper.IgnoreDebugAsserts();
        return await task.ConfigureAwait(false);
    }
}
