namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord.WebSocket;

using System;
using System.Threading.Tasks;

internal interface IHandleUserInteractions
{
    Task<bool> HandleInteractionAsync(IServiceProvider services, SocketMessageComponent component, CancellationToken cancellationToken);
}
