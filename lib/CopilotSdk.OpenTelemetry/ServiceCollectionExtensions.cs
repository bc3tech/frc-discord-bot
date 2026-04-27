namespace CopilotSdk.OpenTelemetry;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MeterProviderBuilder = global::OpenTelemetry.Metrics.MeterProviderBuilder;
using TracerProviderBuilder = global::OpenTelemetry.Trace.TracerProviderBuilder;

/// <summary>DI extensions for registering the Copilot SDK OpenTelemetry instrumentation.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default <see cref="IConversationTracer"/>,
    /// <see cref="TelemetrySessionSubscriber"/> as an <see cref="ISessionEventSubscriber"/>,
    /// <see cref="CopilotSdkOpenTelemetryOptions"/> via the options pattern, and an
    /// <see cref="InMemoryConversationTraceContextStore"/> if no <see cref="IConversationTraceContextStore"/>
    /// has already been registered. Consumers wanting durable persistence should register their
    /// own <see cref="IConversationTraceContextStore"/> before calling this method.
    /// </summary>
    public static IServiceCollection AddCopilotSdkOpenTelemetry(
        this IServiceCollection services,
        Action<CopilotSdkOpenTelemetryOptions>? configure = null)
    {
        services.TryAddSingleton<IConversationTraceContextStore, InMemoryConversationTraceContextStore>();
        services.TryAddSingleton<IConversationTracer, ConversationTracer>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<ISessionEventSubscriber, TelemetrySessionSubscriber>());

        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<CopilotSdkOpenTelemetryOptions>();
        }

        return services;
    }

    /// <summary>
    /// Registers the library's <see cref="System.Diagnostics.ActivitySource"/> with a
    /// <see cref="TracerProviderBuilder"/> so its spans are exported.
    /// </summary>
    public static TracerProviderBuilder AddCopilotSdkOpenTelemetry(this TracerProviderBuilder builder)
    {
        return builder.AddSource(CopilotSdkOpenTelemetry.ActivitySourceName);
    }

    /// <summary>
    /// Registers the library's <see cref="System.Diagnostics.Metrics.Meter"/> with a
    /// <see cref="MeterProviderBuilder"/> so its metrics are exported.
    /// </summary>
    public static MeterProviderBuilder AddCopilotSdkOpenTelemetry(this MeterProviderBuilder builder)
    {
        return builder.AddMeter(CopilotSdkOpenTelemetry.MeterName);
    }
}
