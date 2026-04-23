namespace FunctionApp.Tests;

using BC3Technologies.DiscordGpt.Copilot;
using BC3Technologies.DiscordGpt.Core;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging.Abstractions;

using ChatBotIsolated = global::ChatBot.Diagnostics.IsolatedSessionConfigSource;

public sealed class IsolatedSessionConfigSourceTests : IDisposable
{
    private readonly string _botRoot = Path.Combine(Path.GetTempPath(), "frc-bot-copilot");
    private readonly AsyncLocalConversationContextAccessor _accessor = new();

    private ChatBotIsolated CreateSource() =>
        new(_accessor, NullLogger<ChatBotIsolated>.Instance);

    private static SessionConfig NewSessionConfig() => new();

    private static ConversationContext NewContext(ConversationScope scope, string id) =>
        new(new ConversationKey(scope, id), null, "c", "u", "n", false);

    [Fact]
    public async Task ConfigureAsync_ProducesStableConfigDirForSameConversation()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = NewContext(ConversationScope.DirectMessage, "12345");

        var c1 = NewSessionConfig();
        await source.ConfigureAsync(c1, CancellationToken.None);
        var c2 = NewSessionConfig();
        await source.ConfigureAsync(c2, CancellationToken.None);

        Assert.Equal(c1.ConfigDir, c2.ConfigDir);
        Assert.True(Directory.Exists(c1.ConfigDir));
        Assert.StartsWith(Path.Combine(_botRoot, "cfg"), c1.ConfigDir);
    }

    [Fact]
    public async Task ConfigureAsync_DifferentConversationsProduceDifferentConfigDirs()
    {
        ChatBotIsolated source = CreateSource();
        var a = NewSessionConfig();
        var b = NewSessionConfig();

        _accessor.Current = NewContext(ConversationScope.DirectMessage, "aaa");
        await source.ConfigureAsync(a, CancellationToken.None);
        _accessor.Current = NewContext(ConversationScope.DirectMessage, "bbb");
        await source.ConfigureAsync(b, CancellationToken.None);

        Assert.NotEqual(a.ConfigDir, b.ConfigDir);
    }

    [Fact]
    public async Task ConfigureAsync_DifferentScopesProduceDifferentConfigDirs()
    {
        ChatBotIsolated source = CreateSource();
        var a = NewSessionConfig();
        var b = NewSessionConfig();

        _accessor.Current = NewContext(ConversationScope.DirectMessage, "x");
        await source.ConfigureAsync(a, CancellationToken.None);
        _accessor.Current = NewContext(ConversationScope.Thread, "x");
        await source.ConfigureAsync(b, CancellationToken.None);

        Assert.NotEqual(a.ConfigDir, b.ConfigDir);
    }

    [Fact]
    public async Task ConfigureAsync_ProducesUniquePerTurnWorkingDirectory()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = NewContext(ConversationScope.DirectMessage, "42");

        var c1 = NewSessionConfig();
        var c2 = NewSessionConfig();
        await source.ConfigureAsync(c1, CancellationToken.None);
        await source.ConfigureAsync(c2, CancellationToken.None);

        Assert.NotEqual(c1.WorkingDirectory, c2.WorkingDirectory);
        Assert.True(Directory.Exists(c1.WorkingDirectory));
        Assert.True(Directory.Exists(c2.WorkingDirectory));
    }

    [Fact]
    public async Task ConfigureAsync_ExclusionListIsExactlyThreeTools()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = NewContext(ConversationScope.DirectMessage, "1");

        var cfg = NewSessionConfig();
        await source.ConfigureAsync(cfg, CancellationToken.None);

        Assert.NotNull(cfg.ExcludedTools);
        Assert.Equal(3, cfg.ExcludedTools!.Count);
        Assert.Contains("ask_user", cfg.ExcludedTools);
        Assert.Contains("web_fetch", cfg.ExcludedTools);
        Assert.Contains("web_search", cfg.ExcludedTools);
    }

    [Fact]
    public async Task ConfigureAsync_DisablesConfigDiscovery()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = NewContext(ConversationScope.DirectMessage, "1");

        var cfg = NewSessionConfig();
        await source.ConfigureAsync(cfg, CancellationToken.None);

        Assert.Equal(false, cfg.EnableConfigDiscovery);
    }

    [Fact]
    public async Task ConfigureAsync_ThrowsWhenConversationContextIsMissing()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = null;

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await source.ConfigureAsync(NewSessionConfig(), CancellationToken.None));
    }

    [Fact]
    public async Task ConfigureAsync_LongAndUnicodeIdsProduceBoundedSafePaths()
    {
        ChatBotIsolated source = CreateSource();
        _accessor.Current = NewContext(
            ConversationScope.Thread,
            new string('9', 120) + "-\u4e2d\u6587-🙂");

        var cfg = NewSessionConfig();
        await source.ConfigureAsync(cfg, CancellationToken.None);

        Assert.NotNull(cfg.ConfigDir);
        Assert.True(cfg.ConfigDir!.Length < 240, $"ConfigDir too long: {cfg.ConfigDir.Length}");
        Assert.True(Directory.Exists(cfg.ConfigDir));
    }

    public void Dispose()
    {
        _accessor.Current = null;
    }
}
