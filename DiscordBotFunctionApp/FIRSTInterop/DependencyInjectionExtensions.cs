namespace DiscordBotFunctionApp.FIRSTInterop;

using FIRST.Api;
using FIRST.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureFIRSTApi(this IServiceCollection services)
    {
        var httpClient = new HttpClient();

        return services
            .AddSingleton(sp =>
            {
                var appConfig = sp.GetRequiredService<IConfiguration>();
                return new Configuration
                {
                    BasePath = "https://frc-api.firstinspires.org/v3.0",
                    Username = appConfig.GetValue<string>(Constants.Configuration.FIRST.Username),
                    Password = appConfig.GetValue<string>(Constants.Configuration.FIRST.Password)
                };
            })
            .AddSingleton<IScheduleApi>(sp =>
             {
                 var config = sp.GetRequiredService<Configuration>();
                 return new ScheduleApi(httpClient, config);
             })
            .AddSingleton<IRankingsApi>(sp =>
            {
                var config = sp.GetRequiredService<Configuration>();
                return new RankingsApi(httpClient, config);
            });
    }
}
