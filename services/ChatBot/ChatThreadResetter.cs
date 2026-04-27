namespace ChatBot;

using BC3Technologies.DiscordGpt.Core;
using BC3Technologies.DiscordGpt.Copilot;

using Common.Discord;
using Common.Extensions;

using CopilotSdk.OpenTelemetry;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using GitHub.Copilot.SDK.Rpc;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ChatThreadResetter
{
    public const string ChatResetConfirmButtonId = "chat-reset-confirm";

    private static ILogger? logger;

    public static async Task HandleButtonClickAsync(IServiceProvider services, SocketMessageComponent button, CancellationToken cancellationToken = default)
    {

        logger ??= services.GetService<ILoggerFactory>()?.CreateLogger(typeof(ChatThreadResetter));
        using IDisposable? scope = logger?.CreateMethodScope();

        if (button.Data.CustomId is not ChatResetConfirmButtonId)
        {
            logger?.UnknownChatResetButtonClickedButtonId(button.Data.CustomId);
            return;
        }

        try
        {
            await button.UpdateAsync(p =>
            {
                p.Content = "-# Resetting chat history...";
                p.Embeds = null;
                p.Components = null;
            }, cancellationToken.ToRequestOptions()).ConfigureAwait(false);

            await ResetThreadForUserAsync(services, button.User.Id, cancellationToken).ConfigureAwait(false);

            await button.ModifyOriginalResponseAsync(p =>
            {
                p.Content = "Chat history erased! Who are you again? :stuck_out_tongue_winking_eye:";
                p.Embeds = null;
                p.Components = null;
            }, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        }
        catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
        {
            return;
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            logger?.ErrorDeletingChatThreadForUserUserNameUserId(e, button.User.GlobalName, button.User.Id);

            try
            {
                await button.ModifyOriginalResponseAsync(p =>
                {
                    p.Content = "Uh oh! I hit an error trying to do this. Feel free to try again or contact your admin and let them know about this!";
                    p.Embeds = null;
                    p.Components = null;
                }, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
            catch (HttpException e2) when (e2.DiscordCode is DiscordErrorCode.UnknownInteraction or DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
            {
            }
        }
    }

    public static async Task ResetThreadForUserAsync(IServiceProvider services, ulong userId, CancellationToken cancellationToken = default)
    {

        var conversationKey = ConversationKey.Dm(userId.ToString());

        await ClearConversationStateAsync(services, conversationKey, cancellationToken).ConfigureAwait(false);
    }

    public static async Task CleanupDeletedThreadAsync(IServiceProvider services, ulong threadId, CancellationToken cancellationToken = default)
    {

        var conversationKey = ConversationKey.Thread(threadId.ToString());

        await ClearConversationStateAsync(services, conversationKey, cancellationToken).ConfigureAwait(false);
    }

    private static async Task ClearConversationStateAsync(IServiceProvider services, ConversationKey conversationKey, CancellationToken cancellationToken)
    {
        IConversationStore conversationStore = services.GetRequiredService<IConversationStore>();
        await conversationStore.ClearAsync(conversationKey, cancellationToken).ConfigureAwait(false);

        var storageKey = conversationKey.ToStorageKey();
        await ClearCopilotSessionAsync(services, storageKey, cancellationToken).ConfigureAwait(false);

        IConversationTraceContextStore? traceStore = services.GetService<CopilotSdk.OpenTelemetry.IConversationTraceContextStore>();
        if (traceStore is not null)
        {
            await traceStore.RemoveAsync(storageKey, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task ClearCopilotSessionAsync(IServiceProvider services, string storageKey, CancellationToken cancellationToken)
    {
        IConversationSessionMap? sessionMap = services.GetService<IConversationSessionMap>();
        if (sessionMap is null)
        {
            return;
        }

        string? sessionId = await sessionMap.GetSessionIdAsync(storageKey, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            ISessionFsHandler? sessionFs = services.GetService<ISessionFsHandler>();
            if (sessionFs is not null)
            {
                await sessionFs.RmAsync(
                    new SessionFsRmParams
                    {
                        SessionId = sessionId,
                        Path = "/",
                        Recursive = true,
                    },
                    cancellationToken).ConfigureAwait(false);
            }
        }

        await sessionMap.RemoveAsync(storageKey, cancellationToken).ConfigureAwait(false);
    }
}
