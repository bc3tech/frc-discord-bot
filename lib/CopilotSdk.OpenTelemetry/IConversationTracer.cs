namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;

/// <summary>
/// Two-step API for creating a conversation-level span and per-turn child spans so that all
/// turns of a single conversation share a trace identifier in OpenTelemetry-compatible backends.
/// </summary>
public interface IConversationTracer
{
    /// <summary>
    /// Creates or resumes a conversation-level span. On first invocation for a
    /// <paramref name="conversationId"/>, creates a new <c>conversation</c>
    /// <see cref="Activity"/> and persists its trace context. On subsequent invocations,
    /// reconstitutes the persisted context as a remote parent.
    /// The returned scope keeps the conversation <see cref="Activity"/> alive until disposed.
    /// </summary>
    /// <param name="conversationId">Stable identifier for the conversation (e.g., a Discord conversation key).</param>
    /// <param name="rootTags">Optional tags applied to the conversation span the first time it is created.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A scope that owns the conversation <see cref="Activity"/>; dispose to end it.</returns>
    Task<IConversationScope> CreateOrResumeConversationAsync(
        string conversationId,
        IReadOnlyDictionary<string, object?>? rootTags,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a <c>chat</c> turn span as a child of the current conversation.
    /// Must be called within a live <see cref="IConversationScope"/>
    /// (<see cref="Activity.Current"/> should be the conversation <see cref="Activity"/>).
    /// </summary>
    /// <param name="conversationId">Stable conversation identifier.</param>
    /// <returns>A scope that owns the turn <see cref="Activity"/>; dispose to end it.</returns>
    IConversationTurnScope BeginTurn(string conversationId);
}

/// <summary>Scope that owns the conversation <see cref="Activity"/>. Dispose to end the conversation span.</summary>
public interface IConversationScope : IAsyncDisposable
{
    /// <summary>The conversation <see cref="Activity"/>, or <c>null</c> if no listener is sampling.</summary>
    Activity? Activity { get; }

    /// <summary>The conversation id this scope belongs to.</summary>
    string ConversationId { get; }

    /// <summary>Whether this is the first time this conversation was seen (new trace created).</summary>
    bool IsNew { get; }
}

/// <summary>Scope that owns a per-turn <see cref="Activity"/>. Dispose to end the turn span.</summary>
public interface IConversationTurnScope : IAsyncDisposable
{
    /// <summary>The live turn <see cref="Activity"/>, or <c>null</c> if no listener is sampling.</summary>
    Activity? Activity { get; }

    /// <summary>The conversation id this turn belongs to.</summary>
    string ConversationId { get; }
}
