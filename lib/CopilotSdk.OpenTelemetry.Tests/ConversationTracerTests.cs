namespace CopilotSdk.OpenTelemetry.Tests;

using System.Diagnostics;

using Xunit;

[Collection("ActivityListener")]
public class ConversationTracerTests : IDisposable
{
    private readonly List<Activity> _activities = new();
    private readonly ActivityListener _listener;

    public ConversationTracerTests()
    {
        this._listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == CopilotSdkOpenTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => this._activities.Add(activity),
        };
        ActivitySource.AddActivityListener(this._listener);
    }

    public void Dispose() => this._listener.Dispose();

    [Fact]
    public async Task BeginTurnAsync_CreatesAndPersistsRoot_OnFirstCall()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.BeginTurnAsync("conv-1", null, default))
        {
            // turn is open
        }

        var persisted = await store.TryGetAsync("conv-1", default);
        Assert.NotNull(persisted);
        // Two activities stopped: the root (zero duration) and the turn
        Assert.Equal(2, this._activities.Count);
        var turn = this._activities[1];
        Assert.Equal(persisted!.TraceId, turn.TraceId.ToHexString());
    }

    [Fact]
    public async Task BeginTurnAsync_TwoTurns_ShareTraceId()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.BeginTurnAsync("conv-1", null, default)) { }
        await using (await tracer.BeginTurnAsync("conv-1", null, default)) { }

        // 1 root + 2 turns = 3 stopped activities
        Assert.Equal(3, this._activities.Count);
        var turn1 = this._activities[1];
        var turn2 = this._activities[2];
        Assert.Equal(turn1.TraceId, turn2.TraceId);
    }

    [Fact]
    public async Task BeginTurnAsync_AfterRemove_StartsNewTrace()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.BeginTurnAsync("conv-1", null, default)) { }
        var firstTurn = this._activities[1];

        await store.RemoveAsync("conv-1", default);

        await using (await tracer.BeginTurnAsync("conv-1", null, default)) { }
        var secondTurn = this._activities[^1];

        Assert.NotEqual(firstTurn.TraceId, secondTurn.TraceId);
    }

    [Fact]
    public async Task BeginTurnAsync_TagsTurn_WithGenAiAttributes()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.BeginTurnAsync("conv-42", null, default)) { }

        var turn = this._activities.Last();
        Assert.Equal(CopilotSdkOpenTelemetry.GenAiSystemValue, turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.System));
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Chat, turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName));
        Assert.Equal("conv-42", turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId));
    }

    [Fact]
    public async Task BeginTurnAsync_AppliesRootTags_OnFirstCallOnly()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);
        var rootTags = new Dictionary<string, object?> { ["custom.tag"] = "value-1" };

        await using (await tracer.BeginTurnAsync("conv-1", rootTags, default)) { }

        var root = this._activities[0];
        Assert.Equal("value-1", root.GetTagItem("custom.tag"));
    }

    [Fact]
    public async Task BeginTurnAsync_AmbientActivity_DoesNotBecomeRootParent()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        // Open an unrelated ambient activity to prove the root span ignores it.
        using var unrelated = new ActivitySource("Unrelated.Source").StartActivity("ambient");

        await using (await tracer.BeginTurnAsync("conv-iso", null, default)) { }

        var root = this._activities[0];
        Assert.Equal(default, root.ParentSpanId);
    }
}
