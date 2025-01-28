
namespace DiscordBotFunctionApp.TbaInterop;

using Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TheBlueAlliance.Api;
using TheBlueAlliance.Client;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureTheBlueAllianceApi(this IServiceCollection services)
    {
        services
            .AddHttpClient(DiscordBotFunctionApp.Constants.ServiceKeys.TheBlueAllianceHttpClient, c => c.BaseAddress = new("https://www.thebluealliance.com/api/v3"));
        return services.AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        var apiKey = Throws.IfNullOrWhiteSpace(config[DiscordBotFunctionApp.Constants.Configuration.TbaApiKey], message: $"TheBlueAlliance API key is required. Set the {DiscordBotFunctionApp.Constants.Configuration.TbaApiKey} config variable appropriately");
                        return new Configuration { ApiKey = { { "X-TBA-Auth-Key", apiKey } } };
                    })
                    .AddSingleton<ITeamApi>(sp =>
                    {
                        var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient(DiscordBotFunctionApp.Constants.ServiceKeys.TheBlueAllianceHttpClient);
                        var cfg = sp.GetRequiredService<Configuration>();
                        return new TeamApi(client, cfg);
                    })
                    .AddSingleton<IEventApi>(sp =>
                    {
                        var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient(DiscordBotFunctionApp.Constants.ServiceKeys.TheBlueAllianceHttpClient);
                        var cfg = sp.GetRequiredService<Configuration>();
                        return new EventApi(client, cfg);
                    })
                    .AddSingleton<IMatchApi>(sp =>
                    {
                        var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient(DiscordBotFunctionApp.Constants.ServiceKeys.TheBlueAllianceHttpClient);
                        var cfg = sp.GetRequiredService<Configuration>();
                        return new MatchApi(client, cfg);
                    })
                    .AddHostedService<TbaInitializationService>();
    }
}
