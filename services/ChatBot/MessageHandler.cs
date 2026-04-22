namespace ChatBot;

using BC3Technologies.DiscordGpt.Core;

using global::CopilotSdk.OpenTelemetry;

using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

public sealed partial class MessageHandler(
    IDiscordEventHandler discordEventHandler,
    IConversationKeyResolver conversationKeyResolver,
    IConversationTracer conversationTracer,
    ILogger<MessageHandler> logger)
{
    private readonly IDiscordEventHandler _discordEventHandler = discordEventHandler;
    private readonly IConversationKeyResolver _conversationKeyResolver = conversationKeyResolver;
    private readonly IConversationTracer _conversationTracer = conversationTracer;
    private readonly ILogger<MessageHandler> _logger = logger;

    public async Task HandleUserMessageAsync(IUserMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.Author.IsBot)
        {
            return;
        }

        MessageCreatedEvent discordEvent = BuildMessageCreatedEvent(message, botMentioned: false, mentionedRoleIds: []);
        await InvokeWithTurnSpanAsync(discordEvent, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> TryHandleGuildMessageAsync(
        SocketUserMessage message,
        ulong botUserId,
        IReadOnlyCollection<ulong> botRoleIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(botRoleIds);

        if (message.Author.IsBot)
        {
            return false;
        }

        bool botMentioned = message.MentionedUsers.Any(user => user.Id == botUserId);
        bool botRoleMentioned = message.MentionedRoles.Any(role => botRoleIds.Contains(role.Id));
        bool isThreadMessage = message.Channel is SocketThreadChannel;
        bool shouldProcess = botMentioned || botRoleMentioned || isThreadMessage;

        if (!shouldProcess)
        {
            Log.IgnoringGuildMessage(_logger, message.Id, message.Author.Id, message.Channel.Id);
            return false;
        }

        MessageCreatedEvent discordEvent = BuildMessageCreatedEvent(
            message,
            botMentioned: botMentioned || isThreadMessage,
            mentionedRoleIds: [.. message.MentionedRoles.Select(role => role.Id.ToString())]);

        await InvokeWithTurnSpanAsync(discordEvent, cancellationToken).ConfigureAwait(false);
        return true;
    }

    private async Task InvokeWithTurnSpanAsync(MessageCreatedEvent discordEvent, CancellationToken cancellationToken)
    {
        ConversationKey conversationKey = await _conversationKeyResolver
            .ResolveAsync(discordEvent, cancellationToken)
            .ConfigureAwait(false);

        var rootTags = new Dictionary<string, object?>
        {
            ["discord.conversation.scope"] = (int)conversationKey.Scope,
            ["discord.user.id"] = discordEvent.UserId,
            ["discord.channel.id"] = discordEvent.ChannelId,
            ["discord.guild.id"] = discordEvent.GuildId,
            ["discord.is_dm"] = discordEvent.IsDm,
        };

        await using var turn = await _conversationTracer
            .BeginTurnAsync(conversationKey.ToStorageKey(), rootTags, cancellationToken)
            .ConfigureAwait(false);

        turn.Activity?.SetTag("discord.message.id", discordEvent.MessageId);
        turn.Activity?.SetTag("discord.thread.id", discordEvent.ThreadId);

        await _discordEventHandler.HandleAsync(discordEvent, cancellationToken).ConfigureAwait(false);
    }

    private static MessageCreatedEvent BuildMessageCreatedEvent(
        IUserMessage message,
        bool botMentioned,
        IReadOnlyList<string> mentionedRoleIds)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(mentionedRoleIds);

        string displayName = string.IsNullOrWhiteSpace(message.Author.GlobalName)
            ? message.Author.Username
            : message.Author.GlobalName;

        string content = BuildMessageContent(message.CleanContent, displayName, message.Author.Username);

        return new MessageCreatedEvent
        {
            ChannelId = message.Channel.Id.ToString(),
            GuildId = (message.Channel as IGuildChannel)?.GuildId.ToString(),
            UserId = message.Author.Id.ToString(),
            UserDisplayName = displayName,
            MessageId = message.Id.ToString(),
            Content = content,
            ThreadId = (message.Channel as IThreadChannel)?.Id.ToString(),
            BotMentioned = botMentioned,
            MentionedRoleIds = mentionedRoleIds,
            IsDm = message.Channel is IDMChannel,
            Timestamp = message.Timestamp,
        };
    }

    private static string BuildMessageContent(string cleanContent, string displayName, string userId)
    {
        string normalizedContent = cleanContent;
        return $"{normalizedContent}\n\n===User Display Name: {displayName}\nUser Id: {userId}===";
    }

    private static partial class Log
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Debug,
            Message = "Ignoring guild message {MessageId} from user {UserId} in channel {ChannelId} because it does not target chatbot handling.")]
        public static partial void IgnoringGuildMessage(ILogger logger, ulong messageId, ulong userId, ulong channelId);
    }
}
