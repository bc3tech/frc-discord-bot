namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;

/// <summary>
/// Persisted W3C trace context for the conversation root span.
/// </summary>
/// <remarks>
/// The root span is started and ended atomically (zero duration) so its <see cref="ActivityContext"/>
/// can be captured and later used as a parent for per-turn spans even after the originating process
/// exits. Implementations of <see cref="IConversationTraceContextStore"/> persist these values
/// keyed by a conversation identifier.
/// </remarks>
/// <param name="TraceId">W3C trace id (32-char lowercase hex).</param>
/// <param name="SpanId">W3C span id of the root conversation span (16-char lowercase hex).</param>
/// <param name="TraceFlags">W3C trace flags (typically <c>01</c> when sampled).</param>
/// <param name="StartTimestamp">Optional timestamp of the original conversation span start, used to reconstitute real duration.</param>
public sealed record ConversationTraceContext(string TraceId, string SpanId, byte TraceFlags, DateTimeOffset? StartTimestamp = null)
{
    /// <summary>Build a <see cref="ConversationTraceContext"/> from a live <see cref="ActivityContext"/>.</summary>
    public static ConversationTraceContext FromActivityContext(ActivityContext context, DateTimeOffset? startTimestamp = null) =>
        new(
            context.TraceId.ToHexString(),
            context.SpanId.ToHexString(),
            (byte)context.TraceFlags,
            startTimestamp);

    /// <summary>Materialise this persisted context as an <see cref="ActivityContext"/> usable as a parent.</summary>
    public ActivityContext ToActivityContext() =>
        new(
            ActivityTraceId.CreateFromString(this.TraceId.AsSpan()),
            ActivitySpanId.CreateFromString(this.SpanId.AsSpan()),
            (ActivityTraceFlags)this.TraceFlags,
            traceState: null,
            isRemote: true);
}
