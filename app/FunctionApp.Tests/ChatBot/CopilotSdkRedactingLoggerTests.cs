namespace FunctionApp.Tests;

using global::ChatBot.Copilot;

using Microsoft.Extensions.Logging;

public sealed class CopilotSdkRedactingLoggerTests
{
    [Fact]
    public void LogWhenPayloadContainsBearerTokenRedactsFormattedMessageAndStructuredState()
    {
        CollectingLogger innerLogger = new();
        CopilotSdkRedactingLogger logger = new(innerLogger);
        string payload = "{\"provider\":{\"bearerToken\":\"secret-token-value\",\"authorization\":\"Bearer abc123\"}}";
        IReadOnlyList<KeyValuePair<string, object?>> state =
        [
            new("Payload", payload),
            new("{OriginalFormat}", "session.resume payload {Payload}"),
        ];

        logger.Log(
            LogLevel.Debug,
            new EventId(2046, "redaction-test"),
            state,
            exception: null,
            static (values, _) => $"session.resume payload {values[0].Value}");

        Assert.NotNull(innerLogger.LastEntry);
        Assert.DoesNotContain("secret-token-value", innerLogger.LastEntry!.RenderedMessage, StringComparison.Ordinal);
        Assert.DoesNotContain("Bearer abc123", innerLogger.LastEntry.RenderedMessage, StringComparison.Ordinal);
        Assert.Contains("[REDACTED]", innerLogger.LastEntry.RenderedMessage, StringComparison.Ordinal);

        string structuredState = string.Join(" | ", innerLogger.LastEntry.State.Select(static pair => $"{pair.Key}={pair.Value}"));
        Assert.DoesNotContain("secret-token-value", structuredState, StringComparison.Ordinal);
        Assert.DoesNotContain("Bearer abc123", structuredState, StringComparison.Ordinal);
        Assert.Contains("[REDACTED]", structuredState, StringComparison.Ordinal);
    }

    private sealed class CollectingLogger : ILogger
    {
        public LogEntry? LastEntry { get; private set; }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            IReadOnlyList<KeyValuePair<string, object?>> structuredState = state is IEnumerable<KeyValuePair<string, object?>> pairs
                ? [.. pairs]
                : [];

            this.LastEntry = new(formatter(state, exception), structuredState);
        }
    }

    private sealed record LogEntry(string RenderedMessage, IReadOnlyList<KeyValuePair<string, object?>> State);

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}
