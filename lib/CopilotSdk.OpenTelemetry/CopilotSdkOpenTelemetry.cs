namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;

/// <summary>
/// Constants for the OpenTelemetry instrumentation surface used by this library.
/// All spans emitted by <see cref="ConversationTracer"/> and <see cref="CopilotSessionTelemetry"/>
/// are produced from a single <see cref="ActivitySource"/> named <see cref="ActivitySourceName"/>.
/// </summary>
public static class CopilotSdkOpenTelemetry
{
    /// <summary>
    /// Name of the <see cref="ActivitySource"/> used for all spans emitted by this library.
    /// Register with <c>TracerProviderBuilder.AddSource(CopilotSdkOpenTelemetry.ActivitySourceName)</c>
    /// (or use the <c>AddCopilotSdkOpenTelemetry()</c> extension).
    /// </summary>
    public const string ActivitySourceName = "CopilotSdk.OpenTelemetry";

    /// <summary>OpenTelemetry GenAI system identifier for the GitHub Copilot SDK.</summary>
    public const string GenAiSystemValue = "github.copilot";

    /// <summary>The shared <see cref="ActivitySource"/> used by the library.</summary>
    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>OpenTelemetry GenAI semantic-convention attribute names used by this library.</summary>
    public static class GenAiAttributes
    {
        /// <summary>OTel GenAI: provider/system identifier (e.g. <c>github.copilot</c>).</summary>
        public const string System = "gen_ai.system";

        /// <summary>OTel GenAI: high-level operation name (<c>chat</c>, <c>execute_tool</c>).</summary>
        public const string OperationName = "gen_ai.operation.name";

        /// <summary>OTel GenAI: stable conversation identifier shared across all turns of one conversation.</summary>
        public const string ConversationId = "gen_ai.conversation.id";

        /// <summary>OTel GenAI: model identifier reported by the provider.</summary>
        public const string ResponseModel = "gen_ai.response.model";

        /// <summary>OTel GenAI: prompt token count.</summary>
        public const string UsageInputTokens = "gen_ai.usage.input_tokens";

        /// <summary>OTel GenAI: completion token count.</summary>
        public const string UsageOutputTokens = "gen_ai.usage.output_tokens";

        /// <summary>OTel GenAI: tool name for an <c>execute_tool</c> span.</summary>
        public const string ToolName = "gen_ai.tool.name";

        /// <summary>OTel GenAI: tool call identifier produced by the provider.</summary>
        public const string ToolCallId = "gen_ai.tool.call.id";
    }

    /// <summary>Span operation-name constants.</summary>
    public static class Operations
    {
        /// <summary>Root + per-turn conversation operation.</summary>
        public const string Chat = "chat";

        /// <summary>Per-tool-call operation.</summary>
        public const string ExecuteTool = "execute_tool";
    }
}
