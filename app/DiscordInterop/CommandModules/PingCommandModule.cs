namespace FunctionApp.DiscordInterop.CommandModules;

using Discord.Interactions;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

public sealed class PingCommandModule(ILogger<PingCommandModule> logger) : CommandModuleBase(logger)
{
    [SlashCommand("ping", "Pings the bot to check if it's alive")]
    public async Task PingAsync()
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        await RespondAsync("Pong!", ephemeral: true).ConfigureAwait(false);
    }
}
