namespace FunctionApp.ChatBot;

using Azure;
using Azure.AI.Agents.Persistent;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ChatBotAgentResolver(PersistentAgentsClient agentsClient, IConfiguration configuration, ILogger<ChatBotAgentResolver> logger) : IDisposable
{
    private readonly SemaphoreSlim agentLock = new(1, 1);
    private PersistentAgent? cachedAgent;

    public async Task<PersistentAgent> GetConfiguredAgentAsync(CancellationToken cancellationToken = default)
    {
        if (cachedAgent is not null)
        {
            return cachedAgent;
        }

        await agentLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (cachedAgent is not null)
            {
                return cachedAgent;
            }

            var configuredAgentId = configuration[Constants.Configuration.Azure.AI.Agents.ChatAgentId];
            if (string.IsNullOrWhiteSpace(configuredAgentId))
            {
                throw new InvalidOperationException($"Azure AI Foundry agent ID is required. Set {Constants.Configuration.Azure.AI.Agents.ChatAgentId} to the Foundry agent ID for the configured project.");
            }

            try
            {
                var agent = (await agentsClient.Administration.GetAgentAsync(configuredAgentId, cancellationToken).ConfigureAwait(false)).Value;
                logger.UsingAzureAIFoundryAgentAgentIdAgentName(agent.Id, agent.Name);
                cachedAgent = agent;
                return agent;
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                throw new InvalidOperationException($"Azure AI Foundry agent '{configuredAgentId}' was not found. Create that agent in the configured project or override {Constants.Configuration.Azure.AI.Agents.ChatAgentId}.", e);
            }
        }
        finally
        {
            agentLock.Release();
        }
    }

    public void Dispose() => agentLock.Dispose();
}
