namespace FunctionApp.Tests;

using global::ChatBot;
using global::ChatBot.Agents;
using global::ChatBot.Configuration;
using global::ChatBot.Copilot;
using global::ChatBot.Tools;

using Azure.AI.Projects;
using Azure.Core;
using Azure.Identity;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Agents.AI;

using System.Collections.Concurrent;
using System.Text;

public sealed class CopilotLiveIntegrationTests
{
    private const string ReproPrompt = "who's got meal duty this week";

    [Fact]
    public async Task LiveCopilotSessionAnswersMealDutyPrompt()
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(5));

        IConfiguration configuration = BuildLiveConfiguration();
        Console.WriteLine($"Resolved app directory: {GetAppDirectory()}");
        Console.WriteLine($"appsettings.json exists: {File.Exists(Path.Combine(GetAppDirectory(), "appsettings.json"))}");
        Console.WriteLine($"appsettings.development.json exists: {File.Exists(Path.Combine(GetAppDirectory(), "appsettings.development.json"))}");
        Console.WriteLine($"local.settings.json exists: {File.Exists(Path.Combine(GetAppDirectory(), "local.settings.json"))}");
        foreach (string key in RequiredConfigurationKeys)
        {
            Console.WriteLine($"{key}: {(string.IsNullOrWhiteSpace(configuration[key]) ? "missing" : "present")}");
        }

        Assert.True(
            configuration.HasValidChatBotConfiguration(out string[] validationFailures),
            $"Missing live chatbot configuration:{Environment.NewLine}{string.Join(Environment.NewLine, validationFailures)}");

        using var logCollector = new TestLogCollector();
        await using ServiceProvider serviceProvider = BuildServiceProvider(configuration, logCollector);

        CopilotClientFactory clientFactory = serviceProvider.GetRequiredService<CopilotClientFactory>();
        CopilotAgentCatalog agentCatalog = serviceProvider.GetRequiredService<CopilotAgentCatalog>();
        FoundrySpecialistTool foundrySpecialistTool = serviceProvider.GetRequiredService<FoundrySpecialistTool>();
        IReadOnlyList<AIFunction> localToolFunctions = serviceProvider
            .GetServices<IProvideFunctionTools>()
            .CombineFunctions(FunctionToolScope.LocalFrcData);

        CopilotChatState chatState = new();
        ValueTask PersistStateAsync(CopilotChatState updatedState, CancellationToken cancellationToken)
        {
            chatState = updatedState;
            return ValueTask.CompletedTask;
        }

        AIFunction foundryTool = foundrySpecialistTool.CreateFunction(() => chatState, PersistStateAsync);
        List<AIFunction> sessionTools = [.. localToolFunctions, foundryTool];

        ConcurrentQueue<SessionEvent> events = [];
        CopilotClient client = await clientFactory.GetStartedClientAsync(timeout.Token);
        await using CopilotSession session = await client
            .CreateSessionAsync(
                agentCatalog.CreateSessionConfig(
                    sessionTools,
                    onEvent: @event =>
                    {
                        if (@event is not null)
                        {
                            events.Enqueue(@event);
                        }
                    },
                    cancellationToken: timeout.Token),
                timeout.Token);

        List<AgentResponseUpdate> updates = [];
        Exception? streamFailure = null;
        try
        {
            await foreach (AgentResponseUpdate update in CopilotEventStreamAdapter.StreamTurnAsync(session, ReproPrompt, timeout.Token))
            {
                updates.Add(update);
            }
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            streamFailure = e;
        }

        string finalAssistantMessage = ExtractFinalAssistantMessage(updates);
        Console.WriteLine(BuildDiagnosticSummary(streamFailure, finalAssistantMessage, updates, events, logCollector.Messages));

        Assert.False(string.IsNullOrWhiteSpace(finalAssistantMessage), BuildFailureMessage(streamFailure, updates, events, logCollector.Messages));
    }

    private static ServiceProvider BuildServiceProvider(IConfiguration configuration, TestLogCollector logCollector)
    {
        ServiceCollection services = new();
        services.AddSingleton(configuration);
        services.AddSingleton<TokenCredential>(new AzureCliCredential());
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(logCollector);
        });

        services.ConfigureOptions<ConfigureAiOptions>()
            .ConfigureOptions<ValidateAiOptions>()
            .AddOptionsWithValidateOnStart<AiOptions>();

        services.AddHttpClient(ChatBotConstants.HttpClients.MealSignupInfo, MealSignupInfoTool.ConfigureHttpClient)
            .ConfigurePrimaryHttpMessageHandler(static () => new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All,
            });
        services.AddHttpClient("tba-api", static (sp, client) =>
        {
            client.BaseAddress = new Uri("https://www.thebluealliance.com/api/v3");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-TBA-Auth-Key", Common.Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()["TbaApiKey"]));
        });
        services.AddHttpClient("statbotics-api", static client =>
        {
            client.BaseAddress = new Uri("https://api.statbotics.io");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        services
            .AddSingleton<PromptCatalog>()
            .AddSingleton(sp =>
            {
                AiOptions options = sp.GetRequiredService<IOptions<AiOptions>>().Value;
                TokenCredential credential = sp.GetRequiredService<TokenCredential>();
                return new AIProjectClient(endpoint: options.FoundryEndpoint, tokenProvider: credential);
            })
            .AddSingleton<CopilotFoundryProviderFactory>()
            .AddSingleton<CopilotAgentCatalog>()
            .AddSingleton<CopilotClientFactory>()
            .AddSingleton<CopilotSessionCoordinator>()
            .AddSingleton<Conversation>()
            .AddSingleton<FoundrySpecialistTool>()
            .AddSingleton<IProvideFunctionTools, MealSignupInfoTool>()
            .AddSingleton<IProvideFunctionTools, TbaApiTool>()
            .AddSingleton<IProvideFunctionTools, StatboticsTool>();

        return services.BuildServiceProvider(validateScopes: true);
    }

    [Fact]
    public async Task LiveConversationResumeFlowAnswersMealDutyPromptTwice()
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(5));

        IConfiguration configuration = BuildLiveConfiguration();
        Assert.True(
            configuration.HasValidChatBotConfiguration(out string[] validationFailures),
            $"Missing live chatbot configuration:{Environment.NewLine}{string.Join(Environment.NewLine, validationFailures)}");

        using var logCollector = new TestLogCollector();
        await using ServiceProvider serviceProvider = BuildServiceProvider(configuration, logCollector);
        Conversation conversation = serviceProvider.GetRequiredService<Conversation>();

        CopilotChatState chatState = new();
        ValueTask PersistStateAsync(CopilotChatState updatedState, CancellationToken cancellationToken)
        {
            chatState = updatedState;
            return ValueTask.CompletedTask;
        }

        string firstAssistantMessage = await RunConversationTurnAsync(conversation, chatState, PersistStateAsync, timeout.Token);
        Assert.False(string.IsNullOrWhiteSpace(chatState.CopilotSessionId));
        string resumedSessionId = chatState.CopilotSessionId!;
        string secondAssistantMessage = await RunConversationTurnAsync(conversation, chatState, PersistStateAsync, timeout.Token);

        Assert.False(string.IsNullOrWhiteSpace(firstAssistantMessage));
        Assert.False(string.IsNullOrWhiteSpace(secondAssistantMessage));
        Assert.Equal(resumedSessionId, chatState.CopilotSessionId);
    }

    private static IConfiguration BuildLiveConfiguration()
    {
        string appDirectory = GetAppDirectory();
        IConfigurationRoot rawConfiguration = new ConfigurationBuilder()
            .SetBasePath(appDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: false)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets(typeof(StartupInfrastructureFactory).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

        Dictionary<string, string?> flattenedFunctionValues = rawConfiguration
            .GetSection("Values")
            .AsEnumerable()
            .Where(static pair => pair.Value is not null && pair.Key.Length > "Values:".Length)
            .ToDictionary(
                static pair => pair.Key["Values:".Length..],
                static pair => pair.Value,
                StringComparer.OrdinalIgnoreCase);

        return new ConfigurationBuilder()
            .AddConfiguration(rawConfiguration)
            .AddInMemoryCollection(flattenedFunctionValues)
            .Build();
    }

    private static string GetAppDirectory()
    {
        string? overridePath = Environment.GetEnvironmentVariable("FRC_DISCORD_BOT_APP_DIR");
        return string.IsNullOrWhiteSpace(overridePath)
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."))
            : overridePath;
    }

    private static readonly string[] RequiredConfigurationKeys =
    [
        ChatBotConstants.Configuration.DefaultTeamNumber,
        ChatBotConstants.Configuration.Copilot.Model,
        ChatBotConstants.Configuration.Foundry.Endpoint,
        ChatBotConstants.Configuration.Foundry.AgentId,
        ChatBotConstants.Configuration.Foundry.MealSignupGeniusId,
        ChatBotConstants.Configuration.Foundry.LocalAgentModel,
        ChatBotConstants.Configuration.Foundry.OpenAIApiVersion,
    ];

    private static string ExtractFinalAssistantMessage(IEnumerable<AgentResponseUpdate> updates)
        => updates
            .Where(update => update.Role == ChatRole.Assistant)
            .SelectMany(static update => update.Contents)
            .OfType<TextContent>()
            .Select(static content => content.Text.Trim())
            .LastOrDefault(static text => !string.IsNullOrWhiteSpace(text))
            ?? string.Empty;

    private static string BuildFailureMessage(
        Exception? streamFailure,
        IReadOnlyList<AgentResponseUpdate> updates,
        IEnumerable<SessionEvent> events,
        IReadOnlyCollection<string> logs)
    {
        StringBuilder message = new();
        message.AppendLine("Live Copilot session did not return a final assistant message.");
        if (streamFailure is not null)
        {
            message.AppendLine(streamFailure.ToString());
        }

        AppendUpdates(message, updates);
        AppendEvents(message, events);
        AppendLogs(message, logs);
        return message.ToString();
    }

    private static string BuildDiagnosticSummary(
        Exception? streamFailure,
        string finalAssistantMessage,
        IReadOnlyList<AgentResponseUpdate> updates,
        IEnumerable<SessionEvent> events,
        IReadOnlyCollection<string> logs)
    {
        StringBuilder message = new();
        if (streamFailure is not null)
        {
            message.AppendLine($"Stream failure: {streamFailure}");
        }

        if (!string.IsNullOrWhiteSpace(finalAssistantMessage))
        {
            message.AppendLine($"Final assistant message: {finalAssistantMessage}");
        }

        AppendUpdates(message, updates);
        AppendEvents(message, events);
        AppendLogs(message, logs);
        return message.ToString();
    }

    private static void AppendUpdates(StringBuilder message, IReadOnlyList<AgentResponseUpdate> updates)
    {
        message.AppendLine("Updates:");
        foreach (AgentResponseUpdate update in updates)
        {
            string text = string.Join(
                " | ",
                update.Contents.OfType<TextContent>().Select(static content => content.Text.ReplaceLineEndings(" ").Trim()));
            message.AppendLine($"- Finish={update.FinishReason}; Role={update.Role}; Text={text}");
        }
    }

    private static void AppendEvents(StringBuilder message, IEnumerable<SessionEvent> events)
    {
        message.AppendLine("Events:");
        foreach (SessionEvent @event in events)
        {
            switch (@event)
            {
                case SessionCustomAgentsUpdatedEvent customAgentsUpdatedEvent:
                    message.AppendLine($"- {@event.Type}; warnings={JoinValues(customAgentsUpdatedEvent.Data.Warnings)}; errors={JoinValues(customAgentsUpdatedEvent.Data.Errors)}");
                    break;
                case SessionErrorEvent sessionErrorEvent:
                    message.AppendLine($"- {@event.Type}; errorType={sessionErrorEvent.Data.ErrorType}; message={sessionErrorEvent.Data.Message}");
                    break;
                case ToolExecutionStartEvent toolExecutionStartEvent:
                    message.AppendLine($"- {@event.Type}; tool={toolExecutionStartEvent.Data.ToolName}");
                    break;
                case ToolExecutionProgressEvent toolExecutionProgressEvent:
                    message.AppendLine($"- {@event.Type}; progress={toolExecutionProgressEvent.Data.ProgressMessage}");
                    break;
                case AssistantMessageDeltaEvent assistantMessageDeltaEvent:
                    message.AppendLine($"- {@event.Type}; delta={assistantMessageDeltaEvent.Data.DeltaContent}");
                    break;
                case AssistantMessageEvent assistantMessageEvent:
                    message.AppendLine($"- {@event.Type}; content={assistantMessageEvent.Data.Content}");
                    break;
                default:
                    message.AppendLine($"- {@event.Type}");
                    break;
            }
        }
    }

    private static void AppendLogs(StringBuilder message, IReadOnlyCollection<string> logs)
    {
        message.AppendLine("Logs:");
        foreach (string log in logs)
        {
            message.AppendLine($"- {log}");
        }
    }

    private static string JoinValues(IReadOnlyList<string>? values)
        => values is { Count: > 0 } ? string.Join(" | ", values) : "(none)";

    private static async Task<string> RunConversationTurnAsync(
        Conversation conversation,
        CopilotChatState chatState,
        Func<CopilotChatState, CancellationToken, ValueTask> persistConversationState,
        CancellationToken cancellationToken)
    {
        List<AgentResponseUpdate> updates = [];
        await foreach (AgentResponseUpdate update in conversation.PostUserMessageStreamingAsync(
            chatState,
            ReproPrompt,
            persistConversationState,
            cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            updates.Add(update);
        }

        return ExtractFinalAssistantMessage(updates);
    }

    private sealed class TestLogCollector : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _messages = [];

        public IReadOnlyCollection<string> Messages => [.. _messages];

        public ILogger CreateLogger(string categoryName) => new TestLogger(categoryName, _messages);

        public void Dispose()
        {
        }

        private sealed class TestLogger(string categoryName, ConcurrentQueue<string> messages) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
                where TState : notnull
                => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                string rendered = formatter(state, exception);
                if (exception is not null)
                {
                    rendered = $"{rendered} :: {exception.Message}";
                }

                messages.Enqueue($"{logLevel} {categoryName} [{eventId.Id}] {rendered}");
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}
