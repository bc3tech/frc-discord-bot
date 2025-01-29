namespace DiscordBotFunctionApp.StatboticsInterop;

using Microsoft.Extensions.DependencyInjection;

using Statbotics.Api;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStatboticsApi(this IServiceCollection services)
    {
        services
            .AddHttpClient(Constants.ServiceKeys.StatboticsHttpClient);

        return services
            .AddSingleton<IEventApi>(sp =>
             {
                 var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.StatboticsHttpClient);
                 return new EventApi(httpClient, "https://api.statbotics.io");
             })
            .AddSingleton<IMatchApi>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.StatboticsHttpClient);
                return new MatchApi(httpClient, "https://api.statbotics.io");
            })
            .AddSingleton<ITeamApi>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.StatboticsHttpClient);
                return new TeamApi(httpClient, "https://api.statbotics.io");
            })
            .AddSingleton<ITeamMatchApi>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.StatboticsHttpClient);
                return new TeamMatchApi(httpClient, "https://api.statbotics.io");
            })
            .AddSingleton<ITeamEventApi>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.StatboticsHttpClient);
                return new TeamEventApi(httpClient, "https://api.statbotics.io");
            });
    }
}
