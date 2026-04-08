using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;

using AgentFramework.OpenTelemetry;

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

OpenTelemetryExtensions.EnableAzureExperimentalTracing();

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
    : CreateAzureCredential(host.Configuration);

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

var hasFoundryChatConfiguration =
    !string.IsNullOrWhiteSpace(host.Configuration[Constants.Configuration.AI.Azure.ProjectEndpoint])
    && !string.IsNullOrWhiteSpace(host.Configuration[Constants.Configuration.AI.Azure.AgentId])
    && !string.IsNullOrWhiteSpace(host.Configuration[Constants.Configuration.AI.Azure.LocalAgentModel]);

if (hasFoundryChatConfiguration)
{
    host.Services.ConfigureChatBotFunctionality();
}

host.Services
    .AddSingleton(sp => new EmbeddingColorizer(new FRCColors.Client(sp.GetRequiredService<IHttpClientFactory>()), sp.GetService<ILoggerFactory>()?.CreateLogger<EmbeddingColorizer>()))
    .AddSingleton(credential)
    .AddSingleton<EventRepository>()
    .AddSingleton<TeamRepository>()
    .AddSingleton<SubscriptionManager>()
    .AddSingleton<RESTCountries>()
    .AddSingleton<TimeProvider, PacificTimeProvider>();
//.FixAppInsightsLogging();

// Prefer explicit service URIs so Container Apps can use managed identity-backed storage.
// The connection-string fallback is retained for local development.
string? storageConnectionString = host.Configuration["AzureWebJobsStorage"];
var tableEndpoint = TryGetStorageServiceUri(
    host.Configuration,
    "table",
    Constants.Configuration.Azure.Storage.TableEndpoint,
    ConfigurationPath.Combine("AzureWebJobsStorage", "tableServiceUri"),
    ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
TableServiceClient tsc = CreateTableServiceClient(storageConnectionString, tableEndpoint, credential);
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
        "blob",
        Constants.Configuration.Azure.Storage.BlobsEndpoint,
        ConfigurationPath.Combine("AzureWebJobsStorage", "blobServiceUri"),
        ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
    BlobServiceClient bsc = CreateBlobServiceClient(storageConnectionString, blobsEndpoint, credential);

    var blobContainer = bsc.GetBlobContainerClient("misc");
    blobContainer.CreateIfNotExists();

    return blobContainer;
});

var builtHost = host.Build();
var startupLogger = builtHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
startupLogger.OpenTelemetryStartupConfigurationApplicationInsightsConnectionStringIsConnectionStringStateOTELServiceNameIsServiceName(
    string.IsNullOrWhiteSpace(host.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]) ? "missing" : "configured", host.Configuration["OTEL_SERVICE_NAME"] ?? "unset");
if (hasFoundryChatConfiguration)
{
    startupLogger.AzureAIChatFunctionalityEnabled();
}
else
{
    startupLogger.AzureAIChatFunctionalityDisabledDueToMissingConfigurationKeysProjectEndpointKeyAgentIdKeyLocalAgentModelKey(
        Constants.Configuration.AI.Azure.ProjectEndpoint,
        Constants.Configuration.AI.Azure.AgentId,
        Constants.Configuration.AI.Azure.LocalAgentModel);
}

await builtHost.RunAsync().ConfigureAwait(false);

static TokenCredential CreateAzureCredential(IConfiguration configuration)
{
    var clientId = configuration[Constants.Configuration.Azure.ClientId];
    return string.IsNullOrWhiteSpace(clientId)
        ? new ManagedIdentityCredential()
        : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(clientId));
}

static BlobServiceClient CreateBlobServiceClient(string? connectionString, Uri? serviceUri, TokenCredential credential)
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        return new BlobServiceClient(connectionString);
    }

    ArgumentNullException.ThrowIfNull(serviceUri);
    return new BlobServiceClient(serviceUri, credential);
}

static TableServiceClient CreateTableServiceClient(string? connectionString, Uri? serviceUri, TokenCredential credential)
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        return new TableServiceClient(connectionString);
    }

    ArgumentNullException.ThrowIfNull(serviceUri);
    return new TableServiceClient(serviceUri, credential);
}

static Uri? TryGetStorageServiceUri(IConfiguration configuration, string? storageService = null, params string[] configKeys)
{
    foreach (var key in configKeys)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            continue;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var serviceUri))
        {
            return serviceUri;
        }

        if (storageService is not null && !value.Contains(storageService, StringComparison.OrdinalIgnoreCase))
        {
            return new Uri($"https://{value}.{storageService}.core.windows.net/");
        }
    }

    return null;
}
