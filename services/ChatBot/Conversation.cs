namespace ChatBot;

using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;

using ChatBot.Agents;
using ChatBot.Agents.Models;
using ChatBot.Configuration;
using ChatBot.Telemetry;
using ChatBot.Tools;

using Common.Extensions;

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenAI.Responses;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

#pragma warning disable OPENAI001

internal sealed class Conversation(
    IOptions<AiOptions> agentOptions,
    AIProjectClient projectClient,
    PromptCatalog promptCatalog,
    IEnumerable<IProvideFunctionTools> toolProviders,
    TimeProvider timeProvider,
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
{
    private const int EvaluatorMaxOutputTokens = 128;
    private const int MaxInvalidEvaluatorPayloadRetries = 1;
    private const int MaxSilentTurnProgressSeconds = 10;
    private const int SilentProgressPromptLeadSeconds = 5;
    private const string LocalAgentDefinitionFileName = "local-agent.yaml";
    private const string LocalAgentHandoffStateKey = "local-agent-handoff-count";
    private const string WorkflowStepCountStateKey = "workflow-step-count";
    private const string PrematureLocalLookupRetryStateKey = "premature-local-lookup-retry-count";
    private const string SemanticEvaluatorRetryStateKey = "semantic-evaluator-retry-count";
    internal const string UserStatusPrefix = "USER_STATUS:";

    private readonly AiOptions _agentOptions = agentOptions.Value;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ILogger<Conversation> _logger = loggerFactory.CreateLogger<Conversation>();
    private readonly PromptCatalog _promptCatalog = promptCatalog;
    private readonly Lock _agentSync = new();
    private readonly IReadOnlyList<AIFunction> _functions = toolProviders.CombineFunctions();
    private readonly bool _streamInternalDialog = configuration.GetValue<bool>(ChatBotConstants.Configuration.AI.AgentLogging.StreamInternalDialog) is true;
    private readonly string _foundryEvaluatorModelId = agentOptions.Value.EvaluationSettings.Model.UnlessNullOrWhitespaceThen(agentOptions.Value.LocalAgentModel);
    private readonly string _configuredEvaluatorModelId = agentOptions.Value.EvaluationSettings.Model.UnlessNullOrWhitespaceThen(agentOptions.Value.LocalAgentModel);
    private readonly Lazy<IChatClient> _foundryAnswerEvaluatorChatClient = new(
        () => CreateFoundryEvaluatorChatClient(agentOptions.Value, projectClient, loggerFactory));

    private Task<ChatClientAgent>? _foundryChatAgentTask;

    public Task<string> CreateConversationAsync(CancellationToken cancellationToken = default)
        => CreateProjectConversationIdAsync(cancellationToken);

    public async IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        string conversationState,
        string message,
        IEnumerable<ChatMessage>? leadingMessages,
        Func<string, CancellationToken, ValueTask>? persistConversationState,
        ActivityContext? traceRootContext = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(conversationState);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        using Activity? activity = traceRootContext is { } parentContext
            ? Activities.AppActivitySource.StartActivity("chat.turn", ActivityKind.Server, parentContext)
            : Activities.AppActivitySource.StartActivity("chat.turn", ActivityKind.Server);
        activity?.SetTag("gen_ai.operation.name", "chat_turn");
        activity?.SetTag("span_type", "turn");
        using IDisposable conversationParentScope = Activities.PushConversationParent(activity);

        string turnContext = BuildFoundryTurnSystemMessage();

        FoundryAgent foundryAgent = await CreateFoundryAgentAsync(cancellationToken).ConfigureAwait(false);
        string threadId = await GetOrCreateConversationThreadAsync(foundryAgent, conversationState, persistConversationState, cancellationToken).ConfigureAwait(false);
        activity?.SetTag("gen_ai.conversation.id", threadId);

        List<ChatMessage> initialMessages = leadingMessages is null
            ? [CreateSystemMessage(turnContext), CreateUserMessage(message)]
            : [.. leadingMessages, CreateSystemMessage(turnContext), CreateUserMessage(message)];

        bool emittedOutput = false;
        Exception? workflowFailure = null;

        LocalAgent localAgent = await CreateLocalAgentAsync(threadId, cancellationToken).ConfigureAwait(false);
        Workflow workflow = BuildWorkflow(foundryAgent, localAgent, message, turnContext);
        using CancellationTokenSource workflowCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            initialMessages[0],
            threadId,
            workflowCancellation.Token).ConfigureAwait(false);
        for (int i = 1; i < initialMessages.Count; i++)
        {
            await run.TrySendMessageAsync(initialMessages[i]).ConfigureAwait(false);
        }

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true)).ConfigureAwait(false);
        bool promptedToFinish = false;
        bool userVisibleProgressObserved = false;
        int silentTurnProgressSeconds = Math.Max(
            1,
            Math.Min(MaxSilentTurnProgressSeconds, _agentOptions.WorkflowSoftTimeoutSeconds - SilentProgressPromptLeadSeconds));
        Task silentTurnProgressTask = Task.Delay(TimeSpan.FromSeconds(silentTurnProgressSeconds), timeProvider, workflowCancellation.Token);
        Task softDeadlineTask = Task.Delay(TimeSpan.FromSeconds(_agentOptions.WorkflowSoftTimeoutSeconds), timeProvider, workflowCancellation.Token);
        Task hardDeadlineTask = Task.Delay(TimeSpan.FromSeconds(_agentOptions.WorkflowHardTimeoutSeconds), timeProvider, workflowCancellation.Token);
        await using IAsyncEnumerator<WorkflowEvent> workflowEvents = run.WatchStreamAsync(workflowCancellation.Token).GetAsyncEnumerator(workflowCancellation.Token);
        Task<bool> moveNextTask = workflowEvents.MoveNextAsync().AsTask();

        void SuspendWorkflowDeadlines()
        {
            if (userVisibleProgressObserved)
            {
                return;
            }

            userVisibleProgressObserved = true;
            silentTurnProgressTask = Task.Delay(Timeout.InfiniteTimeSpan, workflowCancellation.Token);
            softDeadlineTask = Task.Delay(Timeout.InfiniteTimeSpan, workflowCancellation.Token);
            hardDeadlineTask = Task.Delay(Timeout.InfiniteTimeSpan, workflowCancellation.Token);
        }

        while (true)
        {
            Task completedTask = await Task.WhenAny(moveNextTask, silentTurnProgressTask, softDeadlineTask, hardDeadlineTask).ConfigureAwait(false);
            if (completedTask == moveNextTask)
            {
                bool hasEvent;
                try
                {
                    hasEvent = await moveNextTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (workflowCancellation.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (!hasEvent)
                {
                    break;
                }

                switch (workflowEvents.Current)
                {
                    case AgentResponseUpdateEvent:
                        break;
                    case WorkflowOutputEvent workflowOutputEvent when workflowOutputEvent.Data is string output
                        && TryExtractUserStatusMessage(output, out _):
                        emittedOutput = true;
                        SuspendWorkflowDeadlines();
                        yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(output)])
                        {
                            RawRepresentation = workflowOutputEvent,
                            FinishReason = ChatFinishReason.Stop,
                        };
                        break;
                    case WorkflowOutputEvent workflowOutputEvent when workflowOutputEvent.Data is string output && !string.IsNullOrWhiteSpace(output):
                        emittedOutput = true;
                        SuspendWorkflowDeadlines();
                        yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(output)])
                        {
                            RawRepresentation = workflowOutputEvent,
                            FinishReason = ChatFinishReason.Stop,
                        };
                        break;
                    case ExecutorFailedEvent e:
                        workflowFailure ??= e.Data;
                        _logger.WorkflowExecutorFailed(e.ExecutorId.UnlessNullOrWhitespaceThen("unknown"), e.Data?.GetType().FullName ?? "null", e.Data?.Message ?? string.Empty);
                        break;
                    case WorkflowErrorEvent e:
                        workflowFailure ??= new InvalidOperationException(e.Data?.ToString() ?? "Workflow error");
                        _logger.WorkflowError(e.GetType().Name, e.Data?.ToString() ?? string.Empty);
                        break;
                    default:
                        _logger.WorkflowEvent(workflowEvents.Current.GetType().Name);
                        break;
                }

                moveNextTask = workflowEvents.MoveNextAsync().AsTask();
                continue;
            }

            if (completedTask == silentTurnProgressTask)
            {
                silentTurnProgressTask = Task.Delay(Timeout.InfiniteTimeSpan, workflowCancellation.Token);
                if (!userVisibleProgressObserved && !emittedOutput)
                {
                    _logger.WorkflowTurnRemainedSilentPostingProgressStatus(silentTurnProgressSeconds);
                    SuspendWorkflowDeadlines();
                    yield return new AgentResponseUpdate(
                        ChatRole.Assistant,
                        [new TextContent(BuildUserStatusOutput("Still cooking on this - pulling the good stuff together now."))])
                    {
                        FinishReason = ChatFinishReason.Stop,
                    };
                }

                continue;
            }

            if (completedTask == softDeadlineTask)
            {
                softDeadlineTask = Task.Delay(Timeout.InfiniteTimeSpan, workflowCancellation.Token);
                if (!promptedToFinish && !emittedOutput && !userVisibleProgressObserved)
                {
                    promptedToFinish = true;
                    _logger.WorkflowTurnReachedSoftTimeout(_agentOptions.WorkflowSoftTimeoutSeconds);
                    yield return new AgentResponseUpdate(ChatRole.Assistant, [new TextContent(BuildUserStatusOutput("Almost there - I've got the facts and I'm polishing the answer now."))])
                    {
                        FinishReason = ChatFinishReason.Stop,
                    };
                    await run.TrySendMessageAsync(CreateSystemMessage(BuildWorkflowSoftTimeoutPrompt())).ConfigureAwait(false);
                    await run.TrySendMessageAsync(new TurnToken(emitEvents: true)).ConfigureAwait(false);
                }

                continue;
            }

            _logger.WorkflowTurnExceededHardTimeout(_agentOptions.WorkflowHardTimeoutSeconds, emittedOutput);
            await workflowCancellation.CancelAsync().ConfigureAwait(false);
            if (emittedOutput)
            {
                break;
            }

            throw new TimeoutException("Azure AI chat workflow exceeded the configured turn time budget.");
        }

        await PersistConversationStateAsync(threadId, persistConversationState, cancellationToken).ConfigureAwait(false);

        if (workflowFailure is not null)
        {
            throw new InvalidOperationException("Azure AI chat workflow failed.", workflowFailure);
        }

        if (!emittedOutput)
        {
            throw new InvalidOperationException("Azure AI chat workflow completed without returning any assistant message content.");
        }
    }

    public IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        string conversationState,
        string message,
        Func<string, CancellationToken, ValueTask>? persistConversationState,
        ActivityContext? traceRootContext = null,
        CancellationToken cancellationToken = default)
        => PostUserMessageStreamingAsync(conversationState, message, null, persistConversationState, traceRootContext, cancellationToken: cancellationToken);

    public IAsyncEnumerable<AgentResponseUpdate> PostUserMessageStreamingAsync(
        string conversationState,
        string message,
        ActivityContext? traceRootContext = null,
        CancellationToken cancellationToken = default)
        => PostUserMessageStreamingAsync(conversationState, message, null, null, traceRootContext, cancellationToken: cancellationToken);

    private Workflow BuildWorkflow(FoundryAgent foundryAgent, LocalAgent localAgent, string currentUserMessage, string turnContext)
    {
        ExecutorBinding foundryExecutor = foundryAgent.BindAsExecutor(new AIAgentHostOptions
        {
            ForwardIncomingMessages = false,
            EmitAgentResponseEvents = _streamInternalDialog,
            EmitAgentUpdateEvents = _streamInternalDialog
        });
        ExecutorBinding localExecutor = localAgent.BindAsExecutor(new AIAgentHostOptions
        {
            ForwardIncomingMessages = false,
            EmitAgentResponseEvents = _streamInternalDialog,
            EmitAgentUpdateEvents = _streamInternalDialog
        });

        async ValueTask IncrementWorkflowStepCountAsync(string routeName, IWorkflowContext context, CancellationToken cancellationToken)
        {
            int stepCount = await context.ReadOrInitStateAsync(WorkflowStepCountStateKey, static () => 0, cancellationToken).ConfigureAwait(false) + 1;
            if (stepCount > _agentOptions.MaxWorkflowSteps)
            {
                _logger.WorkflowExceededStepBudget(_agentOptions.MaxWorkflowSteps, routeName, stepCount);
                throw new InvalidOperationException("Workflow exceeded the maximum number of internal execution steps.");
            }

            await context.QueueStateUpdateAsync(WorkflowStepCountStateKey, stepCount, cancellationToken).ConfigureAwait(false);
        }

        static ValueTask EmitUserStatusAsync(IWorkflowContext context, string? message, CancellationToken cancellationToken)
            => string.IsNullOrWhiteSpace(message)
                ? ValueTask.CompletedTask
                : context.YieldOutputAsync(BuildUserStatusOutput(message), cancellationToken);

        async ValueTask RouteFoundryResultAsync(FoundryAgentResult result, IWorkflowContext context, CancellationToken cancellationToken)
        {
            await IncrementWorkflowStepCountAsync($"foundry:{result.NextStep}", context, cancellationToken).ConfigureAwait(false);
            switch (result.NextStep)
            {
                case "query_local" when !string.IsNullOrWhiteSpace(result.TargetInput):
                    {
                        int handoffCount = await context.ReadOrInitStateAsync(LocalAgentHandoffStateKey, static () => 0, cancellationToken).ConfigureAwait(false);
                        if (handoffCount >= _agentOptions.MaxLocalAgentHandoffs)
                        {
                            throw new InvalidOperationException("Workflow exceeded the maximum number of internal local-agent handoffs.");
                        }

                        await context.QueueStateUpdateAsync(LocalAgentHandoffStateKey, handoffCount + 1, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(PrematureLocalLookupRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await EmitUserStatusAsync(context, result.MessageToUser.UnlessNullOrWhitespaceThen("On it - pulling the numbers together now."), cancellationToken).ConfigureAwait(false);

                        _logger.RoutingHostedFoundryStepToLocalAgent(result.Reason ?? "No reason provided.");
                        Activity.Current?.AddEvent(new ActivityEvent("gen_ai.workflow.handoff", tags: new ActivityTagsCollection
                        {
                            ["gen_ai.agent.handoff.from"] = "foundry",
                            ["gen_ai.agent.handoff.to"] = "local",
                            ["gen_ai.agent.handoff.reason"] = result.Reason,
                        }));
                        await SendMessagesAsync(
                            context,
                            localExecutor.Id,
                            [
                                CreateSystemMessage(BuildLocalAgentContextMessage(result)),
                                CreateUserMessage(result.TargetInput),
                            ],
                            _streamInternalDialog,
                            cancellationToken).ConfigureAwait(false);
                        return;
                    }
                case "ask_user" when !string.IsNullOrWhiteSpace(result.Question):
                    {
                        int evaluationRetryCount = await context.ReadOrInitStateAsync(SemanticEvaluatorRetryStateKey, static () => 0, cancellationToken).ConfigureAwait(false);
                        if (evaluationRetryCount < _agentOptions.EvaluationSettings.MaxAnswerEvaluationRetries)
                        {
                            DecisionEvaluationResult evaluation = await EvaluateAskUserDecisionAsync(turnContext, currentUserMessage, result, cancellationToken).ConfigureAwait(false);
                            if (evaluation.RequiresRepair)
                            {
                                string feedback = evaluation.Feedback.UnlessNullOrWhitespaceThen("Reconsider whether clarification is truly required, and continue the workflow using grounded context and available tools when it is not.");
                                _logger.AnswerEvaluatorRequestedRepairUsingModelModelId(_configuredEvaluatorModelId, feedback);
                                await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, evaluationRetryCount + 1, cancellationToken).ConfigureAwait(false);
                                await SendMessagesAsync(
                                    context,
                                    foundryExecutor.Id,
                                    [
                                        CreateSystemMessage(BuildAskUserDecisionRepairPrompt(turnContext, currentUserMessage, result, feedback)),
                                    ],
                                    _streamInternalDialog,
                                    cancellationToken).ConfigureAwait(false);
                                return;
                            }
                        }

                        await context.QueueStateUpdateAsync(LocalAgentHandoffStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(WorkflowStepCountStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(PrematureLocalLookupRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(result.MessageToUser))
                        {
                            await context.YieldOutputAsync(BuildUserStatusOutput(result.MessageToUser), cancellationToken).ConfigureAwait(false);
                        }

                        await context.YieldOutputAsync(result.Question, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                case "final" when !string.IsNullOrWhiteSpace(result.Answer):
                    {
                        int handoffCount = await context.ReadOrInitStateAsync(LocalAgentHandoffStateKey, static () => 0, cancellationToken).ConfigureAwait(false);
                        int retryCount = await context.ReadOrInitStateAsync(PrematureLocalLookupRetryStateKey, static () => 0, cancellationToken).ConfigureAwait(false);
                        int evaluationRetryCount = await context.ReadOrInitStateAsync(SemanticEvaluatorRetryStateKey, static () => 0, cancellationToken).ConfigureAwait(false);

                        if (handoffCount < _agentOptions.MaxLocalAgentHandoffs
                            && retryCount < _agentOptions.MaxPrematureLocalLookupRetries
                            && ShouldRetryLocallyBeforeFinal(result.Answer))
                        {
                            await context.QueueStateUpdateAsync(PrematureLocalLookupRetryStateKey, retryCount + 1, cancellationToken).ConfigureAwait(false);
                            await EmitUserStatusAsync(context, "Still digging through the grounded details so I can answer this cleanly.", cancellationToken).ConfigureAwait(false);
                            await SendMessagesAsync(
                                context,
                                foundryExecutor.Id,
                                [
                                    CreateSystemMessage(BuildPrematureLocalLookupRetryPrompt(result.Answer)),
                                ],
                                _streamInternalDialog,
                                cancellationToken).ConfigureAwait(false);
                            return;
                        }

                        if (evaluationRetryCount < _agentOptions.EvaluationSettings.MaxAnswerEvaluationRetries
                            && ShouldRepairAllowedRosterContactRefusal(currentUserMessage, result.Answer))
                        {
                            const string feedback = "If the request is for allowed roster info grounded in the knowledge base, provide it directly. Student names, lead roles, subteams, and parent/guardian/adult contact info are allowed. Only student email addresses and student phone numbers are blocked.";
                            _logger.AnswerEvaluatorRequestedRepairUsingModelModelId(_configuredEvaluatorModelId, feedback);
                            await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, evaluationRetryCount + 1, cancellationToken).ConfigureAwait(false);
                            await EmitUserStatusAsync(context, "Quick accuracy lap - double-checking the allowed parent contact details now.", cancellationToken).ConfigureAwait(false);
                            await SendMessagesAsync(
                                context,
                                foundryExecutor.Id,
                                [
                                    CreateSystemMessage(BuildAnswerEvaluationRepairPrompt(turnContext, currentUserMessage, result.Answer, feedback)),
                                ],
                                _streamInternalDialog,
                                cancellationToken).ConfigureAwait(false);
                            return;
                        }

                        bool skipAnswerEvaluation = ShouldSkipAnswerEvaluation(currentUserMessage, result, handoffCount);
                        await EmitUserStatusAsync(context, result.MessageToUser.UnlessNullOrWhitespaceThen(BuildPreFinalUserStatusMessage(result)), cancellationToken).ConfigureAwait(false);
                        if (skipAnswerEvaluation)
                        {
                            _logger.SkippingAnswerEvaluatorForFastMcpRuleAnswer();
                        }
                        else if (evaluationRetryCount < _agentOptions.EvaluationSettings.MaxAnswerEvaluationRetries)
                        {
                            AnswerEvaluationResult evaluation = await EvaluateAnswerAsync(turnContext, currentUserMessage, result, handoffCount, cancellationToken).ConfigureAwait(false);
                            if (evaluation.RequiresRepair)
                            {
                                string feedback = evaluation.Feedback.UnlessNullOrWhitespaceThen("Repair the answer so it directly addresses the user's request with the correct grounded details.");
                                _logger.AnswerEvaluatorRequestedRepairUsingModelModelId(_configuredEvaluatorModelId, feedback);
                                await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, evaluationRetryCount + 1, cancellationToken).ConfigureAwait(false);
                                await EmitUserStatusAsync(context, "Quick fact-check lap - making sure this lines up with the grounded sources.", cancellationToken).ConfigureAwait(false);
                                await SendMessagesAsync(
                                    context,
                                    foundryExecutor.Id,
                                    [
                                        CreateSystemMessage(BuildAnswerEvaluationRepairPrompt(turnContext, currentUserMessage, result.Answer, feedback)),
                                    ],
                                    _streamInternalDialog,
                                    cancellationToken).ConfigureAwait(false);
                                return;
                            }

                            _logger.AnswerEvaluatorAcceptedFinalAnswerUsingModelModelId(_configuredEvaluatorModelId);
                        }

                        await context.QueueStateUpdateAsync(LocalAgentHandoffStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(WorkflowStepCountStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(PrematureLocalLookupRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.QueueStateUpdateAsync(SemanticEvaluatorRetryStateKey, 0, cancellationToken).ConfigureAwait(false);
                        await context.YieldOutputAsync(result.Answer, cancellationToken).ConfigureAwait(false);
                        return;
                    }
                default:
                    throw new InvalidOperationException($"Hosted Foundry agent returned unsupported next_step '{result.NextStep}'.");
            }
        }

        async ValueTask RouteLocalResultAsync(string payload, IWorkflowContext context, CancellationToken cancellationToken)
        {
            await IncrementWorkflowStepCountAsync("local:continuation", context, cancellationToken).ConfigureAwait(false);
            _logger.RoutingLocalAgentResultBackToFoundryAgent();
            await EmitUserStatusAsync(context, "Boom - I have the grounded data. Turning it into a clean answer now.", cancellationToken).ConfigureAwait(false);
            Activity.Current?.AddEvent(new ActivityEvent("gen_ai.workflow.handoff", tags: new ActivityTagsCollection
            {
                ["gen_ai.agent.handoff.from"] = "local",
                ["gen_ai.agent.handoff.to"] = "foundry",
            }));
            await SendMessagesAsync(
                context,
                foundryExecutor.Id,
                [
                    CreateSystemMessage(BuildFoundryContinuationMessage(payload)),
                ],
                _streamInternalDialog,
                cancellationToken).ConfigureAwait(false);
        }

        AgentWorkflowBindings bindings = AgentWorkflowBindingsFactory.Create(
            RouteFoundryResultAsync,
            RouteLocalResultAsync,
            foundryExecutor.Id,
            _streamInternalDialog,
            _promptCatalog,
            _logger);

        return new WorkflowBuilder(foundryExecutor)
            .AddEdge(foundryExecutor, bindings.FoundryMessageRouter)
            .AddEdge(foundryExecutor, bindings.FoundryMessagesRouter)
            .AddEdge(bindings.FoundryMessageRouter, localExecutor)
            .AddEdge(bindings.FoundryMessagesRouter, localExecutor)
            .AddEdge(bindings.FoundryMessageRouter, foundryExecutor)
            .AddEdge(bindings.FoundryMessagesRouter, foundryExecutor)
            .AddEdge(localExecutor, bindings.LocalMessageRouter)
            .AddEdge(localExecutor, bindings.LocalMessagesRouter)
            .AddEdge(bindings.LocalMessageRouter, foundryExecutor)
            .AddEdge(bindings.LocalMessagesRouter, foundryExecutor)
            .WithOutputFrom(bindings.FoundryMessageRouter, bindings.FoundryMessagesRouter)
            .WithOpenTelemetry(o => o.EnableSensitiveData = false, Activities.AppActivitySource)
            .Build();
    }

    private async Task<string> GetOrCreateConversationThreadAsync(
        FoundryAgent agent,
        string conversationState,
        Func<string, CancellationToken, ValueTask>? persistConversationState,
        CancellationToken cancellationToken)
    {
        string? threadId = ConversationThreadState.TryExtractThreadId(conversationState);
        if (!string.IsNullOrWhiteSpace(threadId))
        {
            await agent.BindConversationSessionAsync(threadId, cancellationToken).ConfigureAwait(false);
            return threadId;
        }

        _logger.StoredAgentFrameworkSessionStateCouldNotBeRestoredCreatingANewSessionInstead(new InvalidOperationException("Stored conversation state did not contain a Foundry thread id."));

        string freshThreadId = await CreateProjectConversationIdAsync(cancellationToken).ConfigureAwait(false);
        await agent.BindConversationSessionAsync(freshThreadId, cancellationToken).ConfigureAwait(false);
        await PersistConversationStateAsync(freshThreadId, persistConversationState, cancellationToken).ConfigureAwait(false);
        return freshThreadId;
    }

    private async Task<string> CreateProjectConversationIdAsync(CancellationToken cancellationToken)
    {
        ProjectConversation conversation = (await projectClient
            .GetProjectOpenAIClient()
            .Conversations
            .CreateProjectConversationAsync(new ProjectConversationCreationOptions(), cancellationToken)
            .ConfigureAwait(false)).Value;

        return conversation.Id;
    }

    private static async Task PersistConversationStateAsync(
        string threadId,
        Func<string, CancellationToken, ValueTask>? persistConversationState,
        CancellationToken cancellationToken)
    {
        if (persistConversationState is null)
        {
            return;
        }

        await persistConversationState(threadId, cancellationToken).ConfigureAwait(false);
    }

    private async Task<FoundryAgent> CreateFoundryAgentAsync(CancellationToken cancellationToken)
        => new(await GetFoundryChatAgentAsync(cancellationToken).ConfigureAwait(false), _loggerFactory);

    private async Task<LocalAgent> CreateLocalAgentAsync(string conversationId, CancellationToken cancellationToken)
    {
        _logger.CreatingLocalToolAgentForConversationConversationIdModelId(conversationId, _agentOptions.LocalAgentModel);
        return await LocalAgent.CreateFromYamlAsync(
            projectClient.GetProjectOpenAIClient().GetProjectResponsesClientForModel(_agentOptions.LocalAgentModel, conversationId),
            GetAgentDefinitionPath(LocalAgentDefinitionFileName),
            _configuration,
            _loggerFactory,
            options =>
            {
                options.AllowMultipleToolCalls = true;
                options.ConversationId = conversationId;
                options.ModelId = _agentOptions.LocalAgentModel;
                options.ToolMode = ChatToolMode.Auto;
            },
            _functions,
            cancellationToken).ConfigureAwait(false);
    }

    private Task<ChatClientAgent> GetFoundryChatAgentAsync(CancellationToken cancellationToken)
    {
        lock (_agentSync)
        {
            _foundryChatAgentTask ??= LoadFoundryChatAgentAsync(cancellationToken);
            return _foundryChatAgentTask;
        }
    }

    private async Task<ChatClientAgent> LoadFoundryChatAgentAsync(CancellationToken cancellationToken)
    {
        AgentReference agentIdentifier = ParseAgentIdentifier(_agentOptions.AgentId);
        _logger.LoadingHostedFoundryAgentAgentId(_agentOptions.AgentId);
        return projectClient.AsAIAgent(
        agentIdentifier.Version is { Length: > 0 } agentVersion
            ? (await projectClient.Agents.GetAgentVersionAsync(agentIdentifier.Name, agentVersion, cancellationToken).ConfigureAwait(false)).Value
            : (await projectClient.Agents.GetAgentAsync(agentIdentifier.Name, cancellationToken).ConfigureAwait(false)).Value);
    }

    private async Task<AnswerEvaluationResult> EvaluateAnswerAsync(
        string turnContext,
        string currentUserMessage,
        FoundryAgentResult result,
        int handoffCount,
        CancellationToken cancellationToken)
    {
        string answer = result.Answer.UnlessNullOrWhitespaceThen(string.Empty);
        string hostedSource = result.HostedSource.UnlessNullOrWhitespaceThen(handoffCount > 0 ? "local" : "[none]");
        string skipEvaluator = result.SkipEvaluator?.ToString() ?? "[none]";
        ChatMessage[] baseMessages =
        [
            CreateSystemMessage(BuildAnswerEvaluatorSystemPrompt()),
            CreateUserMessage(BuildAnswerEvaluatorUserPrompt(
                turnContext,
                currentUserMessage,
                answer,
                hostedSource,
                handoffCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
                skipEvaluator)),
        ];
        int evaluatorTimeoutSeconds = _agentOptions.EvaluationSettings.TimeoutSeconds;
        using CancellationTokenSource evaluatorTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        evaluatorTimeoutCancellation.CancelAfter(TimeSpan.FromSeconds(evaluatorTimeoutSeconds));

        for (int attempt = 0; attempt <= MaxInvalidEvaluatorPayloadRetries; attempt++)
        {
            ChatResponse response;
            try
            {
                response = await GetEvaluatorResponseAsync(
                    attempt is 0
                        ? baseMessages
                        : [.. baseMessages, CreateSystemMessage(BuildInvalidEvaluatorPayloadRetryPrompt())],
                    AnswerEvaluationResultParser.ResponseFormat,
                    evaluatorTimeoutCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && evaluatorTimeoutCancellation.IsCancellationRequested)
            {
                _logger.SemanticEvaluatorTimedOutAcceptingCurrentOutput("answer", evaluatorTimeoutSeconds);
                return new("accept");
            }

            string payload = ExtractStructuredJsonPayload(response);
            if (AnswerEvaluationResultParser.TryParse(payload, out AnswerEvaluationResult? evalResult, out JsonException? exception, out bool recoveredMalformedJson)
                && evalResult is not null
                && (string.Equals(evalResult.Decision, "accept", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(evalResult.Decision, "repair", StringComparison.OrdinalIgnoreCase)))
            {
                if (recoveredMalformedJson)
                {
                    _logger.AnswerEvaluatorOutputRequiredMalformedJsonRecovery();
                }

                return evalResult;
            }

            if (attempt >= MaxInvalidEvaluatorPayloadRetries)
            {
                return new("repair", "Repair the answer with exact grounded details, official sources when needed, and no ambiguity drift.");
            }
        }

        return new("repair", "Repair the answer with exact grounded details, official sources when needed, and no ambiguity drift.");
    }

    private async Task<DecisionEvaluationResult> EvaluateAskUserDecisionAsync(
        string turnContext,
        string currentUserMessage,
        FoundryAgentResult result,
        CancellationToken cancellationToken)
    {
        ChatMessage[] baseMessages =
        [
            CreateSystemMessage(BuildAskUserDecisionEvaluatorSystemPrompt()),
            CreateUserMessage(BuildAskUserDecisionEvaluatorUserPrompt(turnContext, currentUserMessage, result)),
        ];
        int evaluatorTimeoutSeconds = _agentOptions.EvaluationSettings.TimeoutSeconds;
        using CancellationTokenSource evaluatorTimeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        evaluatorTimeoutCancellation.CancelAfter(TimeSpan.FromSeconds(evaluatorTimeoutSeconds));

        for (int attempt = 0; attempt <= MaxInvalidEvaluatorPayloadRetries; attempt++)
        {
            ChatResponse response;
            try
            {
                response = await GetEvaluatorResponseAsync(
                    attempt is 0
                        ? baseMessages
                        : [.. baseMessages, CreateSystemMessage(BuildInvalidEvaluatorPayloadRetryPrompt())],
                    DecisionEvaluationResultParser.ResponseFormat,
                    evaluatorTimeoutCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && evaluatorTimeoutCancellation.IsCancellationRequested)
            {
                _logger.SemanticEvaluatorTimedOutAcceptingCurrentOutput("ask_user", evaluatorTimeoutSeconds);
                return new("accept");
            }

            string payload = ExtractStructuredJsonPayload(response);
            if (DecisionEvaluationResultParser.TryParse(payload, out DecisionEvaluationResult? resultEvaluation, out JsonException? exception, out bool recoveredMalformedJson)
                && resultEvaluation is not null
                && (string.Equals(resultEvaluation.Decision, "accept", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(resultEvaluation.Decision, "repair", StringComparison.OrdinalIgnoreCase)))
            {
                if (recoveredMalformedJson)
                {
                    _logger.AnswerEvaluatorOutputRequiredMalformedJsonRecovery();
                }

                return resultEvaluation;
            }

            if (attempt >= MaxInvalidEvaluatorPayloadRetries)
            {
                return new("repair", "Reconsider the workflow decision using grounded context and tools before asking the user to clarify.");
            }
        }

        return new("repair", "Reconsider the workflow decision using grounded context and tools before asking the user to clarify.");
    }

    private async Task<ChatResponse> GetEvaluatorResponseAsync(
        IReadOnlyList<ChatMessage> messages,
        ChatResponseFormat responseFormat,
        CancellationToken cancellationToken)
        => await _foundryAnswerEvaluatorChatClient.Value.GetResponseAsync(
            messages,
            CreateEvaluatorChatOptions(responseFormat, _foundryEvaluatorModelId),
            cancellationToken).ConfigureAwait(false);

    private static IChatClient CreateFoundryEvaluatorChatClient(AiOptions options, AIProjectClient projectClient, ILoggerFactory loggerFactory)
    {
        string evaluatorModel = options.EvaluationSettings.Model.UnlessNullOrWhitespaceThen(options.LocalAgentModel);
        loggerFactory.CreateLogger<Conversation>().CreatingAnswerEvaluatorChatClientUsingModelModelId(evaluatorModel);

        ProjectOpenAIClientOptions openAIOptions = new()
        {
            EnableDistributedTracing = true,
            ApiVersion = options.OpenAIApiVersion,
        };
        openAIOptions.AddPolicy(W3CTraceContextClientModelPipelinePolicy.Instance, System.ClientModel.Primitives.PipelinePosition.PerTry);

        ResponsesClient responseClient = projectClient
            .GetProjectOpenAIClient(openAIOptions)
            .GetProjectResponsesClientForModel(evaluatorModel);

        return responseClient
            .AsIChatClient()
            .AsBuilder()
            .ConfigureOptions(chatOptions =>
            {
                chatOptions.ModelId = evaluatorModel;
                chatOptions.ToolMode = ChatToolMode.None;
            })
            .UseOpenTelemetry(loggerFactory)
            .Build();
    }

    private static ChatOptions CreateEvaluatorChatOptions(
        ChatResponseFormat responseFormat,
        string modelId)
        => new()
        {
            ModelId = modelId,
            MaxOutputTokens = EvaluatorMaxOutputTokens,
            ResponseFormat = responseFormat,
            ToolMode = ChatToolMode.None,
        };

    private static string ExtractStructuredJsonPayload(ChatResponse response)
    {
        for (int messageIndex = response.Messages.Count - 1; messageIndex >= 0; messageIndex--)
        {
            ChatMessage message = response.Messages[messageIndex];
            for (int contentIndex = message.Contents.Count - 1; contentIndex >= 0; contentIndex--)
            {
                switch (message.Contents[contentIndex])
                {
                    case DataContent { Data.Length: > 0 } dataContent when dataContent.MediaType.Contains("json", StringComparison.OrdinalIgnoreCase):
                        return Encoding.UTF8.GetString(dataContent.Data.Span);
                    case TextContent { Text.Length: > 0 } textContent:
                        return textContent.Text;
                }
            }

            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                return message.Text;
            }
        }

        return response.Text;
    }

    private static string GetAgentDefinitionPath(string fileName)
        => Path.Combine(AppContext.BaseDirectory, "ChatBot", "Agents", fileName);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0009:Use 'System.MemoryExtensions.Split' for improved performance", Justification = "Not a hot path; doesn't justify the unreadability")]
    private static AgentReference ParseAgentIdentifier(string agentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(agentId);

        string[] parts = agentId.Split(':', count: 2, StringSplitOptions.TrimEntries);
        return parts switch
        {
            [var agentName] when !string.IsNullOrWhiteSpace(agentName) => new(agentName),
            [var agentName, var agentVersion]
                when !string.IsNullOrWhiteSpace(agentName)
                && !string.IsNullOrWhiteSpace(agentVersion) => new(agentName, agentVersion),
            _ => throw new InvalidOperationException(
                $"Configured Foundry agent id '{agentId}' must be either '<agent-name>' to use the latest version or '<agent-name>:<version>' to pin a specific Azure AI Foundry prompt agent version.")
        };
    }

    private string BuildFoundryTurnSystemMessage()
    {
        DateTimeOffset localNow = timeProvider.GetLocalNow();
        return _promptCatalog.Format(
            _promptCatalog.FoundryTurnSystemMessage,
            ("CURRENT_LOCAL_TIME", localNow.ToString("O")),
            ("FALLBACK_SEASON_YEAR", localNow.Year.ToString()),
            ("DEFAULT_TEAM_NUMBER", _agentOptions.DefaultTeamNumber.ToString()));
    }

    private string BuildLocalAgentContextMessage(FoundryAgentResult result)
        => _promptCatalog.Format(
            _promptCatalog.LocalAgentContextMessage,
            ("DEFAULT_TEAM_NUMBER", _agentOptions.DefaultTeamNumber.ToString()),
            ("ROUTING_REASON", result.Reason ?? "No reason provided."));

    private string BuildFoundryContinuationMessage(string localAgentPayload)
        => _promptCatalog.Format(
            _promptCatalog.FoundryContinuationMessage,
            ("LOCAL_AGENT_PAYLOAD", localAgentPayload));

    private string BuildPrematureLocalLookupRetryPrompt(string previousAnswer)
        => _promptCatalog.Format(
            _promptCatalog.PrematureLocalLookupRetryPrompt,
            ("PREVIOUS_ANSWER", previousAnswer));

    private string BuildWorkflowSoftTimeoutPrompt()
        => _promptCatalog.WorkflowSoftTimeoutPrompt;

    private string BuildAskUserDecisionEvaluatorSystemPrompt()
        => _promptCatalog.AskUserDecisionEvaluatorSystemPrompt;

    private string BuildAskUserDecisionEvaluatorUserPrompt(string turnContext, string currentUserMessage, FoundryAgentResult result)
        => _promptCatalog.Format(
            _promptCatalog.AskUserDecisionEvaluatorUserPrompt,
            ("TURN_CONTEXT", turnContext),
            ("CURRENT_USER_MESSAGE", currentUserMessage),
            ("NEXT_STEP", result.NextStep),
            ("QUESTION", result.Question ?? "[none]"),
            ("REASON", result.Reason ?? "[none]"),
            ("MESSAGE_TO_USER", result.MessageToUser ?? "[none]"));

    private string BuildAnswerEvaluatorSystemPrompt()
        => _promptCatalog.AnswerEvaluatorSystemPrompt;

    private string BuildInvalidEvaluatorPayloadRetryPrompt()
        => _promptCatalog.InvalidEvaluatorPayloadRetryPrompt;

    private string BuildAnswerEvaluatorUserPrompt(
        string turnContext,
        string currentUserMessage,
        string answer,
        string hostedSource,
        string localHandoffCount,
        string skipEvaluator)
        => _promptCatalog.Format(
            _promptCatalog.AnswerEvaluatorUserPrompt,
            ("TURN_CONTEXT", turnContext),
            ("CURRENT_USER_MESSAGE", currentUserMessage),
            ("ANSWER", answer),
            ("HOSTED_SOURCE", hostedSource),
            ("LOCAL_HANDOFF_COUNT", localHandoffCount),
            ("SKIP_EVALUATOR", skipEvaluator));

    private string BuildAnswerEvaluationRepairPrompt(string turnContext, string currentUserMessage, string previousAnswer, string feedback)
        => _promptCatalog.Format(
            _promptCatalog.AnswerEvaluationRepairPrompt,
            ("TURN_CONTEXT", turnContext),
            ("CURRENT_USER_MESSAGE", currentUserMessage),
            ("PREVIOUS_ANSWER", previousAnswer),
            ("FEEDBACK", feedback));

    private string BuildAskUserDecisionRepairPrompt(string turnContext, string currentUserMessage, FoundryAgentResult result, string feedback)
        => _promptCatalog.Format(
            _promptCatalog.AskUserDecisionRepairPrompt,
            ("TURN_CONTEXT", turnContext),
            ("CURRENT_USER_MESSAGE", currentUserMessage),
            ("NEXT_STEP", result.NextStep),
            ("QUESTION", result.Question ?? "[none]"),
            ("REASON", result.Reason ?? "[none]"),
            ("MESSAGE_TO_USER", result.MessageToUser ?? "[none]"),
            ("FEEDBACK", feedback));

    private static string BuildUserStatusOutput(string messageToUser)
        => $"{UserStatusPrefix}{messageToUser.Trim()}";

    internal static bool TryExtractUserStatusMessage(string output, out string? messageToUser)
    {
        if (!output.StartsWith(UserStatusPrefix, StringComparison.Ordinal))
        {
            messageToUser = null;
            return false;
        }

        string payload = output[UserStatusPrefix.Length..].Trim();
        if (string.IsNullOrWhiteSpace(payload))
        {
            messageToUser = null;
            return false;
        }

        messageToUser = payload;
        return true;
    }

    private static string? BuildPreFinalUserStatusMessage(FoundryAgentResult result)
        => result.HostedSource?.ToLowerInvariant() switch
        {
            "mcp" => "Got the manual guidance - translating it into plain robot English now.",
            "browser" => "Got the official FIRST page - turning it into a crisp answer now.",
            "local" => "Boom - the grounded stats are in. Stitching them into a clear answer now.",
            "mixed" => "Nice - I have all the puzzle pieces. Wrapping them into one clear answer now.",
            _ => null,
        };

    private static bool ShouldRetryLocallyBeforeFinal(string answer)
    {
        string normalized = answer.ToLowerInvariant();
        bool limitationWithDataSource = ContainsAny(
                normalized,
                "could not reliably compute",
                "couldn't reliably compute",
                "could not determine",
                "couldn't determine",
                "could not confirm",
                "couldn't confirm",
                "did not include",
                "does not include",
                "unable to compute",
                "unable to determine",
                "not determined yet")
            && ContainsAny(
                normalized,
                "blue alliance",
                "tba",
                "statbotics",
                "page text",
                "snippet",
                "search result",
                "team history page",
                "match score",
                "event");

        bool partialAggregation = ContainsAny(
                normalized,
                "sampled",
                "sampled source set",
                "partial data",
                "partial dataset",
                "subset",
                "only grounded count i can state",
                "all-matches-to-date dataset",
                "do not yet have a complete",
                "don't yet have a complete",
                "season-wide count")
            && ContainsAny(
                normalized,
                "count",
                "total",
                "season",
                "match",
                "matches",
                "1000");

        return limitationWithDataSource || partialAggregation;
    }

    private static bool ShouldSkipAnswerEvaluation(string currentUserMessage, FoundryAgentResult result, int handoffCount)
        => handoffCount is 0
            && result.SkipEvaluator is true
            && string.Equals(result.HostedSource, "mcp", StringComparison.OrdinalIgnoreCase)
            && IsLikelyRulesOrManualQuestion(currentUserMessage);

    private static bool ShouldRepairAllowedRosterContactRefusal(string currentUserMessage, string answer)
    {
        string normalizedMessage = currentUserMessage.ToLowerInvariant();
        string normalizedAnswer = answer.ToLowerInvariant();

        bool asksForContactInfo = ContainsAny(normalizedMessage, "email", "e-mail", "contact", "phone", "call", "reach");
        bool pointsToAllowedAdults = ContainsAny(normalizedMessage, "parent", "parents", "guardian", "guardians", "adult", "adults");
        bool refusalMentionsContactInfo = ContainsAny(normalizedAnswer, "can't provide", "cannot provide", "can't share", "cannot share", "won't provide", "unable to provide")
            && ContainsAny(normalizedAnswer, "email", "e-mail", "contact");
        bool overbroadRosterRefusal = ContainsAny(
            normalizedAnswer,
            "even for parents",
            "personal email addresses from the roster",
            "i can't provide personal email addresses",
            "i cannot provide personal email addresses",
            "i can't share personal email addresses",
            "i cannot share personal email addresses");

        return asksForContactInfo && pointsToAllowedAdults && (refusalMentionsContactInfo || overbroadRosterRefusal);
    }

    private static bool IsLikelyRulesOrManualQuestion(string currentUserMessage)
    {
        string normalized = currentUserMessage.ToLowerInvariant();
        return ContainsAny(
            normalized,
            "rule",
            "manual",
            "glossary",
            "q&a",
            "ranking point",
            "rp",
            "bonus",
            "penalty",
            "foul",
            "tech foul",
            "scoring",
            "score",
            "field");
    }

    private static bool ContainsAny(string value, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (value.Contains(candidate, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static ChatMessage CreateSystemMessage(string content)
        => new(ChatRole.System, [new TextContent(content)]);

    private static ChatMessage CreateUserMessage(string content)
        => new(ChatRole.User, [new TextContent(content)]);

    private static async ValueTask SendMessagesAsync(
        IWorkflowContext context,
        string executorId,
        IEnumerable<ChatMessage> messages,
        bool emitEvents,
        CancellationToken cancellationToken)
    {
        foreach (ChatMessage message in messages)
        {
            await context.SendMessageAsync(message, executorId, cancellationToken).ConfigureAwait(false);
        }

        await context.SendMessageAsync(new TurnToken(emitEvents: emitEvents), executorId, cancellationToken).ConfigureAwait(false);
    }
}

#pragma warning restore OPENAI001
