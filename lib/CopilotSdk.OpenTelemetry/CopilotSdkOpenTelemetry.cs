namespace CopilotSdk.OpenTelemetry;

using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// Constants for the OpenTelemetry instrumentation surface used by this library.
/// All spans emitted by <see cref="ConversationTracer"/>
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

    /// <summary>
    /// Name of the <see cref="Meter"/> used for all metrics emitted by this library.
    /// Register with <c>MeterProviderBuilder.AddMeter(CopilotSdkOpenTelemetry.MeterName)</c>
    /// (or use the <c>AddCopilotSdkOpenTelemetry(MeterProviderBuilder)</c> extension).
    /// </summary>
    public const string MeterName = "CopilotSdk.OpenTelemetry";

    /// <summary>OpenTelemetry GenAI provider identifier for the GitHub Copilot SDK.</summary>
    public const string GenAiProviderValue = "github.copilot";

    /// <summary>The shared <see cref="ActivitySource"/> used by the library.</summary>
    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>The shared <see cref="Meter"/> used by the library for GenAI metrics.</summary>
    internal static readonly Meter Meter = new(MeterName);

    /// <summary>Histogram recording token usage per LLM call, tagged by model and operation.</summary>
    internal static readonly Histogram<long> TokenUsageHistogram =
        Meter.CreateHistogram<long>("gen_ai.client.token.usage", unit: "token", description: "Number of tokens used per GenAI request");

    /// <summary>OpenTelemetry GenAI semantic-convention attribute names used by this library.</summary>
    public static class GenAiAttributes
    {
        /// <summary>OTel GenAI: provider name (e.g. <c>github.copilot</c>). Replaces deprecated <c>gen_ai.system</c>.</summary>
        public const string ProviderName = "gen_ai.provider.name";

        /// <summary>OTel GenAI: high-level operation name (<c>chat</c>, <c>execute_tool</c>, <c>conversation</c>, <c>invoke_agent</c>).</summary>
        public const string OperationName = "gen_ai.operation.name";

        /// <summary>OTel GenAI: stable conversation identifier shared across all turns of one conversation.</summary>
        public const string ConversationId = "gen_ai.conversation.id";

        /// <summary>OTel GenAI: model identifier requested by the caller.</summary>
        public const string RequestModel = "gen_ai.request.model";

        /// <summary>OTel GenAI: model identifier reported by the provider.</summary>
        public const string ResponseModel = "gen_ai.response.model";

        /// <summary>OTel GenAI: finish reasons reported by the provider.</summary>
        public const string ResponseFinishReasons = "gen_ai.response.finish_reasons";

        /// <summary>OTel GenAI: prompt token count.</summary>
        public const string UsageInputTokens = "gen_ai.usage.input_tokens";

        /// <summary>OTel GenAI: completion token count.</summary>
        public const string UsageOutputTokens = "gen_ai.usage.output_tokens";

        /// <summary>OTel GenAI: cached read token count (prompt tokens served from cache).</summary>
        public const string UsageCacheReadTokens = "gen_ai.usage.cache_read_input_tokens";

        /// <summary>OTel GenAI: cached write token count (prompt tokens written to cache).</summary>
        public const string UsageCacheWriteTokens = "gen_ai.usage.cache_creation_input_tokens";

        /// <summary>OTel GenAI: tool name for an <c>execute_tool</c> span.</summary>
        public const string ToolName = "gen_ai.tool.name";

        /// <summary>OTel GenAI: tool call identifier produced by the provider.</summary>
        public const string ToolCallId = "gen_ai.tool.call.id";

        /// <summary>OTel GenAI: agent name for an <c>invoke_agent</c> span.</summary>
        public const string AgentName = "gen_ai.agent.name";

        /// <summary>OTel GenAI: agent identifier for an <c>invoke_agent</c> span.</summary>
        public const string AgentId = "gen_ai.agent.id";

        /// <summary>OTel GenAI: prompt content (sensitive — gated by <see cref="CopilotSdkOpenTelemetryOptions.RecordSensitiveData"/>).</summary>
        public const string ContentPrompt = "gen_ai.content.prompt";

        /// <summary>OTel GenAI: completion content (sensitive — gated by <see cref="CopilotSdkOpenTelemetryOptions.RecordSensitiveData"/>).</summary>
        public const string ContentCompletion = "gen_ai.content.completion";

        /// <summary>OTel: server address (endpoint hostname) for the upstream LLM service.</summary>
        public const string ServerAddress = "server.address";

        /// <summary>Custom: MCP server name for tool calls routed through an MCP server.</summary>
        public const string McpServerName = "mcp.server.name";
    }

    /// <summary>Span operation-name constants.</summary>
    public static class Operations
    {
        /// <summary>Multi-turn conversation root span.</summary>
        public const string Conversation = "conversation";

        /// <summary>Per-turn chat operation.</summary>
        public const string Chat = "chat";

        /// <summary>Agent/sub-agent invocation.</summary>
        public const string InvokeAgent = "invoke_agent";

        /// <summary>Per-tool-call operation.</summary>
        public const string ExecuteTool = "execute_tool";
    }
}
