namespace TestCommon;

using Microsoft.Extensions.Logging;

using Moq;

public abstract class TestWithLogger : Test
{
    protected TestLogger Logger { get; } = new();
    protected TestWithLogger() : base()
    {
        this.Mocker.WithSelfMock<ILoggerFactory>();
        this.Mocker.Use<ILogger>(this.Logger);

        this.Mocker.GetMock<ILoggerFactory>()
            .Setup(l => l.CreateLogger(It.IsAny<string>()))
            .Returns(this.Logger);
    }

    protected TestWithLogger(Type loggerType) : base()
    {
        this.Logger = (TestLogger)Activator.CreateInstance(typeof(TestLogger<>).MakeGenericType(loggerType))!;

        this.Mocker.WithSelfMock<ILoggerFactory>();
        this.Mocker.Use<ILogger>(this.Logger);
        this.Mocker.Use(typeof(ILogger<>).MakeGenericType(loggerType), this.Logger);

        this.Mocker.GetMock<ILoggerFactory>()
            .Setup(l => l.CreateLogger(It.IsAny<string>()))
            .Returns(this.Logger);
    }
}
