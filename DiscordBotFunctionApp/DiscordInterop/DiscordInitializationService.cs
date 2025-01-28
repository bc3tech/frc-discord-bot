namespace DiscordBotFunctionApp.DiscordInterop;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using DiscordBotFunctionApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal sealed partial class DiscordInitializationService(DiscordSocketClient client, InteractionService interactionService, IConfiguration appConfig, ILoggerFactory logFactory, IServiceProvider services) : IHostedService
{
    private readonly ILogger _logger = logFactory.CreateLogger<DiscordInitializationService>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tsc = new TaskCompletionSource();
        client.Ready += () =>
        {
            tsc.SetResult();
            return Task.CompletedTask;
        };

        _logger.LogDebug("Logging in to Discord...");
        client.LoggedIn += () =>
        {
            _logger.LogInformation("Discord client logged in");
            return Task.CompletedTask;
        };
        await client.LoginAsync(TokenType.Bot, appConfig[Constants.Configuration.Discord.Token], validateToken: true)
            .ConfigureAwait(false);

        _logger.LogDebug("Starting Discord client...");
        await client.StartAsync()
            .ConfigureAwait(false);
        _logger.LogInformation("Discord client started");

        _logger.LogDebug("Waiting for client to be Ready");
        await tsc.Task.ConfigureAwait(false);

        _logger.LogInformation("Discord client ready");
        _logger.LogDebug("Currently active guilds:\n{ActiveGuilds}", string.Join($"\n", client.Guilds.Select(g => $"- {g.Name}")));

        await InstallCommandsAsync(cancellationToken).ConfigureAwait(false);

        client.InteractionCreated += async x =>
        {
            _logger.LogInformation("Interaction received: {InteractionType}", x.Type);
            _logger.LogTrace("Interaction data: {InteractionData}", JsonSerializer.Serialize(x.Data, _debugSerializerOptions));
            var ctx = new SocketInteractionContext(client, x);
            await interactionService.ExecuteCommandAsync(ctx, services).ConfigureAwait(false);
        };

        client.ApplicationCommandUpdated += cmd =>
        {
            _logger.LogDebug("Application command updated: {CommandName}", cmd.Name);
            return Task.CompletedTask;
        };

        client.IntegrationUpdated += integration =>
        {
            _logger.LogDebug("Integration updated: {IntegrationName}", integration.Name);
            return Task.CompletedTask;
        };

        client.LatencyUpdated += (i, j) =>
        {
            _logger.LogTrace("Ping from gateway ({int1},{int2})", i, j);
            return Task.CompletedTask;
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

        _logger.LogTrace("Loading command modules...");
        var m = await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), services).ConfigureAwait(false);
        _logger.LogDebug("{NumCommands} command modules loaded", interactionService.Modules.Count);

        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogTrace("Adding modules globally...");
        var r = await interactionService.AddModulesGloballyAsync(deleteMissing: true, [.. m]).ConfigureAwait(false);
        _logger.LogDebug("{NumCommands} commands added globally ({Available Commands})", r.Count, string.Join(", ", r.Select(i => i.Name)));
    }

    private void LogMessageExecuted(SocketMessageCommand msg)
    {
        _logger.LogInformation("Received message: {MessageName}", msg.Data.Name);
        _logger.LogTrace("Message data: {MessageData}", JsonSerializer.Serialize(msg.Data, _debugSerializerOptions));
    }

    private static readonly JsonSerializerOptions _debugSerializerOptions = new(JsonSerializerDefaults.Web) { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };

    private void LogCommandExecuted(SocketSlashCommand command)
    {
        _logger.LogInformation("Received command: {CommandName}", command.Data.Name);
        _logger.LogTrace("Command data: {CommandData}", JsonSerializer.Serialize(command.Data, _debugSerializerOptions));
    }
}