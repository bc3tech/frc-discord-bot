namespace TestCommon;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;

public class TestLogger : ILogger
{
    protected ConcurrentStack<(LogLevel logLevel, string message)> CollectedLogMessages { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => this.CollectedLogMessages.Push((logLevel, formatter(state, exception)));
    public void Verify(LogLevel logLevel, string? message = null) => Assert.True(message is null ? this.CollectedLogMessages.Any(i => i.logLevel == logLevel) : this.CollectedLogMessages.Any(i => i == (logLevel, message)));

    public IReadOnlyCollection<(LogLevel logLevel, string message)> LogMessages => this.CollectedLogMessages;
}

public sealed class TestLogger<T> : TestLogger, ILogger<T> { }
