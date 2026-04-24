namespace ChatBot.Diagnostics;

using BC3Technologies.DiscordGpt.Copilot;

using CopilotSdk.OpenTelemetry;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging;

/// <summary>
/// Bridges the Copilot harness's per-session lifecycle to
/// <see cref="CopilotSessionTelemetry"/>, emitting OTel <c>execute_tool</c> child spans for
/// each tool call inside the in-flight <c>chat.turn</c> activity.
/// </summary>
internal sealed class CopilotTelemetrySessionSubscriber(ILogger<CopilotTelemetrySessionSubscriber> logger)
    : ISessionEventSubscriber
{
    public IDisposable Subscribe(CopilotSession session)
    {
        return CopilotSessionTelemetry.Subscribe(session, logger);
    }
}
