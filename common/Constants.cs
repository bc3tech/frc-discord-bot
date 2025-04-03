namespace Common;

using Microsoft.Extensions.Logging;

public static class Constants
{
    public const string ALL = "all";

    public static readonly Action<ILogger, Exception> LogErrorException = LoggerMessage.Define(LogLevel.Error, default, string.Empty);
    public static readonly Action<ILogger, Exception> LogWarningException = LoggerMessage.Define(LogLevel.Warning, default, string.Empty);
}
