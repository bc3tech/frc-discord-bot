namespace Common;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(17, LogLevel.Warning, "Discord write failed during {Operation}; retrying attempt {Attempt}/{MaxAttempts} after {Delay}.")]
    internal static partial void DiscordWriteRetrying(this ILogger logger, Exception exception, string Operation, int Attempt, int MaxAttempts, TimeSpan Delay);

    [LoggerMessage(18, LogLevel.Warning, "Discord write failed during {Operation}; keeping the latest buffered response and retrying on a future update.")]
    internal static partial void DiscordWriteDeferred(this ILogger logger, Exception exception, string Operation);

    [LoggerMessage(19, LogLevel.Warning, "Discord write failed during {Operation} after {MaxAttempts} attempts; keeping the latest buffered response and retrying on a future update.")]
    internal static partial void DiscordWriteDeferredAfterRetries(this ILogger logger, Exception? exception, string Operation, int MaxAttempts);

    [LoggerMessage(20, LogLevel.Warning, "Discord failure embed could not be delivered.")]
    internal static partial void DiscordFailureEmbedCouldNotBeDelivered(this ILogger logger);
}
