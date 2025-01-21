namespace DiscordBotFunctionApp.DiscordInterop;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

using DiscordBotFunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TheBlueAlliance.Api.Notifications;

using AllianceSelection = Embeds.AllianceSelection;
using MatchScore = Embeds.MatchScore;
using UpcomingMatch = Embeds.UpcomingMatch;

internal static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureDiscord(this IServiceCollection services)
    {
        return services.AddSingleton(sp =>
        {
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Logging.Discord.LogLevel] ?? "Info";
            return new DiscordSocketConfig()
            {
                HandlerTimeout = 10_000,
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel),
                LogGatewayIntentWarnings = true,
                GatewayIntents = GatewayIntents.GuildMessages,
            };
        })
        .AddSingleton<EmbedBuilderFactory>()
        .AddSingleton<EmbeddingGenerator>()
        .AddSingleton(sp =>
        {
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Logging.Discord.LogLevel] ?? "Info";
            return new DiscordRestConfig()
            {
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel)
            };
        }).AddSingleton(sp =>
        {
            var c = new DiscordRestClient(sp.GetRequiredService<DiscordRestConfig>());
            c.Log += m =>
            {
                sp.GetRequiredService<ILogger<DiscordRestClient>>().Log(m.Severity.ToLogLevel(), m.Message);
                return Task.CompletedTask;
            };
            return c;
        })
        .AddSingleton(sp =>
        {
            var c = new DiscordSocketClient(sp.GetRequiredService<DiscordSocketConfig>());
            c.Log += m =>
            {
                sp.GetRequiredService<ILogger<DiscordSocketClient>>().Log(m.Severity.ToLogLevel(), m.Message);
                return Task.CompletedTask;
            };
            return c;
        })
        .AddSingleton<DiscordMessageDispatcher>()
        .AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<InteractionService>();
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Logging.Discord.LogLevel] ?? "Info";
            var i = new InteractionService(sp.GetRequiredService<DiscordSocketClient>().Rest, new InteractionServiceConfig
            {
                UseCompiledLambda = true,
                EnableAutocompleteHandlers = true,
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel),
                DefaultRunMode = RunMode.Async,
            });

            i.Log += m =>
            {
                logger.Log(m.Severity.ToLogLevel(), m.Exception, $"[{m.Source}]: {m.Message}");
                return Task.CompletedTask;
            };

            return i;
        })
        .AddHostedService<DiscordInitializationService>()
        .AddKeyedSingleton<IEmbedCreator, MatchScore>(MatchScore.TargetType.ToInvariantString())
        .AddKeyedSingleton<IEmbedCreator, AllianceSelection>(AllianceSelection.TargetType.ToInvariantString())
        .AddKeyedSingleton<IEmbedCreator, UpcomingMatch>(UpcomingMatch.TargetType.ToInvariantString())
        .AddKeyedSingleton<IEmbedCreator, Award>(Award.TargetType.ToInvariantString());
    }
}
