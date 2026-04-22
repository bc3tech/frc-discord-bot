namespace CopilotSdk.OpenTelemetry.Tests;

using Xunit;

public class InMemoryConversationTraceContextStoreTests
{
    [Fact]
    public async Task TryGetAsync_ReturnsNull_WhenAbsent()
    {
        var store = new InMemoryConversationTraceContextStore();
        Assert.Null(await store.TryGetAsync("conv-1", default));
    }

    [Fact]
    public async Task SetAsync_ThenTryGetAsync_ReturnsSameContext()
    {
        var store = new InMemoryConversationTraceContextStore();
        var ctx = new ConversationTraceContext("0123456789abcdef0123456789abcdef", "0123456789abcdef", 1);

        await store.SetAsync("conv-1", ctx, default);

        var got = await store.TryGetAsync("conv-1", default);
        Assert.Equal(ctx, got);
    }

    [Fact]
    public async Task RemoveAsync_DropsEntry()
    {
        var store = new InMemoryConversationTraceContextStore();
        var ctx = new ConversationTraceContext("0123456789abcdef0123456789abcdef", "0123456789abcdef", 1);
        await store.SetAsync("conv-1", ctx, default);

        await store.RemoveAsync("conv-1", default);

        Assert.Null(await store.TryGetAsync("conv-1", default));
    }

    [Fact]
    public async Task SetAsync_OverwritesExisting()
    {
        var store = new InMemoryConversationTraceContextStore();
        var first = new ConversationTraceContext("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "1111111111111111", 1);
        var second = new ConversationTraceContext("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "2222222222222222", 1);

        await store.SetAsync("conv-1", first, default);
        await store.SetAsync("conv-1", second, default);

        Assert.Equal(second, await store.TryGetAsync("conv-1", default));
    }
}
