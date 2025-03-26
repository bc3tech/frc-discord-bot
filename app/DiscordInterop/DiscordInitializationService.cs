namespace FunctionApp.DiscordInterop;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using FunctionApp;
using FunctionApp.DiscordInterop.CommandModules;
using FunctionApp.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal sealed partial class DiscordInitializationService(IDiscordClient discordClient, InteractionService interactionService, ChatBot.MessageHandler chatBot, TimeProvider time, Meter meter, IConfiguration appConfig, ILoggerFactory logFactory, IServiceProvider services) : IHostedService
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
        meter.LogMetric("DiscordInitTime", initTime);

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
            meter.LogMetric("Ping", j, [new("PreviousLatencyMs", i)]);
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

        client.ButtonExecuted += async (button) =>
        {
            if (button.Data.CustomId is Constants.InteractionElements.CancelButtonDeleteMessage)
            {
                try
                {
                    await button.InteractionChannel.DeleteMessageAsync(button.Message.Id, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
                }
                catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                {
                    try
                    {
                        await button.UpdateAsync(p =>
                        {
                            p.Content = "Canceled.";
                            p.Attachments = null;
                            p.Components = null;
                            p.Embeds = null;
                        });
                    }
                    catch (Exception e2) when (e2 is not OperationCanceledException or TaskCanceledException)
                    {
                        _logger.ErrorDeletingMessageMessageId(e, button.Message.Id);
                    }
                }
            }
            else
            {
                try
                {
                    foreach (var s in services.GetServices<IHandleUserInteractions>())
                    {
                        if (await s.HandleInteractionAsync(services, button, cancellationToken))
                        {
                            _logger.ServiceTypeHandledButtonClickButtonId(s.GetType().Name, button.Data.CustomId);
                            return;
                        }
                    }
                }
                catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                {
                    _logger.ErrorHandlingButtonClickButtonId(e, button.Data.CustomId);
                    throw;
                }
            }
        };

        client.SelectMenuExecuted += async (menu) =>
        {
            foreach (var s in services.GetServices<IHandleUserInteractions>())
            {
                if (await s.HandleInteractionAsync(services, menu, cancellationToken))
                {
                    _logger.ServiceTypeHandledMenuSelectionMenuIdValueId(s.GetType().Name, menu.Data.CustomId, menu.Data.Values.FirstOrDefault() ?? "[unknown]");
                    return;
                }
            }

            _logger.UnknownMenuSelectionReceivedMenuData(JsonSerializer.Serialize(menu.Data));
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
    }

    private void LogMessageExecuted(SocketMessageCommand msg)
    {
        _logger.ReceivedMessageMessageName(msg.Data.Name);
        _logger.MessageDataMessageData(JsonSerializer.Serialize(msg.Data, _debugSerializerOptions));
    }

    private static readonly JsonSerializerOptions _debugSerializerOptions = new(JsonSerializerDefaults.Web) { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

    private void LogCommandExecuted(SocketSlashCommand command)
    {
        _logger.CommandDataCommandData(JsonSerializer.Serialize(command.Data, _debugSerializerOptions));
    }
}