namespace ChatBot;

using AgentFramework.OpenTelemetry;

using Azure.AI.Projects;
using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;

using ChatBot.Agents;
using ChatBot.Copilot;
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

using Throws = Common.Throws;

public static class DependencyInjectionExtensions
{
    public static bool HasValidChatBotConfiguration(this IConfiguration configuration, out string[] validationFailures)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        AiOptions options = new()
        {
            FoundryEndpoint = new Uri("https://bootstrap.invalid"),
            AgentId = "bootstrap",
            MealSignupGeniusId = "bootstrap",
            LocalAgentModel = "bootstrap",
            OpenAIApiVersion = "2025-06-01",
            DefaultTeamNumber = 1,
        };

        new ConfigureAiOptions(configuration).Configure(options);

        ValidateOptionsResult validationResult = new ValidateAiOptions().Validate(name: null, options);
        validationFailures = validationResult.Failures?.ToArray() ?? [];
        return validationResult.Succeeded;
    }

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
            .AddSingleton<CopilotFoundryProviderFactory>()
            .AddSingleton<CopilotAgentCatalog>()
            .AddSingleton<CopilotClientFactory>()
            .AddSingleton<FoundrySpecialistTool>()
            .AddSingleton<CopilotSessionCoordinator>()
            .AddSingleton<IProvideFunctionTools, MealSignupInfoTool>()
            .AddSingleton<IProvideFunctionTools, TbaApiTool>()
            .AddSingleton<IProvideFunctionTools, StatboticsTool>()
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

                logger.ConnectingToAzureAIFoundryProjectEndpointEndpoint(options.FoundryEndpoint);
                return new AIProjectClient(endpoint: options.FoundryEndpoint, tokenProvider: credential, options: clientOptions);
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
}
