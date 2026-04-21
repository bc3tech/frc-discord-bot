using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;

using BC3Technologies.DiscordGpt.Hosting;

using ChatBot;

using Common;

using FunctionApp;
using FunctionApp.Apis;
using FunctionApp.DiscordInterop;
using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.FIRSTInterop;
using FunctionApp.StatboticsInterop;
using FunctionApp.Storage;
using FunctionApp.Subscription;
using FunctionApp.TbaInterop;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using System.Diagnostics.Metrics;
using System.Globalization;

using Constants = FunctionApp.Constants;

CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

var host = FunctionsApplication.CreateBuilder(args)
    .ConfigureFunctionsWebApplication();

host.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>();

host.Logging
    .AddConfiguration(host.Configuration.GetSection("Logging"))
    .AddOpenTelemetry(o =>
    {
        o.IncludeFormattedMessage = true;
        o.IncludeScopes = true;

        o.AddConsoleExporter(c => c.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Debug);
    })
    .AddDebug();

TokenCredential credential = host.Environment.IsDevelopment()
    ? new AzureCliCredential()
    : StartupInfrastructureFactory.CreateAzureCredential(host.Configuration);

host.Services
    .AddSingleton(sp => sp.GetRequiredService<IMeterFactory>().Create(Constants.Telemetry.AppMeterName))
    .AddOpenTelemetry()
    .WithTracing(b => b.AddProcessor<AzureIdentityActivityFilteringProcessor>())
    .WithMetrics(b => b.AddMeter(Constants.Telemetry.AppMeterName))
    .UseFunctionsWorkerDefaults()
    .UseAzureMonitorExporter();

host.Services
    .ConfigureDiscord()
    .ConfigureTheBlueAllianceApi()
    .ConfigureStatboticsApi()
    .ConfigureFIRSTApi();

host.Services
    .AddSingleton(sp => new EmbeddingColorizer(new FRCColors.Client(sp.GetRequiredService<IHttpClientFactory>()), sp.GetService<ILoggerFactory>()?.CreateLogger<EmbeddingColorizer>()))
    .AddSingleton(credential)
    .AddSingleton<EventRepository>()
    .AddSingleton<TeamRepository>()
    .AddSingleton<SubscriptionManager>()
    .AddSingleton<RESTCountries>()
    .AddSingleton<TimeProvider, PacificTimeProvider>();
//.FixAppInsightsLogging();

host.Services.AddFrcChatBot(
    host.Configuration,
    out bool hasValidChatBotConfiguration,
    out string[] chatBotConfigurationFailures);

// Prefer explicit service URIs so Container Apps can use managed identity-backed storage.
// The connection-string fallback is retained for local development.
string? storageConnectionString = host.Configuration["AzureWebJobsStorage"];
var tableEndpoint = StartupInfrastructureFactory.TryGetStorageServiceUri(
    host.Configuration,
    "table",
    Constants.Configuration.Azure.Storage.TableEndpoint,
    ConfigurationPath.Combine("AzureWebJobsStorage", "tableServiceUri"),
    ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
TableServiceClient tsc = StartupInfrastructureFactory.CreateTableServiceClient(storageConnectionString, tableEndpoint, credential);
host.Services.AddSingleton(tsc);
var storageTables = host.Configuration
    .GetSection(Constants.Configuration.Azure.Storage.Tables)
    .Get<IEnumerable<string>>() ?? [];

foreach (var tableName in storageTables)
{
    host.Services.AddKeyedSingleton(tableName, (sp, _) =>
    {
        var logger = sp.GetService<ILogger<TableClient>>();
        logger?.CreatingTableClientForTable(tableName);
        var tableClient = tsc.GetTableClient(tableName);
        logger?.EnsuringTableTableExists(tableName);
        tableClient.CreateIfNotExists();
        logger?.TableTableExists(tableName);
        return tableClient;
    });
}

host.Services.AddSingleton(sp =>
{
    var blobsEndpoint = StartupInfrastructureFactory.TryGetStorageServiceUri(
        host.Configuration,
        "blob",
        Constants.Configuration.Azure.Storage.BlobsEndpoint,
        ConfigurationPath.Combine("AzureWebJobsStorage", "blobServiceUri"),
        ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
    BlobServiceClient bsc = StartupInfrastructureFactory.CreateBlobServiceClient(storageConnectionString, blobsEndpoint, credential);

    var blobContainer = bsc.GetBlobContainerClient("misc");
    blobContainer.CreateIfNotExists();

    return blobContainer;
});

var builtHost = host.Build();
var startupLogger = builtHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
startupLogger.OpenTelemetryStartupConfigurationApplicationInsightsConnectionStringIsConnectionStringStateOTELServiceNameIsServiceName(
    string.IsNullOrWhiteSpace(host.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]) ? "missing" : "configured", host.Configuration["OTEL_SERVICE_NAME"] ?? "unset");
if (hasValidChatBotConfiguration)
{
    startupLogger.AzureAIChatFunctionalityEnabled();
}
else
{
    startupLogger.AzureAIChatFunctionalityDisabledDueToOptionsValidationFailures(string.Join("; ", chatBotConfigurationFailures));
}

await builtHost.RunAsync().ConfigureAwait(false);
