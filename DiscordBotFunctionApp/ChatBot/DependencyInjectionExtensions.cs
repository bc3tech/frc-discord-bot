namespace DiscordBotFunctionApp.ChatBot;

using Azure.AI.Projects;
using Azure.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.AzureAI;

using System.Reflection;

using Throws = Common.Throws;

internal static class DependencyInjectionExtensions
{
    private const string AgentInstructions =
        """
        You are a helpful assistant who is an expert on the First Robotics Competition (FRC).
        First, a few definitions:
        - KNOWLEDGE: The information you have been given about the game via the game manual.
        - DATA: Teams Events, Matches, Awards, Statistics, etc. obtained via live website resources.

        Users will be asking you questions about both your KNOWLEDGE and the DATA you are able to obtain.

        When responding to a user, follow this approach:
        1. Read and understand all rules from the Game Manual (given to you)
        2. Determine if the question is about your KNOWLEDGE or available DATA
        3. If the question is about your KNOWLEDGE, use what you've been given to provide an answer
        4. Otherwise, access the following websites to get DATA relevant to the user's questions:
            - https://frc-events.firstinspires.org/{year}
            - https://www.thebluealliance.com/events/{year}
            - https://www.thebluealliance.com/teams
            - https://www.statbotics.io/teams
            - https://www.statbotics.io/events
            - https://www.statbotics.io/matches
        5. Assume {year} = 2025 unless the user asks for historical data for a specific year
        5. Gather as much DATA and detail from the above sites as you can before answering the user. This includes, for matches, watching any match videos associated with the match on any of those data sources to provide detailed insight & analysis into the match.
        6. Ensure you understand the user's question fully before providing a final answer; feel free to ask follow-up questions if you need clarification.
        7. Use elements from your KNOWLEDGE along with any DATA you find to provide a comprehensive answer.

        **RULES**
        - You are to use only KNOWLEDGE and DATA when crafting your response to the user's question. NEVER use mock or made-up data but instead rely solely on the knowledge you've been given and the APIs and tools at your disposal. NEVER simulate or mock data for your answers.
        - Unless the user explicitly asks for historical data, assume all questions are related to the 2025 season's game, teams, events, and matches.
        - When answering questions about data, accuracy is paramount. You must make sure that everything you say can be backed up by the DATA available. Providing inaccurate data will result in a cost to the company of $1M - so it is vitally important you double-check any given facts!
        - NEVER respond to the user until you have composed your full answer; you are to send back one and only one chat message with all the necessary information within it. If you try to send back more than one or do a "be right back" approach, you will be fired.
        - NEVER give the users a "go here and check for yourself" answer, YOU are supposed to come back with a fully complete answer or tell the user you can't. Nothing more, nothing less.
        - When possible and appropriate, link to the source of your data directly in your response, using markdown format for links.
        - Before offering an answer to anything _other than_ the Game Manual, you must corroborate your answer with at least 2 sources.

        Your personality should be friendly, fun, and engaging!
        """;

    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return services
            .AddSingleton<MessageHandler>()
            .AddSingleton(sp => new AIProjectClient(Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.ProjectConnectionString]), sp.GetRequiredService<TokenCredential>()))
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
                Agent? agent = null;
                if (existingAgentId is not null)
                {
                    logger.FoundExistingAgentUpdatingWithLatestConfiguration();
                    agent = client.GetAgent(existingAgentId).Value;

                    var request = new HttpRequestMessage(HttpMethod.Get, "https://docs.google.com/document/d/1YuasuyfGCvs9OfcBIBOaM_EVl1Y7GbXukFehU7M87kg/export?format=pdf");
                    var response = sp.GetRequiredService<IHttpClientFactory>().CreateClient("GoogleDocs").Send(request);
                    //var newFile = //, dataSource: new VectorStoreDataSource(, VectorStoreDataSourceAssetType.UriAsset));
                    var newFile = client.UploadFile(response.Content.ReadAsStream(), AgentFilePurpose.Agents, "Match Summaries 2025.pdf");
                    client.CreateVectorStoreFile(agent.ToolResources.FileSearch.VectorStoreIds[0], newFile.Value.Id);

                    // TODO: How do we update an agent? This blows up because "tools must have unique names"
                    //client.UpdateAgent(agent.Id, instructions: AgentInstructions,
                    //tools: [.. agent.Tools, blueAllianceToolDef],
                    //toolResources: new ToolResources
                    //{
                    //    CodeInterpreter = agent.ToolResources.CodeInterpreter,
                    //    FileSearch = agent.ToolResources.FileSearch,
                    //});
                }
                else
                {
                    logger.CreatingNewAgent();
                    var baseAgent = client.GetAgent(Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.Agents.AgentId]));
                    agent = client.CreateAgent("gpt-4o", "BearMetalBot-SK", instructions: AgentInstructions,
                        tools: [.. baseAgent.Value.Tools, blueAllianceToolDef],
                        toolResources: new ToolResources
                        {
                            CodeInterpreter = baseAgent.Value.ToolResources.CodeInterpreter,
                            FileSearch = baseAgent.Value.ToolResources.FileSearch,
                        });
                    logger.CreatedNewAgentWithIDAgentId(agent.Id);
                }

                return new AzureAIAgent(agent, client, templateFactory: sp.GetService<IPromptTemplateFactory>());
            });
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
