namespace TestCommon;

using Moq;
using Moq.AutoMock;

public abstract class Test
{
    protected AutoMockerWithKeyedServiceSupport Mocker { get; } = new();

    protected Mock<TimeProvider> TimeMock { get; }

    protected Test()
    {
        this.TimeMock = new();
        this.TimeMock.SetupGet(tp => tp.LocalTimeZone).Returns(TimeZoneInfo.Utc);
        this.TimeMock.Setup(tp => tp.GetUtcNow()).Returns(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        this.Mocker.Use(this.TimeMock);
    }

    protected static void AssertDebugException(Action action, string? messageContents = null)
    {
        try
        {
            action();
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

    protected static async Task AssertDebugExceptionAsync(Task task, string? messageContents = null)
    {
        try
        {
            await task;
            if (task.Exception is not null)
            {
                throw task.Exception;
            }
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

}
