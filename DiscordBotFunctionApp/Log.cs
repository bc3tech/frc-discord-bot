
namespace DiscordBotFunctionApp;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
using System.Text.Json.Nodes;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Waiting for notifications to be sent...")]
    internal static partial void WaitingForNotificationsToBeSent(this ILogger logger);

    [LoggerMessage(1, LogLevel.Trace, "Permutated results into {TeamRecordsCount} team records and {EventRecordsCount} event records")]
    internal static partial void PermutatedResultsIntoTeamRecordsCountTeamRecordsAndEventRecordsCountEventRecords(this ILogger logger, int TeamRecordsCount, int EventRecordsCount);

    [LoggerMessage(2, LogLevel.Debug, "Teams: {TeamsInMessage}, Events: {EventsInMessage}")]
    internal static partial void TeamsTeamsInMessageEventsEventsInMessage(this ILogger logger, int TeamsInMessage, int EventsInMessage);

    [LoggerMessage(3, LogLevel.Trace, "Checking {TargetTable} for {PartitionKey} / {RowKey} ...")]
    internal static partial void CheckingTargetTableForPartitionKeyRowKey(this ILogger logger, string TargetTable, string PartitionKey, string RowKey);

    [LoggerMessage(4, LogLevel.Trace, "Found record for {TargetTable} for {PartitionKey} / {RowKey}")]
    internal static partial void FoundRecordForTargetTableForPartitionKeyRowKey(this ILogger logger, string TargetTable, string PartitionKey, string RowKey);

    [LoggerMessage(5, LogLevel.Trace, "Retrieved channel {ChannelId} - '{ChannelName}'")]
    internal static partial void RetrievedChannelChannelIdChannelName(this ILogger logger, ulong ChannelId, string ChannelName);

    [LoggerMessage(6, LogLevel.Trace, "Sending notification to channel {ChannelId} - '{ChannelName}'")]
    internal static partial void SendingNotificationToChannelChannelIdChannelName(this ILogger logger, ulong ChannelId, string ChannelName);

    [LoggerMessage(7, LogLevel.Warning, "Channel {ChannelId} is not a message channel")]
    internal static partial void ChannelChannelIdIsNotAMessageChannel(this ILogger logger, ulong ChannelId);

    [LoggerMessage(8, LogLevel.Debug, "Loading Events from TBA for {EventYear}...")]
    internal static partial void LoadingEventsFromTBAForEventYear(this ILogger logger, int EventYear);

    [LoggerMessage(9, LogLevel.Trace, "Loaded {EventCount} events")]
    internal static partial void LoadedEventCountEvents(this ILogger logger, int EventCount);

    [LoggerMessage(10, LogLevel.Error, "An error occurred while loading events from the TBA API: {ErrorMessage}")]
    internal static partial void AnErrorOccurredWhileLoadingEventsFromTheTBAAPIErrorMessage(this ILogger logger, Exception exception, string ErrorMessage);

    [LoggerMessage(11, LogLevel.Warning, "Event {EventKey} not found in cache")]
    internal static partial void EventEventKeyNotFoundInCache(this ILogger logger, string EventKey);

    [LoggerMessage(12, LogLevel.Warning, "No country found for {Country}")]
    internal static partial void NoCountryFoundForCountry(this ILogger logger, string Country);

    [LoggerMessage(13, LogLevel.Trace, "First country: {Country}")]
    internal static partial void FirstCountryCountry(this ILogger logger, JsonNode? Country);

    [LoggerMessage(14, LogLevel.Debug, "{NumCountries} country(ies) returned.")]
    internal static partial void NumCountriesCountryIesReturned(this ILogger logger, int NumCountries);

    [LoggerMessage(15, LogLevel.Error, "Error getting country code for {Country}")]
    internal static partial void ErrorGettingCountryCodeForCountry(this ILogger logger, Exception exception, string Country);

    [LoggerMessage(16, LogLevel.Warning, "Failed to deserialize notification data as {NotificationType}")]
    internal static partial void FailedToDeserializeNotificationDataAsNotificationType(this ILogger logger, TbaInterop.Models.Notifications.NotificationType NotificationType);

    [LoggerMessage(17, LogLevel.Warning, "Event key is missing from notification data")]
    internal static partial void EventKeyIsMissingFromNotificationData(this ILogger logger);

    [LoggerMessage(18, LogLevel.Warning, "Failed to retrieve alliance selection data for {EventKey}")]
    internal static partial void FailedToRetrieveAllianceSelectionDataForEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(19, LogLevel.Warning, "Failed to retrieve detailed awards data for {EventKey}")]
    internal static partial void FailedToRetrieveDetailedAwardsDataForEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(20, LogLevel.Warning, "No awards found for {EventKey}")]
    internal static partial void NoAwardsFoundForEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(21, LogLevel.Trace, "Adding subscription for team {SubscriptionTeam}")]
    internal static partial void AddingSubscriptionForTeamSubscriptionTeam(this ILogger logger, uint? SubscriptionTeam);

    [LoggerMessage(22, LogLevel.Information, "Creating new subscription for team {SubscriptionTeam} and event {SubscriptionEvent}")]
    internal static partial void CreatingNewSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEvent(this ILogger logger, uint? SubscriptionTeam, string SubscriptionEvent);

    [LoggerMessage(23, LogLevel.Error, "Failed to upsert subscription for team {SubscriptionTeam} and event {SubscriptionEvent} ({StatusCode}): {Reason}")]
    internal static partial void FailedToUpsertSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEventStatusCodeReason(this ILogger logger, uint? SubscriptionTeam, string SubscriptionEvent, int StatusCode, string Reason);

    [LoggerMessage(24, LogLevel.Warning, "'All' subscription already exists for team {SubscriptionTeam}")]
    internal static partial void AllSubscriptionAlreadyExistsForTeamSubscriptionTeam(this ILogger logger, uint? SubscriptionTeam);

    [LoggerMessage(25, LogLevel.Trace, "Adding subscription for event {SubscriptionEvent}")]
    internal static partial void AddingSubscriptionForEventSubscriptionEvent(this ILogger logger, string SubscriptionEvent);

    [LoggerMessage(26, LogLevel.Information, "Creating new subscription for event {SubscriptionEvent} and team {SubscriptionTeam}")]
    internal static partial void CreatingNewSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeam(this ILogger logger, string SubscriptionEvent, string SubscriptionTeam);

    [LoggerMessage(27, LogLevel.Error, "Failed to upsert subscription for event {SubscriptionEvent} and team {SubscriptionTeam} ({StatusCode}): {Reason}")]
    internal static partial void FailedToUpsertSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeamStatusCodeReason(this ILogger logger, string SubscriptionEvent, string SubscriptionTeam, int StatusCode, string Reason);

    [LoggerMessage(28, LogLevel.Warning, "'All' subscription already exists for event {SubscriptionEvent}")]
    internal static partial void AllSubscriptionAlreadyExistsForEventSubscriptionEvent(this ILogger logger, string SubscriptionEvent);

    [LoggerMessage(29, LogLevel.Trace, "Retrieved {TeamCount} teams")]
    internal static partial void RetrievedTeamCountTeams(this ILogger logger, int TeamCount);

    [LoggerMessage(30, LogLevel.Error, "An error occurred while loading teams from the TBA API: {ErrorMessage}")]
    internal static partial void AnErrorOccurredWhileLoadingTeamsFromTheTBAAPIErrorMessage(this ILogger logger, Exception exception, string ErrorMessage);

    [LoggerMessage(31, LogLevel.Warning, "Team {TeamNumber} not found in cache")]
    internal static partial void TeamTeamNumberNotFoundInCache(this ILogger logger, string TeamNumber);

    [LoggerMessage(32, LogLevel.Warning, "Match key is missing from notification data")]
    internal static partial void MatchKeyIsMissingFromNotificationData(this ILogger logger);

    [LoggerMessage(33, LogLevel.Warning, "Failed to retrieve detailed match data for {MatchKey}")]
    internal static partial void FailedToRetrieveDetailedMatchDataForMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(34, LogLevel.Debug, "Loading Teams from TBA...")]
    internal static partial void LoadingTeamsFromTBA(this ILogger logger);
}
