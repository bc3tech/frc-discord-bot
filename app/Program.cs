namespace DiscordBotFunctionApp;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Storage.Blobs;

using Common;

using DiscordBotFunctionApp.Apis;
using DiscordBotFunctionApp.ChatBot;
using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.DiscordInterop.Embeds;
using DiscordBotFunctionApp.Extensions;
using DiscordBotFunctionApp.FIRSTInterop;
using DiscordBotFunctionApp.StatboticsInterop;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Subscription;
using DiscordBotFunctionApp.TbaInterop;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using OpenTelemetry;
using OpenTelemetry.Metrics;

using System;
using System.Diagnostics.Metrics;
using System.Globalization;

using Throws = Common.Throws;

internal sealed class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Not applicable to main()")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for main()")]
    private static async Task Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
        AppContext.SetSwitch("Azure.Experimental.TraceGenAIMessageContent", true);

        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration(b =>
            {
                b.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets<Program>();
            })
            .ConfigureServices((context, services) =>
            {
                var credential = new DefaultAzureCredential(
#if DEBUG
                    includeInteractiveCredentials: true
#endif
                    );

                services
                    .AddMetrics()
                    .AddSingleton(sp => sp.GetRequiredService<IMeterFactory>().Create(Constants.Telemetry.AppMeterName))
                    .ConfigureOpenTelemetryMeterProvider(b => b.AddMeter(Constants.Telemetry.AppMeterName))
                    .AddOpenTelemetry()
                    .WithTracing()
                    .UseAzureMonitor()
                    .UseFunctionsWorkerDefaults();

                services
                    .AddApplicationInsightsTelemetryWorkerService()
                    .ConfigureFunctionsApplicationInsights()
                    .ConfigureDiscord()
                    .ConfigureTheBlueAllianceApi()
                    .ConfigureStatboticsApi()
                    .ConfigureFIRSTApi()
                    .ConfigureChatBotFunctionality()
                    .AddSingleton(sp => new EmbeddingColorizer(new FRCColors.Client(sp.GetRequiredService<IHttpClientFactory>()), sp.GetService<ILoggerFactory>()?.CreateLogger<EmbeddingColorizer>()))
                    .AddSingleton<EventRepository>()
                    .AddSingleton<TeamRepository>()
                    .AddSingleton<SubscriptionManager>()
                    .AddSingleton<TokenCredential>(credential)
                    .AddSingleton<RESTCountries>()
                    .FixAppInsightsLogging();

                var tableStorageEndpointConfigValue = context.Configuration[Constants.Configuration.Azure.Storage.TableEndpoint];
                TableServiceClient tsc = !string.IsNullOrWhiteSpace(tableStorageEndpointConfigValue) && Uri.TryCreate(tableStorageEndpointConfigValue, UriKind.Absolute, out var tableEndpoint)
                    ? new TableServiceClient(tableEndpoint, credential)
                    : new TableServiceClient(Throws.IfNullOrWhiteSpace(context.Configuration["AzureWebJobsStorage"]));
                foreach (var i in context.Configuration.GetSection(Constants.Configuration.Azure.Storage.Tables).Get<IEnumerable<string>>() ?? [])
                {
                    services.AddKeyedSingleton(i, (sp, _) =>
                    {
                        var logger = sp.GetService<ILogger<TableClient>>();
                        logger?.CreatingTableClientForTable(i);
                        var c = tsc.GetTableClient(i);
                        logger?.EnsuringTableTableExists(i);
                        c.CreateIfNotExists();
                        logger?.TableTableExists(i);
                        return c;
                    });
                }

                services.AddSingleton(sp =>
                {
                    var blobStorageEndpointConfigValue = context.Configuration[Constants.Configuration.Azure.Storage.BlobsEndpoint];
                    BlobServiceClient bsc = !string.IsNullOrWhiteSpace(blobStorageEndpointConfigValue) && Uri.TryCreate(blobStorageEndpointConfigValue, UriKind.Absolute, out var blobsEndpoint)
                        ? new BlobServiceClient(blobsEndpoint, credential)
                        : new BlobServiceClient(Throws.IfNullOrWhiteSpace(context.Configuration["AzureWebJobsStorage"]));

                    var blobContainer = bsc.GetBlobContainerClient("misc");
                    blobContainer.CreateIfNotExists();

                    return blobContainer;
                });

                services.AddSingleton<TimeProvider, PacificTimeProvider>();
            })
            .ConfigureLogging((context, builder) => builder
                .AddConfiguration(context.Configuration.GetSection("Logging"))
                .AddOpenTelemetry(o =>
                {
                    o.IncludeFormattedMessage = true;
                    o.IncludeScopes = true;
                })
                .AddApplicationInsights()
                .AddDebug())
            .Build();

        await host.RunAsync().ConfigureAwait(false);
    }
}