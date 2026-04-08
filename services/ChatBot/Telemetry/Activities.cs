namespace ChatBot.Telemetry;

using Microsoft.Agents.AI;

using System.Diagnostics;
using System.Globalization;
using System.Threading;

internal static class Activities
{
    private const string AppActivitySourceName = "foundry-chat";

    public static readonly ActivitySource AppActivitySource = new(AppActivitySourceName, tags: [new("gen_ai.system", AppActivitySourceName)]);

    public const string AgentActivitySourceName = "foundry-chat.agent";

    public static readonly ActivitySource AgentActivitySource = new(AgentActivitySourceName, tags: [new("gen_ai.system", AppActivitySourceName)]);

    public static ActivitySource GetSourceForAgent(AIAgent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);
        return AgentActivitySource;
    }

    private static readonly AsyncLocal<ConversationActivityContextNode?> s_conversationActivityContext = new();

    public static IDisposable PushConversationParent(Activity? activity = null)
    {
        Activity? effectiveActivity = activity ?? Activity.Current;
        if (effectiveActivity is null || effectiveActivity.IdFormat != ActivityIdFormat.W3C)
        {
            return NoopDisposable.Instance;
        }

        ConversationActivityContextNode? previous = s_conversationActivityContext.Value;
        s_conversationActivityContext.Value = new(effectiveActivity.Context, previous);
        return new ConversationActivityContextScope(previous);
    }

    public static bool TryGetConversationParentContext(out ActivityContext context)
    {
        if (s_conversationActivityContext.Value is { } node)
        {
            context = node.Context;
            return true;
        }

        context = default;
        return false;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Make code ugly")]
    public static Activity? StartActivityWithFallbackParent(ActivitySource source, string name, ActivityKind kind = ActivityKind.Internal)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Activity? currentActivity = Activity.Current;
        if (currentActivity is not null && IsPreferredParentActivity(currentActivity))
        {
            return source.StartActivity(name, kind);
        }

        if (TryGetConversationParentContext(out ActivityContext conversationParent))
        {
            return source.StartActivity(name, kind, conversationParent);
        }

        return currentActivity is not null
            ? source.StartActivity(name, kind)
            : source.StartActivity(name, kind);
    }

    public static bool TryGetPreferredParentContext(out ActivityContext context)
    {
        Activity? currentActivity = Activity.Current;
        if (currentActivity is not null
            && currentActivity.IdFormat == ActivityIdFormat.W3C
            && !string.IsNullOrWhiteSpace(currentActivity.Id)
            && IsPreferredParentActivity(currentActivity))
        {
            context = currentActivity.Context;
            return true;
        }

        if (TryGetConversationParentContext(out ActivityContext parentContext))
        {
            context = parentContext;
            return true;
        }

        if (currentActivity is not null && currentActivity.IdFormat == ActivityIdFormat.W3C && !string.IsNullOrWhiteSpace(currentActivity.Id))
        {
            context = currentActivity.Context;
            return true;
        }

        context = default;
        return false;
    }

    public static ActivityContext CreateConversationRootContext(string scopeType, string scopeKey, string? conversationId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scopeType);
        ArgumentException.ThrowIfNullOrWhiteSpace(scopeKey);

        Activity? previousActivity = Activity.Current;

        try
        {
            // Force a real root span instead of inheriting from the current request/activity.
            Activity.Current = null;

            using Activity? rootActivity = AppActivitySource.StartActivity("chat.conversation", ActivityKind.Internal);
            if (rootActivity is not null)
            {
                rootActivity.DisplayName = "chat.conversation";
                rootActivity.SetTag("gen_ai.operation.name", "chat_conversation");
                rootActivity.SetTag("discord.chat.scope.type", scopeType);
                rootActivity.SetTag("discord.chat.scope.key", scopeKey);

                if (!string.IsNullOrWhiteSpace(conversationId))
                {
                    rootActivity.SetTag("gen_ai.conversation.id", conversationId);
                }

                return rootActivity.Context;
            }

            using Activity fallbackActivity = new("chat.conversation");
            fallbackActivity.SetIdFormat(ActivityIdFormat.W3C);
            fallbackActivity.Start();
            return fallbackActivity.Context;
        }
        finally
        {
            Activity.Current = previousActivity;
        }
    }

    public static bool TryParseTraceParent(string? traceParent, out ActivityContext context)
        => !string.IsNullOrWhiteSpace(traceParent)
            && ActivityContext.TryParse(traceParent, null, out context);

    public static string FormatTraceParent(ActivityContext context)
        => $"00-{context.TraceId.ToHexString()}-{context.SpanId.ToHexString()}-{FormatTraceFlags(context.TraceFlags)}";

    private static string FormatTraceFlags(ActivityTraceFlags flags)
        => (((int)flags) & 0xFF).ToString("x2", CultureInfo.InvariantCulture);

    private static bool IsPreferredParentActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        string? operationName = activity.GetTagItem("gen_ai.operation.name") as string;
        return operationName is "invoke_agent" or "execute_tool" or "chat_turn"
            || string.Equals(activity.OperationName, "chat.turn", StringComparison.Ordinal)
            || string.Equals(activity.DisplayName, "chat.turn", StringComparison.Ordinal);
    }

    private sealed record ConversationActivityContextNode(ActivityContext Context, ConversationActivityContextNode? Previous);

    private sealed class ConversationActivityContextScope(ConversationActivityContextNode? previous) : IDisposable
    {
        public void Dispose() => s_conversationActivityContext.Value = previous;
    }

    private sealed class NoopDisposable : IDisposable
    {
        public static NoopDisposable Instance { get; } = new();

        private NoopDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}
