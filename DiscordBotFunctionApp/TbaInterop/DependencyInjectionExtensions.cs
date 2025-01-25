
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
            .AddHttpClient(ApiClient.HttpClientKey, c => c.BaseAddress = new("https://www.thebluealliance.com/api/v3"));
        return services.AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        var apiKey = Throws.IfNullOrWhiteSpace(config[DiscordBotFunctionApp.Constants.Configuration.TbaApiKey]);
                        return new Configuration { ApiKey = { { "X-TBA-Auth-Key", apiKey } } };
                    })
                    .AddSingleton<ITeamApi, TeamApi>()
                    .AddSingleton<IEventApi, EventApi>()
                    .AddSingleton<IMatchApi, MatchApi>()
                    .AddHostedService<TbaInitializationService>();
    }
}
