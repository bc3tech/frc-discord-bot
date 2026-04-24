namespace CopilotSdk.OpenTelemetry;

using System.Collections.Concurrent;

/// <summary>
/// Process-local <see cref="IConversationTraceContextStore"/> backed by a concurrent dictionary.
/// Suitable for single-process scenarios and tests; multi-instance deployments should use a
/// durable store (Azure Tables, Redis, etc.).
/// </summary>
public sealed class InMemoryConversationTraceContextStore : IConversationTraceContextStore
{
    private readonly ConcurrentDictionary<string, ConversationTraceContext> _store = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<ConversationTraceContext?> TryGetAsync(string conversationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);
        return Task.FromResult(_store.TryGetValue(conversationId, out ConversationTraceContext? ctx) ? ctx : null);
    }

    /// <inheritdoc />
    public Task SetAsync(string conversationId, ConversationTraceContext context, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);
        _store[conversationId] = context;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string conversationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);
        _store.TryRemove(conversationId, out _);
        return Task.CompletedTask;
    }
}
