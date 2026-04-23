namespace ChatBot.Diagnostics;

using System.Security.Cryptography;
using System.Text;

using BC3Technologies.DiscordGpt.Copilot;
using BC3Technologies.DiscordGpt.Core;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging;

/// <summary>
/// <see cref="ISessionConfigSource"/> that isolates each Copilot turn from the host machine's
/// ambient Copilot CLI state. Two independent scopes are applied:
/// <list type="bullet">
///   <item>
///     <description>
///       <c>ConfigDir</c> is pinned to a <b>per-conversation</b> directory so the SDK's built-in
///       cloud session store (<c>sql</c> / <c>session_store_sql</c>) and user-scope memory see only
///       that conversation's history — never <c>~/.copilot/</c>, never a sibling Discord
///       conversation, never a dev's local Copilot CLI sessions.
///     </description>
///   </item>
///   <item>
///     <description>
///       <c>WorkingDirectory</c> is set to a fresh <b>per-turn</b> temp folder so scratch writes
///       by <c>edit</c> / <c>create</c> / <c>powershell</c> can't collide across turns.
///     </description>
///   </item>
/// </list>
/// </summary>
/// <remarks>
/// <para>
/// The conversation key is obtained via <see cref="IConversationContextAccessor"/>, which the
/// prompt harness populates for the duration of the source pipeline. A missing context indicates
/// the source was invoked outside a harness turn and is treated as a programmer error.
/// </para>
/// <para>
/// Conversation persistence is still owned by <c>WithBlobSessionStorage</c> (an SDK
/// <c>ISessionFsHandler</c>) — this source governs the CLI-side built-in surface only.
/// </para>
/// </remarks>
public sealed partial class IsolatedSessionConfigSource(
    IConversationContextAccessor conversationContextAccessor,
    ILogger<IsolatedSessionConfigSource> logger) : ISessionConfigSource
{
    private static readonly string s_botConfigRoot = Path.Combine(Path.GetTempPath(), "frc-bot-copilot");

    // Intentionally small: the CLI's identity-scoped leaks are closed by per-conversation
    // ConfigDir (above). What remains are built-ins with no purpose in a headless Discord
    // turn: there's no human to answer ask_user, and the agent prompt forbids outbound
    // web traffic from a Discord-triggered turn (R15). Everything else — including sql,
    // store_memory, task — is safe to expose because ConfigDir isolation now scopes the
    // data those tools see to the current conversation.
    private static readonly string[] s_excludedBuiltInTools =
    [
        "ask_user",
        "web_fetch",
        "web_search",
    ];

    private readonly IConversationContextAccessor _conversationContextAccessor =
        conversationContextAccessor ?? throw new ArgumentNullException(nameof(conversationContextAccessor));
    private readonly ILogger<IsolatedSessionConfigSource> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public ValueTask ConfigureAsync(SessionConfig sessionConfig, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sessionConfig);

        ConversationContext? context = _conversationContextAccessor.Current
            ?? throw new InvalidOperationException(
                $"{nameof(IsolatedSessionConfigSource)} requires an active {nameof(ConversationContext)} " +
                $"on {nameof(IConversationContextAccessor)}. The prompt harness sets this for the duration " +
                "of the session config pipeline; invoking this source outside a harness turn is not supported.");

        var safeKey = BuildSafeConversationKey(context.Key);
        var convConfigDir = Path.Combine(s_botConfigRoot, "cfg", safeKey);
        Directory.CreateDirectory(convConfigDir);

        var perTurnWorkingDir = Path.Combine(s_botConfigRoot, "work", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(perTurnWorkingDir);

        sessionConfig.ConfigDir = convConfigDir;
        sessionConfig.WorkingDirectory = perTurnWorkingDir;
        sessionConfig.EnableConfigDiscovery = false;

        sessionConfig.ExcludedTools ??= [];
        foreach (var name in s_excludedBuiltInTools)
        {
            sessionConfig.ExcludedTools.Add(name);
        }

        LogIsolated(_logger, convConfigDir, perTurnWorkingDir, sessionConfig.ExcludedTools.Count);
        return ValueTask.CompletedTask;
    }

    internal static string BuildSafeConversationKey(ConversationKey key)
    {
        // Stable per-conversation directory name: "{scope}-{16-char hex of SHA256(id)}".
        // Hash the id so exotic Discord snowflakes / unicode / path-breaking chars can't
        // escape the bot config root, and so we stay well under Windows MAX_PATH margins.
        Span<byte> hash = stackalloc byte[SHA256.HashSizeInBytes];
        _ = SHA256.HashData(Encoding.UTF8.GetBytes(key.Id), hash);
        var hex = Convert.ToHexString(hash[..8]).ToLowerInvariant();
        return $"{(int)key.Scope}-{hex}";
    }

    [LoggerMessage(EventId = 5100, Level = LogLevel.Debug,
        Message = "Copilot session isolated: ConfigDir={ConfigDir}, WorkingDirectory={WorkingDirectory}, EnableConfigDiscovery=false, ExcludedTools={ExcludedToolCount}")]
    private static partial void LogIsolated(ILogger logger, string configDir, string workingDirectory, int excludedToolCount);
}
