namespace FunctionApp.DiscordInterop.CommandModules;

using Azure.Data.Tables;

using Common.Extensions;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using FunctionApp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

[Group("chat", "Manages private chats between me & you")]
public sealed class ChatCommandModule(ILogger<ChatCommandModule> logger) : CommandModuleBase(logger)
{
    private const string ChatResetConfirmButtonId = "chat-reset-confirm";

    private static readonly EmbedBuilder _embedBuilder = new();

    [SlashCommand("reset", "Resets your personal (DM) chat thread; makes me forget everything we've talked about!")]
    public async Task ResetThreadAsync()
    {
        using var typing = await TryDeferAsync().ConfigureAwait(false);
        if (typing is null)
        {
            return;
        }

        var ephemeral = this.Context.Channel is not IDMChannel;
        var embed = _embedBuilder
            .WithTitle("Are you sure?")
            .WithDescription($"This will reset your chat thread! I will forget everything we've talked about.{(ephemeral ? "If you've changed your mind, you can just ignore or dismiss this message :grin:" : string.Empty)}")
            .WithColor(Color.Red)
            .Build();

        var buttons = new ComponentBuilder().WithButton("Confirm", ChatResetConfirmButtonId, ButtonStyle.Danger);
        if (!ephemeral)
        {
            buttons.WithButton("Cancel", Constants.InteractionElements.CancelButtonDeleteMessage, ButtonStyle.Secondary);
        }

        await ModifyOriginalResponseAsync(p =>
        {
            p.Embed = embed;
            p.Components = buttons.Build();
        }).ConfigureAwait(false);
    }

    internal static async Task HandleButtonClickAsync(IServiceProvider services, SocketMessageComponent button)
    {
        var logger = services.GetService<ILogger<ChatCommandModule>>();
        using var scope = logger?.CreateMethodScope();
        if (button.Data.CustomId is ChatResetConfirmButtonId)
        {
            bool successful = false;
            try
            {
                var r = await DeleteThreadRecordForUserAsync(services, button.User.Id).ConfigureAwait(false);
                successful = !r.IsError;
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                logger?.ErrorDeletingThreadForUserNameUserId(e, button.User.GlobalName, button.User.Id);

                await button.RespondAsync("Uh oh! I hit an error trying to do this.Feel free to try again or contact your admin and let them know about this!").ConfigureAwait(false);
            }

            if (successful)
            {
                try
                {
                    await button.UpdateAsync(p =>
                    {
                        p.Content = "Chat history erased! Who are you again? :stuck_out_tongue_winking_eye:";
                        p.Embeds = null;
                        p.Components = null;
                    }).ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    await button.RespondAsync("Sorry, Discord only allows 3 seconds for you to respond to me, but I did reset your chat thread!", ephemeral: true).ConfigureAwait(false);
                }
                catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                {
                    logger?.DiscordErrorWhileTryingToModifyOriginalResponse(e);
                    Debug.Fail(e.Message);
                    throw;
                }
            }
        }
        else
        {
            services.GetService<ILogger<ChatCommandModule>>()?.UnknownButtonClickedButtonId(button.Data.CustomId);
        }
    }

    private static Task<Azure.Response> DeleteThreadRecordForUserAsync(IServiceProvider services, ulong userId, CancellationToken cancellationToken = default) => services.GetRequiredKeyedService<TableClient>(Constants.ServiceKeys.TableClient_UserChatAgentThreads).DeleteEntityAsync(partitionKey: userId.ToString(), rowKey: userId.ToString(), cancellationToken: cancellationToken);
}
