namespace DiscordBotFunctionApp;

using Common.Extensions;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

using System;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal class DiscordInitializationService(DiscordSocketClient client, IConfiguration appConfig, ILoggerFactory logFactory, IServiceProvider services) : IHostedService
{
    private readonly ILogger _logger = logFactory.CreateLogger<DiscordInitializationService>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.CreateMethodScope();
        client.Log += (msg) =>
        {
            var logger = logFactory.CreateLogger<DiscordSocketClient>();
            logger.Log(msg.Severity.ToLogLevel(), msg.Exception, msg.Message);
            return Task.CompletedTask;
        };

        var tsc = new TaskCompletionSource();

        _logger.LogDebug("Logging in to Discord...");
        await client.LoginAsync(TokenType.Bot, appConfig[Constants.Configuration.Discord.Token], validateToken: true)
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false);

        client.LoggedIn += () =>
        {
            _logger.LogInformation("Discord client logged in");
            tsc.SetResult();
            return Task.CompletedTask;
        };

        _logger.LogDebug("Starting Discord client...");
        await client.StartAsync()
            .WithCancellation(cancellationToken)
            .ConfigureAwait(false);
        _logger.LogInformation("Discord client started");

        tsc = new();

        client.Ready += () =>
        {
            tsc.SetResult();
            return Task.CompletedTask;
        };

        await tsc.Task.WithCancellation(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Discord client ready");
        _logger.LogDebug("Currently active guilds:\n{ActiveGuilds}", string.Join($"\n", client.Guilds.Select(g => $"- {g.Name}")));

        await InstallCommandsAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
        await client.DisposeAsync();
    }

    public async Task InstallCommandsAsync(CancellationToken cancellationToken)
    {
        using var scope = _logger.CreateMethodScope();
        // Hook the MessageReceived event into our command handler
        client.SlashCommandExecuted += cmd => HandleCommandAsync(cmd, cancellationToken);
        client.MessageCommandExecuted += msg => HandleMessageAsync(msg, cancellationToken);

        var subscribeCommand = new SlashCommandBuilder() { Name = "subscription-subscribe-channel", Description = "Subscribe to notifications" }
            .WithContextTypes(InteractionContextType.Guild)
            .WithIntegrationTypes(ApplicationIntegrationType.GuildInstall)
            .AddOption(new()
            {
                Name = "team",
                Description = "The team number to subscribe to",
                Type = ApplicationCommandOptionType.Integer,
                IsRequired = false
            })
            .AddOption(new()
            {
                Name = "event",
                Description = "The event key to subscribe to",
                Type = ApplicationCommandOptionType.String,
                IsRequired = false,
            });

        //foreach (var c in await client.BulkOverwriteGlobalApplicationCommandsAsync([subscribeCommand.Build()], new RequestOptions { CancelToken = cancellationToken }))
        //{
        //    _logger.LogDebug("Created global command: {CommandName}", c.Name);
        //}
    }

    private Task HandleMessageAsync(SocketMessageCommand msg, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received message: {MessageName}", msg.Data.Name);
        _logger.LogTrace("Message data: {MessageData}", JsonSerializer.Serialize(msg.Data, new JsonSerializerOptions(JsonSerializerDefaults.Web) { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve }));

        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketSlashCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received command: {CommandName}", command.Data.Name);
        _logger.LogTrace("Command data: {CommandData}", JsonSerializer.Serialize(command.Data, new JsonSerializerOptions(JsonSerializerDefaults.Web) { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve }));

        await command.RespondAsync($"got it", ephemeral: true, options: CreateCancelRequestOptions(cancellationToken));
    }

    private static RequestOptions CreateCancelRequestOptions(CancellationToken cancellationToken) => new() { CancelToken = cancellationToken };
}