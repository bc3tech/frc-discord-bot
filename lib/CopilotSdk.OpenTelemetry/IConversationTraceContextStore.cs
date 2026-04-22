namespace CopilotSdk.OpenTelemetry;

/// <summary>
/// Persists per-conversation root <see cref="ConversationTraceContext"/> values keyed by a stable
/// conversation identifier. The same identifier must be supplied for every turn of a logical
/// conversation; calling <see cref="RemoveAsync"/> ends the conversation, after which the next
/// turn starts a brand-new trace.
/// </summary>
public interface IConversationTraceContextStore
{
    /// <summary>Returns the persisted context for <paramref name="conversationId"/> or <c>null</c> if none exists.</summary>
    Task<ConversationTraceContext?> TryGetAsync(string conversationId, CancellationToken cancellationToken);

    /// <summary>Persists or replaces the context for <paramref name="conversationId"/>.</summary>
    Task SetAsync(string conversationId, ConversationTraceContext context, CancellationToken cancellationToken);

    /// <summary>Removes the persisted context for <paramref name="conversationId"/>; subsequent turns start a new trace.</summary>
    Task RemoveAsync(string conversationId, CancellationToken cancellationToken);
}
