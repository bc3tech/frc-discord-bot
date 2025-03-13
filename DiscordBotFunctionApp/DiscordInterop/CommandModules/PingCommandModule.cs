namespace DiscordBotFunctionApp.DiscordInterop.CommandModules;

using Discord.Interactions;

using System.Threading.Tasks;

public sealed class PingCommandModule : CommandModuleBase
{
    [SlashCommand("ping", "Pings the bot to check if it's alive")]
    public async Task PingAsync() => await this.RespondAsync("Pong!", ephemeral: true).ConfigureAwait(false);
}
