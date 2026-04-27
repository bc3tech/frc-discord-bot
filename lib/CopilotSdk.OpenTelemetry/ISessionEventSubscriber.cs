namespace CopilotSdk.OpenTelemetry;

using GitHub.Copilot.SDK;

/// <summary>
/// Attaches additional listeners to a freshly-created Copilot <see cref="CopilotSession"/>.
/// </summary>
/// <remarks>
/// All subscribers registered in DI are invoked by the prompt harness
/// immediately after a session is created, before any prompts are sent. The
/// <see cref="IDisposable"/> returned by <see cref="Subscribe"/> is disposed when the harness
/// finishes the prompt and tears down the session. Use this hook to wire cross-cutting
/// concerns (telemetry, audit logging, custom metrics) without modifying the harness itself.
/// </remarks>
public interface ISessionEventSubscriber
{
    /// <summary>
    /// Subscribe to events on <paramref name="session"/>.
    /// </summary>
    /// <param name="session">The freshly-created Copilot session.</param>
    /// <returns>
    /// A disposable representing the subscription. Disposed when the harness completes the
    /// current prompt run.
    /// </returns>
    IDisposable Subscribe(CopilotSession session);
}
