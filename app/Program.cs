using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;

using ChatBot;

using Common;

using Discord.Net;

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
using OpenTelemetry.Trace;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;

using Constants = FunctionApp.Constants;

CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

FunctionsApplicationBuilder host = FunctionsApplication.CreateBuilder(args)
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

        //o.AddConsoleExporter(c => c.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Debug);
    })
    .AddDebug();

TokenCredential credential = host.Environment.IsDevelopment()
    ? new AzureCliCredential()
    : StartupInfrastructureFactory.CreateAzureCredential(host.Configuration);

host.Services
    .AddSingleton(sp => sp.GetRequiredService<IMeterFactory>().Create(Constants.Telemetry.AppMeterName))
    .AddOpenTelemetry()
    .WithTracing(b => b
        .AddProcessor<AzureIdentityActivityFilteringProcessor>()
        .AddSource(CopilotSdk.OpenTelemetry.CopilotSdkOpenTelemetry.ActivitySourceName)
        .AddHttpClientInstrumentation(o =>
        {
            o.EnrichWithHttpRequestMessage = (activity, request) =>
            {
                if (host.Environment.IsDevelopment() && !IsGrpcRequest(request))
                {
                    activity.SetTag("http.request_headers", request.Headers.ToString());
                    if (request.Options.TryGetValue(DevHttpBodyLoggingHandler.RequestBodyKey, out string? reqBody))
                    {
                        activity.SetTag("http.request_body", reqBody);
                    }
                }
            };

            o.EnrichWithHttpResponseMessage = (activity, response) =>
            {
                if (host.Environment.IsDevelopment() && !IsGrpcResponse(response))
                {
                    activity.SetTag("http.response_headers", response.Headers.ToString());
                    if (response.RequestMessage?.Options.TryGetValue(DevHttpBodyLoggingHandler.ResponseBodyKey, out string? respBody) is true)
                    {
                        activity.SetTag("http.response_body", respBody);
                    }
                }
            };
        }))
    .WithMetrics(b => b.AddMeter(Constants.Telemetry.AppMeterName))
    .UseFunctionsWorkerDefaults()
    .UseAzureMonitorExporter();

if (host.Environment.IsDevelopment())
{
    host.Services.AddTransient<DevHttpBodyLoggingHandler>();
    host.Services.ConfigureHttpClientDefaults(b => b.AddHttpMessageHandler<DevHttpBodyLoggingHandler>());
}

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

// Prefer explicit service URIs so Container Apps can use managed identity-backed storage.
// The connection-string fallback is retained for local development.
string? storageConnectionString = host.Configuration["AzureWebJobsStorage"];
Uri? tableEndpoint = StartupInfrastructureFactory.TryGetStorageServiceUri(
    host.Configuration,
    "table",
    Constants.Configuration.Azure.Storage.TableEndpoint,
    ConfigurationPath.Combine("AzureWebJobsStorage", "tableServiceUri"),
    ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
TableServiceClient tsc = StartupInfrastructureFactory.CreateTableServiceClient(storageConnectionString, tableEndpoint, credential);
host.Services.AddSingleton(tsc);
IEnumerable<string> storageTables = host.Configuration
    .GetSection(Constants.Configuration.Azure.Storage.Tables)
    .Get<IEnumerable<string>>() ?? [];

foreach (var tableName in storageTables)
{
    host.Services.AddKeyedSingleton(tableName, (sp, _) =>
    {
        ILogger<TableClient>? logger = sp.GetService<ILogger<TableClient>>();
        logger?.CreatingTableClientForTable(tableName);
        TableClient tableClient = tsc.GetTableClient(tableName);
        logger?.EnsuringTableTableExists(tableName);
        tableClient.CreateIfNotExists();
        logger?.TableTableExists(tableName);
        return tableClient;
    });
}

Uri? blobsEndpoint = StartupInfrastructureFactory.TryGetStorageServiceUri(
    host.Configuration,
    "blob",
    Constants.Configuration.Azure.Storage.BlobsEndpoint,
    ConfigurationPath.Combine("AzureWebJobsStorage", "blobServiceUri"),
    ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));
BlobServiceClient bsc = StartupInfrastructureFactory.CreateBlobServiceClient(storageConnectionString, blobsEndpoint, credential);
host.Services.AddSingleton(_ =>
{
    BlobContainerClient blobContainer = bsc.GetBlobContainerClient("misc");
    blobContainer.CreateIfNotExists();
    return blobContainer;
});

host.Services.TryAddChatBot(
    host.Configuration,
    credential,
    ResolveCopilotBlobStorageUri(blobsEndpoint, bsc.Uri),
    out bool hasValidChatBotConfiguration,
    out string[] chatBotConfigurationFailures);

IHost builtHost = host.Build();
ILogger startupLogger = builtHost.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
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

static Uri ResolveCopilotBlobStorageUri(Uri? configuredBlobServiceUri, Uri blobServiceClientUri)
{
    Uri candidate = configuredBlobServiceUri ?? blobServiceClientUri;
    return !Uri.UriSchemeHttps.Equals(candidate.Scheme, StringComparison.OrdinalIgnoreCase)
        ? throw new InvalidOperationException(
            $"Copilot blob storage URI must be HTTPS when using credential-based auth. Configure '{Constants.Configuration.Azure.Storage.BlobsEndpoint}' as an HTTPS endpoint. Received: {candidate}")
        : candidate;
}

static bool IsGrpcRequest(HttpRequestMessage req)
{
    var contentType = req.Content?.Headers.ContentType?.MediaType;
    if (!string.IsNullOrEmpty(contentType) &&
        contentType.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase))
        return true;

    if (req.Headers.TryGetValues("te", out var te) &&
        te.Any(v => v.Contains("trailers", StringComparison.OrdinalIgnoreCase)))
        return true;

    if (req.Version.Major >= 2)
    {
        ReadOnlySpan<char> path = req.RequestUri?.AbsolutePath;
        Span<Range> _ = stackalloc Range[3];
        if (path.Split(_, '/', StringSplitOptions.RemoveEmptyEntries) is 2)
            return true;
    }

    return false;
}

static bool IsGrpcResponse(HttpResponseMessage resp)
{
    var contentType = resp.Content.Headers.ContentType?.MediaType;
    return !string.IsNullOrEmpty(contentType) &&
           contentType.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// Captures HTTP request/response bodies onto the current Activity for dev diagnostics.
/// Runs in the async DelegatingHandler pipeline so body reads are safe and non-blocking.
/// </summary>
sealed class DevHttpBodyLoggingHandler : DelegatingHandler
{
    internal static readonly HttpRequestOptionsKey<string> RequestBodyKey = new("dev.request_body");
    internal static readonly HttpRequestOptionsKey<string> ResponseBodyKey = new("dev.response_body");

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool isGrpc = IsGrpc(request);
        if (!isGrpc && request.Content is not null)
        {
            string requestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            request.Options.Set(RequestBodyKey, requestBody);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!isGrpc)
        {
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            request.Options.Set(ResponseBodyKey, responseBody);
        }

        return response;
    }

    private static bool IsGrpc(HttpRequestMessage req) =>
        req.Content?.Headers.ContentType?.MediaType?.StartsWith("application/grpc", StringComparison.OrdinalIgnoreCase) is true;
}