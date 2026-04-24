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

    // Aggressively narrow the CLI's built-in surface for headless Discord turns.
    //
    // Background: the GHCP CLI ships a developer toolkit (sql, powershell, view/edit/create,
    // grep/glob, store_memory, task, agent management, etc.) intended for an interactive
    // coding agent. In a Discord-triggered turn there is no human on the other end, no
    // codebase to navigate, no past chat to mine — yet the model has been observed reaching
    // for `sql` ("SELECT FROM sqlite_master") and `powershell` ("Get-ChildItem -Recurse") to
    // "find past data about X" instead of calling the configured FRC tools (TBA, Statbotics).
    //
    // Policy: exclude every built-in that doesn't directly serve a Discord answer. The
    // built-ins we keep are `report_intent` (harmless telemetry) and `web_search` /
    // `web_fetch` (current FRC news, rules clarifications, FIRST blog posts that TBA
    // doesn't expose). The configured FRC tools (tba_api_surface, tba_api, tba_last_comp,
    // statbotics_api, fetch_meal_signup_info, plus any sub-agents like frc-data) live
    // outside this list and remain available.
    private static readonly string[] s_excludedBuiltInTools =
    [
        // User-interaction tools — no human in the loop on a Discord turn.
        "ask_user",
        // Local data stores the model wrongly treats as "past chat history" / "session notes".
        "sql",
        "session_store_sql",
        "store_memory",
        "vote_memory",
        // Shell / process tools — bot has no business shelling out on a host machine.
        "powershell",
        "read_powershell",
        "write_powershell",
        "stop_powershell",
        "list_powershell",
        "bash",
        // Filesystem tools — no codebase to navigate during a Discord turn.
        "view",
        "create",
        "edit",
        "glob",
        "grep",
        // Task / agent orchestration tools — single-turn Q&A doesn't need them.
        "task",
        "task_complete",
        "read_agent",
        "list_agents",
        "write_agent",
        // CLI self-introspection — irrelevant to FRC questions.
        "skill",
        "fetch_copilot_cli_documentation",
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
