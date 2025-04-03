namespace FunctionApp.ChatBot;

using Discord;
using Discord.WebSocket;

using FunctionApp.DiscordInterop.CommandModules;

using Microsoft.Extensions.Hosting;

using System;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ChatBotInitializationService(IServiceProvider services, IDiscordClient discordClient) : IHostedService
{
    private readonly DiscordSocketClient client = discordClient as DiscordSocketClient ?? throw new ArgumentException(nameof(discordClient));

    public Task StartAsync(CancellationToken cancellationToken)
    {
        client.ButtonExecuted += async (button) =>
        {
            if (button.Data.CustomId is
                "chat-reset-confirm" or
                "chat-reset-cancel")
            {
                await ChatCommandModule.HandleButtonClickAsync(services, button);
            }
        };

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
