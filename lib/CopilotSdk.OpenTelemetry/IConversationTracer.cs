namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;

/// <summary>
/// Opens a per-turn span that is parented to the persisted conversation root, so that all turns
/// of a single conversation share a trace identifier in OpenTelemetry-compatible backends.
/// </summary>
public interface IConversationTracer
{
    /// <summary>
    /// Begins a new <c>chat</c> turn span for <paramref name="conversationId"/>.
    /// On the first turn for an id, a zero-duration root span is created and persisted via the
    /// configured <see cref="IConversationTraceContextStore"/>; subsequent turns are parented to
    /// that persisted root.
    /// </summary>
    /// <param name="conversationId">Stable identifier for the conversation (e.g., a Discord conversation key).</param>
    /// <param name="rootTags">Optional tags applied to the root span the first time it is created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A scope that owns the active turn <see cref="Activity"/>; dispose to end the turn.</returns>
    Task<IConversationTurnScope> BeginTurnAsync(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags,
        CancellationToken cancellationToken);
}

/// <summary>Scope returned by <see cref="IConversationTracer.BeginTurnAsync"/>.</summary>
public interface IConversationTurnScope : IAsyncDisposable
{
    /// <summary>The live turn <see cref="Activity"/>, or <c>null</c> if no listener is sampling.</summary>
    Activity? Activity { get; }

    /// <summary>The conversation id this turn belongs to.</summary>
    string ConversationId { get; }
}
