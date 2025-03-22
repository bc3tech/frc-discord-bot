namespace DiscordBotFunctionApp;

using DiscordBotFunctionApp.TbaInterop.Models;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
using System.Text.Json.Nodes;

using TheBlueAlliance.Model;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Debug, "Waiting for notifications to be sent...")]
    internal static partial void WaitingForNotificationsToBeSent(this ILogger logger);

    [LoggerMessage(1, LogLevel.Trace, "Permutated results into {TeamRecordsCount} team records and {EventRecordsCount} event records")]
    internal static partial void PermutatedResultsIntoTeamRecordsCountTeamRecordsAndEventRecordsCountEventRecords(this ILogger logger, int TeamRecordsCount, int EventRecordsCount);

    [LoggerMessage(2, LogLevel.Trace, "Teams: {TeamsInMessage}, Events: {EventsInMessage}")]
    internal static partial void TeamsTeamsInMessageEventsEventsInMessage(this ILogger logger, int TeamsInMessage, int EventsInMessage);

    [LoggerMessage(3, LogLevel.Trace, "Checking {TargetTable}...")]
    internal static partial void CheckingTargetTable(this ILogger logger, string TargetTable);

    [LoggerMessage(4, LogLevel.Trace, "Found record")]
    internal static partial void FoundRecord(this ILogger logger);

    [LoggerMessage(5, LogLevel.Trace, "Retrieved channel {ChannelId} - '{ChannelName}'")]
    internal static partial void RetrievedChannelChannelIdChannelName(this ILogger logger, ulong ChannelId, string ChannelName);

    [LoggerMessage(6, LogLevel.Trace, "Sending notification to channel {ChannelId} - '{ChannelName}'")]
    internal static partial void SendingNotificationToChannelChannelIdChannelName(this ILogger logger, ulong ChannelId, string ChannelName);

    [LoggerMessage(7, LogLevel.Warning, "Channel {ChannelId} is not a message channel")]
    internal static partial void ChannelChannelIdIsNotAMessageChannel(this ILogger logger, ulong ChannelId);

    [LoggerMessage(8, LogLevel.Debug, "Loading Events from TBA for {EventYear}...")]
    internal static partial void LoadingEventsFromTBAForEventYear(this ILogger logger, int EventYear);

    [LoggerMessage(9, LogLevel.Trace, "Retrieved {EventCount} events")]
    internal static partial void RetrievedEventCountEvents(this ILogger logger, int EventCount);

    [LoggerMessage(10, LogLevel.Error, "An error occurred while loading events from the TBA API: {ErrorMessage}")]
    internal static partial void AnErrorOccurredWhileLoadingEventsFromTheTBAAPIErrorMessage(this ILogger logger, Exception exception, string ErrorMessage);

    [LoggerMessage(11, LogLevel.Debug, "Event {EventKey} not found in cache, fetching...")]
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
    internal static partial void AddingSubscriptionForTeamSubscriptionTeam(this ILogger logger, string SubscriptionTeam);

    [LoggerMessage(22, LogLevel.Information, "Creating new subscription for team {SubscriptionTeam} and event {SubscriptionEvent}")]
    internal static partial void CreatingNewSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEvent(this ILogger logger, string SubscriptionTeam, string SubscriptionEvent);

    [LoggerMessage(23, LogLevel.Error, "Failed to upsert subscription for team {SubscriptionTeam} and event {SubscriptionEvent} ({StatusCode}): {Reason}")]
    internal static partial void FailedToUpsertSubscriptionForTeamSubscriptionTeamAndEventSubscriptionEventStatusCodeReason(this ILogger logger, string SubscriptionTeam, string SubscriptionEvent, int StatusCode, string Reason);

    [LoggerMessage(24, LogLevel.Warning, "'All' subscription already exists for team {SubscriptionTeam}")]
    internal static partial void AllSubscriptionAlreadyExistsForTeamSubscriptionTeam(this ILogger logger, string SubscriptionTeam);

    [LoggerMessage(25, LogLevel.Trace, "Adding subscription for event {SubscriptionEvent}")]
    internal static partial void AddingSubscriptionForEventSubscriptionEvent(this ILogger logger, string SubscriptionEvent);

    [LoggerMessage(26, LogLevel.Information, "Creating new subscription for event {SubscriptionEvent}")]
    internal static partial void CreatingNewSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeam(this ILogger logger, string SubscriptionEvent);

    [LoggerMessage(27, LogLevel.Error, "Failed to upsert subscription for event {SubscriptionEvent} ({StatusCode}): {Reason}")]
    internal static partial void FailedToUpsertSubscriptionForEventSubscriptionEventAndTeamSubscriptionTeamStatusCodeReason(this ILogger logger, string SubscriptionEvent, int StatusCode, string Reason);

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

    [LoggerMessage(61, LogLevel.Trace, "Command data: {CommandData}")]
    internal static partial void CommandDataCommandData(this ILogger logger, string CommandData);

    [LoggerMessage(62, LogLevel.Debug, "Creating match score embed")]
    internal static partial void CreatingMatchScoreEmbed(this ILogger logger);

    [LoggerMessage(63, LogLevel.Trace, "Keepalive ({arg0})")]
    internal static partial void KeepaliveArg0(this ILogger logger, DateTimeOffset arg0);

    [LoggerMessage(64, LogLevel.Trace, "Next timer schedule at: {arg0}")]
    internal static partial void NextTimerScheduleAtArg0(this ILogger logger, DateTime arg0);

    [LoggerMessage(65, LogLevel.Trace, "Creating TableClient for {Table}")]
    internal static partial void CreatingTableClientForTable(this ILogger logger, string Table);

    [LoggerMessage(66, LogLevel.Trace, "Ensuring table {Table} exists")]
    internal static partial void EnsuringTableTableExists(this ILogger logger, string Table);

    [LoggerMessage(67, LogLevel.Debug, "Table {Table} exists")]
    internal static partial void TableTableExists(this ILogger logger, string Table);

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

    [LoggerMessage(81, LogLevel.Trace, "Rankings: {Rankings}")]
    internal static partial void RankingsRankings(this ILogger logger, string Rankings);

    [LoggerMessage(82, LogLevel.Trace, "Match Stats: {MatchStats}")]
    internal static partial void MatchStatsMatchStats(this ILogger logger, string MatchStats);

    [LoggerMessage(83, LogLevel.Debug, "Creating match score embed for match {Match}")]
    internal static partial void CreatingMatchScoreEmbedForMatch(this ILogger logger, string? Match);

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

    [LoggerMessage(94, LogLevel.Trace, "Uploading Team Match Summaries PDF to Azure AI")]
    internal static partial void UploadingTeamMatchSummariesPDFToAzureAI(this ILogger logger);

    [LoggerMessage(95, LogLevel.Information, "Uploaded Team Match Summaries PDF to Azure AI Vector Store, waiting for it to be ready...")]
    internal static partial void UploadedTeamMatchSummariesPDFToAzureAI(this ILogger logger);

    [LoggerMessage(96, LogLevel.Debug, "{NumCommands} commands added to Guild {GuildName}({GuildId}) globally ({AvailableCommands})")]
    internal static partial void NumCommandsCommandsAddedToGuildGuildNameGuildIdGloballyAvailableCommands(this ILogger logger, int NumCommands, string GuildName, ulong GuildId, string AvailableCommands);

    [LoggerMessage(98, LogLevel.Error, "Error deleting thread for {UserName}({UserId}")]
    internal static partial void ErrorDeletingThreadForUserNameUserId(this ILogger logger, Exception exception, string UserName, ulong UserId);

    [LoggerMessage(99, LogLevel.Error, "Discord error while trying to modify original response")]
    internal static partial void DiscordErrorWhileTryingToModifyOriginalResponse(this ILogger logger, Exception exception);

    [LoggerMessage(100, LogLevel.Warning, "Unknown button clicked: {ButtonId}")]
    internal static partial void UnknownButtonClickedButtonId(this ILogger logger, string ButtonId);

    [LoggerMessage(101, LogLevel.Debug, "Match Summaries PDF already uploaded and is the same size as the existing file. No need to re-upload.")]
    internal static partial void MatchSummariesPDFAlreadyUploadedAndIsTheSameSizeAsTheExistingFileNoNeedToReUpload(this ILogger logger);

    [LoggerMessage(102, LogLevel.Debug, "Didn't find the target agent to update. Waiting for it to be available...")]
    internal static partial void DidnTFindTheTargetAgentToUpdateWaitingForItToBeAvailable(this ILogger logger);

    [LoggerMessage(103, LogLevel.Information, "Running Match Summary doc update...")]
    internal static partial void RunningMatchSummaryDocUpdate(this ILogger logger);

    [LoggerMessage(104, LogLevel.Trace, "No tracking record found in table, uploading new file")]
    internal static partial void NoTrackingRecordFoundInTableUploadingNewFile(this ILogger logger);

    [LoggerMessage(105, LogLevel.Trace, "Found tracking record in table, checking if reupload is needed")]
    internal static partial void FoundTrackingRecordInTableCheckingIfReuploadIsNeeded(this ILogger logger);

    [LoggerMessage(106, LogLevel.Debug, "Not running on Friday, Saturday, or Sunday. Skipping match summaries update.")]
    internal static partial void NotRunningOnFridaySaturdayOrSundaySkippingMatchSummariesUpdate(this ILogger logger);

    [LoggerMessage(107, LogLevel.Debug, "Outside normal match hours, skipping update.")]
    internal static partial void OutsideNormalMatchHoursSkippingUpdate(this ILogger logger);

    [LoggerMessage(108, LogLevel.Error, "Failed to register commands to guild {GuildName}({GuildId}) after 10 attempts. Please check the logs for more details.")]
    internal static partial void FailedToRegisterCommandsToGuildGuildNameGuildIdAfter10AttemptsPleaseCheckTheLogsForMoreDetails(this ILogger logger, string GuildName, ulong GuildId);

    [LoggerMessage(109, LogLevel.Warning, "[{RetryAttempt}] Operation cancelled while trying to register commands to guild {GuildName}({GuildId}) - assuming throttling, waiting 1m for retry.")]
    internal static partial void RetryAttemptOperationCancelledWhileTryingToRegisterCommandsToGuildGuildNameGuildIdAssumingThrottlingWaiting1mForRetry(this ILogger logger, int RetryAttempt, string GuildName, ulong GuildId);

    [LoggerMessage(110, LogLevel.Debug, "Removed {NumCommands} from guild {GuildName} ({GuildId})")]
    internal static partial void RemovedNumCommandsFromGuildGuildNameGuildId(this ILogger logger, int NumCommands, string GuildName, ulong GuildId);

    [LoggerMessage(111, LogLevel.Debug, "Deleted {GuildCommands} commands from guild {GuildName}({GuildId})")]
    internal static partial void DeletedGuildCommandsCommandsFromGuildGuildNameGuildId(this ILogger logger, int GuildCommands, string GuildName, ulong GuildId);

    [LoggerMessage(112, LogLevel.Trace, "Still waiting for file to be processed...")]
    internal static partial void StillWaitingForFileToBeProcessed(this ILogger logger);

    [LoggerMessage(113, LogLevel.Information, "File {FileId} has been processed successfully.")]
    internal static partial void FileFileIdHasBeenProcessedSuccessfully(this ILogger logger, string FileId);

    [LoggerMessage(114, LogLevel.Error, "Failed to uplooad Match Summaries file: {FileId} with status: {Status}")]
    internal static partial void FailedToUplooadMatchSummariesFileFileIdWithStatusStatus(this ILogger logger, string FileId, Azure.AI.Projects.VectorStoreFileStatus Status);

    [LoggerMessage(115, LogLevel.Warning, "Unknown file status: {Status}")]
    internal static partial void UnknownFileStatusStatus(this ILogger logger, Azure.AI.Projects.VectorStoreFileStatus Status);

    [LoggerMessage(116, LogLevel.Warning, "Vector store processing is taking a VERY long time. This may be a sign of a problem. File: {FileId}")]
    internal static partial void VectorStoreProcessingIsTakingAVERYLongTimeThisMayBeASignOfAProblemFileFileId(this ILogger logger, string FileId);

    [LoggerMessage(117, LogLevel.Error, "Vector store processing has taken over 10 minutes. Bailing. File: {FileId}")]
    internal static partial void VectorStoreProcessingHasTakenOver10MinutesBailingFileFileId(this ILogger logger, string FileId);

    [LoggerMessage(118, LogLevel.Trace, "Response: {Response}")]
    internal static partial void ResponseResponse(this ILogger logger, string Response);

    [LoggerMessage(119, LogLevel.Warning, "Didn't get a MessageChannel from thread {ThreadId}")]
    internal static partial void DidnTGetAMessageChannelFromThreadThreadId(this ILogger logger, ulong ThreadId);

    [LoggerMessage(120, LogLevel.Debug, "Creating Alliance Selection embed")]
    internal static partial void CreatingAllianceSelectionEmbed(this ILogger logger);

    [LoggerMessage(121, LogLevel.Trace, "Already processed alliance selection for event {EventKey}")]
    internal static partial void AlreadyProcessedAllianceSelectionForEventEventKey(this ILogger logger, string EventKey);

    [LoggerMessage(122, LogLevel.Warning, "We could not find the match in the schedule using the comp level ({CompLevel}) & the match number ({MatchNumber})")]
    internal static partial void WeCouldNotFindTheMatchInTheScheduleUsingTheCompLevelCompLevelTheMatchNumberMatchNumber(this ILogger logger, string CompLevel, int MatchNumber);

    [LoggerMessage(123, LogLevel.Warning, "We could not find the event in the schedule using the event code ({EventCode}) & season ({Season})")]
    internal static partial void WeCouldNotFindTheEventInTheScheduleUsingTheEventCodeEventCodeSeasonSeason(this ILogger logger, string EventCode, string Season);

    [LoggerMessage(124, LogLevel.Trace, "Webhook payload deserialized into {WebhookMessage} {SerializedWebhookMessage}")]
    internal static partial void WebhookPayloadDeserializedIntoWebhookMessageSerializedWebhookMessage(this ILogger logger, WebhookMessage WebhookMessage, string SerializedWebhookMessage);

    [LoggerMessage(125, LogLevel.Warning, "Event {EventKey} not known at all!")]
    internal static partial void EventEventKeyNotKnownAtAll(this ILogger logger, string EventKey);

    [LoggerMessage(126, LogLevel.Warning, "Team {TeamKey} not known at all!")]
    internal static partial void TeamTeamKeyNotKnownAtAll(this ILogger logger, string TeamKey);

    [LoggerMessage(127, LogLevel.Information, "Cached {TeamCount} teams from TBA")]
    internal static partial void CachedTeamCountTeamsFromTBA(this ILogger logger, int TeamCount);

    [LoggerMessage(128, LogLevel.Information, "Cached {EventCount} teams from TBA")]
    internal static partial void CachedEventCountTeamsFromTBA(this ILogger logger, int EventCount);

    [LoggerMessage(129, LogLevel.Trace, "{EmbeddingName} built: {EmbeddingDetail}")]
    internal static partial void EmbeddingNameBuiltEmbeddingDetail(this ILogger logger, string EmbeddingName, string EmbeddingDetail);

    [LoggerMessage(130, LogLevel.Error, "There was an error creating a Guild event for {EventKey} in Guild {GuildName}({GuildId})")]
    internal static partial void ThereWasAnErrorCreatingAGuildEventForEventKeyInGuildGuildNameGuildId(this ILogger logger, Exception exception, string EventKey, string GuildName, ulong GuildId);

    [LoggerMessage(131, LogLevel.Debug, "Removing subscription for team subscription team {Team}")]
    internal static partial void RemovingSubscriptionForTeamSubscriptionTeamTeam(this ILogger logger, string Team);

    [LoggerMessage(132, LogLevel.Error, "Failed to remove subscription for team {Team} ({Status}): {Reason}")]
    internal static partial void FailedToRemoveSubscriptionForTeamTeamStatusReason(this ILogger logger, string Team, int Status, string Reason);

    [LoggerMessage(133, LogLevel.Warning, "No subscriptions found for {Subscription}")]
    internal static partial void NoSubscriptionsFoundForSubscription(this ILogger logger, Subscription.NotificationSubscription Subscription);

    [LoggerMessage(134, LogLevel.Error, "Failed to remove subscription for event {Event} ({Status}): {Reason}")]
    internal static partial void FailedToRemoveSubscriptionForEventEventStatusReason(this ILogger logger, string? Event, int Status, string Reason);

    [LoggerMessage(135, LogLevel.Debug, "Removing subscription for event subscription event {Event}")]
    internal static partial void RemovingSubscriptionForEventSubscriptionEventEvent(this ILogger logger, string? Event);

    [LoggerMessage(136, LogLevel.Debug, "Received menu selection: {SelectionData}")]
    internal static partial void ReceivedMenuSelectionSelectionData(this ILogger logger, string SelectionData);

    [LoggerMessage(137, LogLevel.Error, "Error deleting message {MessageId}")]
    internal static partial void ErrorDeletingMessageMessageId(this ILogger logger, Exception exception, ulong MessageId);

    [LoggerMessage(138, LogLevel.Warning, "Unknown menu selection received: {MenuData}")]
    internal static partial void UnknownMenuSelectionReceivedMenuData(this ILogger logger, string MenuData);

    [LoggerMessage(139, LogLevel.Error, "Error updating the original message for the Delete Subscription interaction")]
    internal static partial void ErrorUpdatingTheOriginalMessageForTheDeleteSubscriptionInteraction(this ILogger logger, Exception exception);

    [LoggerMessage(140, LogLevel.Information, "All notifications dispatched.")]
    internal static partial void AllNotificationsDispatched(this ILogger logger);

    [LoggerMessage(141, LogLevel.Error, "Error while trying to send threaded notification to channel {ChannelId} ({ChannelName})")]
    internal static partial void ErrorWhileTryingToSendThreadedNotificationToChannelChannelIdChannelName(this ILogger logger, Exception exception, ulong ChannelId, string ChannelName);

    [LoggerMessage(142, LogLevel.Error, "Tried to create thread on an `ITextChannel` but it failed")]
    internal static partial void TriedToCreateThreadOnAnITextChannelButItFailed(this ILogger logger, Exception exception);

    [LoggerMessage(143, LogLevel.Warning, "Score breakdown for match {MatchKey} not available after 5 minutes")]
    internal static partial void ScoreBreakdownForMatchMatchKeyNotAvailableAfter5Minutes(this ILogger logger, string MatchKey);

    [LoggerMessage(144, LogLevel.Warning, "Invalid Red RP value for match {MatchKey}: {RpValue}")]
    internal static partial void InvalidRedRPValueForMatchMatchKeyRpValue(this ILogger logger, string MatchKey, int? RpValue);

    [LoggerMessage(145, LogLevel.Warning, "Invalid Blue RP value for match {MatchKey}: {RpValue}")]
    internal static partial void InvalidBlueRPValueForMatchMatchKeyRpValue(this ILogger logger, string MatchKey, int? RpValue);

    [LoggerMessage(146, LogLevel.Debug, "No score breakdown available for match {MatchKey}")]
    internal static partial void NoScoreBreakdownAvailableForMatchMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(147, LogLevel.Error, "Error getting match data for {MatchKey}. Continuing to try...")]
    internal static partial void ErrorGettingMatchDataForMatchKeyContinuingToTry(this ILogger logger, Exception exception, string MatchKey);

    [LoggerMessage(148, LogLevel.Error, "Error getting event schedule for {EventKey}")]
    internal static partial void ErrorGettingEventScheduleForEventKey(this ILogger logger, Exception exception, string EventKey);

    [LoggerMessage(149, LogLevel.Warning, "No district data found for team {TeamKey}")]
    internal static partial void NoDistrictDataFoundForTeamTeamKey(this ILogger logger, string TeamKey);

    [LoggerMessage(150, LogLevel.Warning, "No district points data found for team {TeamKey} at event {EventKey}")]
    internal static partial void NoDistrictPointsDataFoundForTeamTeamKeyAtEventEventKey(this ILogger logger, string TeamKey, string EventKey);

    [LoggerMessage(151, LogLevel.Error, "Error checking for duplicate webhook payload.")]
    internal static partial void ErrorCheckingForDuplicateWebhookPayload(this ILogger logger, Exception exception);

    [LoggerMessage(152, LogLevel.Debug, "Not duplicate payload. Saving...")]
    internal static partial void NotDuplicatePayloadSaving(this ILogger logger);

    [LoggerMessage(153, LogLevel.Warning, "Duplicate webhook payload")]
    internal static partial void DuplicateWebhookPayload(this ILogger logger);

    [LoggerMessage(154, LogLevel.Error, "Error during response geneation")]
    internal static partial void ErrorDuringResponseGeneation(this ILogger logger, Exception exception);

    [LoggerMessage(155, LogLevel.Trace, "Deleting old thread for {ThreadEvent} message from {RecordTimestamp}...")]
    internal static partial void DeletingOldThreadForThreadEventMessageFromRecordTimestamp(this ILogger logger, string ThreadEvent, DateTimeOffset? RecordTimestamp);

    [LoggerMessage(156, LogLevel.Error, "Failed to delete thread from {RecordTimestamp}: {ErrorMessage}")]
    internal static partial void FailedToDeleteThreadFromRecordTimestampErrorMessage(this ILogger logger, DateTimeOffset? RecordTimestamp, string ErrorMessage);

    [LoggerMessage(157, LogLevel.Error, "Failed to delete message from {RecordTimestamp}: {ErrorMessage}")]
    internal static partial void FailedToDeleteMessageFromRecordTimestampErrorMessage(this ILogger logger, DateTimeOffset? RecordTimestamp, string ErrorMessage);

    [LoggerMessage(158, LogLevel.Trace, "Deleting message from {RecordTimestamp}...")]
    internal static partial void DeletingMessageFromRecordTimestamp(this ILogger logger, DateTimeOffset? RecordTimestamp);

    [LoggerMessage(159, LogLevel.Information, "Executing cleanup...")]
    internal static partial void ExecutingCleanup(this ILogger logger);

    [LoggerMessage(160, LogLevel.Debug, "No *actual* subscribers found for {PartitionKey} {RowKey}")]
    internal static partial void NoActualSubscribersFoundForPartitionKeyRowKey(this ILogger logger, string PartitionKey, string RowKey);

    [LoggerMessage(161, LogLevel.Warning, "No stats given from Statbotics for match {MatchKey}")]
    internal static partial void NoStatsGivenFromStatboticsForMatchMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(162, LogLevel.Error, "Didn't find any match data for {MatchKey}")]
    internal static partial void DidnTFindAnyMatchDataForMatchKey(this ILogger logger, string MatchKey);

    [LoggerMessage(163, LogLevel.Debug, "{ServiceType} handled button click {ButtonId}")]
    internal static partial void ServiceTypeHandledButtonClickButtonId(this ILogger logger, string ServiceType, string ButtonId);

    [LoggerMessage(164, LogLevel.Debug, "{ServiceType} handled menu selection {MenuId}[{ValueId}]")]
    internal static partial void ServiceTypeHandledMenuSelectionMenuIdValueId(this ILogger logger, string ServiceType, string MenuId, string ValueId);

    [LoggerMessage(165, LogLevel.Debug, "Cleaning up subscriptions for team {Team}")]
    internal static partial void CleaningUpSubscriptionsForTeamTeam(this ILogger logger, string Team);

    [LoggerMessage(166, LogLevel.Warning, "Failed to retrieve event for team subscription {Team} {Event}")]
    internal static partial void FailedToRetrieveEventForTeamSubscriptionTeamEvent(this ILogger logger, string Team, string Event);

    [LoggerMessage(167, LogLevel.Trace, "Event {Event} has ended >= 5 days ago, cleaning up subscription to it for team {TeamKey}")]
    internal static partial void EventEventHasEnded5DaysAgoCleaningUpSubscriptionToItForTeamTeamKey(this ILogger logger, string Event, string TeamKey);

    [LoggerMessage(168, LogLevel.Warning, "Failed to delete subscription for team {Team}: {ErrorMessage}")]
    internal static partial void FailedToDeleteSubscriptionForTeamTeamErrorMessage(this ILogger logger, string Team, string ErrorMessage);

    [LoggerMessage(169, LogLevel.Debug, "Cleaning up subscriptions for event {Event}")]
    internal static partial void CleaningUpSubscriptionsForEventEvent(this ILogger logger, string Event);

    [LoggerMessage(170, LogLevel.Warning, "Failed to retrieve event for event subscription {Event}")]
    internal static partial void FailedToRetrieveEventForEventSubscriptionEvent(this ILogger logger, string Event);

    [LoggerMessage(171, LogLevel.Trace, "Event {Event} has ended >= 5 days ago, cleaning up event subscription")]
    internal static partial void EventEventHasEnded5DaysAgoCleaningUpEventSubscription(this ILogger logger, string Event);

    [LoggerMessage(172, LogLevel.Warning, "Failed to delete subscription for event {Event}: {ErrorMessage}")]
    internal static partial void FailedToDeleteSubscriptionForEventEventErrorMessage(this ILogger logger, string Event, string ErrorMessage);

    [LoggerMessage(173, LogLevel.Debug, "Ranking points were empty for {MatchKey} - 1s poll until they go live...")]
    internal static partial void RankingPointsWereEmptyForMatchKey1sPollUntilTheyGoLive(this ILogger logger, string MatchKey);

    [LoggerMessage(174, LogLevel.Debug, "Found empty subscription for team {TeamKey} ({GuildId}) - Removing...")]
    internal static partial void FoundEmptySubscriptionForTeamTeamKeyGuildIdRemoving(this ILogger logger, string TeamKey, string GuildId);

    [LoggerMessage(175, LogLevel.Debug, "Subscriptions for team {TeamKey} cleaned.")]
    internal static partial void SubscriptionsForTeamTeamKeyCleaned(this ILogger logger, string TeamKey);

    [LoggerMessage(176, LogLevel.Debug, "Found empty subscription for event {Event} guild {GuildId} - Removing...")]
    internal static partial void FoundEmptySubscriptionForEventEventGuildGuildIdRemoving(this ILogger logger, string Event, string GuildId);

    [LoggerMessage(177, LogLevel.Debug, "Subscriptions for event {Event} cleaned")]
    internal static partial void SubscriptionsForEventEventCleaned(this ILogger logger, string Event);

    [LoggerMessage(178, LogLevel.Warning, "Attempted to remove subscription from non-existent guild.")]
    internal static partial void AttemptedToRemoveSubscriptionFromNonExistentGuild(this ILogger logger);

    [LoggerMessage(179, LogLevel.Warning, "Attempted to remove subscription {Subscription} from guild {GuildId} but it wasn't found.")]
    internal static partial void AttemptedToRemoveSubscriptionSubscriptionFromGuildGuildIdButItWasnTFound(this ILogger logger, string Subscription, string GuildId);

    [LoggerMessage(180, LogLevel.Error, "Error when trying to fetch color for {TeamNumber}")]
    internal static partial void ErrorWhenTryingToFetchColorForTeamNumber(this ILogger logger, Exception exception, ushort TeamNumber);

    [LoggerMessage(181, LogLevel.Error, "Error processing webhook message. {InvocationId}")]
    internal static partial void ErrorProcessingWebhookMessage(this ILogger logger, Exception exception, string InvocationId);

    [LoggerMessage(182, LogLevel.Warning, "Webhook task {InvocationId} faulted without exception.")]
    internal static partial void WebhookTaskInvocationIdFaultedWithoutException(this ILogger logger, string InvocationId);

    [LoggerMessage(183, LogLevel.Warning, "Webhook task already in progress for invocation ID: {InvocationId}")]
    internal static partial void WebhookTaskAlreadyInProgressForInvocationIDInvocationId(this ILogger logger, string InvocationId);

    [LoggerMessage(184, LogLevel.Warning, "No matches found for event {EventKey}, team {TeamKey}")]
    internal static partial void NoMatchesFoundForEventEventKeyTeamTeamKey(this ILogger logger, string EventKey, string TeamKey);

    [LoggerMessage(185, LogLevel.Warning, "Invalid number of matches requested: {numMatches}")]
    internal static partial void InvalidNumberOfMatchesRequestedNumMatches(this ILogger logger, uint numMatches);

    [LoggerMessage(186, LogLevel.Error, "Could not figure out how to send the breakdown to this user: {User}({UserId}, {ChannelName}({ChannelId} - type {ChannelType})")]
    internal static partial void CouldNotFigureOutHowToSendTheBreakdownToThisUserUserUserIdChannelNameChannelIdTypeChannelType(this ILogger logger, Exception exception, string User, ulong UserId, string ChannelName, ulong ChannelId, string? ChannelType);

    [LoggerMessage(187, LogLevel.Warning, "Took too long to reply to 'Get Breakdown' button")]
    internal static partial void TookTooLongToReplyToGetBreakdownButton(this ILogger logger);

    [LoggerMessage(188, LogLevel.Warning, "Error on initial action for score breakdown")]
    internal static partial void ErrorOnInitialActionForScoreBreakdown(this ILogger logger, Exception exception);

    [LoggerMessage(189, LogLevel.Warning, "Error on FollowUp action for score breakdown")]
    internal static partial void ErrorOnFollowUpActionForScoreBreakdown(this ILogger logger, Exception exception);

    [LoggerMessage(190, LogLevel.Error, "Error handling button click {ButtonId}")]
    internal static partial void ErrorHandlingButtonClickButtonId(this ILogger logger, Exception exception, string ButtonId);
}
