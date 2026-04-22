namespace FunctionApp.DiscordInterop.CommandModules;

using Discord.Interactions;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

public class PingCommandModule(ILogger<PingCommandModule> logger) : CommandModuleBase(logger)
{
    [SlashCommand("ping", "Pings the bot to check if it's alive")]
    public async Task PingAsync()
    {
        using IDisposable? typing = await TryDeferAsync(ephemeral: true).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        await SendResponseAsync("Pong!", ephemeral: true).ConfigureAwait(false);
    }
}
