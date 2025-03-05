namespace DiscordBotFunctionApp.ChatBot;

using Azure.AI.Projects;
using Azure.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;

using Throws = Common.Throws;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return services
            .AddSingleton<MessageHandler>()
            .AddSingleton(sp => new AIProjectClient(Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.ProjectConnectionString]), sp.GetRequiredService<TokenCredential>()))
            .AddSingleton(sp => sp.GetRequiredService<AIProjectClient>().GetAgentsClient())
            .AddSingleton(sp =>
            {
                AgentsClient client = sp.GetRequiredService<AgentsClient>();
                var agent = client.GetAgent(Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.Agents.AgentId]));
                return new AzureAIAgent(agent, client, templateFactory: sp.GetService<IPromptTemplateFactory>());
            });
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
