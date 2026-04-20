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
    private const string AgentThreadIdColumnName = "AgentThreadId";

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
        await using IAsyncDisposable userLock = await userChatSynchronization.AcquireAsync(BuildDirectMessageLockKey(userIdString), cancellationToken).ConfigureAwait(false);
        TableClient table = services.GetRequiredKeyedService<TableClient>(ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads);
        NullableResponse<TableEntity> existingThread = await table.GetEntityIfExistsAsync<TableEntity>(
            userIdString,
            userIdString,
            [AgentThreadIdColumnName],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string? threadId = existingThread.HasValue
            ? ConversationThreadState.TryExtractThreadId(existingThread.Value?[AgentThreadIdColumnName]?.ToString())
            : null;
        await DeleteFoundryConversationIfPresentAsync(services, threadId, cancellationToken).ConfigureAwait(false);

        try
        {
            await table.DeleteEntityAsync(partitionKey: userIdString, rowKey: userIdString, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
        }
    }

    public static async Task CleanupDeletedThreadAsync(IServiceProvider services, ulong threadId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        string threadIdString = threadId.ToString();
        UserChatSynchronization userChatSynchronization = services.GetRequiredService<UserChatSynchronization>();
        await using IAsyncDisposable threadLock = await userChatSynchronization.AcquireAsync(BuildGuildThreadLockKey(threadIdString), cancellationToken).ConfigureAwait(false);

        TableClient table = services.GetRequiredKeyedService<TableClient>(ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads);
        HashSet<string> foundryConversationIds = [];
        List<TableEntity> entities = [];

        await foreach (TableEntity entity in table.QueryAsync<TableEntity>(
            filter: $"PartitionKey eq '{threadIdString}'",
            cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            entities.Add(entity);

            if (entity.TryGetValue(AgentThreadIdColumnName, out object? storedConversationState)
                && ConversationThreadState.TryExtractThreadId(storedConversationState.ToString()) is { Length: > 0 } foundryConversationId)
            {
                foundryConversationIds.Add(foundryConversationId);
            }
        }

        foreach (string foundryConversationId in foundryConversationIds)
        {
            await DeleteFoundryConversationIfPresentAsync(services, foundryConversationId, cancellationToken).ConfigureAwait(false);
        }

        foreach (TableEntity entity in entities)
        {
            try
            {
                await table.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, ETag.All, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
            }
        }
    }

    private static async Task DeleteFoundryConversationIfPresentAsync(IServiceProvider services, string? threadId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(threadId))
        {
            return;
        }

        AIProjectClient? projectClient = services.GetService<AIProjectClient>();
        if (projectClient is null)
        {
            return;
        }

        try
        {
            await projectClient.GetProjectOpenAIClient().GetProjectConversationsClient().DeleteConversationAsync(threadId, options: null).ConfigureAwait(false);
        }
        catch (ClientResultException e) when (e.Status == 404)
        {
        }
    }

    private static string BuildDirectMessageLockKey(string userId) => $"dm:{userId}";

    private static string BuildGuildThreadLockKey(string threadId) => $"guild-thread:{threadId}";
}
