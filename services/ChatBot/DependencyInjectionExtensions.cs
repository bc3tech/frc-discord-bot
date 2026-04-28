namespace ChatBot;

using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.AI.Extensions.OpenAI;
using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;

using AgentFramework.OpenTelemetry;

using ChatBot.Agents;
using ChatBot.AgentIdentity;
using ChatBot.Configuration;
using ChatBot.Tools;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Trace;

using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.ClientModel;

using Throws = Common.Throws;

public static class DependencyInjectionExtensions
{
    private static readonly FieldInfo CachedOpenAIClientField = typeof(AIProjectClient).GetField("_cachedOpenAIClient", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("AIProjectClient no longer exposes the expected cached OpenAI client field.");

    private static readonly FieldInfo TokenProviderField = typeof(AIProjectClient).GetField("_tokenProvider", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("AIProjectClient no longer exposes the expected token provider field.");

    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
        OpenTelemetryExtensions.EnableAzureExperimentalTracing();

        services
            .AddHttpClient(ChatBotConstants.HttpClients.MealSignupInfo, MealSignupInfoTool.ConfigureHttpClient)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });
        services.AddHttpClient("tba-api", static (sp, client) =>
        {
            client.BaseAddress = new Uri("https://www.thebluealliance.com/api/v3");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-TBA-Auth-Key", Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()["TbaApiKey"], "TBA_AUTH_KEY environment variable is not set."));
        });
        services.AddHttpClient("statbotics-api", static client =>
        {
            client.BaseAddress = new Uri("https://api.statbotics.io");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.ConfigureOptions<ConfigureAiOptions>()
            .ConfigureOptions<ValidateAiOptions>()
            .AddOptionsWithValidateOnStart<AiOptions>();

        services
            .AddSingleton<PromptCatalog>()
            .AddSingleton<IProvideFunctionTools, MealSignupInfoTool>()
            .AddSingleton<IProvideFunctionTools, TbaApiTool>()
            .AddSingleton<IProvideFunctionTools, StatboticsTool>()
            .AddSingleton<AgentIdentityContextProvider>()
            .AddSingleton<UserChatSynchronization>()
            .AddSingleton<MessageHandler>()
            .AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;
                ILogger<AIProjectClient> logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<AIProjectClient>();
                TokenCredential credential = sp.GetRequiredService<TokenCredential>();

                AIProjectClientOptions clientOptions = new()
                {
                    EnableDistributedTracing = true
                };
                clientOptions.AddPolicy(W3CTraceContextClientModelPipelinePolicy.Instance, System.ClientModel.Primitives.PipelinePosition.PerTry);

                logger.ConnectingToAzureAIFoundryProjectEndpointEndpoint(options.ProjectEndpoint);
                AIProjectClient projectClient = new(endpoint: options.ProjectEndpoint!, tokenProvider: credential, options: clientOptions);
                ConfigureProjectOpenAIClient(projectClient, options);
                return projectClient;
            })
            .AddSingleton(sp =>
            {
                PersistentAgentsAdministrationClientOptions options = new()
                {
                    Diagnostics =
                    {
                        IsDistributedTracingEnabled = true,
                    },
                };
                options.AddPolicy(W3CTraceContextAzureHttpPipelinePolicy.Instance, HttpPipelinePosition.PerRetry);

                return new PersistentAgentsClient(
                    Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[ChatBotConstants.Configuration.AI.Azure.ProjectEndpoint]),
                    sp.GetRequiredService<TokenCredential>(),
                    options);
            })
            .AddSingleton<ChatRunner>()
            .AddSingleton<Conversation>();

        Sdk.CreateTracerProviderBuilder()
            .AddAgentFrameworkOpenTelemetry("Discord.ChatBot")
            .AddProcessor<AzureIdentityActivityFilteringProcessor>()
            .AddAzureMonitorTraceExporter()
            .Build();

        return services;
    }

    private static void ConfigureProjectOpenAIClient(AIProjectClient projectClient, AiOptions options)
    {
        ArgumentNullException.ThrowIfNull(projectClient);
        ArgumentNullException.ThrowIfNull(options);

        AuthenticationTokenProvider tokenProvider = TokenProviderField.GetValue(projectClient) as AuthenticationTokenProvider
            ?? throw new InvalidOperationException("The Azure AI project token provider could not be read for OpenAI client configuration.");

        ProjectOpenAIClientOptions openAIClientOptions = new()
        {
            ApiVersion = options.OpenAIApiVersion,
            EnableDistributedTracing = true,
        };
        openAIClientOptions.AddPolicy(W3CTraceContextClientModelPipelinePolicy.Instance, System.ClientModel.Primitives.PipelinePosition.PerTry);

        ProjectOpenAIClient openAIClient = new(
            options.ProjectEndpoint,
            tokenProvider,
            openAIClientOptions);

        CachedOpenAIClientField.SetValue(projectClient, openAIClient);
    }
}
