namespace DiscordBotFunctionApp.ChatBot;

using Azure;
using Azure.AI.Projects;
using Azure.Data.Tables;
using Azure.Identity;

using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Diagnostics;
using System.Reflection;

using Throws = Common.Throws;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return services
            .AddSingleton<MessageHandler>()
            .AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var credential = new ClientSecretCredential(
                    Throws.IfNullOrWhiteSpace(config[Constants.Configuration.Azure.AI.Project.Credentials.TenantId]),
                    Throws.IfNullOrWhiteSpace(config[Constants.Configuration.Azure.AI.Project.Credentials.ClientId]),
                    Throws.IfNullOrWhiteSpace(config[Constants.Configuration.Azure.AI.Project.Credentials.ClientSecret]));

                return new AIProjectClient(Throws.IfNullOrWhiteSpace(config[Constants.Configuration.Azure.AI.Project.ConnectionString]), credential);
            })
            .AddSingleton(sp => sp.GetRequiredService<AIProjectClient>().GetAgentsClient())
            .AddSingleton(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("ConfigureChatBotFunctionality");
                AgentsClient client = sp.GetRequiredService<AgentsClient>();
                var projectClient = sp.GetRequiredService<AIProjectClient>();

                // Statbotics' open api def is invalid and bombs when creating the agent
                //var statboticsToolDef = new OpenApiToolDefinition("statbotics", "Statbotics API", BinaryData.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DiscordBotFunctionApp.Apis.statbotics.json")!), new OpenApiAnonymousAuthDetails());

                var blueAllianceConnId = $"/subscriptions/c6311630-ca87-4f08-be8f-100203cec93c/resourceGroups/hurlburb-bearmetal/providers/Microsoft.MachineLearningServices/workspaces/msft-bearmetal/connections/the-blue-alliance-2";
                var blueAllianceToolDef = new OpenApiToolDefinition("thebluealliance", "The Blue Alliance API", BinaryData.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DiscordBotFunctionApp.Apis.thebluealliance.json")!), new OpenApiConnectionAuthDetails(new OpenApiConnectionSecurityScheme(blueAllianceConnId)));

                // FRC-Events API def is invalid and bombs when creating the agent
                //var frcEventsConnId = $"/subscriptions/c6311630-ca87-4f08-be8f-100203cec93c/resourceGroups/hurlburb-bearmetal/providers/Microsoft.MachineLearningServices/workspaces/msft-bearmetal/connections/frc-events-api";
                //var frcEventsYaml = Assembly.GetExecutingAssembly().GetManifestResourceStream("DiscordBotFunctionApp.Apis.frc-events.yaml")!;
                //var yamlReader = new OpenApiStreamReader().Read(frcEventsYaml, out var _);
                //var jsonOutput = yamlReader.SerializeAsJson(Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

                //var frcEventsToolDef = new OpenApiToolDefinition("frcevents", "FRC Events API", BinaryData.FromString(jsonOutput), new OpenApiConnectionAuthDetails(new OpenApiConnectionSecurityScheme(frcEventsConnId)));

                var existingAgentId = client.GetAgents().Value.FirstOrDefault(i => i.Name is "BearMetalBot-SK")?.Id;
                Agent? agent = !string.IsNullOrWhiteSpace(existingAgentId) ? client.GetAgent(existingAgentId).Value : null;
                if (existingAgentId is null)
                {
                    logger.CreatingNewAgent();
                    var baseAgent = client.GetAgent(Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.Agents.AgentId]));
                    agent = client.CreateAgent(
                        model: baseAgent.Value.Model,
                        name: $"{baseAgent.Value.Name}-SK",
                        instructions: baseAgent.Value.Instructions,
                        tools: [.. baseAgent.Value.Tools, blueAllianceToolDef],
                        toolResources: new ToolResources
                        {
                            CodeInterpreter = baseAgent.Value.ToolResources.CodeInterpreter,
                            FileSearch = baseAgent.Value.ToolResources.FileSearch,
                        });
                    logger.CreatedNewAgentWithIDAgentId(agent.Id);
                }

                Debug.Assert(agent is not null);
                return new AzureAIAgent(agent, client, templateFactory: sp.GetService<IPromptTemplateFactory>());
            })
            .AddSingleton<ChatRunner>()
            .AddHostedService<ChatBotInitializationService>();
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
