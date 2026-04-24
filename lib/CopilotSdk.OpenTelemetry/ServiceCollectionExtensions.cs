namespace CopilotSdk.OpenTelemetry;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TracerProviderBuilder = global::OpenTelemetry.Trace.TracerProviderBuilder;

/// <summary>DI extensions for registering the Copilot SDK OpenTelemetry instrumentation.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default <see cref="IConversationTracer"/> and an
    /// <see cref="InMemoryConversationTraceContextStore"/> if no <see cref="IConversationTraceContextStore"/>
    /// has already been registered. Consumers wanting durable persistence should register their
    /// own <see cref="IConversationTraceContextStore"/> before calling this method.
    /// </summary>
    public static IServiceCollection AddCopilotSdkOpenTelemetry(this IServiceCollection services)
    {

        services.TryAddSingleton<IConversationTraceContextStore, InMemoryConversationTraceContextStore>();
        services.TryAddSingleton<IConversationTracer, ConversationTracer>();

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
}
