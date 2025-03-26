namespace FunctionApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection FixAppInsightsLogging(this IServiceCollection services)
    {
        // You will need extra configuration because above will only log per default Warning (default AI configuration) and above because of following line:
        // https://github.com/microsoft/ApplicationInsights-dotnet/blob/main/NETCORE/src/Shared/Extensions/ApplicationInsightsExtensions.cs#L427
        // This is documented here:
        // https://github.com/microsoft/ApplicationInsights-dotnet/issues/2610#issuecomment-1316672650
        // So remove the default logger rule (warning and above). This will result that the default will be Information.
        services.Configure<LoggerFilterOptions>(options =>
        {
            //var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
            //    is "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            //if (toRemove is not null)
            //{
            //    options.Rules.Remove(toRemove);
            //}
        });

        return services;
    }
}
