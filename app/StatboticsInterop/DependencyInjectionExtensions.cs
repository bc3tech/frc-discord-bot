namespace DiscordBotFunctionApp.StatboticsInterop;

using Microsoft.Extensions.DependencyInjection;

using Statbotics.Api;
using Statbotics.Client;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStatboticsApi(this IServiceCollection services)
    {
        var config = new Configuration { BasePath = "https://api.statbotics.io" };
        var httpClient = new HttpClient();

        return services
            .AddSingleton<IEventApi>(new EventApi(httpClient, config))
            .AddSingleton<IMatchApi>(new MatchApi(httpClient, config))
            .AddSingleton<ITeamApi>(new TeamApi(httpClient, config))
            .AddSingleton<ITeamMatchApi>(new TeamMatchApi(httpClient, config))
            .AddSingleton<ITeamYearApi>(new TeamYearApi(httpClient, config))
            .AddSingleton<ITeamEventApi>(new TeamEventApi(httpClient, config));
    }
}
