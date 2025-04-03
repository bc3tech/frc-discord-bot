namespace FunctionApp.DiscordInterop.CommandModules;

using Discord;

using System;
using System.Threading.Tasks;

internal interface IHandleUserInteractions
{
    Task<bool> HandleInteractionAsync(IServiceProvider services, IComponentInteraction component, CancellationToken cancellationToken);
}
