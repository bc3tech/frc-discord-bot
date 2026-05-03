
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

    [LoggerMessage(50, LogLevel.Warning, "Agent response contained a parenthesized opt-out hint, suggesting a possible R6 (clarifying-question-as-statement) violation. Snippet: {Snippet}")]
    internal static partial void AgentResponseContainedParentheticalOptOutHint(this ILogger logger, string Snippet);
}