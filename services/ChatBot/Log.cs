
namespace ChatBot;

#pragma warning disable CS8019
using Microsoft.Extensions.Logging;

using System;
#pragma warning restore CS8019

static partial class Log
{

    [LoggerMessage(0, LogLevel.Warning, "Unknown chat reset button clicked: {ButtonId}")]
    internal static partial void UnknownChatResetButtonClickedButtonId(this ILogger logger, string ButtonId);

    [LoggerMessage(1, LogLevel.Warning, "Failed while updating the chatbot progress indicator.")]
    internal static partial void FailedWhileUpdatingTheChatbotProgressIndicator(this ILogger logger, Exception exception);

    [LoggerMessage(2, LogLevel.Information, "Connecting to Azure AI Foundry project endpoint {Endpoint}")]
    internal static partial void ConnectingToAzureAIFoundryProjectEndpointEndpoint(this ILogger logger, Uri? Endpoint);

    [LoggerMessage(3, LogLevel.Warning, "Failed to remove transient thinking message.")]
    internal static partial void FailedToRemoveTransientThinkingMessage(this ILogger logger, Exception exception);

    [LoggerMessage(4, LogLevel.Error, "Error responding to DM from {UserName}")]
    internal static partial void ErrorRespondingToDMFromUserName(this ILogger logger, Exception exception, string UserName);

    [LoggerMessage(5, LogLevel.Error, "Error deleting chat thread for user {UserName} ({UserId})")]
    internal static partial void ErrorDeletingChatThreadForUserUserNameUserId(this ILogger logger, Exception exception, string UserName, ulong UserId);

    [LoggerMessage(7, LogLevel.Warning, "Stored Agent Framework session state could not be restored. Creating a new session instead.")]
    internal static partial void StoredAgentFrameworkSessionStateCouldNotBeRestoredCreatingANewSessionInstead(this ILogger logger, Exception exception);

    [LoggerMessage(8, LogLevel.Warning, "Http API tool call failed for {ClientName} {RequestPath} with status {StatusCode}")]
    internal static partial void HttpAPIToolCallFailedForClientNameRequestPathWithStatusStatusCode(this ILogger logger, string ClientName, Uri? RequestPath, int StatusCode);

    [LoggerMessage(9, LogLevel.Information, "Loading hosted Foundry agent {AgentId}")]
    internal static partial void LoadingHostedFoundryAgentAgentId(this ILogger logger, string AgentId);

    [LoggerMessage(10, LogLevel.Information, "Creating new Foundry project conversation.")]
    internal static partial void CreatingNewFoundryProjectConversation(this ILogger logger);

    [LoggerMessage(11, LogLevel.Information, "Creating local tool agent for conversation {ConversationId} using model {ModelId}")]
    internal static partial void CreatingLocalToolAgentForConversationConversationIdModelId(this ILogger logger, string ConversationId, string ModelId);

    [LoggerMessage(12, LogLevel.Information, "Routing hosted Foundry step to local agent. Reason: {Reason}")]
    internal static partial void RoutingHostedFoundryStepToLocalAgent(this ILogger logger, string Reason);

    [LoggerMessage(13, LogLevel.Information, "Routing local agent result back to hosted Foundry agent.")]
    internal static partial void RoutingLocalAgentResultBackToFoundryAgent(this ILogger logger);

    [LoggerMessage(14, LogLevel.Debug, "Workflow event: {EventType}")]
    internal static partial void WorkflowEvent(this ILogger logger, string EventType);

    [LoggerMessage(15, LogLevel.Error, "Workflow executor failed: {ExecutorId} ({ExceptionType}) {Message}")]
    internal static partial void WorkflowExecutorFailed(this ILogger logger, string ExecutorId, string ExceptionType, string Message);

    [LoggerMessage(16, LogLevel.Error, "Workflow error: {EventType} {Details}")]
    internal static partial void WorkflowError(this ILogger logger, string EventType, string Details);

    [LoggerMessage(17, LogLevel.Warning, "Failed to load chatbot progress messages from embedded resource {ResourceName}. Falling back to built-in defaults.")]
    internal static partial void FailedToLoadChatbotProgressMessages(this ILogger logger, Exception exception, string ResourceName);

    [LoggerMessage(18, LogLevel.Warning, "Failed while refreshing the chatbot typing indicator.")]
    internal static partial void FailedWhileRefreshingTheChatbotTypingIndicator(this ILogger logger, Exception exception);

    [LoggerMessage(19, LogLevel.Information, "Creating answer evaluator chat client using model {ModelId}")]
    internal static partial void CreatingAnswerEvaluatorChatClientUsingModelModelId(this ILogger logger, string ModelId);

    [LoggerMessage(20, LogLevel.Information, "Answer evaluator requested repair using model {ModelId}. Feedback: {Feedback}")]
    internal static partial void AnswerEvaluatorRequestedRepairUsingModelModelId(this ILogger logger, string ModelId, string Feedback);

    [LoggerMessage(21, LogLevel.Debug, "Answer evaluator accepted final answer using model {ModelId}")]
    internal static partial void AnswerEvaluatorAcceptedFinalAnswerUsingModelModelId(this ILogger logger, string ModelId);

    [LoggerMessage(22, LogLevel.Information, "Answer evaluator output required malformed JSON recovery.")]
    internal static partial void AnswerEvaluatorOutputRequiredMalformedJsonRecovery(this ILogger logger);

    [LoggerMessage(23, LogLevel.Warning, "Chat completion requested but no Conversation object available to handle it. Returning empty response.")]
    internal static partial void ChatCompletionRequestedButNoConversationObjectAvailableToHandleItReturningEmptyResponse(this ILogger logger);

    [LoggerMessage(24, LogLevel.Information, "Workflow turn reached the soft timeout at {SoftTimeoutSeconds}s. Prompting the hosted agent to finish.")]
    internal static partial void WorkflowTurnReachedSoftTimeout(this ILogger logger, int SoftTimeoutSeconds);

    [LoggerMessage(25, LogLevel.Warning, "Workflow turn exceeded the hard timeout at {HardTimeoutSeconds}s. Emitted output: {EmittedOutput}")]
    internal static partial void WorkflowTurnExceededHardTimeout(this ILogger logger, int HardTimeoutSeconds, bool EmittedOutput);

    [LoggerMessage(26, LogLevel.Warning, "Workflow exceeded the maximum step budget of {MaxWorkflowSteps} while routing {RouteName} at step {StepCount}.")]
    internal static partial void WorkflowExceededStepBudget(this ILogger logger, int MaxWorkflowSteps, string RouteName, int StepCount);

    [LoggerMessage(27, LogLevel.Information, "Auto-approving {ApprovalCount} hosted MCP request(s) for agent {AgentNameOrId}: {ApprovalResponses}")]
    internal static partial void AutoApprovingHostedMcpRequests(this ILogger logger, string AgentNameOrId, int ApprovalCount, string ApprovalResponses);

    [LoggerMessage(28, LogLevel.Debug, "Skipping answer evaluator for a fast MCP-grounded rules/manual final answer.")]
    internal static partial void SkippingAnswerEvaluatorForFastMcpRuleAnswer(this ILogger logger);

    [LoggerMessage(29, LogLevel.Information, "Workflow turn stayed silent for {SilentProgressSeconds}s. Posting a user-visible progress update and suspending workflow deadlines.")]
    internal static partial void WorkflowTurnRemainedSilentPostingProgressStatus(this ILogger logger, int SilentProgressSeconds);

    [LoggerMessage(30, LogLevel.Warning, "Semantic evaluator '{EvaluationName}' timed out after {TimeoutSeconds}s. Accepting the current workflow output to keep the turn moving.")]
    internal static partial void SemanticEvaluatorTimedOutAcceptingCurrentOutput(this ILogger logger, string EvaluationName, int TimeoutSeconds);

    [LoggerMessage(31, LogLevel.Trace, "Formatting a chatbot prompt template with {ReplacementCount} replacement(s).")]
    internal static partial void FormattingPromptTemplate(this ILogger logger, int ReplacementCount);

    [LoggerMessage(32, LogLevel.Debug, "Formatted chatbot prompt template still contains unreplaced tokens after {ReplacementCount} replacement(s).")]
    internal static partial void FormattedPromptTemplateStillContainsUnreplacedTokens(this ILogger logger, int ReplacementCount);

    [LoggerMessage(33, LogLevel.Debug, "Http API tool call for {ClientName} {RequestPath} failed after {ElapsedMilliseconds}ms.")]
    internal static partial void HttpAPIToolCallFailedAfterElapsedMilliseconds(this ILogger logger, string ClientName, Uri? RequestPath, double ElapsedMilliseconds);

    [LoggerMessage(34, LogLevel.Debug, "Http API tool call for {ClientName} returned failure response snippet: {ResponseSnippet}")]
    internal static partial void HttpAPIToolCallFailedResponseSnippet(this ILogger logger, string ClientName, string ResponseSnippet);

    [LoggerMessage(35, LogLevel.Information, "Unable to resume Copilot session {SessionId}; falling back to a fresh session.")]
    internal static partial void UnableToResumeCopilotSessionFallingBackToAFreshSession(this ILogger logger, Exception exception, string SessionId);

    [LoggerMessage(36, LogLevel.Warning, "Hosted Foundry specialist returned non-JSON content; returning raw payload.")]
    internal static partial void HostedFoundrySpecialistReturnedNonJsonContentReturningRawPayload(this ILogger logger);

}
