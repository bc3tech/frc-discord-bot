namespace ChatBot.Copilot;

using Microsoft.Extensions.Logging;

using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

internal sealed partial class CopilotSdkRedactingLogger(ILogger innerLogger) : ILogger
{
    private readonly ILogger _innerLogger = innerLogger;

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => this._innerLogger.BeginScope(SanitizeScopeState(state));

    public bool IsEnabled(LogLevel logLevel) => this._innerLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        if (!this._innerLogger.IsEnabled(logLevel))
        {
            return;
        }

        string renderedMessage = Redact(formatter(state, exception));
        SanitizedLogState sanitizedState = new(SanitizeStructuredState(state), renderedMessage);
        this._innerLogger.Log(logLevel, eventId, sanitizedState, exception, static (sanitized, _) => sanitized.RenderedMessage);
    }

    private static IReadOnlyList<KeyValuePair<string, object?>> SanitizeStructuredState<TState>(TState state)
    {
        if (state is not IEnumerable<KeyValuePair<string, object?>> pairs)
        {
            return [];
        }

        List<KeyValuePair<string, object?>> sanitized = [];
        foreach (KeyValuePair<string, object?> pair in pairs)
        {
            sanitized.Add(new(pair.Key, SanitizeValue(pair.Value)));
        }

        return sanitized.AsReadOnly();
    }

    private static object SanitizeScopeState<TState>(TState state)
        => state is IEnumerable<KeyValuePair<string, object?>> pairs
            ? SanitizeEnumerable(pairs)
            : Redact(state?.ToString() ?? string.Empty);

    private static IReadOnlyList<KeyValuePair<string, object?>> SanitizeEnumerable(IEnumerable<KeyValuePair<string, object?>> pairs)
    {
        List<KeyValuePair<string, object?>> sanitized = [];
        foreach (KeyValuePair<string, object?> pair in pairs)
        {
            sanitized.Add(new(pair.Key, SanitizeValue(pair.Value)));
        }

        return sanitized.AsReadOnly();
    }

    private static object? SanitizeValue(object? value)
        => value switch
        {
            null => null,
            string text => Redact(text),
            IEnumerable<KeyValuePair<string, object?>> nestedPairs => SanitizeEnumerable(nestedPairs),
            IEnumerable enumerable when value is not string => SanitizeSequence(enumerable),
            _ => value,
        };

    private static IReadOnlyList<object?> SanitizeSequence(IEnumerable values)
    {
        List<object?> sanitized = [];
        foreach (object? value in values)
        {
            sanitized.Add(SanitizeValue(value));
        }

        return new ReadOnlyCollection<object?>(sanitized);
    }

    private static string Redact(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        string redacted = BearerTokenJsonPattern().Replace(value, "\"bearerToken\":\"[REDACTED]\"");
        redacted = AuthorizationBearerJsonPattern().Replace(redacted, "\"authorization\":\"Bearer [REDACTED]\"");
        redacted = BearerTokenAssignmentPattern().Replace(redacted, "${prefix}[REDACTED]");
        return redacted;
    }

    [GeneratedRegex("\"bearerToken\"\\s*:\\s*\"[^\"]+\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BearerTokenJsonPattern();

    [GeneratedRegex("\"authorization\"\\s*:\\s*\"Bearer\\s+[^\"]+\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AuthorizationBearerJsonPattern();

    [GeneratedRegex("(?<prefix>bearerToken\\s*[=:]\\s*)(?<value>\\S+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BearerTokenAssignmentPattern();

    private sealed class SanitizedLogState(IReadOnlyList<KeyValuePair<string, object?>> values, string renderedMessage)
        : IReadOnlyList<KeyValuePair<string, object?>>
    {
        public string RenderedMessage { get; } = renderedMessage;

        public int Count => values.Count;

        public KeyValuePair<string, object?> this[int index] => values[index];

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
    }
}
