namespace DiscordBotFunctionApp.ChatBot;

using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

internal sealed class ChatBotInitializationService(HubConnectionFactory hubConnectionFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
