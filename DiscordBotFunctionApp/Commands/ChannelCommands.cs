namespace DiscordBotFunctionApp.Commands;

using Discord;
using Discord.Commands;
using Discord.Interactions;

using System.Threading.Tasks;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0004:Make types declared in an executable internal", Justification = "Must be public to be discovered")]
public class ChannelCommands : ModuleBase<SocketCommandContext>
{
    internal SubscriptionManager subscriptionManager { get; set; }

    public async Task SubscribeAsync(ITextChannel targetChannel, string eventKey, uint teamNumber, CancellationToken cancellationToken)
    {
        await subscriptionManager.SaveSubscriptionAsync(new SubscriptionRequest(targetChannel.GuildId, targetChannel.Id, eventKey, teamNumber), cancellationToken).ConfigureAwait(false);

        await targetChannel.SendMessageAsync("Subscribed!", flags: MessageFlags.Ephemeral).ConfigureAwait(false);
    }
}