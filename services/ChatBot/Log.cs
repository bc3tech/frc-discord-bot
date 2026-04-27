
namespace ChatBot;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
#pragma warning restore CS8019

static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "Unknown chat reset button clicked: {ButtonId}")]
    internal static partial void UnknownChatResetButtonClickedButtonId(this ILogger logger, string ButtonId);

    [LoggerMessage(5, LogLevel.Error, "Error deleting chat thread for user {UserName} ({UserId})")]
    internal static partial void ErrorDeletingChatThreadForUserUserNameUserId(this ILogger logger, Exception exception, string UserName, ulong UserId);

    [LoggerMessage(8, LogLevel.Warning, "Http API tool call failed for {ClientName} {RequestPath} with status {StatusCode}")]
    internal static partial void HttpAPIToolCallFailedForClientNameRequestPathWithStatusStatusCode(this ILogger logger, string ClientName, Uri? RequestPath, int StatusCode);

    [LoggerMessage(33, LogLevel.Debug, "Http API tool call for {ClientName} {RequestPath} failed after {ElapsedMilliseconds}ms.")]
    internal static partial void HttpAPIToolCallFailedAfterElapsedMilliseconds(this ILogger logger, string ClientName, Uri? RequestPath, double ElapsedMilliseconds);

    [LoggerMessage(34, LogLevel.Debug, "Http API tool call for {ClientName} returned failure response snippet: {ResponseSnippet}")]
    internal static partial void HttpAPIToolCallFailedResponseSnippet(this ILogger logger, string ClientName, string ResponseSnippet);

    [LoggerMessage(35, LogLevel.Information, "Statbotics query parameter validation rejected call to {Path} with query '{Query}'")]
    internal static partial void StatboticsQueryValidationRejected(this ILogger logger, string Path, string Query);

    [LoggerMessage(36, LogLevel.Information, "Statbotics returned HTTP 500 for {Path} with query '{Query}'; rewrote response with constraint guidance")]
    internal static partial void StatboticsApi500Rewritten(this ILogger logger, string Path, string Query);
}