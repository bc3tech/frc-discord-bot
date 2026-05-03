namespace ChatBot;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal sealed partial class FrcSystemPromptChatClient : IChatClient
{
    // U4d backstop: detect parenthesized opt-out hints in the final response
    // text. The R10 footer pattern is intentional ("(Numbers only if you'd
    // prefer.)") but a clarifying question disguised as a parenthetical hint
    // (e.g., "...(if you'd prefer team names instead?)") is a R6 violation.
    // Observation-only — we log a Warning and let the response through.
    [GeneratedRegex(@"\([^)]*?(if you'?d|prefer|want|instead)[^)]*?\?\s*\)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 200)]
    private static partial Regex ParentheticalOptOutHintRegex();

    private readonly IChatClient _innerClient;
    private readonly string _systemPrompt;
    private readonly ILogger<FrcSystemPromptChatClient> _logger;

    public FrcSystemPromptChatClient(IChatClient innerClient, string systemPrompt, ILogger<FrcSystemPromptChatClient> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(systemPrompt);

        _innerClient = innerClient;
        _systemPrompt = systemPrompt;
        _logger = logger;
    }

#pragma warning disable CA1822 // Interface shape parity with IChatClient implementations
    public ChatClientMetadata Metadata => new("frc-system-prompt-chat-client");
#pragma warning restore CA1822

    public object? GetService(Type serviceType, object? serviceKey = null)
        => _innerClient.GetService(serviceType, serviceKey);

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ChatResponse response = await _innerClient.GetResponseAsync(PrependSystemPrompt(messages), options, cancellationToken).ConfigureAwait(false);
        InspectForParentheticalOptOutHint(response.Text);
        return response;
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var aggregated = new System.Text.StringBuilder();
        await foreach (ChatResponseUpdate update in _innerClient.GetStreamingResponseAsync(
            PrependSystemPrompt(messages),
            options,
            cancellationToken).ConfigureAwait(false))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                aggregated.Append(update.Text);
            }

            yield return update;
        }

        InspectForParentheticalOptOutHint(aggregated.ToString());
    }

    public void Dispose()
        => _innerClient.Dispose();

    private void InspectForParentheticalOptOutHint(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return;
        }

        Match match;
        try
        {
            match = ParentheticalOptOutHintRegex().Match(responseText);
        }
        catch (RegexMatchTimeoutException)
        {
            return;
        }

        if (match.Success)
        {
            _logger.AgentResponseContainedParentheticalOptOutHint(match.Value);
        }
    }

    private IReadOnlyList<ChatMessage> PrependSystemPrompt(IEnumerable<ChatMessage> messages)
    {

        IReadOnlyList<ChatMessage> messageList = messages as IReadOnlyList<ChatMessage> ?? [.. messages];
        return [new ChatMessage(ChatRole.System, _systemPrompt), .. messageList];
    }
}
