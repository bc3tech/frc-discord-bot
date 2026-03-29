using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;

using Common;

using FunctionApp;
using FunctionApp.Apis;
using FunctionApp.ChatBot;
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

using OpenTelemetry.Metrics;

using System.Diagnostics.Metrics;
using System.Globalization;

using Constants = FunctionApp.Constants;
using Throws = Common.Throws;

CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
AppContext.SetSwitch("Azure.Experimental.TraceGenAIMessageContent", true);

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
        })
        .AddDebug();

#if DEBUG
var credential = new AzureCliCredential();
#else
var credential = new DefaultAzureCredential();
#endif

host.Services
    .AddMetrics()
    .AddSingleton(sp => sp.GetRequiredService<IMeterFactory>().Create(Constants.Telemetry.AppMeterName))
    .ConfigureOpenTelemetryMeterProvider(b => b.AddMeter(Constants.Telemetry.AppMeterName))
    .AddOpenTelemetry()
    .WithTracing()
    .UseFunctionsWorkerDefaults()
    .UseAzureMonitorExporter();

host.Services
    .ConfigureDiscord()
    .ConfigureTheBlueAllianceApi()
    .ConfigureStatboticsApi()
    .ConfigureFIRSTApi();

if (!string.IsNullOrWhiteSpace(host.Configuration[Constants.Configuration.Azure.AI.Project.Endpoint]))
{
    host.Services.ConfigureChatBotFunctionality();
}

host.Services
    .AddSingleton(sp => new EmbeddingColorizer(new FRCColors.Client(sp.GetRequiredService<IHttpClientFactory>()), sp.GetService<ILoggerFactory>()?.CreateLogger<EmbeddingColorizer>()))
    .AddSingleton<EventRepository>()
    .AddSingleton<TeamRepository>()
    .AddSingleton<SubscriptionManager>()
    .AddSingleton<TokenCredential>(credential)
    .AddSingleton<RESTCountries>()
    .AddSingleton<TimeProvider, PacificTimeProvider>();
//.FixAppInsightsLogging();

// Prefer explicit service URIs so Container Apps can use managed identity-backed storage.
// The connection-string fallback is retained for local development.
var tableEndpoint = TryGetStorageServiceUri(
    host.Configuration,
    Constants.Configuration.Azure.Storage.TableEndpoint,
    ConfigurationPath.Combine("AzureWebJobsStorage", "tableServiceUri"));
TableServiceClient tsc = tableEndpoint is not null
    ? new TableServiceClient(tableEndpoint, credential)
    : new TableServiceClient(Throws.IfNullOrWhiteSpace(host.Configuration["AzureWebJobsStorage"]));
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
    var blobsEndpoint = TryGetStorageServiceUri(
        host.Configuration,
        Constants.Configuration.Azure.Storage.BlobsEndpoint,
        ConfigurationPath.Combine("AzureWebJobsStorage", "blobServiceUri"));
    BlobServiceClient bsc = blobsEndpoint is not null
        ? new BlobServiceClient(blobsEndpoint, credential)
        : new BlobServiceClient(Throws.IfNullOrWhiteSpace(host.Configuration["AzureWebJobsStorage"]));

    var blobContainer = bsc.GetBlobContainerClient("misc");
    blobContainer.CreateIfNotExists();

    return blobContainer;
});

var builtHost = host.Build();
var startupLogger = builtHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
startupLogger.LogInformation(
    "OpenTelemetry startup configuration: Application Insights connection string is {ConnectionStringState}. OTEL service name is {ServiceName}.",
    string.IsNullOrWhiteSpace(host.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]) ? "missing" : "configured",
    host.Configuration["OTEL_SERVICE_NAME"] ?? "unset");

await builtHost.RunAsync().ConfigureAwait(false);

static Uri? TryGetStorageServiceUri(IConfiguration configuration, string primaryKey, string secondaryKey)
{
    var configuredValue = configuration[primaryKey] ?? configuration[secondaryKey];
    return Uri.TryCreate(configuredValue, UriKind.Absolute, out var serviceUri)
        ? serviceUri
        : null;
}
