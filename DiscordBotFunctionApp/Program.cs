namespace DiscordBotFunctionApp;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;

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
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

using Throws = Common.Throws;

internal sealed class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Not applicable to main()")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for main()")]
    private static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
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
                    .AddSingleton<EventRepository>()
                    .AddSingleton<TeamRepository>()
                    .AddSingleton<SubscriptionManager>()
                    .AddSingleton<TokenCredential>(credential)
                    .AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        var apiKey = Throws.IfNullOrWhiteSpace(config[Constants.Configuration.TbaApiKey]);
                        return new Common.Tba.Api.ApiClient(new HttpClientRequestAdapter(new ApiKeyAuthenticationProvider(apiKey, "X-TBA-Auth-Key", ApiKeyAuthenticationProvider.KeyLocation.Header)));
                    })
                    .AddHostedService<TbaInitializationService>();

                TableServiceClient tsc = Uri.TryCreate(context.Configuration[Constants.Configuration.Azure.Storage.TableEndpoint], UriKind.Absolute, out var tableEndpoint)
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
            }).Build();

        await host.RunAsync();
    }
}