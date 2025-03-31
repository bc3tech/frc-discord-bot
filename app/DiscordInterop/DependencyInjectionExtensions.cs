namespace FunctionApp.DiscordInterop;
using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

using FunctionApp;
using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models.Notifications;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AllianceSelection = Embeds.AllianceSelection;
using CompLevelStarting = Embeds.CompLevelStarting;
using MatchScore = Embeds.MatchScore;
using MatchVideo = Embeds.MatchVideo;
using ScheduleUpdate = Embeds.ScheduleUpdate;
using UpcomingMatch = Embeds.UpcomingMatch;

internal static class DependencyInjectionExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0000:Use source generated logging methods for improved performance", Justification = "Piping log messages from Discord on through; severity is set by Discord so we can't use strongly-typed methods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Piping log messages from Discord on through; severity is set by Discord so we can't use strongly-typed methods")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Piping log messages from Discord on through; severity is set by Discord so we can't use strongly-typed methods")]
    public static IServiceCollection ConfigureDiscord(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Discord.LogLevel] ?? "Info";
            return new DiscordSocketConfig()
            {
                HandlerTimeout = 10_000,
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel),
                LogGatewayIntentWarnings = false,
                GatewayIntents = GatewayIntents.AllUnprivileged,
            };
        })
        .AddSingleton<EmbedBuilderFactory>()
        .AddSingleton<WebhookEmbeddingGenerator>()
        .AddSingleton(sp =>
        {
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Discord.LogLevel] ?? "Info";
            return new DiscordRestConfig()
            {
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel)
            };
        })
        .AddSingleton<IDiscordClient>(sp =>
        {
            var logger = sp.GetService<ILogger<DiscordSocketClient>>();
            var c = new DiscordSocketClient(sp.GetRequiredService<DiscordSocketConfig>());
            c.Log += m =>
            {
                logger?.Log(m.Severity.ToLogLevel(), m.Message);
                return Task.CompletedTask;
            };
            return c;
        })
        .AddSingleton<DiscordMessageDispatcher>()
        .AddSingleton(sp =>
        {
            var logger = sp.GetService<ILogger<InteractionService>>();
            var discordLogLevel = sp.GetRequiredService<IConfiguration>()[Constants.Configuration.Discord.LogLevel] ?? "Info";
            var i = new InteractionService(((DiscordSocketClient)sp.GetRequiredService<IDiscordClient>()).Rest, new InteractionServiceConfig
            {
                UseCompiledLambda = true,
                EnableAutocompleteHandlers = true,
                LogLevel = Enum.Parse<LogSeverity>(discordLogLevel),
                DefaultRunMode = RunMode.Async,
            });

            i.Log += m =>
            {
                logger?.Log(m.Severity.ToLogLevel(), m.Exception, $"[{m.Source}]: {m.Message}");
                return Task.CompletedTask;
            };

            return i;
        })
        .AddHostedService<DiscordInitializationService>()
        .AddSingleton<MatchScore>()
        .AddKeyedSingleton<INotificationEmbedCreator, MatchScore>(MatchScore.TargetType.ToInvariantString(), (sp, _) => sp.GetRequiredService<MatchScore>())
        .AddKeyedSingleton<INotificationEmbedCreator, AllianceSelection>(AllianceSelection.TargetType.ToInvariantString())
        .AddKeyedSingleton<INotificationEmbedCreator, UpcomingMatch>(UpcomingMatch.TargetType.ToInvariantString())
        .AddKeyedSingleton<INotificationEmbedCreator, ScheduleUpdate>(ScheduleUpdate.TargetType.ToInvariantString())
        .AddKeyedSingleton<INotificationEmbedCreator, Award>(Award.TargetType.ToInvariantString())
        //.AddKeyedSingleton<INotificationEmbedCreator, MatchVideo>(MatchVideo.TargetType.ToInvariantString())
        .AddKeyedSingleton<INotificationEmbedCreator, CompLevelStarting>(CompLevelStarting.TargetType.ToInvariantString())
        .AddKeyedSingleton<IEmbedCreator<string>, EventDetail>(nameof(EventDetail))
        .AddKeyedSingleton<IEmbedCreator<(string eventKey, string teamKey)>, UpcomingMatch>(nameof(UpcomingMatch))
        .AddKeyedSingleton<IEmbedCreator<(string?, ushort)>, Schedule>(nameof(Schedule))
        .AddKeyedSingleton<IEmbedCreator<(string, bool)>, MatchScore>(nameof(MatchScore), (sp, _) => sp.GetRequiredService<MatchScore>())
        .AddKeyedSingleton<IEmbedCreator<string>, MatchVideo>(nameof(MatchVideo))
        .AddKeyedSingleton<IEmbedCreator<(int? Year, string TeamKey, string? EventKey)>, TeamRank>(nameof(TeamRank))
        .AddKeyedSingleton<IEmbedCreator<string>, TeamDetail>(nameof(TeamDetail));

        // Register the IHandleUserInteraction class instances as such
        services
            .AddSingleton<IHandleUserInteractions, SubscriptionCommandModule>()
            .AddSingleton<IHandleUserInteractions>(sp => sp.GetRequiredService<MatchScore>());

        return services;
    }
}
