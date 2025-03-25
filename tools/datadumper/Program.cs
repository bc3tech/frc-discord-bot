namespace DataDumper;

using Azure.Identity;
using Azure.Storage.Blobs;

using Common;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TheBlueAlliance.Api;
using TheBlueAlliance.Client;

internal sealed class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Not applicable to Main()")]
    private static async Task Main(string[] args)
    {
        var b = new HostBuilder()
            .ConfigureDefaults(args)
            .ConfigureLogging((context, builder) => builder
                .AddSimpleConsole(i =>
                {
                    i.IncludeScopes = true;
                    i.SingleLine = true;
                }))
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<ConsoleHost>();

                var httpClient = new HttpClient();
                var config = new Configuration
                {
                    ApiKey = { { "X-TBA-Auth-Key", Throws.IfNullOrWhiteSpace(context.Configuration["TbaApiKey"], message: $"TheBlueAlliance API key is required. Set the 'TbaApiKey' config variable appropriately") } },
                    BasePath = "https://www.thebluealliance.com/api/v3"
                };
                services
                    .AddSingleton<ITeamApi>(sp => new TeamApi(httpClient, config))
                    .AddSingleton<IEventApi>(sp => new EventApi(httpClient, config))
                    .AddSingleton<IMatchApi>(sp => new MatchApi(httpClient, config))
                    .AddSingleton<IDistrictApi>(sp => new DistrictApi(httpClient, config));

                services.AddSingleton(sp => new BlobServiceClient(new Uri(Throws.IfNullOrWhiteSpace(context.Configuration["StorageAccountUri"]), UriKind.Absolute), new DefaultAzureCredential(includeInteractiveCredentials: context.HostingEnvironment.IsDevelopment())));
            });

        await b.RunConsoleAsync();
    }
}