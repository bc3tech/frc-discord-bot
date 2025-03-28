namespace TestCommon;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit.Abstractions;

public abstract class TestWithLogger : Test
{
    protected TestLogger Logger { get; }
    protected TestWithLogger(ITestOutputHelper outputHelper) : base()
    {
        this.Logger = new(outputHelper);

        this.Mocker.WithSelfMock<ILoggerFactory>();
        this.Mocker.Use<ILogger>(this.Logger);

        this.Mocker.GetMock<ILoggerFactory>()
            .Setup(l => l.CreateLogger(It.IsAny<string>()))
            .Returns(this.Logger);
    }

    protected TestWithLogger(Type loggerType, ITestOutputHelper outputHelper) : base()
    {
        this.Logger = (TestLogger)Activator.CreateInstance(typeof(TestLogger<>).MakeGenericType(loggerType), outputHelper)!;

        this.Mocker.WithSelfMock<ILoggerFactory>();
        this.Mocker.Use<ILogger>(this.Logger);
        this.Mocker.Use(typeof(ILogger<>).MakeGenericType(loggerType), this.Logger);

        this.Mocker.GetMock<ILoggerFactory>()
            .Setup(l => l.CreateLogger(It.IsAny<string>()))
            .Returns(this.Logger);
    }
}
