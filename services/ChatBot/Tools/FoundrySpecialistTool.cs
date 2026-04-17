namespace ChatBot.Tools;

using AgentFramework.OpenTelemetry.Agents;

using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;

using ChatBot.Agents;
using ChatBot.Agents.Models;
using ChatBot.Configuration;
using ChatBot.Copilot;

using Common.Extensions;

using GitHub.Copilot.SDK;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.ObjectModel;
using System.ComponentModel;

internal sealed class FoundrySpecialistTool(
    AIProjectClient projectClient,
    IOptions<AiOptions> options,
    PromptCatalog promptCatalog,
    ILoggerFactory loggerFactory)
{
    public const string ToolName = "foundry_specialist_lookup";
    private const string ToolDescription = "Call the hosted Azure Foundry specialist for official rules, glossary, Q&A, and FoundryIQ roster or directory knowledge that should stay in the remote specialist lane.";

    private readonly AIProjectClient _projectClient = projectClient;
    private readonly AiOptions _options = options.Value;
    private readonly PromptCatalog _promptCatalog = promptCatalog;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ILogger<FoundrySpecialistTool> _logger = loggerFactory.CreateLogger<FoundrySpecialistTool>();

    public AIFunction CreateFunction(
        Func<CopilotChatState> getState,
        Func<CopilotChatState, CancellationToken, ValueTask> persistState)
        => AIFunctionFactory.Create(
            async (
                [Description("The user request that should be handled by the hosted Foundry specialist, such as a rules, glossary, Q&A, or roster question.")] string request,
                CancellationToken cancellationToken) => await QueryFoundrySpecialistAsync(request, getState, persistState, cancellationToken).ConfigureAwait(false),
            new AIFunctionFactoryOptions
            {
                Name = ToolName,
                Description = ToolDescription,
                AdditionalProperties = new ReadOnlyDictionary<string, object?>(
                    new Dictionary<string, object?>
                    {
                        ["skip_permission"] = true,
                    }),
            });

    private async Task<object> QueryFoundrySpecialistAsync(
        string request,
        Func<CopilotChatState> getState,
        Func<CopilotChatState, CancellationToken, ValueTask> persistState,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request);

        CopilotChatState chatState = getState();
        string? threadId = FoundrySpecialistConversationStore.GetThreadId(chatState);
        if (string.IsNullOrWhiteSpace(threadId))
        {
            threadId = await CreateProjectConversationIdAsync(cancellationToken).ConfigureAwait(false);
            chatState = FoundrySpecialistConversationStore.SetThreadId(chatState, threadId);
            await persistState(chatState, cancellationToken).ConfigureAwait(false);
        }

        FoundryAgent foundryAgent = new(await LoadFoundryChatAgentAsync(cancellationToken).ConfigureAwait(false), _loggerFactory);
        await foundryAgent.BindConversationSessionAsync(threadId, cancellationToken).ConfigureAwait(false);

        AgentResponse response = await foundryAgent.RunAsync(
            [
                new(ChatRole.System, _promptCatalog.FoundrySpecialistBridgePrompt),
                new(ChatRole.User, request),
            ],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string payload = response.Text;
        if (!FoundryAgentResultParser.TryParse(payload, out FoundryAgentResult? result, out _, out _)
            || result is null)
        {
            Log.HostedFoundrySpecialistReturnedNonJsonContentReturningRawPayload(_logger);
            return new
            {
                answer = payload,
                hostedSource = "foundry",
            };
        }

        return result.NextStep switch
        {
            "final" => new
            {
                answer = result.Answer,
                hostedSource = result.HostedSource.UnlessNullOrWhitespaceThen("foundry"),
            },
            "ask_user" => new
            {
                answer = result.Question,
                hostedSource = "foundry",
                needsClarification = true,
            },
            _ => new
            {
                error = "The hosted Foundry specialist requested a different specialist lane. Use the FRC data specialist for local competition or meal data.",
                hostedSource = "foundry",
            },
        };
    }

    private Task<AIAgent> LoadFoundryChatAgentAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;

        AgentReference agentIdentifier = ParseAgentIdentifier(_options.AgentId);
        Log.LoadingHostedFoundryAgentAgentId(_logger, _options.AgentId);
        return Task.FromResult<AIAgent>(_projectClient.AsAIAgent(agentIdentifier));
    }

    private async Task<string> CreateProjectConversationIdAsync(CancellationToken cancellationToken)
    {
        ProjectConversation conversation = (await _projectClient
            .GetProjectOpenAIClient()
            .GetProjectConversationsClient()
            .CreateProjectConversationAsync(new ProjectConversationCreationOptions(), cancellationToken)
            .ConfigureAwait(false)).Value;

        return conversation.Id;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0009:Use 'System.MemoryExtensions.Split' for improved performance", Justification = "Configuration parsing is not hot-path work.")]
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
                $"Configured Foundry agent id '{agentId}' must be either '<agent-name>' to use the latest version or '<agent-name>:<version>' to pin a specific Azure AI Foundry prompt agent version."),
        };
    }
}
