﻿
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

    [LoggerMessage(2, LogLevel.Trace, "Teams: {TeamsInMessage}, Events: {EventsInMessage}")]
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

    [LoggerMessage(35, LogLevel.Debug, "Received webhook payload: {WebhookPayload}")]
    internal static partial void ReceivedWebhookPayloadWebhookPayload(this ILogger logger, string WebhookPayload);

    [LoggerMessage(36, LogLevel.Information, "Received verification message from The Blue Alliance. Key: {VerificationKey}")]
    internal static partial void ReceivedVerificationMessageFromTheBlueAllianceKeyVerificationKey(this ILogger logger, string VerificationKey);

    [LoggerMessage(37, LogLevel.Information, "Received ping message from The Blue Alliance:\nTitle: {PingTitle}\nDescription: {PingDesc}")]
    internal static partial void ReceivedPingMessageFromTheBlueAllianceTitlePingTitleDescriptionPingDesc(this ILogger logger, string PingTitle, string PingDesc);

    [LoggerMessage(38, LogLevel.Warning, "Unhandled webhook message {WebhookPayload}")]
    internal static partial void UnhandledWebhookMessageWebhookPayload(this ILogger logger, string WebhookPayload);

    [LoggerMessage(39, LogLevel.Error, "Operation cancelled.")]
    internal static partial void OperationCancelled(this ILogger logger, Exception exception);

    [LoggerMessage(40, LogLevel.Error, "Unknown/unhandled message")]
    internal static partial void UnknownUnhandledMessage(this ILogger logger);

    [LoggerMessage(41, LogLevel.Error, "Error getting data from Statbotics for {EventKey}")]
    internal static partial void ErrorGettingDataFromStatboticsForEventKey(this ILogger logger, Exception exception, string EventKey);

    [LoggerMessage(42, LogLevel.Trace, "Logging in to Discord...")]
    internal static partial void LoggingInToDiscord(this ILogger logger);

    [LoggerMessage(43, LogLevel.Information, "Discord client logged in")]
    internal static partial void DiscordClientLoggedIn(this ILogger logger);

    [LoggerMessage(44, LogLevel.Trace, "Starting Discord client...")]
    internal static partial void StartingDiscordClient(this ILogger logger);

    [LoggerMessage(45, LogLevel.Information, "Discord client started")]
    internal static partial void DiscordClientStarted(this ILogger logger);

    [LoggerMessage(46, LogLevel.Debug, "Waiting for client to be Ready")]
    internal static partial void WaitingForClientToBeReady(this ILogger logger);

    [LoggerMessage(47, LogLevel.Information, "Discord client ready")]
    internal static partial void DiscordClientReady(this ILogger logger);

    [LoggerMessage(48, LogLevel.Trace, "Currently active guilds:\n{ActiveGuilds}")]
    internal static partial void CurrentlyActiveGuildsActiveGuilds(this ILogger logger, string ActiveGuilds);

    [LoggerMessage(49, LogLevel.Debug, "Discord initialization time: {DiscordInitTime:#.000}(s)")]
    internal static partial void DiscordInitializationTimeDiscordInitTimeS(this ILogger logger, double DiscordInitTime);

    [LoggerMessage(50, LogLevel.Trace, "Interaction received: {InteractionType}\nData: {InteractionData}")]
    internal static partial void InteractionReceivedInteractionTypeDataInteractionData(this ILogger logger, Discord.InteractionType InteractionType, string InteractionData);

    [LoggerMessage(51, LogLevel.Debug, "Application command updated: {CommandName}")]
    internal static partial void ApplicationCommandUpdatedCommandName(this ILogger logger, string CommandName);

    [LoggerMessage(52, LogLevel.Debug, "Integration updated: {IntegrationName}")]
    internal static partial void IntegrationUpdatedIntegrationName(this ILogger logger, string IntegrationName);

    [LoggerMessage(53, LogLevel.Trace, "Ping from gateway - Latency = Was: {PreviousLatencyMs}ms, Now: {LatencyMs}ms")]
    internal static partial void PingFromGatewayLatencyWasPreviousLatencyMsMsNowLatencyMsMs(this ILogger logger, int PreviousLatencyMs, int LatencyMs);

    [LoggerMessage(54, LogLevel.Trace, "Loading command modules...")]
    internal static partial void LoadingCommandModules(this ILogger logger);

    [LoggerMessage(55, LogLevel.Debug, "{NumCommands} command modules loaded")]
    internal static partial void NumCommandsCommandModulesLoaded(this ILogger logger, int NumCommands);

    [LoggerMessage(56, LogLevel.Trace, "Adding modules to guilds...")]
    internal static partial void AddingModulesToGuilds(this ILogger logger);

    [LoggerMessage(57, LogLevel.Debug, "{NumCommands} commands added globally ({AvailableCommands})")]
    internal static partial void NumCommandsCommandsAddedGloballyAvailableCommands(this ILogger logger, int NumCommands, string AvailableCommands);

    [LoggerMessage(58, LogLevel.Debug, "Received message: {MessageName}")]
    internal static partial void ReceivedMessageMessageName(this ILogger logger, string MessageName);

    [LoggerMessage(59, LogLevel.Trace, "Message data: {MessageData}")]
    internal static partial void MessageDataMessageData(this ILogger logger, string MessageData);

    [LoggerMessage(60, LogLevel.Debug, "Received command: {CommandName}")]
    internal static partial void ReceivedCommandCommandName(this ILogger logger, string CommandName);

    [LoggerMessage(61, LogLevel.Trace, "Command data: {CommandData}")]
    internal static partial void CommandDataCommandData(this ILogger logger, string CommandData);

    [LoggerMessage(62, LogLevel.Debug, "Creating match score embed for {msg}")]
    internal static partial void CreatingMatchScoreEmbedForMsg(this ILogger logger, TbaInterop.Models.WebhookMessage msg);

    [LoggerMessage(63, LogLevel.Trace, "Keepalive ({arg0})")]
    internal static partial void KeepaliveArg0(this ILogger logger, DateTimeOffset arg0);

    [LoggerMessage(64, LogLevel.Trace, "Next timer schedule at: {arg0}")]
    internal static partial void NextTimerScheduleAtArg0(this ILogger logger, DateTime arg0);

    [LoggerMessage(65, LogLevel.Trace, "Creating TableClient for {Table}")]
    internal static partial void CreatingTableClientForTable(this ILogger? logger, string Table);

    [LoggerMessage(66, LogLevel.Trace, "Ensuring table {Table} exists")]
    internal static partial void EnsuringTableTableExists(this ILogger? logger, string Table);

    [LoggerMessage(67, LogLevel.Debug, "Table {Table} exists")]
    internal static partial void TableTableExists(this ILogger? logger, string Table);

    [LoggerMessage(68, LogLevel.Warning, "No embedding creator registered for message type {MessageType}")]
    internal static partial void NoEmbeddingCreatorRegisteredForMessageTypeMessageType(this ILogger logger, TbaInterop.Models.Notifications.NotificationType MessageType);

    [LoggerMessage(69, LogLevel.Trace, "Generating embeddings for webhook message type '{WebhookMessageType}'...")]
    internal static partial void GeneratingEmbeddingsForWebhookMessageTypeWebhookMessageType(this ILogger logger, TbaInterop.Models.Notifications.NotificationType WebhookMessageType);

    [LoggerMessage(70, LogLevel.Warning, "Bad data for match {MatchKey} - {MatchData}")]
    internal static partial void BadDataForMatchMatchKeyMatchData(this ILogger logger, string MatchKey, string MatchData);

    [LoggerMessage(71, LogLevel.Error, "Error getting next match for {teamKey} at {eventKey}")]
    internal static partial void ErrorGettingNextMatchForTeamKeyAtEventKey(this ILogger logger, Exception exception, string teamKey, string eventKey);

    [LoggerMessage(72, LogLevel.Debug, "Interaction already acknowledged, skipping response.")]
    internal static partial void InteractionAlreadyAcknowledgedSkippingResponse(this ILogger logger);

    [LoggerMessage(73, LogLevel.Error, "Error responding with match data for Event {EventKey} match key {MatchKey}")]
    internal static partial void ErrorRespondingWithMatchDataForEventEventKeyMatchKeyMatchKey(this ILogger logger, Exception exception, string EventKey, string MatchKey);

    [LoggerMessage(74, LogLevel.Error, "Unable to get team number from key {TeamKey}")]
    internal static partial void UnableToGetTeamNumberFromKeyTeamKey(this ILogger logger, string TeamKey);

    [LoggerMessage(75, LogLevel.Trace, "Skipping posting to subscriber {ChannelId} because we already posted to a thread in that channel.")]
    internal static partial void SkippingPostingToSubscriberChannelIdBecauseWeAlreadyPostedToAThreadInThatChannel(this ILogger logger, ulong ChannelId);

    [LoggerMessage(77, LogLevel.Warning, "Error while trying to create thread in channel {ChannelId} '{ChannelName}': {Message}")]
    internal static partial void ErrorWhileTryingToCreateThreadInChannelChannelIdChannelNameMessage(this ILogger logger, Exception exception, ulong ChannelId, string ChannelName, string Message);

    [LoggerMessage(78, LogLevel.Debug, "Prediction: {Prediction}")]
    internal static partial void PredictionPrediction(this ILogger logger, System.Text.StringBuilder Prediction);

    [LoggerMessage(79, LogLevel.Debug, "Webcasts: {Webcasts}")]
    internal static partial void WebcastsWebcasts(this ILogger logger, System.Text.StringBuilder Webcasts);

    [LoggerMessage(80, LogLevel.Debug, "Creating Upcoming Match embedding for {WebhookMessage}: {Notification}")]
    internal static partial void CreatingUpcomingMatchEmbeddingForWebhookMessageNotification(this ILogger logger, string WebhookMessage, string Notification);

    [LoggerMessage(81, LogLevel.Debug, "Rankings: {Rankings}")]
    internal static partial void RankingsRankings(this ILogger logger, string Rankings);

    [LoggerMessage(82, LogLevel.Debug, "Match Stats: {MatchStats}")]
    internal static partial void MatchStatsMatchStats(this ILogger logger, string MatchStats);

    [LoggerMessage(83, LogLevel.Debug, "Creating match score embed for {WebhookMessage}")]
    internal static partial void CreatingMatchScoreEmbedForWebhookMessage(this ILogger logger, string WebhookMessage);

    [LoggerMessage(84, LogLevel.Debug, "Score breakdown: {ScoreBreakdown}")]
    internal static partial void ScoreBreakdownScoreBreakdown(this ILogger logger, string ScoreBreakdown);

    [LoggerMessage(85, LogLevel.Debug, "Red score breakdown: {RedScoreBreakdown}")]
    internal static partial void RedScoreBreakdownRedScoreBreakdown(this ILogger logger, string RedScoreBreakdown);

    [LoggerMessage(86, LogLevel.Debug, "Blue score breakdown: {BlueScoreBreakdown}")]
    internal static partial void BlueScoreBreakdownBlueScoreBreakdown(this ILogger logger, string BlueScoreBreakdown);

    [LoggerMessage(87, LogLevel.Error, "Error responding to DM from {DMUser}")]
    internal static partial void ErrorRespondingToDMFromDMUser(this ILogger logger, Exception exception, string DMUser);

    [LoggerMessage(88, LogLevel.Trace, "Message received from gateway ({GatewayMessage})")]
    internal static partial void MessageReceivedFromGatewayGatewayMessage(this ILogger logger, Discord.WebSocket.SocketMessage GatewayMessage);

    [LoggerMessage(89, LogLevel.Error, "NullRef hit while streaming response back from agent.")]
    internal static partial void NullRefHitWhileStreamingResponseBackFromAgent(this ILogger logger, Exception exception);

    [LoggerMessage(90, LogLevel.Debug, "Found existing agent, updating with latest configuration...")]
    internal static partial void FoundExistingAgentUpdatingWithLatestConfiguration(this ILogger logger);

    [LoggerMessage(91, LogLevel.Debug, "Creating new agent...")]
    internal static partial void CreatingNewAgent(this ILogger logger);

    [LoggerMessage(92, LogLevel.Trace, "Created new agent with ID {AgentId}")]
    internal static partial void CreatedNewAgentWithIDAgentId(this ILogger logger, string AgentId);

    [LoggerMessage(93, LogLevel.Debug, "Loading Team Match Summaries PDF from Google Docs")]
    internal static partial void LoadingTeamMatchSummariesPDFFromGoogleDocs(this ILogger logger);

    [LoggerMessage(94, LogLevel.Debug, "Uploading Team Match Summaries PDF to Azure AI")]
    internal static partial void UploadingTeamMatchSummariesPDFToAzureAI(this ILogger logger);

    [LoggerMessage(95, LogLevel.Information, "Uploaded Team Match Summaries PDF to Azure AI")]
    internal static partial void UploadedTeamMatchSummariesPDFToAzureAI(this ILogger logger);

    [LoggerMessage(96, LogLevel.Debug, "{NumCommands} commands added to Guild {GuildName}({GuildId}) globally ({AvailableCommands})")]
    internal static partial void NumCommandsCommandsAddedToGuildGuildNameGuildIdGloballyAvailableCommands(this ILogger logger, int NumCommands, string GuildName, ulong GuildId, string AvailableCommands);

    [LoggerMessage(98, LogLevel.Error, "Error deleting thread for {UserName}({UserId}")]
    internal static partial void ErrorDeletingThreadForUserNameUserId(this ILogger? logger, Exception exception, string UserName, ulong UserId);

    [LoggerMessage(99, LogLevel.Error, "Discord error while trying to modify original response")]
    internal static partial void DiscordErrorWhileTryingToModifyOriginalResponse(this ILogger? logger, Exception exception);

    [LoggerMessage(100, LogLevel.Warning, "Unknown button clicked: {ButtonId}")]
    internal static partial void UnknownButtonClickedButtonId(this ILogger? logger, string ButtonId);
}
