namespace ChatBot;

using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

public sealed class ChatRunner(ILogger<ChatRunner> logger)
{
    private readonly Conversation? _conversation;
    private readonly ILogger<ChatRunner> _logger = logger;

    internal ChatRunner(ILogger<ChatRunner> logger, Conversation? conversation = null) : this(logger)
    {
        _conversation = conversation;
    }

    public async IAsyncEnumerable<string> GetCompletionsAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_conversation is not null)
        {
            await foreach (var update in _conversation.PostUserMessageStreamingAsync(new Copilot.CopilotChatState(), prompt, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                if (!string.IsNullOrWhiteSpace(update.Text))
                {
                    yield return update.Text;
                }
            }
        }
        else
        {
            _logger.ChatCompletionRequestedButNoConversationObjectAvailableToHandleItReturningEmptyResponse();
            yield break;
        }
    }
}
