namespace ChatBot;

using Azure;
using Azure.AI.Projects;
using Azure.Data.Tables;

using Common.Discord;
using Common.Extensions;

using Discord;
using Discord.Net;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.ClientModel;

public static class ChatThreadResetter
{
    public const string ChatResetConfirmButtonId = "chat-reset-confirm";

    private static ILogger? logger;

    public static async Task HandleButtonClickAsync(IServiceProvider services, SocketMessageComponent button, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(button);

        logger ??= services.GetService<ILoggerFactory>()?.CreateLogger(typeof(ChatThreadResetter));
        using var scope = logger?.CreateMethodScope();

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
        string userIdString = userId.ToString();
        UserChatSynchronization userChatSynchronization = services.GetRequiredService<UserChatSynchronization>();
        await using IAsyncDisposable userLock = await userChatSynchronization.AcquireAsync(userIdString, cancellationToken).ConfigureAwait(false);
        TableClient table = services.GetRequiredKeyedService<TableClient>(ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads);
        NullableResponse<TableEntity> existingThread = await table.GetEntityIfExistsAsync<TableEntity>(
            userIdString,
            userIdString,
            ["AgentThreadId"],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string? threadId = existingThread.HasValue
            ? existingThread.Value?["AgentThreadId"]?.ToString()
            : null;
        threadId = ConversationThreadState.TryExtractThreadId(threadId);
        if (!string.IsNullOrWhiteSpace(threadId))
        {
            AIProjectClient? projectClient = services.GetService<AIProjectClient>();
            if (projectClient is not null)
            {
                try
                {
                    await projectClient.GetProjectOpenAIClient().GetProjectConversationsClient().DeleteConversationAsync(threadId, options: null).ConfigureAwait(false);
                }
                catch (ClientResultException e) when (e.Status == 404)
                {
                }
            }
        }

        try
        {
            await table.DeleteEntityAsync(partitionKey: userIdString, rowKey: userIdString, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
        }
    }
}
