namespace DiscordBotFunctionApp.FIRSTInterop;

using FIRST.Api;
using FIRST.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureFIRSTApi(this IServiceCollection services)
    {
        services
            .AddHttpClient(Constants.ServiceKeys.FIRSTHttpClient);

        return services
            .AddSingleton<IScheduleApi>(sp =>
             {
                 var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.ServiceKeys.FIRSTHttpClient);
                 var appConfig = sp.GetRequiredService<IConfiguration>();
                 return new ScheduleApi(httpClient,
                     new Configuration
                     {
                         BasePath = "https://frc-api.firstinspires.org/v3.0",
                         Username = appConfig.GetValue<string>(Constants.Configuration.FIRST.Username),
                         Password = appConfig.GetValue<string>(Constants.Configuration.FIRST.Password)
                     }
                     );
             });
    }
}
