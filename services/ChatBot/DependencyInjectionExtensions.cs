namespace ChatBot;

using Azure.Core;

using BC3Technologies.DiscordGpt.Copilot;
using BC3Technologies.DiscordGpt.Copilot.Foundry;
using BC3Technologies.DiscordGpt.Core;
using BC3Technologies.DiscordGpt.Hosting;
using BC3Technologies.DiscordGpt.Storage.Blob;

using ChatBot.Diagnostics;
using ChatBot.Tools;

using CopilotSdk.OpenTelemetry;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OpenAI.Chat;

using System.Net;
using System.Net.Http.Headers;

using Throws = Common.Throws;

public static class DependencyInjectionExtensions
{
    private static readonly string[] s_discordTokenKeys = ["Discord:Token"];
    private static readonly string[] s_discordApplicationIdKeys = ["Discord:ApplicationId", "Discord:AppId"];

    public static bool HasValidChatBotConfiguration(this IConfiguration configuration, out string[] validationFailures)
    {

        List<string> failures = [];

        var foundryEndpoint = GetOptionalConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.Endpoint);
        if (!Uri.TryCreate(foundryEndpoint, UriKind.Absolute, out _))
        {
            failures.Add($"Required configuration value '{ChatBotConstants.Configuration.Foundry.Endpoint}' must be an absolute URI.");
        }

        if (string.IsNullOrWhiteSpace(GetOptionalConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.LocalAgentModel)))
        {
            failures.Add($"Required configuration value '{ChatBotConstants.Configuration.Foundry.LocalAgentModel}' is missing or empty.");
        }

        if (string.IsNullOrWhiteSpace(GetOptionalConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.AgentId)))
        {
            failures.Add($"Required configuration value '{ChatBotConstants.Configuration.Foundry.AgentId}' is missing or empty.");
        }

        if (string.IsNullOrWhiteSpace(GetOptionalConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.MealSignupGeniusId)))
        {
            failures.Add($"Required configuration value '{ChatBotConstants.Configuration.Foundry.MealSignupGeniusId}' is missing or empty.");
        }

        if (string.IsNullOrWhiteSpace(GetOptionalConfigurationValue(configuration, s_discordTokenKeys)))
        {
            failures.Add("Required configuration value 'Discord:Token' is missing or empty.");
        }

        validationFailures = [.. failures];
        return validationFailures.Length is 0;
    }

    public static IServiceCollection TryAddChatBot(
        this IServiceCollection services,
        IConfiguration configuration,
        Uri blobServiceUri,
        out bool success,
        out string[] validationFailures)
    {
        if (!configuration.HasValidChatBotConfiguration(out validationFailures))
        {
            success = false;
            return services;
        }

        if (!Uri.UriSchemeHttps.Equals(blobServiceUri.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            success = false;
            validationFailures = [$"Copilot blob storage URI must use HTTPS when credential-based auth is enabled. Received: {blobServiceUri}"];
            return services;
        }

        services
            .AddHttpClient(ChatBotConstants.HttpClients.MealSignupInfo, MealSignupInfoTool.ConfigureHttpClient)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });
        services.AddHttpClient(ChatBotConstants.HttpClients.TbaApi, static (sp, client) =>
        {
            client.BaseAddress = new Uri("https://www.thebluealliance.com/api/v3/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-TBA-Auth-Key", Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()["TbaApiKey"], "TBA_AUTH_KEY environment variable is not set."));
        });
        services.AddHttpClient(ChatBotConstants.HttpClients.StatboticsApi, static client =>
        {
            client.BaseAddress = new Uri("https://api.statbotics.io/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.Configure<DiscordGptCoreOptions>(options =>
        {
            options.BotToken = GetRequiredConfigurationValue(configuration, s_discordTokenKeys);
            options.ApplicationId = GetOptionalConfigurationValue(configuration, s_discordApplicationIdKeys);
            options.MaxHistoryLength = 50;
        });

        // Bind DiscordGpt section so consumers can set GitHubToken, Telemetry, and CliLogLevel
        // via any IConfiguration source (appsettings, environment, Key Vault, container-app secrets).
        services.Configure<DiscordGptOptions>(configuration.GetSection("DiscordGpt"));

        // Bind agent-facing logging/progress settings from AI:AgentLogging.
        services.Configure<AgentLoggingOptions>(configuration.GetSection("AI:AgentLogging"));

        services.TryAddSingleton<TbaApiTool>();
        services.TryAddSingleton<StatboticsTool>();
        services
            .AddDiscordGpt()
            .WithFoundryModels(options =>
            {
                options.Endpoint = GetRequiredConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.Endpoint);
                options.DeploymentName = GetRequiredConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.LocalAgentModel);
            })
            .WithTableConversationStore(options => options.TableName = ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads)
            .UseCopilot(c => c
                // Intentionally NOT calling .WithBlobSessionStorage here. The bot
                // runs as a single-instance Container App (max replicas: 1), so
                // cross-host session.resume is impossible regardless of where
                // session.db lives. Keeping session-state on the SDK's default
                // local disk also keeps tool-output spills on real disk where
                // shelled-out CLI tools (rg, cat) can read them — routing those
                // through a virtual ISessionFsHandler breaks that handoff.
                // The WithBlobSessionStorage extension remains available in the
                // BC3Technologies.DiscordGpt.Storage.Blob package for SDK
                // consumers who do need cross-host resume.
                .WithTableConversationSessionMap(options => options.TableName = ChatBotConstants.ServiceKeys.TableClient_CopilotSessions)
                .ConfigureOptions(options =>
                {
                    options.AllowAll = true;

                    var timeoutSeconds= configuration.GetValue<int?>(ChatBotConstants.Configuration.Copilot.ResponseTimeoutSeconds);
                    options.ResponseTimeout = TimeSpan.FromSeconds(timeoutSeconds ?? 300);

                    // Replace the Copilot CLI's default coding-agent persona with the Bear Metal Bot
                    // Discord persona. Without this, the orchestrator has no team context (doesn't know
                    // "we" = Team 2046, what "dcmp" means, that it should call FRC tools) and ends up
                    // asking clarifying questions instead of looking up data.
                    options.SystemMessage = LoadPromptFile("agent_prompt.txt");
                    options.SystemMessageMode = SystemMessageMode.Replace;
                })
                .WithAzureFoundryAgent(GetRequiredConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.AgentId))
                .WithSessionConfigSource<IsolatedSessionConfigSource>()
                .WithSessionConfigSource<Configuration.ModelSessionConfigSource>()
                .WithSessionDiagnosticsLogging()
                .WithLocalAgent(cfg =>
                {
                    cfg.Name = "frc-data";
                    cfg.DisplayName = "FRC live data lookup";
                    cfg.Description =
                        "Live FIRST Robotics Competition data lookups for Team 2046 (Bear Metal) and any other team: " +
                        "team rosters/metadata, events, schedules, matches, alliance partners, rankings, awards, " +
                        "district keys, season aggregations, EPA/Elo/predictions, and Bear Metal meal-signup state. " +
                        "Use for any question that needs grounded TBA, Statbotics, or meal-signup data.";
                    cfg.Prompt = LoadPromptFile("local_agent_prompt.txt");
                    cfg.Tools = [
                        TbaApiTool.DescribeSurfaceToolName,
                        TbaApiTool.QueryToolName,
                        TbaApiTool.LastCompetitionToolName,
                        StatboticsTool.DescribeSurfaceToolName,
                        StatboticsTool.QueryToolName,
                        "fetch_meal_signup_info",
                    ];
                    cfg.Infer = true;
                })
            )
            .AddTool<MealSignupInfoTool>()
            .AddTool<TbaApiTool>()
            .AddTool<TbaApiSurfaceTool>()
            .AddTool<StatboticsTool>()
            .AddTool<StatboticsApiSurfaceTool>();

        // Conversation telemetry: persist root span context across Function invocations so all
        // turns of a Discord conversation roll up into a single Application Insights Trace.
        services.AddCopilotSdkOpenTelemetry();
        services.Configure<CopilotSdkOpenTelemetryOptions>(configuration.GetSection(ChatBotConstants.Configuration.Copilot.Telemetry.Name));
        services.Configure<TableConversationTraceContextStoreOptions>(options => options.TableName = ChatBotConstants.ServiceKeys.TableClient_ConversationTraces);
        services.Replace(ServiceDescriptor.Singleton<IConversationTraceContextStore, TableConversationTraceContextStore>());

        services.AddSingleton<IChatClient>(sp =>
        {
            IChatClient innerClient = sp.GetRequiredService<ChatClient>().AsIChatClient();
            return new FrcSystemPromptChatClient(innerClient, LoadPromptFile("agent_prompt.txt"));
        });

        services.Configure<CopilotToolAuthorizationOptions>(options =>
        {
            options.AllowAllTools = true;
            options.AllowToolsInDirectMessages = true;
        });

        services.AddSingleton<MessageHandler>();

        success = true;
        return services;
    }

    private static string LoadPromptFile(string fileName)
    {
        string promptPath = Path.Combine(AppContext.BaseDirectory, "ChatBot", fileName);
        if (!File.Exists(promptPath))
        {
            throw new FileNotFoundException($"Required chatbot prompt file was not found: {promptPath}", promptPath);
        }

        return File.ReadAllText(promptPath).ReplaceLineEndings("\n").Trim();
    }

    private static string GetRequiredConfigurationValue(IConfiguration configuration, params string[] keys)
    {
        string? value = GetOptionalConfigurationValue(configuration, keys);
        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Required configuration value '{string.Join("' or '", keys)}' is missing or empty.");
    }

    private static string? GetOptionalConfigurationValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            string? value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return null;
    }
}
