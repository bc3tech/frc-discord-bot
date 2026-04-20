namespace ChatBot;

using AgentFramework.OpenTelemetry;

using Azure.Data.Tables;

using ChatBot.Agents;
using ChatBot.Copilot;

using Common.Discord;
using Common.Extensions;

using Discord;
using Discord.WebSocket;

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using static Common.Discord.Helpers;

public sealed partial class MessageHandler(
    [FromKeyedServices(ChatBotConstants.ServiceKeys.TableClient_UserChatAgentThreads)] TableClient userThreadMappings,
    UserChatSynchronization userChatSynchronization,
    TimeProvider time,
    Meter meter,
    ILogger<MessageHandler> logger,
    IServiceProvider services)
{
    private readonly Conversation conversation = services.GetRequiredService<Conversation>();
    private readonly PromptCatalog _promptCatalog = services.GetRequiredService<PromptCatalog>();

    private const string DisclaimerText = "-# AI generated response; may have mistakes.";
    private const string PartialResponseRemovedText = "-# Partial response removed because Discord delivery failed.";
    private static readonly TimeSpan TypingIndicatorRefreshInterval = TimeSpan.FromSeconds(3);
    private static readonly string ProgressMessagesResourceName = $"{typeof(MessageHandler).Namespace}.progress_messages.txt";
    private const string AgentThreadIdColumnName = "AgentThreadId";
    private const string CanonicalRowKeyColumnName = "CanonicalRowKey";
    private const string TraceRootContextColumnName = "TraceRootContext";
    private const string MentionActivatedColumnName = "MentionActivated";

    public Task HandleUserMessageAsync(IUserMessage msg, CancellationToken cancellationToken = default)
        => HandleMessageCoreAsync(msg, CreateDirectMessageConversationContext(msg), botUserId: null, cancellationToken);

    public async Task<bool> TryHandleGuildMessageAsync(SocketUserMessage msg, ulong botUserId, IReadOnlyCollection<ulong> botRoleIds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(msg);

        bool mentionedBot = msg.MentionedUsers.Any(u => u.Id == botUserId) || msg.MentionedRoles.Select(i => i.Id).Intersect(botRoleIds).Any();
        if (!mentionedBot && msg.MentionedUsers.Count > 0)
        {
            return false;
        }

        ChatConversationContext? context = await TryResolveGuildConversationContextAsync(msg, mentionedBot, cancellationToken).ConfigureAwait(false);
        if (context is null)
        {
            return false;
        }

        await HandleMessageCoreAsync(msg, context, botUserId, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task HandleMessageCoreAsync(IUserMessage msg, ChatConversationContext context, ulong? botUserId, CancellationToken cancellationToken)
    {
        string prompt = BuildPromptContent(msg, botUserId);
        await using IAsyncDisposable userLock = await userChatSynchronization.AcquireAsync(context.Scope.LockKey, cancellationToken).ConfigureAwait(false);
        var interactionStartTime = time.GetUtcNow();
        IMessageChannel responseChannel = context.Scope.ResponseChannel;
        using CancellationTokenSource sorryForTheDelayCanceler = new();
        using CancellationTokenSource typingIndicatorCanceler = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        IUserMessage? transientStatusMessage = null;
        Task? progressTask = null;
        Task? typingIndicatorTask = null;
        List<IUserMessage> streamedMessages = [];
        string? conversationId = null;

        async Task StopTypingIndicatorAsync()
        {
            if (!typingIndicatorCanceler.IsCancellationRequested)
            {
                await typingIndicatorCanceler.CancelAsync().ConfigureAwait(false);
            }

            if (typingIndicatorTask is null)
            {
                return;
            }

            try
            {
#pragma warning disable VSTHRD003 // Task lifecycle is managed within this context with proper cancellation
                await typingIndicatorTask.ConfigureAwait(false);
#pragma warning restore VSTHRD003
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
            {
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                logger.FailedWhileRefreshingTheChatbotTypingIndicator(e);
            }
        }

        try
        {
            typingIndicatorTask = KeepTypingIndicatorAliveAsync(responseChannel, typingIndicatorCanceler, logger);
            IUserMessage? firstMessage = null;
            HashSet<string> responseMessageAliases = [];
            string serializedAuthor = JsonSerializer.Serialize(msg.Author);

            if (string.IsNullOrWhiteSpace(prompt))
            {
                await SendEmptyPromptResponseAsync(msg, context, cancellationToken).ConfigureAwait(false);
                return;
            }

            async Task<IUserMessage?> TrySendResponseMessageAsync(string content, CancellationToken ct)
            {
                return await ExecuteDiscordWriteWithRetryAsync(
                    "send response message",
                    async requestOptions =>
                    {
                        if (firstMessage is null && context.Scope.InitialReplyMessageId is ulong replyToMessageId)
                        {
                            return await responseChannel.SendMessageAsync(
                                content,
                                messageReference: new MessageReference(replyToMessageId),
                                flags: MessageFlags.SuppressEmbeds,
                                options: requestOptions).ConfigureAwait(false);
                        }

#pragma warning disable IDE0046 // Convert to conditional expression; ends up cascading up to the one above this and the expression is just ridiculous
                        if (firstMessage is null)
                        {
                            return await responseChannel.SendMessageAsync(content, flags: MessageFlags.SuppressEmbeds, options: requestOptions).ConfigureAwait(false);
                        }
#pragma warning restore IDE0046 // Convert to conditional expression

                        return await firstMessage.ReplyAsync(content, flags: MessageFlags.SuppressEmbeds, options: requestOptions).ConfigureAwait(false);
                    },
                    required: false, time, logger,
                    cancellationToken: ct).ConfigureAwait(false);
            }

            Task<bool> TryModifyMessageAsync(IUserMessage targetMessage, string content, CancellationToken ct)
                => targetMessage.TryModifyDiscordMessageAsync(content, time, logger, ct);

            async Task TryUpsertTransientStatusMessageAsync(string content, CancellationToken ct)
            {
                await StopProgressRotationAsync().ConfigureAwait(false);
                if (transientStatusMessage is null)
                {
                    transientStatusMessage = await TrySendResponseMessageAsync(content, ct).ConfigureAwait(false);
                    return;
                }

                if (string.Equals(transientStatusMessage.Content, content, StringComparison.Ordinal))
                {
                    return;
                }

                await TryModifyMessageAsync(transientStatusMessage, content, ct).ConfigureAwait(false);
            }

            async Task StopProgressRotationAsync()
            {
                if (!sorryForTheDelayCanceler.IsCancellationRequested)
                {
                    await sorryForTheDelayCanceler.CancelAsync().ConfigureAwait(false);
                }

                if (progressTask is not null)
                {
                    try
                    {
#pragma warning disable VSTHRD003 // Task lifecycle is managed within this context with proper cancellation
                        await progressTask.ConfigureAwait(false);
#pragma warning restore VSTHRD003
                    }
                    catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
                    {
                    }
                    catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                    {
                        logger.FailedWhileUpdatingTheChatbotProgressIndicator(e);
                    }
                }
            }

            void CancelProgressRotation()
            {
                if (!sorryForTheDelayCanceler.IsCancellationRequested)
                {
                    _ = sorryForTheDelayCanceler.CancelAsync();
                }
            }

            if (context.Style.ShowProgressMessages)
            {
                var progressMessages = _progressMessages.Value;
                progressTask = RunProgressMessagesAsync();

                async Task RunProgressMessagesAsync()
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), time, sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                        int progressIndex = GetRandomProgressMessageIndex(progressMessages);
                        transientStatusMessage = await TrySendResponseMessageAsync(progressMessages[progressIndex], sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                        if (transientStatusMessage is null)
                        {
                            return;
                        }

                        while (!sorryForTheDelayCanceler.IsCancellationRequested)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(4), time, sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                            if (transientStatusMessage is null)
                            {
                                return;
                            }

                            progressIndex = GetRandomProgressMessageIndex(progressMessages, progressIndex);
                            await TryModifyMessageAsync(
                                transientStatusMessage,
                                progressMessages[progressIndex],
                                sorryForTheDelayCanceler.Token).ConfigureAwait(false);
                        }
                    }
                    catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                    {
                        logger.FailedWhileUpdatingTheChatbotProgressIndicator(e);
                    }
                }
            }

            static async Task KeepTypingIndicatorAliveAsync(IMessageChannel channel, CancellationTokenSource cts, ILogger logger)
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        await channel.TriggerTypingAsync(cts.Token.ToRequestOptions()).ConfigureAwait(false);
                    }
                    catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
                    {
                        return;
                    }
                    catch (Exception e)
                    {
                        logger.FailedWhileRefreshingTheChatbotTypingIndicator(e);
                    }

                    try
                    {
                        await Task.Delay(TypingIndicatorRefreshInterval, cts.Token).ConfigureAwait(false);
                    }
                    catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
                    {
                        return;
                    }
                }
            }

            ChatConversationRuntimeState conversationRuntimeState = await GetOrCreateConversationStateAsync(context.Scope, serializedAuthor, cancellationToken).ConfigureAwait(false);
            conversationId = conversationRuntimeState.ChatState.CopilotSessionId.UnlessNullOrWhitespaceThen(conversationRuntimeState.ChatState.FoundryThreadId);

            StringBuilder currentResponseText = new();
            bool renderedVisibleOutput = false;
            string currentCommittedText = string.Empty;
            bool startNewResponseGroupOnNextText = false;
            List<IUserMessage> currentResponseMessages = [];

            await foreach (var response in conversation.PostUserMessageStreamingAsync(
                conversationRuntimeState.ChatState,
                prompt,
                BuildUserContextMessages(msg, context.Style),
                PersistConversationStateAsync,
                traceRootContext: conversationRuntimeState.RootParentContext,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                bool responseCompleted = response.FinishReason is not null;
                bool startsNewTopLevelResponse = false;
                string streamedText = GetStreamedText(response);
                bool isUserStatusMessage = Conversation.TryExtractUserStatusMessage(streamedText, out string? userStatusMessage);
                string sanitized = SanitizeResponseText(isUserStatusMessage
                    ? FormatUserStatusMessage(userStatusMessage!)
                    : streamedText);
                if (isUserStatusMessage)
                {
                    if (!renderedVisibleOutput && !string.IsNullOrWhiteSpace(sanitized))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await TryUpsertTransientStatusMessageAsync(sanitized, cancellationToken).ConfigureAwait(false);
                    }

                    startNewResponseGroupOnNextText |= responseCompleted;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(sanitized))
                {
                    if (startNewResponseGroupOnNextText || startsNewTopLevelResponse)
                    {
                        currentResponseText.Clear();
                        currentCommittedText = string.Empty;
                        currentResponseMessages = [];
                        startNewResponseGroupOnNextText = false;
                    }

                    currentResponseText.Append(sanitized);
                }

                if (string.IsNullOrWhiteSpace(sanitized) && !responseCompleted)
                {
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();

                string nextCommittedText = GetCommittedStreamingText(currentResponseText, flushUnterminatedTail: responseCompleted, isFinal: false);
                if (string.IsNullOrEmpty(nextCommittedText) || string.Equals(nextCommittedText, currentCommittedText, StringComparison.Ordinal))
                {
                    startNewResponseGroupOnNextText |= responseCompleted;
                    continue;
                }

                if (!renderedVisibleOutput)
                {
                    CancelProgressRotation();
                    logger.RenderingFirstVisibleChatbotOutput(nextCommittedText.Length);
                }

                if (await TryRenderCommittedTextAsync(currentResponseMessages, nextCommittedText, includeDisclaimer: false, cancellationToken).ConfigureAwait(false))
                {
                    currentCommittedText = nextCommittedText;
                    renderedVisibleOutput = true;
                }

                startNewResponseGroupOnNextText |= responseCompleted;
            }

            CancelProgressRotation();
            string finalCommittedText = GetCommittedStreamingText(currentResponseText, flushUnterminatedTail: true, isFinal: true);
            if (string.IsNullOrWhiteSpace(finalCommittedText))
            {
                throw new InvalidOperationException("Copilot chat runtime completed without returning any message content.");
            }

            CopilotChatState updatedChatState = conversationRuntimeState.ChatState with
            {
                Transcript = CopilotTranscriptWindow.AppendTurn(conversationRuntimeState.ChatState.Transcript, prompt, finalCommittedText),
            };
            conversationRuntimeState = conversationRuntimeState with
            {
                ChatState = updatedChatState,
            };
            logger.PersistingCompletedChatbotTurn(finalCommittedText.Length, updatedChatState.Transcript.Count);
            await PersistConversationStateCoreAsync(
                context.Scope,
                serializedAuthor,
                updatedChatState,
                conversationRuntimeState.TraceRootContext,
                cancellationToken).ConfigureAwait(false);

            logger.RenderingFinalChatbotOutput(finalCommittedText.Length);
            await RenderCommittedTextAsync(finalCommittedText, includeDisclaimer: true, cancellationToken).ConfigureAwait(false);

            async Task RenderCommittedTextAsync(string content, bool includeDisclaimer, CancellationToken ct)
            {
                bool success = await TryRenderCommittedTextAsync(currentResponseMessages, content, includeDisclaimer, ct).ConfigureAwait(false);
                if (!success)
                {
                    throw new DiscordDeliveryException("Discord refused the final streamed response update.");
                }
            }

            async Task<bool> TryRenderCommittedTextAsync(List<IUserMessage> targetMessages, string content, bool includeDisclaimer, CancellationToken ct)
            {
                string renderedContent = includeDisclaimer
                    ? AppendDisclaimer(content)
                    : content;
                IReadOnlyList<string> chunks = [.. ChunkForDiscord(renderedContent, context.Style, msg.Author.Mention)];

                for (int i = 0; i < chunks.Count; i++)
                {
                    if (i < targetMessages.Count)
                    {
                        if (!string.Equals(targetMessages[i].Content, chunks[i], StringComparison.Ordinal))
                        {
                            bool updated = await TryModifyMessageAsync(targetMessages[i], chunks[i], ct).ConfigureAwait(false);
                            if (!updated)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        IUserMessage? createdMessage = await TrySendResponseMessageAsync(chunks[i], ct).ConfigureAwait(false);
                        if (createdMessage is null)
                        {
                            return false;
                        }

                        streamedMessages.Add(createdMessage);
                        targetMessages.Add(createdMessage);
                        firstMessage ??= createdMessage;
                        if (context.Scope.PersistReplyAliases && responseMessageAliases.Add(createdMessage.Id.ToString()))
                        {
                            await PersistReplyAliasAsync(context.Scope, createdMessage.Id, cancellationToken).ConfigureAwait(false);
                        }

                        if (streamedMessages.Count is 1)
                        {
                            meter.LogMetric("AgentFirstMessageDelaySec", (time.GetUtcNow() - interactionStartTime).TotalSeconds);
                        }
                    }
                }

                return true;
            }

            ValueTask PersistConversationStateAsync(CopilotChatState updatedConversationState, CancellationToken ct)
            {
                conversationRuntimeState = conversationRuntimeState with
                {
                    ChatState = updatedConversationState,
                };

                return PersistConversationStateCoreAsync(
                    context.Scope,
                    serializedAuthor,
                    conversationRuntimeState.ChatState,
                    conversationRuntimeState.TraceRootContext,
                    ct);
            }
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            await CleanupStreamedMessagesAsync(streamedMessages, CancellationToken.None).ConfigureAwait(false);
            logger.ErrorRespondingToDMFromUserName(e, msg.Author.GlobalName.UnlessNullOrWhitespaceThen(msg.Author.Username));
            string description = e is DiscordDeliveryException
                ? $"I had trouble delivering the full response to Discord, so I cleared the partial output to avoid leaving you with a half response.\n\nConversation ID: `{conversationId ?? "unavailable"}`"
                : $"Oh no! I hit an error when trying to answer you, sorry! Try again or let your admin know about this so they can investigate.\n\nConversation ID: `{conversationId ?? "unavailable"}`"
#if DEBUG
                + $"\n\n```\n{e}\n```"
#endif
                ;
            await responseChannel.SendFailureEmbedAsync(description, time, logger, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await StopTypingIndicatorAsync().ConfigureAwait(false);
            if (transientStatusMessage is not null)
            {
                try
                {
                    await transientStatusMessage.TryDeleteDiscordMessageAsync("delete transient chatbot status message", time, logger, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
                {
                    logger.FailedToRemoveTransientThinkingMessage(e);
                }
            }
        }

        meter.LogMetric("InteractionTimeSec", (time.GetUtcNow() - interactionStartTime).TotalSeconds);
    }

    [GeneratedRegex(@"\w*【[^】]+】\w*", RegexOptions.Compiled)]
    private static partial Regex AnnotationFinder();

    [GeneratedRegex(@"<@!?(?<id>\d+)>", RegexOptions.Compiled)]
    private static partial Regex UserMentionFinder();

    private static string SanitizeResponseText(string content)
    {
        string sanitized = content;
        foreach (Match annotation in AnnotationFinder().Matches(sanitized))
        {
            sanitized = sanitized.Replace(annotation.Value, string.Empty);
        }

        return sanitized;
    }

    private static string GetStreamedText(AgentResponseUpdate update)
    {
        StringBuilder? text = null;
        foreach (AIContent content in update.Contents)
        {
            switch (content)
            {
                case TextContent { Text: { Length: > 0 } textValue }:
                    text ??= new();
                    text.Append(textValue);
                    break;
                case TextReasoningContent { Text: { Length: > 0 } reasoningValue }:
                    text ??= new();
                    text.Append(reasoningValue);
                    break;
            }
        }

        return text?.ToString() ?? string.Empty;
    }

    private static string GetCommittedStreamingText(StringBuilder streamedText, bool flushUnterminatedTail, bool isFinal)
    {
        string content = streamedText.ToString();
        if (isFinal)
        {
            return content.TrimEnd('\r', '\n');
        }

        int lastNewLineIndex = content.LastIndexOf('\n');
        if (lastNewLineIndex < 0)
        {
            return flushUnterminatedTail ? content : string.Empty;
        }

        string committedLines = content[..(lastNewLineIndex + 1)];
        return !flushUnterminatedTail || lastNewLineIndex == content.Length - 1 ? committedLines : content;
    }

    private static string AppendDisclaimer(string content)
        => $"{content}\n\n{DisclaimerText}";

    private static string FormatUserStatusMessage(string content)
    {
        StringBuilder formatted = new();
        foreach (ReadOnlySpan<char> rawLine in content.AsSpan().EnumerateLines())
        {
            ReadOnlySpan<char> line = rawLine.Trim();
            if (line.IsEmpty)
            {
                continue;
            }

            if (formatted.Length > 0)
            {
                formatted.Append('\n');
            }

            formatted.Append("-# ");
            formatted.Append(line);
        }

        return formatted.Length > 0
            ? formatted.ToString()
            : "-# Working on it...";
    }

    private static bool StartsWithDiscordStatusLine(string content)
        => content.TrimStart().StartsWith("-# ", StringComparison.Ordinal);

    private static IEnumerable<string> ChunkForDiscord(string content, ChatResponseStyle style, string userMention)
    {
        string prefix = style.MentionUserInFirstResponse && !StartsWithDiscordStatusLine(content)
            ? $"{userMention} "
            : string.Empty;

        if (content.Length + prefix.Length <= 2000)
        {
            yield return prefix + content;
            yield break;
        }

        int firstChunkLength = Math.Max(1, 2000 - prefix.Length);
        yield return prefix + content[..firstChunkLength];

        for (int start = firstChunkLength; start < content.Length; start += 2000)
        {
            int length = Math.Min(2000, content.Length - start);
            yield return content.Substring(start, length);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Already using a conditional, don't want a multi-layered one")]
    private static string BuildPromptContent(IUserMessage msg, ulong? botUserId)
    {
        string cleaned = msg.CleanContent.Trim();
        if (msg.Channel is IDMChannel || botUserId is null)
        {
            return cleaned;
        }

        string withoutBotMention = UserMentionFinder().Replace(
            msg.Content,
            match => ulong.TryParse(match.Groups["id"].Value, out ulong mentionedUserId) && mentionedUserId == botUserId.Value
                ? string.Empty
                : match.Value).Trim();

        if (string.Equals(withoutBotMention, msg.Content.Trim(), StringComparison.Ordinal))
        {
            return cleaned;
        }

        return string.IsNullOrWhiteSpace(withoutBotMention)
            ? cleaned
            : withoutBotMention;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "EA0009:Use 'System.MemoryExtensions.Split' for improved performance", Justification = "Small, one time load")]
    private readonly Lazy<IReadOnlyList<string>> _progressMessages = new(() =>
    {
        try
        {
            using Stream? resourceStream = typeof(MessageHandler).Assembly.GetManifestResourceStream(ProgressMessagesResourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{ProgressMessagesResourceName}' was not found.");

            using StreamReader reader = new(resourceStream);
            string[] loadedMessages = reader.ReadToEnd()
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            Debug.Assert(loadedMessages.Length is not 0);
            return [.. loadedMessages];
        }
        catch (Exception e)
        {
            logger.FailedToLoadChatbotProgressMessages(e, ProgressMessagesResourceName);
            return [];
        }
    });

    private static int GetRandomProgressMessageIndex(IReadOnlyCollection<string> progressMessages, int excludedIndex = -1)
    {
        if (progressMessages.Count <= 1)
        {
            return 0;
        }

        int index = Random.Shared.Next(progressMessages.Count);
        if (index == excludedIndex)
        {
            index = (index + 1 + Random.Shared.Next(progressMessages.Count - 1)) % progressMessages.Count;
        }

        return index;
    }

    private static Task<IUserMessage> SendEmptyPromptResponseAsync(IUserMessage msg, ChatConversationContext context, CancellationToken cancellationToken)
    {
        string response = context.Style.IsDm
            ? "Hey! Send me a question anytime and I will do my best to help."
            : context.Style.MentionUserInFirstResponse
                ? $"{msg.Author.Mention} hey! Ask me a question with the mention and I will jump in."
                : "Hey! Send me a message anytime in here and I will do my best to help.";

        return context.Scope.InitialReplyMessageId is ulong replyToMessageId
            ? context.Scope.ResponseChannel.SendMessageAsync(
                response,
                messageReference: new MessageReference(replyToMessageId),
                options: cancellationToken.ToRequestOptions())
            : context.Scope.ResponseChannel.SendMessageAsync(response, options: cancellationToken.ToRequestOptions());
    }

    private ChatMessage[] BuildUserContextMessages(IUserMessage msg, ChatResponseStyle style)
    {
        string displayName = msg.Author.GlobalName.UnlessNullOrWhitespaceThen(msg.Author.Username);
        return
        [
            new(ChatRole.System, _promptCatalog.Format(
                _promptCatalog.UserContextMessage,
                ("USER_DISPLAY_NAME", displayName),
                ("DISCORD_USERNAME", msg.Author.Username),
                ("DISCORD_USER_ID", msg.Author.Id.ToString()),
                ("MESSAGE_SOURCE", style.IsDm ? "Direct Message" : "Server Mention"))),
        ];
    }

    private static ChatConversationContext CreateDirectMessageConversationContext(IUserMessage msg)
    {
        string userId = msg.Author.Id.ToString();
        return new(
            new(IsDm: true, ShowProgressMessages: true, MentionUserInFirstResponse: false),
            new(
                LockKey: $"dm:{userId}",
                PartitionKey: userId,
                RowKey: userId,
                ResponseChannel: msg.Channel,
                InitialReplyMessageId: null,
                MentionActivated: false,
                PersistReplyAliases: false,
                PersistThreadChannelAlias: false));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Convert to conditional expression", Justification = "Easier to read this way")]
    private async Task<ChatConversationContext?> TryResolveGuildConversationContextAsync(SocketUserMessage msg, bool mentionedBot, CancellationToken cancellationToken)
    {
        if (await TryGetExistingThreadConversationContextAsync(msg, requireMentionActivation: !mentionedBot, activateMention: mentionedBot, cancellationToken: cancellationToken).ConfigureAwait(false) is { } threadContext)
        {
            return threadContext;
        }

        if (await TryGetExistingReplyConversationContextAsync(msg, requireMentionActivation: !mentionedBot, activateMention: mentionedBot, cancellationToken: cancellationToken).ConfigureAwait(false) is { } replyContext)
        {
            return replyContext;
        }

        if (!mentionedBot)
        {
            return null;
        }

        if (await TryCreateMentionThreadConversationContextAsync(msg, cancellationToken).ConfigureAwait(false) is { } newThreadContext)
        {
            return newThreadContext;
        }

        return CreateMentionReplyConversationContext(msg);
    }

    private async Task<ChatConversationContext?> TryGetExistingThreadConversationContextAsync(SocketUserMessage msg, bool requireMentionActivation, bool activateMention, CancellationToken cancellationToken)
    {
        if (msg.Channel is not IThreadChannel)
        {
            return null;
        }

        string threadId = msg.Channel.Id.ToString();
        TableEntity? threadAlias = await GetConversationMappingAsync(threadId, threadId, cancellationToken).ConfigureAwait(false);
        if (threadAlias is null)
        {
            return null;
        }

        string canonicalRowKey = GetCanonicalRowKey(threadAlias, threadId);
        TableEntity? canonicalConversation = string.Equals(canonicalRowKey, threadId, StringComparison.Ordinal)
            ? threadAlias
            : await GetConversationMappingAsync(threadId, canonicalRowKey, cancellationToken).ConfigureAwait(false);
        if (!HasConversationState(canonicalConversation))
        {
            return null;
        }

        bool mentionActivated = IsMentionActivated(threadAlias) || canonicalConversation is not null && IsMentionActivated(canonicalConversation);
        if (activateMention)
        {
            mentionActivated = true;
        }

        if (requireMentionActivation && !mentionActivated)
        {
            return null;
        }

        return new(
            new(IsDm: false, ShowProgressMessages: true, MentionUserInFirstResponse: false),
            new(
                LockKey: $"guild-thread:{threadId}",
                PartitionKey: threadId,
                RowKey: canonicalRowKey,
                ResponseChannel: msg.Channel,
                InitialReplyMessageId: null,
                MentionActivated: mentionActivated,
                PersistReplyAliases: false,
                PersistThreadChannelAlias: false));
    }

    private async Task<ChatConversationContext?> TryGetExistingReplyConversationContextAsync(SocketUserMessage msg, bool requireMentionActivation, bool activateMention, CancellationToken cancellationToken)
    {
        if (msg.Reference?.MessageId.IsSpecified is not true || msg.Reference.MessageId.Value is 0)
        {
            return null;
        }

        string channelId = msg.Channel.Id.ToString();
        string referencedMessageId = msg.Reference.MessageId.Value.ToString();
        TableEntity? referencedConversation = await GetConversationMappingAsync(channelId, referencedMessageId, cancellationToken).ConfigureAwait(false);
        if (referencedConversation is null)
        {
            return null;
        }

        string canonicalRowKey = GetCanonicalRowKey(referencedConversation, referencedMessageId);
        TableEntity? canonicalConversation = string.Equals(canonicalRowKey, referencedMessageId, StringComparison.Ordinal)
            ? referencedConversation
            : await GetConversationMappingAsync(channelId, canonicalRowKey, cancellationToken).ConfigureAwait(false);
        if (!HasConversationState(canonicalConversation))
        {
            return null;
        }

        bool mentionActivated = IsMentionActivated(referencedConversation) || canonicalConversation is not null && IsMentionActivated(canonicalConversation);
        if (activateMention)
        {
            mentionActivated = true;
        }

        if (requireMentionActivation && !mentionActivated)
        {
            return null;
        }

        return new(
            new(IsDm: false, ShowProgressMessages: true, MentionUserInFirstResponse: false),
            new(
                LockKey: $"guild-reply:{channelId}:{canonicalRowKey}",
                PartitionKey: channelId,
                RowKey: canonicalRowKey,
                ResponseChannel: msg.Channel,
                InitialReplyMessageId: msg.Id,
                MentionActivated: mentionActivated,
                PersistReplyAliases: true,
                PersistThreadChannelAlias: false));
    }

    private async Task<ChatConversationContext?> TryCreateMentionThreadConversationContextAsync(SocketUserMessage msg, CancellationToken cancellationToken)
    {
        if (msg.Channel is not ITextChannel threadableChannel)
        {
            return null;
        }

        IThreadChannel? createdThread = await ExecuteDiscordWriteWithRetryAsync(
            "create chatbot mention thread",
            requestOptions => threadableChannel.CreateThreadAsync(
                BuildMentionThreadName(msg),
                autoArchiveDuration: ThreadArchiveDuration.OneDay,
                message: msg,
                options: requestOptions),
            required: false,
            time,
            logger,
            cancellationToken).ConfigureAwait(false);

        if (createdThread is null)
        {
            return null;
        }

        string threadId = createdThread.Id.ToString();
        return new(
            new(IsDm: false, ShowProgressMessages: true, MentionUserInFirstResponse: true),
            new(
                LockKey: $"guild-thread:{threadId}",
                PartitionKey: threadId,
                RowKey: threadId,
                ResponseChannel: createdThread,
                InitialReplyMessageId: null,
                MentionActivated: true,
                PersistReplyAliases: false,
                PersistThreadChannelAlias: false));
    }

    private static ChatConversationContext CreateMentionReplyConversationContext(SocketUserMessage msg)
    {
        string channelId = msg.Channel.Id.ToString();
        string messageId = msg.Id.ToString();
        return new(
            new(IsDm: false, ShowProgressMessages: true, MentionUserInFirstResponse: true),
            new(
                LockKey: $"guild-reply:{channelId}:{messageId}",
                PartitionKey: channelId,
                RowKey: messageId,
                ResponseChannel: msg.Channel,
                InitialReplyMessageId: msg.Id,
                MentionActivated: true,
                PersistReplyAliases: true,
                PersistThreadChannelAlias: msg.Channel is IThreadChannel));
    }

    private async Task<TableEntity?> GetConversationMappingAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
    {
        var existingConversation = await userThreadMappings.GetEntityIfExistsAsync<TableEntity>(
            partitionKey,
            rowKey,
            [AgentThreadIdColumnName, CanonicalRowKeyColumnName, TraceRootContextColumnName, MentionActivatedColumnName],
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return existingConversation.HasValue
            ? existingConversation.Value
            : null;
    }

    private async Task<ChatConversationRuntimeState> GetOrCreateConversationStateAsync(ChatConversationScope scope, string serializedAuthor, CancellationToken cancellationToken)
    {
        TableEntity? existingConversation = await GetConversationMappingAsync(scope.PartitionKey, scope.RowKey, cancellationToken).ConfigureAwait(false);
        string? storedConversationState = existingConversation is not null
            ? GetEntityStringValue(existingConversation, AgentThreadIdColumnName)
            : null;
        string? storedTraceRootContext = existingConversation is not null
            ? GetEntityStringValue(existingConversation, TraceRootContextColumnName)
            : null;

        CopilotChatState chatState = ConversationThreadState.Parse(storedConversationState);
        ActivityContext? rootParentContext = Activities.TryParseTraceParent(storedTraceRootContext, out ActivityContext parsedRootContext)
            ? parsedRootContext
            : null;

        string serializedConversationState = ConversationThreadState.Serialize(chatState);

        if (rootParentContext is null)
        {
            rootParentContext = Activities.CreateConversationRootContext(
                GetTraceRootScopeType(scope),
                $"{scope.PartitionKey}/{scope.RowKey}",
                chatState.CopilotSessionId.UnlessNullOrWhitespaceThen(chatState.FoundryThreadId).UnlessNullOrWhitespaceThen(scope.RowKey));
            storedTraceRootContext = Activities.FormatTraceParent(rootParentContext.Value);
        }

        if (existingConversation is null
            || string.IsNullOrWhiteSpace(storedConversationState)
            || !string.Equals(storedConversationState, serializedConversationState, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(GetEntityStringValue(existingConversation, TraceRootContextColumnName))
            || IsMentionActivated(existingConversation) != scope.MentionActivated)
        {
            await PersistConversationStateCoreAsync(
                scope,
                serializedAuthor,
                chatState,
                storedTraceRootContext!,
                cancellationToken).ConfigureAwait(false);
        }

        return new(chatState, storedTraceRootContext!, rootParentContext.Value);
    }

    private ValueTask PersistConversationStateCoreAsync(
        ChatConversationScope scope,
        string serializedAuthor,
        CopilotChatState updatedConversationState,
        string traceRootContext,
        CancellationToken cancellationToken)
        => PersistConversationStateCoreInternalAsync(scope, serializedAuthor, updatedConversationState, traceRootContext, cancellationToken);

    private async ValueTask PersistConversationStateCoreInternalAsync(
        ChatConversationScope scope,
        string serializedAuthor,
        CopilotChatState updatedConversationState,
        string traceRootContext,
        CancellationToken cancellationToken)
    {
        await userThreadMappings.UpsertEntityAsync(
            new TableEntity(scope.PartitionKey, scope.RowKey)
            {
                [AgentThreadIdColumnName] = ConversationThreadState.Serialize(updatedConversationState),
                ["Author"] = serializedAuthor,
                [CanonicalRowKeyColumnName] = scope.RowKey,
                [TraceRootContextColumnName] = traceRootContext,
                [MentionActivatedColumnName] = scope.MentionActivated,
            },
            TableUpdateMode.Replace,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!scope.PersistThreadChannelAlias || string.Equals(scope.PartitionKey, scope.RowKey, StringComparison.Ordinal))
        {
            return;
        }

        await userThreadMappings.UpsertEntityAsync(
            new TableEntity(scope.PartitionKey, scope.PartitionKey)
            {
                [CanonicalRowKeyColumnName] = scope.RowKey,
                [MentionActivatedColumnName] = scope.MentionActivated,
            },
            TableUpdateMode.Replace,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private ValueTask PersistReplyAliasAsync(ChatConversationScope scope, ulong replyMessageId, CancellationToken cancellationToken)
        => new(userThreadMappings.UpsertEntityAsync(
            new TableEntity(scope.PartitionKey, replyMessageId.ToString())
            {
                [CanonicalRowKeyColumnName] = scope.RowKey,
                [MentionActivatedColumnName] = scope.MentionActivated,
            },
            TableUpdateMode.Replace,
            cancellationToken: cancellationToken));

    private static string GetCanonicalRowKey(TableEntity entity, string fallbackRowKey)
        => entity.TryGetValue(CanonicalRowKeyColumnName, out object? canonicalRowKey)
            && canonicalRowKey is string { Length: > 0 } canonical
                ? canonical
                : fallbackRowKey;

    private static string? GetEntityStringValue(TableEntity entity, string propertyName)
        => entity.TryGetValue(propertyName, out object? value) ? value.ToString() : null;

    private static bool HasConversationState(TableEntity? entity)
        => entity is not null && !string.IsNullOrWhiteSpace(GetEntityStringValue(entity, AgentThreadIdColumnName));

    private static bool IsMentionActivated(TableEntity entity)
    {
        if (!entity.TryGetValue(MentionActivatedColumnName, out object? mentionActivated))
        {
            return false;
        }

        if (mentionActivated is bool b)
        {
            return b;
        }

        return mentionActivated is string text && bool.TryParse(text, out bool parsed) && parsed;
    }

    private static string GetTraceRootScopeType(ChatConversationScope scope)
        => scope.LockKey.StartsWith("dm:", StringComparison.Ordinal)
            ? "dm"
            : scope.LockKey.StartsWith("guild-thread:", StringComparison.Ordinal)
                ? "guild_thread"
                : scope.LockKey.StartsWith("guild-reply:", StringComparison.Ordinal)
                    ? "guild_reply"
                    : "conversation";

    private static string BuildMentionThreadName(SocketUserMessage msg)
    {
        string displayName = msg.Author.GlobalName.UnlessNullOrWhitespaceThen(msg.Author.Username);
        string threadName = $"{displayName} + Bear Metal chat";
        return threadName.Length <= 100
            ? threadName
            : threadName[..100];
    }

    private sealed record ChatResponseStyle(bool IsDm, bool ShowProgressMessages, bool MentionUserInFirstResponse);

    private sealed record ChatConversationScope(
        string LockKey,
        string PartitionKey,
        string RowKey,
        IMessageChannel ResponseChannel,
        ulong? InitialReplyMessageId,
        bool MentionActivated,
        bool PersistReplyAliases,
        bool PersistThreadChannelAlias);

    private sealed record ChatConversationRuntimeState(
        CopilotChatState ChatState,
        string TraceRootContext,
        ActivityContext RootParentContext);

    private sealed record ChatConversationContext(ChatResponseStyle Style, ChatConversationScope Scope);

    private async Task CleanupStreamedMessagesAsync(IEnumerable<IUserMessage> messages, CancellationToken cancellationToken)
    {
        foreach (IUserMessage message in messages.Reverse())
        {
            bool deleted = false;
            try
            {
                deleted = await message.TryDeleteDiscordMessageAsync("delete streamed response message", time, logger, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                logger.FailedToRemoveTransientThinkingMessage(e);
            }

            if (deleted)
            {
                continue;
            }

            try
            {
                await message.TryModifyDiscordMessageAsync(PartialResponseRemovedText, time, logger, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                logger.FailedToRemoveTransientThinkingMessage(e);
            }
        }
    }
}
