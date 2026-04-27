namespace CopilotSdk.OpenTelemetry;

using System.Collections.Concurrent;
using System.Diagnostics;

/// <summary>
/// Default <see cref="IConversationTracer"/> implementation backed by an
/// <see cref="IConversationTraceContextStore"/>.
/// </summary>
public sealed class ConversationTracer(IConversationTraceContextStore store) : IConversationTracer
{
    private readonly IConversationTraceContextStore _store = store ?? throw new ArgumentNullException(nameof(store));
    private readonly ConcurrentDictionary<string, Activity> _liveConversations = new(StringComparer.Ordinal);

    private static readonly KeyValuePair<string, object?>[] ConversationTags =
    [
        new(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName, CopilotSdkOpenTelemetry.GenAiProviderValue),
        new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.Conversation),
    ];

    /// <inheritdoc />
    public async Task<IConversationScope> CreateOrResumeConversationAsync(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);

        ConversationTraceContext? existing = await _store.TryGetAsync(conversationId, cancellationToken).ConfigureAwait(false);

        if (existing is null)
        {
            return await CreateNewConversationAsync(conversationId, rootTags, cancellationToken).ConfigureAwait(false);
        }

        return ResumeConversation(conversationId, existing);
    }

    /// <inheritdoc />
    public IConversationTurnScope BeginTurn(string conversationId)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);

        KeyValuePair<string, object?>[] turnTags =
        [
            new(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName, CopilotSdkOpenTelemetry.GenAiProviderValue),
            new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.Chat),
            new(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId, conversationId),
        ];

        Activity? turn;
        if (_liveConversations.TryGetValue(conversationId, out Activity? conversationActivity))
        {
            turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                CopilotSdkOpenTelemetry.Operations.Chat,
                kind: ActivityKind.Client,
                parentContext: conversationActivity.Context,
                tags: turnTags,
                links: null);
        }
        else
        {
            turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                CopilotSdkOpenTelemetry.Operations.Chat,
                kind: ActivityKind.Client,
                parentContext: default,
                tags: turnTags,
                links: null);
        }

        return new ConversationTurnScope(conversationId, turn);
    }

    private async Task<IConversationScope> CreateNewConversationAsync(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags,
        CancellationToken cancellationToken)
    {
        Activity? previous = Activity.Current;
        Activity.Current = null;

        Activity? conversation = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
            CopilotSdkOpenTelemetry.Operations.Conversation,
            kind: ActivityKind.Internal,
            parentContext: default,
            tags:
            [
                .. ConversationTags,
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId, conversationId),
            ],
            links: null);

        if (conversation is null)
        {
            Activity.Current = previous;

            var sampledOut = new ConversationTraceContext(
                ActivityTraceId.CreateRandom().ToHexString(),
                ActivitySpanId.CreateRandom().ToHexString(),
                TraceFlags: 0,
                StartTimestamp: DateTimeOffset.UtcNow);

            await _store.SetAsync(conversationId, sampledOut, cancellationToken).ConfigureAwait(false);
            return new ConversationScope(conversationId, activity: null, isNew: true, previous, _liveConversations);
        }

        if (rootTags is not null)
        {
            foreach ((string? key, object? value) in rootTags)
            {
                conversation.SetTag(key, value);
            }
        }

        _liveConversations[conversationId] = conversation;

        var context = ConversationTraceContext.FromActivityContext(conversation.Context, DateTimeOffset.UtcNow);
        await _store.SetAsync(conversationId, context, cancellationToken).ConfigureAwait(false);

        return new ConversationScope(conversationId, conversation, isNew: true, previous, _liveConversations);
    }

    private IConversationScope ResumeConversation(string conversationId, ConversationTraceContext existing)
    {
        var parentContext = existing.ToActivityContext();

        Activity? previous = Activity.Current;
        Activity.Current = null;

        Activity? conversation = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
            CopilotSdkOpenTelemetry.Operations.Conversation,
            kind: ActivityKind.Internal,
            parentContext: parentContext,
            tags:
            [
                .. ConversationTags,
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId, conversationId),
            ],
            links: null);

        if (conversation is not null)
        {
            _liveConversations[conversationId] = conversation;
        }

        return new ConversationScope(conversationId, conversation, isNew: false, previous, _liveConversations);
    }

    private sealed class ConversationScope : IConversationScope
    {
        private readonly Activity? _previousAmbient;
        private readonly ConcurrentDictionary<string, Activity> _liveConversations;

        public ConversationScope(
            string conversationId,
            Activity? activity,
            bool isNew,
            Activity? previousAmbient,
            ConcurrentDictionary<string, Activity> liveConversations)
        {
            ConversationId = conversationId;
            Activity = activity;
            IsNew = isNew;
            _previousAmbient = previousAmbient;
            _liveConversations = liveConversations;
        }

        public Activity? Activity { get; }

        public string ConversationId { get; }

        public bool IsNew { get; }

        public ValueTask DisposeAsync()
        {
            _liveConversations.TryRemove(ConversationId, out _);
            this.Activity?.Dispose();
            System.Diagnostics.Activity.Current = _previousAmbient;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class ConversationTurnScope(string conversationId, Activity? activity) : IConversationTurnScope
    {
        public Activity? Activity { get; } = activity;

        public string ConversationId { get; } = conversationId;

        public ValueTask DisposeAsync()
        {
            this.Activity?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
