namespace ChatBot.Configuration;

using BC3Technologies.DiscordGpt.Copilot;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Sets the orchestrator model on the session config from <c>AI:Copilot:Model</c>.
/// Registered after <see cref="BC3Technologies.DiscordGpt.Copilot.DefaultSessionConfigSource"/>
/// so the model override is applied last without triggering a provider-config reset.
/// </summary>
internal sealed class ModelSessionConfigSource(IConfiguration configuration) : ISessionConfigSource
{
    public ValueTask ConfigureAsync(SessionConfig sessionConfig, CancellationToken cancellationToken)
    {
        string model = configuration.GetValue<string>(ChatBotConstants.Configuration.Copilot.Model)
            ?? throw new InvalidOperationException($"Required configuration '{ChatBotConstants.Configuration.Copilot.Model}' is missing.");

        sessionConfig.Model = model;

        return ValueTask.CompletedTask;
    }
}
