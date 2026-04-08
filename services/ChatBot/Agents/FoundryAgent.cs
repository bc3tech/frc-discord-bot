namespace ChatBot.Agents;

using ChatBot.Telemetry;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

#pragma warning disable MEAI001

internal sealed class FoundryAgent(ChatClientAgent innerAgent, ILoggerFactory loggerFactory)
    : AgentBase(new OpenTelemetryAgent(innerAgent, innerAgent.Name ?? innerAgent.Id) { EnableSensitiveData = true })
{
    private readonly ChatClientAgent _innerAgent = innerAgent;
    private readonly ILogger _logger = loggerFactory.CreateLogger<FoundryAgent>();
    private ChatClientAgentSession? _boundConversationSession;

    public ValueTask<AgentSession> CreateSessionAsync(string conversationId, CancellationToken cancellationToken = default)
        => _innerAgent.CreateSessionAsync(conversationId, cancellationToken);

    protected override ValueTask<AgentSession> CreateSessionCoreAsync(CancellationToken cancellationToken = default)
        => _boundConversationSession is { } boundConversationSession
            ? ValueTask.FromResult<AgentSession>(boundConversationSession)
            : base.CreateSessionCoreAsync(cancellationToken);

    public async ValueTask<ChatClientAgentSession> BindConversationSessionAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(conversationId);

        _boundConversationSession = (ChatClientAgentSession)await _innerAgent.CreateSessionAsync(conversationId, cancellationToken).ConfigureAwait(false);
        return _boundConversationSession;
    }

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        using Activity? invokeActivity = this.StartInvokeAgentActivity();
        List<ChatMessage> pendingMessages = [.. messages];

        while (true)
        {
            AgentResponse response = await this.InnerAgent.RunAsync(pendingMessages, session, options, cancellationToken).ConfigureAwait(false);

            List<McpServerToolApprovalResponseContent> approvalResponses = CreateApprovalResponses(response.Messages);
            if (approvalResponses.Count is 0)
            {
                return response;
            }

            _logger.AutoApprovingHostedMcpRequests(this.AgentNameOrId, approvalResponses.Count, JsonSerializer.Serialize(approvalResponses));
            pendingMessages = [.. approvalResponses.Select(CreateApprovalMessage)];
        }
    }

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using Activity? invokeActivity = this.StartInvokeAgentActivity();
        using ToolTelemetryTracker toolTelemetry = new(this.ActivitySource, this.AgentNameOrId, invokeActivity?.Context);
        List<ChatMessage> pendingMessages = [.. messages];

        while (true)
        {
            List<AgentResponseUpdate> rawUpdates = [];
            List<McpServerToolApprovalResponseContent> approvalResponses = [];

            await foreach (AgentResponseUpdate update in this.InnerAgent
                .RunStreamingAsync(pendingMessages, session, options, cancellationToken)
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false))
            {
                toolTelemetry.Observe(update);
                rawUpdates.Add(update);

                AgentResponseUpdate? filteredUpdate = FilterApprovalRequests(update, approvalResponses);
                if (filteredUpdate is not null)
                {
                    yield return filteredUpdate;
                }
            }

            if (approvalResponses.Count is 0)
            {
                yield break;
            }

            _logger.AutoApprovingHostedMcpRequests(this.AgentNameOrId, approvalResponses.Count, JsonSerializer.Serialize(approvalResponses));
            pendingMessages = [.. approvalResponses.Select(CreateApprovalMessage)];
        }
    }

    private static List<McpServerToolApprovalResponseContent> CreateApprovalResponses(IEnumerable<ChatMessage> messages)
    {
        HashSet<string> seenRequestIds = [];
        List<McpServerToolApprovalResponseContent> approvalResponses = [];

        foreach (ChatMessage message in messages)
        {
            foreach (AIContent content in message.Contents)
            {
                if (content is McpServerToolApprovalRequestContent approvalRequest
                    && seenRequestIds.Add(approvalRequest.Id))
                {
                    approvalResponses.Add(CreateApprovalResponse(approvalRequest));
                }
            }
        }

        return approvalResponses;
    }

    private static AgentResponseUpdate? FilterApprovalRequests(
        AgentResponseUpdate update,
        ICollection<McpServerToolApprovalResponseContent> approvalResponses)
    {
        HashSet<string> seenRequestIds = [];
        List<AIContent>? filteredContents = null;
        bool removedApprovalRequest = false;

        foreach (AIContent content in update.Contents)
        {
            if (content is McpServerToolApprovalRequestContent approvalRequest)
            {
                removedApprovalRequest = true;
                if (seenRequestIds.Add(approvalRequest.Id))
                {
                    approvalResponses.Add(CreateApprovalResponse(approvalRequest));
                }

                continue;
            }

            filteredContents ??= [];
            filteredContents.Add(content);
        }

        if (!removedApprovalRequest)
        {
            return update;
        }

        if (filteredContents is not { Count: > 0 })
        {
            return null;
        }

        return new AgentResponseUpdate(update.Role, filteredContents)
        {
            AdditionalProperties = update.AdditionalProperties,
            AgentId = update.AgentId,
            AuthorName = update.AuthorName,
            CreatedAt = update.CreatedAt,
            MessageId = update.MessageId,
            RawRepresentation = update.RawRepresentation,
            ResponseId = update.ResponseId,
        };
    }

    private static McpServerToolApprovalResponseContent CreateApprovalResponse(McpServerToolApprovalRequestContent approvalRequest)
    {
        McpServerToolApprovalResponseContent approvalResponse = approvalRequest.CreateResponse(approved: true);
        if (approvalRequest.AdditionalProperties is { Count: > 0 })
        {
            approvalResponse.AdditionalProperties = [];
            foreach (KeyValuePair<string, object?> property in approvalRequest.AdditionalProperties)
            {
                approvalResponse.AdditionalProperties[property.Key] = property.Value;
            }
        }

        return approvalResponse;
    }

    private static ChatMessage CreateApprovalMessage(McpServerToolApprovalResponseContent approvalResponse)
        => new(ChatRole.Tool, [approvalResponse]);
}

#pragma warning restore MEAI001
