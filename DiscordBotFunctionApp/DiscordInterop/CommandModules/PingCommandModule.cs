namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord.Interactions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

public sealed class PingCommandModule(ILogger<PingCommandModule> logger) : CommandModuleBase(logger)
{
    [SlashCommand("ping", "Pings the bot to check if it's alive")]
    public async Task PingAsync()
    {
        if (!await TryDeferAsync().ConfigureAwait(false))
        {
            return;
        }

        await this.RespondAsync("Pong!", ephemeral: true).ConfigureAwait(false);
    }
}
