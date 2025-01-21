namespace DiscordBotFunctionApp;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

using DiscordBotFunctionApp.DiscordInterop;
using DiscordBotFunctionApp.Storage;
using DiscordBotFunctionApp.Subscription;
using DiscordBotFunctionApp.TbaInterop;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using Throws = Common.Throws;

internal sealed class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Not applicable to main()")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for main()")]
    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureLogging((context, builder) => builder
                .AddConfiguration(context.Configuration.GetSection("Logging"))
                .AddDebug()
                .AddSimpleConsole(i =>
                {
                    i.IncludeScopes = true;
                    i.SingleLine = true;
                    i.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                }))
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
                    .AddApplicationInsightsTelemetryWorkerService()
                    .ConfigureFunctionsApplicationInsights()
                    .ConfigureDiscord()
                    .ConfigureTheBlueAllianceApi()
                    .AddSingleton<EventRepository>()
                    .AddSingleton<TeamRepository>()
                    .AddSingleton<SubscriptionManager>()
                    .AddSingleton<TokenCredential>(credential);

                var tableStorageEndpointConfigValue = context.Configuration[Constants.Configuration.Azure.Storage.TableEndpoint];
                TableServiceClient tsc = !string.IsNullOrWhiteSpace(tableStorageEndpointConfigValue) && Uri.TryCreate(tableStorageEndpointConfigValue, UriKind.Absolute, out var tableEndpoint)
                    ? new TableServiceClient(tableEndpoint, credential)
                    : new TableServiceClient(Throws.IfNullOrWhiteSpace(context.Configuration["AzureWebJobsStorage"]));
                foreach (var i in context.Configuration.GetSection(Constants.Configuration.Azure.Storage.Tables).Get<IEnumerable<string>>() ?? [])
                {
                    services.AddKeyedSingleton(i, (sp, _) =>
                    {
                        var logger = sp.GetService<ILogger<TableClient>>();

                        logger?.LogTrace("Creating TableClient for {Table}", i);
                        var c = tsc.GetTableClient(i);
                        logger?.LogTrace("Ensuring table {Table} exists", i);
                        c.CreateIfNotExists();
                        logger?.LogDebug("Table {Table} exists", i);
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
            }).Build();

        await host.RunAsync().ConfigureAwait(false);
    }
}