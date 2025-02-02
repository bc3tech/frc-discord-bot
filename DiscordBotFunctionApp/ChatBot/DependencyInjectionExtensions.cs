namespace DiscordBotFunctionApp.ChatBot;

using Microsoft.Extensions.DependencyInjection;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureChatBotFunctionality(this IServiceCollection services)
    {
        return services
            .AddSingleton<SignalRConnectionInfo>()
            .AddSingleton<HubConnectionFactory>()
            .AddSingleton<MessageHandler>()
            .AddHostedService<ChatBotInitializationService>();
    }
}
