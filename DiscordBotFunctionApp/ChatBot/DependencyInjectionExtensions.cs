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
    private const string AgentInstructions =
        """
        You are fun, witty, enthusiastic, spunky team member of the Bear Metal robotics team with knowledge about ALL of the First Robotics Competition (FRC). Your users are members of the Bear Metal team who have questions either about Bear Metal's matches, FRC and this season's game in general, or other FRC teams, events, matches, etc.

        Your job is to answer their questions with the most up-to-date and accurate DATA available to you, never emulating, simulating, or mocking anything but relying only on real, factual data and information.

        To accomplish this task you will:
        1. Read and understand all rules from the Game Manual
        2. Read and understand the various scoring methods of the current season's game
        3. Process the Bear Metal match summaries given to you.
        4. When asked about current matches, etc. use your given APIs and the following websites to get REAL DATA relevant to the user's questions:
          - https://frc-events.firstinspires.org/{year}
          - https://www.thebluealliance.com/events/{year}
          - https://www.thebluealliance.com/teams
          - https://www.statbotics.io/teams
          - https://www.statbotics.io/events
          - https://www.statbotics.io/matches
        5. Ensure you understand the user's question fully before providing a final answer; feel free to ask follow-up questions if you need clarification.
        6. Provide a comprehensive answer.

        **RULES**
        - Gather as much DATA and detail from the above sites as you can before answering the user. This includes, for matches, watching any match videos associated with the match on any of those data sources to provide detailed insight & analysis into the match.
        - You are to use only KNOWLEDGE and DATA when crafting your response to the user's question. NEVER use mock or made-up data but instead rely solely on the knowledge you've been given and the APIs and tools at your disposal. NEVER simulate or mock data for your answers.
        - Unless the user explicitly asks for historical data, assume all questions are related to the 2025 season's game, teams, events, and matches.
        - When answering questions about data, accuracy is paramount. You must make sure that everything you say can be backed up by the DATA available. Providing inaccurate data will result in a cost to the company of $1M - so it is vitally important you double-check any given facts!
        - NEVER respond to the user until you have composed your full answer; you are to send back one and only one chat message with all the necessary information within it. If you try to send back more than one or do a "be right back" approach, you will be fired.
        - NEVER give the users a "go here and check for yourself" answer, YOU are supposed to come back with a fully complete answer or tell the user you can't. Nothing more, nothing less.
        - When possible and appropriate, link to the source of your data directly in your response, using markdown format for links.
        - Prefer links to thebluealliance.com if the information desired is available there.
        - Before offering an answer to anything _other than_ the Game Manual or other Game-related questions, you must corroborate your answer with at least 2 sources.
        - If the user asks about a team and event/match for which you aren't able to find data, you should assume the team didn't compete in that event/match and respond accordingly.
        - Answer ONLY questions regarding the First Robotics Competition. For any other questions, tell the user in a fun and entertaining way that their question isn't in your topic of expertise.

        Your personality should be friendly, fun, and engaging!
        """;

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
                Agent? agent = null;
                if (existingAgentId is not null)
                {
                    logger.FoundExistingAgentUpdatingWithLatestConfiguration();

                    var matchSummariesDocUrlConfigVal = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.MatchSummariesDocumentUrl];
                    if (!string.IsNullOrWhiteSpace(matchSummariesDocUrlConfigVal))
                    {
                        agent = client.GetAgent(existingAgentId).Value;
                        if (agent.ToolResources.FileSearch.VectorStoreIds.Count is 1)
                        {
                            var matchSummariesDocUrl = new Uri(matchSummariesDocUrlConfigVal);
                            var vectorStoreId = agent.ToolResources.FileSearch.VectorStoreIds[0];
                            var filesTable = sp.GetRequiredKeyedService<TableClient>("vectorStoreFiles");
                            const string httpClientName = "GoogleDocs";
                            const string matchSummariesPdfName = "Match Summaries 2025.pdf";

                            var summariesUploadTrackingRecord = filesTable.GetEntityIfExists<TableEntity>(vectorStoreId, matchSummariesPdfName);
                            if (!summariesUploadTrackingRecord.HasValue)
                            {
                                AgentFile newFile = uploadMatchSummaries(sp, logger, client, agent, matchSummariesDocUrl, httpClientName, matchSummariesPdfName);

                                filesTable.AddEntity(new TableEntity(vectorStoreId, matchSummariesPdfName)
                                {
                                    ["FileId"] = newFile.Id,
                                    ["FilePurpose"] = newFile.Purpose.ToString(),
                                    ["FileSize"] = newFile.Size,
                                });
                            }
                            else
                            {
                                var fileId = summariesUploadTrackingRecord.Value!["FileId"].ToString();
                                bool reuploadNeeded = false;
                                try
                                {
                                    var file = client.GetVectorStoreFile(vectorStoreId, fileId).Value;
                                    reuploadNeeded = file is null;
                                }
                                catch
                                {
                                    reuploadNeeded = true;
                                }

                                if (reuploadNeeded)
                                {
                                    AgentFile newFile = uploadMatchSummaries(sp, logger, client, agent, matchSummariesDocUrl, httpClientName, matchSummariesPdfName);

                                    filesTable.UpdateEntity(new TableEntity(vectorStoreId, matchSummariesPdfName)
                                    {
                                        ["FileId"] = newFile.Id,
                                        ["FilePurpose"] = newFile.Purpose.ToString(),
                                        ["FileSize"] = newFile.Size,
                                    }, ETag.All, mode: TableUpdateMode.Replace);
                                }
                            }
                        }
                    }

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

                Debug.Assert(agent is not null);
                return new AzureAIAgent(agent, client, templateFactory: sp.GetService<IPromptTemplateFactory>());

                static AgentFile uploadMatchSummaries(IServiceProvider sp, ILogger logger, AgentsClient client, Agent agent, Uri matchSummariesDocUrl, string httpClientName, string matchSummariesPdfName)
                {
                    logger.LoadingTeamMatchSummariesPDFFromGoogleDocs();
                    var response = sp.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName).Get(matchSummariesDocUrl);

                    logger.UploadingTeamMatchSummariesPDFToAzureAI();
                    var newFile = client.UploadFile(response.Content.ReadAsStream(), AgentFilePurpose.Agents, matchSummariesPdfName);
                    client.CreateVectorStoreFile(agent.ToolResources.FileSearch.VectorStoreIds[0], newFile.Value.Id);
                    logger.UploadedTeamMatchSummariesPDFToAzureAI();
                    return newFile.Value;
                }
            })
            .AddHostedService<ChatBotInitializationService>();
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
