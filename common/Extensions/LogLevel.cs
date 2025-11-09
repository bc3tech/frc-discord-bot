namespace Common.Extensions;

using Discord;

using Microsoft.Extensions.Logging;

public static class LogLevelExtensions
{
    public static LogLevel ToLogLevel(this LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.None
        };
    }

    public static LogSeverity ToLogSeverity(this LogLevel level)
    {
        return level switch
        {
            LogLevel.Critical => LogSeverity.Critical,
            LogLevel.Error => LogSeverity.Error,
            LogLevel.Warning => LogSeverity.Warning,
            LogLevel.Information => LogSeverity.Info,
            LogLevel.Trace => LogSeverity.Verbose,
            LogLevel.Debug => LogSeverity.Debug,
            _ => LogSeverity.Info
        };
    }
}
