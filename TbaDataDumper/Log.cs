namespace TbaDataDumper;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Trace, "Uploading team {TeamKey}...")]
    internal static partial void UploadingTeamTeamKey(this ILogger logger, string TeamKey);

    [LoggerMessage(1, LogLevel.Information, "Getting events from TBA for {Year}...")]
    internal static partial void GettingEventsFromTBAForYear(this ILogger logger, int Year);

    [LoggerMessage(2, LogLevel.Information, "Got {NumEvents} events from TBA")]
    internal static partial void GotNumEventsEventsFromTBA(this ILogger logger, int NumEvents);

    [LoggerMessage(3, LogLevel.Debug, "Got {NumMatches} matches for {EventKey}")]
    internal static partial void GotNumMatchesMatchesForEventKey(this ILogger logger, int NumMatches, string EventKey);

    [LoggerMessage(4, LogLevel.Error, "Error getting matches for {EventKey}")]
    internal static partial void ErrorGettingMatchesForEventKey(this ILogger logger, Exception exception, string EventKey);

    [LoggerMessage(5, LogLevel.Error, "Error getting events for {Year}")]
    internal static partial void ErrorGettingEventsForYear(this ILogger logger, Exception exception, int Year);

    [LoggerMessage(6, LogLevel.Information, "Got {NumTeams} teams from TBA")]
    internal static partial void GotNumTeamsTeamsFromTBA(this ILogger logger, int NumTeams);

    [LoggerMessage(7, LogLevel.Error, "Error getting teams for page {PageNum}")]
    internal static partial void ErrorGettingTeamsForPagePageNum(this ILogger logger, Exception exception, int PageNum);

    [LoggerMessage(8, LogLevel.Warning, "Operation cancelled")]
    internal static partial void OperationCancelled(this ILogger logger);

    [LoggerMessage(9, LogLevel.Error, "Error")]
    internal static partial void Error(this ILogger logger, Exception exception);

    [LoggerMessage(10, LogLevel.Trace, "Uploading event {EventKey}...")]
    internal static partial void UploadingEventEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(11, LogLevel.Debug, "Uploaded event {EventKey}")]
    internal static partial void UploadedEventEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(12, LogLevel.Trace, "Uploading match {MatchKey}...")]
    internal static partial void UploadingMatchMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(13, LogLevel.Debug, "Uploaded match {MatchKey}")]
    internal static partial void UploadedMatchMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(14, LogLevel.Information, "Starting TBA Data Dumper")]
    internal static partial void StartingTBADataDumper(this ILogger logger);

    [LoggerMessage(15, LogLevel.Information, "Connecting to Storage Account (you may be prompted to log in)...")]
    internal static partial void ConnectingToStorageAccountYouMayBePromptedToLogIn(this ILogger logger);

    [LoggerMessage(16, LogLevel.Information, "Pulling events & matches since {startYear}...")]
    internal static partial void PullingEventsMatchesSinceStartYear(this ILogger logger, int startYear);

    [LoggerMessage(17, LogLevel.Debug, "Uploaded team {TeamKey}")]
    internal static partial void UploadedTeamTeamKey(this ILogger logger, string TeamKey);
}
