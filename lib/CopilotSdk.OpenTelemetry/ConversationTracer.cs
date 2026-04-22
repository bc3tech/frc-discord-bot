namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;

/// <summary>
/// Default <see cref="IConversationTracer"/> implementation backed by an
/// <see cref="IConversationTraceContextStore"/>.
/// </summary>
public sealed class ConversationTracer(IConversationTraceContextStore store) : IConversationTracer
{
    private readonly IConversationTraceContextStore _store = store ?? throw new ArgumentNullException(nameof(store));

    /// <inheritdoc />
    public async Task<IConversationTurnScope> BeginTurnAsync(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);

        var existing = await this._store.TryGetAsync(conversationId, cancellationToken).ConfigureAwait(false);
        ConversationTraceContext rootContext;

        if (existing is null)
        {
            rootContext = CreateAndPersistRoot(conversationId, rootTags);
            await this._store.SetAsync(conversationId, rootContext, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            rootContext = existing;
        }

        var parent = rootContext.ToActivityContext();
        var turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
            CopilotSdkOpenTelemetry.Operations.Chat,
            kind: ActivityKind.Server,
            parentContext: parent,
            tags:
            [
                new(CopilotSdkOpenTelemetry.GenAiAttributes.System, CopilotSdkOpenTelemetry.GenAiSystemValue),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.Chat),
                new(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId, conversationId),
            ],
            links: null);

        return new ConversationTurnScope(conversationId, turn);
    }

    private static ConversationTraceContext CreateAndPersistRoot(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags)
    {
        // Open the root span detached from any ambient activity so the trace id is fresh.
        var previous = Activity.Current;
        Activity.Current = null;
        try
        {
            using var root = CopilotSdkOpenTelemetry.ActivitySource.StartActivity(
                CopilotSdkOpenTelemetry.Operations.Chat,
                kind: ActivityKind.Server,
                parentContext: default(ActivityContext),
                tags:
                [
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.System, CopilotSdkOpenTelemetry.GenAiSystemValue),
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName, CopilotSdkOpenTelemetry.Operations.Chat),
                    new(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId, conversationId),
                ],
                links: null);

            if (root is null)
            {
                // No listener — synthesise a sampled-out context so the rest of the API still works.
                return new ConversationTraceContext(
                    ActivityTraceId.CreateRandom().ToHexString(),
                    ActivitySpanId.CreateRandom().ToHexString(),
                    TraceFlags: 0);
            }

            if (rootTags is not null)
            {
                foreach (var (key, value) in rootTags)
                {
                    root.SetTag(key, value);
                }
            }

            return ConversationTraceContext.FromActivityContext(root.Context);
        }
        finally
        {
            Activity.Current = previous;
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
