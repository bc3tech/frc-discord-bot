namespace FunctionApp.DiscordInterop.CommandModules;

using ChatBot;

using Discord;
using Discord.Interactions;

using FunctionApp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

[Group("chat", "Manages private chats between me & you")]
public sealed class ChatCommandModule(IServiceProvider services) : CommandModuleBase(services.GetRequiredService<ILogger<ChatCommandModule>>())
{
    private static readonly EmbedBuilder _embedBuilder = new();

    [SlashCommand("reset", "Resets your personal (DM) chat thread; makes me forget everything we've talked about!")]
    public async Task ResetThreadAsync()
    {
        using IDisposable? typing = await TryDeferAsync(ephemeral: true).ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        var ephemeral = Context.Channel is not IDMChannel;
        Embed embed = _embedBuilder
            .WithTitle("Are you sure?")
            .WithDescription($"This will reset your chat thread! I will forget everything we've talked about.{(ephemeral ? "If you've changed your mind, you can just ignore or dismiss this message :grin:" : string.Empty)}")
            .WithColor(Color.Red)
            .Build();

        ComponentBuilder buttons = new ComponentBuilder().WithButton("Confirm", ChatThreadResetter.ChatResetConfirmButtonId, ButtonStyle.Danger);
        if (!ephemeral)
        {
            buttons.WithButton("Cancel", Constants.InteractionElements.CancelButtonDeleteMessage, ButtonStyle.Secondary);
        }

        await UpdateOriginalResponseAsync(p =>
        {
            p.Flags = MessageFlags.Ephemeral;
            p.Embed = embed;
            p.Components = buttons.Build();
        }).ConfigureAwait(false);
    }

    [SlashCommand("new", "Starts a new personal (DM) chat thread and forgets the current one.")]
    public Task NewThreadAsync() => ResetThreadAsync();
}
