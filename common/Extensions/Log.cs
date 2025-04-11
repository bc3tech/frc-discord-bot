namespace Common.Extensions;
using Microsoft.Extensions.Logging;

using System;
using System.Runtime.CompilerServices;

public static class LogExtensions
{
    public static IDisposable CreateMethodScope(this ILogger logger, [CallerMemberName] string? methodName = null) => logger.BeginScope(Throws.IfNullOrWhiteSpace(methodName))!;
}