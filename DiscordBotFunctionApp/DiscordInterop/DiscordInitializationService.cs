namespace DiscordBotFunctionApp.DiscordInterop;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DiscordBotFunctionApp;
using DiscordBotFunctionApp.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal sealed partial class DiscordInitializationService(IDiscordClient discordClient, InteractionService interactionService, ChatBot.MessageHandler chatBot, TimeProvider time, IConfiguration appConfig, ILoggerFactory logFactory, IServiceProvider services) : IHostedService
{
    private readonly ILogger _logger = logFactory.CreateLogger<DiscordInitializationService>();
    private readonly DiscordSocketClient client = discordClient as DiscordSocketClient ?? throw new ArgumentException(nameof(discordClient));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startTime = time.GetTimestamp();
        var tsc = new TaskCompletionSource();
        client.Ready += () =>
        {
            tsc.SetResult();
            return Task.CompletedTask;
        };

        _logger.LoggingInToDiscord();
        client.LoggedIn += () =>
        {
            _logger.DiscordClientLoggedIn();
            return Task.CompletedTask;
        };
        await client.LoginAsync(TokenType.Bot, appConfig[Constants.Configuration.Discord.Token], validateToken: true)
            .ConfigureAwait(false);

        _logger.StartingDiscordClient();
        await client.StartAsync()
            .ConfigureAwait(false);
        _logger.DiscordClientStarted();

        _logger.WaitingForClientToBeReady();
        await tsc.Task.ConfigureAwait(false);

        _logger.DiscordClientReady();
        _logger.CurrentlyActiveGuildsActiveGuilds(string.Join($"\n", client.Guilds.Select(g => $"- {g.Name}")));

        await InstallCommandsAsync(cancellationToken).ConfigureAwait(false);

        var initTime = time.GetElapsedTime(startTime).TotalSeconds;
        _logger.DiscordInitializationTimeDiscordInitTimeS(initTime);
        _logger.LogMetric("DiscordInitTime", initTime);

        client.InteractionCreated += async x =>
        {
            _logger.InteractionReceivedInteractionTypeDataInteractionData(x.Type, JsonSerializer.Serialize(x.Data, _debugSerializerOptions));
            var ctx = new SocketInteractionContext(client, x);
            await interactionService.ExecuteCommandAsync(ctx, services).ConfigureAwait(false);
        };

        client.ApplicationCommandUpdated += cmd =>
        {
            _logger.ApplicationCommandUpdatedCommandName(cmd.Name);
            return Task.CompletedTask;
        };

        client.IntegrationUpdated += integration =>
        {
            _logger.IntegrationUpdatedIntegrationName(integration.Name);
            return Task.CompletedTask;
        };

        client.LatencyUpdated += (i, j) =>
        {
            _logger.PingFromGatewayLatencyWasPreviousLatencyMsMsNowLatencyMsMs(i, j);
            _logger.LogMetric("Ping", j, new Dictionary<string, object>() { { "PreviousLatencyMs", i } });
            return Task.CompletedTask;
        };

        client.MessageReceived += async i =>
        {
            _logger.MessageReceivedFromGatewayGatewayMessage(i);
            if (i is SocketUserMessage msg && i.Channel is IDMChannel && i.Author.GlobalName is not null)
            {
                await chatBot.HandleUserMessageAsync(msg).ConfigureAwait(false);
            }
        };
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync().ConfigureAwait(false);
        await client.DisposeAsync().ConfigureAwait(false);
    }

    private async Task InstallCommandsAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.CreateMethodScope();
        // Hook the MessageReceived event into our command handler
        client.SlashCommandExecuted += cmd =>
        {
            LogCommandExecuted(cmd);
            return Task.CompletedTask;
        };
        client.MessageCommandExecuted += msg =>
        {
            LogMessageExecuted(msg);
            return Task.CompletedTask;
        };

        cancellationToken.ThrowIfCancellationRequested();

        _logger.LoadingCommandModules();

        var discoveredModules = await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), services).ConfigureAwait(false);
        _logger.NumCommandsCommandModulesLoaded(interactionService.Modules.Count);

        await interactionService.RegisterCommandsGloballyAsync();

        //foreach (var g in client.Guilds)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            var registeredCommands = await interactionService.AddModulesToGuildAsync(g, deleteMissing: true, modules: [.. discoveredModules]).ConfigureAwait(false);
        //            _logger.NumCommandsCommandsAddedToGuildGuildNameGuildIdGloballyAvailableCommands(registeredCommands.Count, g.Name, g.Id, string.Join(", ", registeredCommands.Select(i => i.Name)));
        //            break;
        //        }
        //        catch (Exception e) when (e is TimeoutException or OperationCanceledException)
        //        {
        //            _logger.LogError(e, "Error while trying to do command installation for guild {GuildName} ({GuildId})", g.Name, g.Id);
        //            // When this happens, we usually have seen a throttling error from Discord which requires us to wait anywhere up to 1m. So, just pause for a minute and try again.

        //            await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
        //        }
        //    }
        //}
    }

    private void LogMessageExecuted(SocketMessageCommand msg)
    {
        _logger.ReceivedMessageMessageName(msg.Data.Name);
        _logger.MessageDataMessageData(JsonSerializer.Serialize(msg.Data, _debugSerializerOptions));
    }

    private static readonly JsonSerializerOptions _debugSerializerOptions = new(JsonSerializerDefaults.Web) { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

    private void LogCommandExecuted(SocketSlashCommand command)
    {
        _logger.ReceivedCommandCommandName(command.Data.Name);
        _logger.CommandDataCommandData(JsonSerializer.Serialize(command.Data, _debugSerializerOptions));
    }
}