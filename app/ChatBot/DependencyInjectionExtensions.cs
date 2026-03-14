namespace FunctionApp.ChatBot;

using Azure.AI.Agents.Persistent;
using Azure.Core;
using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Throws = Common.Throws;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
        AppContext.SetSwitch("Azure.Experimental.TraceGenAIMessageContent", true);

        services
            .AddSingleton<MessageHandler>()
            .AddSingleton(sp => new PersistentAgentsClient(
                Throws.IfNullOrWhiteSpace(sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Azure.AI.Project.Endpoint]),
                sp.GetRequiredService<TokenCredential>(),
                new PersistentAgentsAdministrationClientOptions
                {
                    Diagnostics =
                    {
                        IsDistributedTracingEnabled = true,
                    },
                }))
            .AddSingleton<ChatBotAgentResolver>()
            .AddSingleton<ChatRunner>()
            .AddHostedService<ChatBotInitializationService>();

        Sdk.CreateTracerProviderBuilder()
            .AddSource("Azure.AI.Agents.Persistent.*")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Discord.ChatBot"))
            .AddAzureMonitorTraceExporter()
            .Build();

        return services;
    }
}
