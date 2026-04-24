namespace ChatBot;

using Microsoft.Extensions.AI;

using System.Runtime.CompilerServices;

internal sealed class FrcSystemPromptChatClient : IChatClient
{
    private readonly IChatClient _innerClient;
    private readonly string _systemPrompt;

    public FrcSystemPromptChatClient(IChatClient innerClient, string systemPrompt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemPrompt);

        _innerClient = innerClient;
        _systemPrompt = systemPrompt;
    }

#pragma warning disable CA1822 // Interface shape parity with IChatClient implementations
    public ChatClientMetadata Metadata => new("frc-system-prompt-chat-client");
#pragma warning restore CA1822

    public object? GetService(Type serviceType, object? serviceKey = null)
        => _innerClient.GetService(serviceType, serviceKey);

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
        => _innerClient.GetResponseAsync(PrependSystemPrompt(messages), options, cancellationToken);

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (ChatResponseUpdate update in _innerClient.GetStreamingResponseAsync(
            PrependSystemPrompt(messages),
            options,
            cancellationToken).ConfigureAwait(false))
        {
            yield return update;
        }
    }

    public void Dispose()
        => _innerClient.Dispose();

    private IReadOnlyList<ChatMessage> PrependSystemPrompt(IEnumerable<ChatMessage> messages)
    {

        IReadOnlyList<ChatMessage> messageList = messages as IReadOnlyList<ChatMessage> ?? [.. messages];
        return [new ChatMessage(ChatRole.System, _systemPrompt), .. messageList];
    }
}
