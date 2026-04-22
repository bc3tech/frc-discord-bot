namespace ChatBot;

using BC3Technologies.DiscordGpt.Core;
using BC3Technologies.DiscordGpt.Copilot;
using BC3Technologies.DiscordGpt.Copilot.Foundry;
using BC3Technologies.DiscordGpt.Hosting;
using BC3Technologies.DiscordGpt.Storage.Blob;
using BC3Technologies.DiscordGpt.Storage.TableStorage;

using ChatBot.Tools;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Chat;

using System.Net;
using System.Net.Http.Headers;

using Throws = Common.Throws;

using Azure.Core;

public static class DependencyInjectionExtensions
{
    private static readonly string[] s_discordTokenKeys = ["Discord:Token"];
    private static readonly string[] s_discordApplicationIdKeys = ["Discord:ApplicationId", "Discord:AppId"];

    public static bool HasValidChatBotConfiguration(this IConfiguration configuration, out string[] validationFailures)
    {
        ArgumentNullException.ThrowIfNull(configuration);

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

    public static IServiceCollection AddFrcChatBot(
        this IServiceCollection services,
        IConfiguration configuration,
        TokenCredential? tokenCredential = null,
        Uri? blobServiceUri = null)
        => AddFrcChatBot(services, configuration, out _, out _, tokenCredential, blobServiceUri);

    public static IServiceCollection AddFrcChatBot(
        this IServiceCollection services,
        IConfiguration configuration,
        out bool isEnabled,
        out string[] validationFailures,
        TokenCredential? tokenCredential = null,
        Uri? blobServiceUri = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        if (!configuration.HasValidChatBotConfiguration(out validationFailures))
        {
            isEnabled = false;
            return services;
        }

        isEnabled = true;
        ArgumentNullException.ThrowIfNull(tokenCredential);
        ArgumentNullException.ThrowIfNull(blobServiceUri);
        if (!Uri.UriSchemeHttps.Equals(blobServiceUri.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Copilot blob storage URI must use HTTPS when credential-based auth is enabled. Received: {blobServiceUri}");
        }

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

        services.Configure<DiscordGptCoreOptions>(options =>
        {
            options.BotToken = GetRequiredConfigurationValue(configuration, s_discordTokenKeys);
            options.ApplicationId = GetOptionalConfigurationValue(configuration, s_discordApplicationIdKeys);
            options.MaxHistoryLength = 50;
        });

        services.AddSingleton<TbaApiTool>();

        services.AddDiscordGpt(options => options.AllowAll = true)
            .UseFoundry(options =>
            {
                options.Endpoint = GetRequiredConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.Endpoint);
                options.DeploymentName = GetRequiredConfigurationValue(configuration, ChatBotConstants.Configuration.Foundry.LocalAgentModel);
            })
            .WithBlobSessionStorage(tokenCredential, blobServiceUri, options =>
            {
                options.ContainerName = ChatBotConstants.ServiceKeys.BlobContainer_CopilotSessions;
            })
            .WithConversationStore<TableConversationStore>()
            .AddTool<MealSignupInfoTool>()
            .AddTool<TbaApiSurfaceTool>()
            .AddTool<TbaApiTool>()
            .AddTool<StatboticsTool>();

        services.AddTableConversationStore(options =>
        {
            options.TableName = ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads;
        });

        services.AddSingleton<IChatClient>(sp =>
        {
            IChatClient innerClient = sp.GetRequiredService<ChatClient>().AsIChatClient();

            string promptPath = Path.Combine(AppContext.BaseDirectory, "ChatBot", "agent_prompt.txt");
            if (!File.Exists(promptPath))
            {
                throw new FileNotFoundException($"Required chatbot prompt file was not found: {promptPath}", promptPath);
            }

            string prompt = File.ReadAllText(promptPath).ReplaceLineEndings("\n").Trim();
            return new FrcSystemPromptChatClient(innerClient, prompt);
        });

        services.Configure<CopilotToolAuthorizationOptions>(options =>
        {
            options.AllowAllTools = true;
            options.AllowAllSkills = true;
            options.AllowToolsInDirectMessages = true;
            options.AllowSkillsInDirectMessages = true;
        });

        services.AddSingleton<MessageHandler>();

        return services;
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
