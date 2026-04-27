namespace CopilotSdk.OpenTelemetry.Tests;

using System.Diagnostics;

using Xunit;

[Collection("ActivityListener")]
public class ConversationTracerTests : IDisposable
{
    private readonly List<Activity> _activities = [];
    private readonly ActivityListener _listener;

    public ConversationTracerTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == CopilotSdkOpenTelemetry.ActivitySourceName,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => _activities.Add(activity),
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose() => _listener.Dispose();

    [Fact]
    public async Task CreateOrResumeConversation_CreatesConversationActivity_OnFirstCall()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
        }

        ConversationTraceContext? persisted = await store.TryGetAsync("conv-1", default);
        Assert.NotNull(persisted);
        Assert.Single(_activities);
        Activity conversation = _activities[0];
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Conversation, conversation.OperationName);
        Assert.Equal(ActivityKind.Internal, conversation.Kind);
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Conversation, conversation.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName));
    }

    [Fact]
    public async Task CreateOrResumeConversation_PlusBeginTurn_CreatesChildChatActivity()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (IConversationScope scope = await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
            await using IConversationTurnScope turn = tracer.BeginTurn("conv-1");
            Assert.NotNull(turn.Activity);
            Assert.Equal(ActivityKind.Client, turn.Activity!.Kind);
            Assert.Equal(scope.Activity!.SpanId, turn.Activity.ParentSpanId);
        }

        Assert.Equal(2, _activities.Count);
        Activity turnActivity = _activities[0];
        Activity conversationActivity = _activities[1];
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Chat, turnActivity.OperationName);
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Conversation, conversationActivity.OperationName);
    }

    [Fact]
    public async Task ConversationActivity_HasRealDuration()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-dur", null, default))
        {
            await Task.Delay(50);
        }

        Activity conversation = _activities[0];
        Assert.True(conversation.Duration > TimeSpan.Zero);
    }

    [Fact]
    public async Task TurnActivity_HasCorrectAttributes()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-42", null, default))
        {
            await using IConversationTurnScope turn = tracer.BeginTurn("conv-42");
        }

        Activity turnActivity = _activities[0];
        Assert.Equal(CopilotSdkOpenTelemetry.GenAiProviderValue, turnActivity.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName));
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.Chat, turnActivity.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName));
        Assert.Equal("conv-42", turnActivity.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId));
    }

    [Fact]
    public async Task ConversationActivity_HasConversationId()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-id-check", null, default))
        {
        }

        Activity conversation = _activities[0];
        Assert.Equal("conv-id-check", conversation.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ConversationId));
    }

    [Fact]
    public async Task SecondInvocation_SharesTraceId()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
        }

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
        }

        Assert.Equal(2, _activities.Count);
        Assert.Equal(_activities[0].TraceId, _activities[1].TraceId);
    }

    [Fact]
    public async Task StartTimestamp_RoundTrips_ThroughInMemoryStore()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-ts", null, default))
        {
        }

        ConversationTraceContext? persisted = await store.TryGetAsync("conv-ts", default);
        Assert.NotNull(persisted);
        Assert.NotNull(persisted!.StartTimestamp);
        Assert.True(persisted.StartTimestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task AmbientActivity_DoesNotBecomeConversationParent()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        using Activity? unrelated = new ActivitySource("Unrelated.Source").StartActivity("ambient");

        await using (await tracer.CreateOrResumeConversationAsync("conv-iso", null, default))
        {
        }

        Activity conversation = _activities[0];
        Assert.Equal(default, conversation.ParentSpanId);
    }

    [Fact]
    public async Task RootTags_AppliedToConversation_OnFirstCall()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);
        var rootTags = new Dictionary<string, object?> { ["custom.tag"] = "value-1" };

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", rootTags, default))
        {
        }

        Activity conversation = _activities[0];
        Assert.Equal("value-1", conversation.GetTagItem("custom.tag"));
    }

    [Fact]
    public async Task AfterRemove_NewConversation_GetsNewTraceId()
    {
        var store = new InMemoryConversationTraceContextStore();
        var tracer = new ConversationTracer(store);

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
        }

        ActivityTraceId firstTraceId = _activities[0].TraceId;
        await store.RemoveAsync("conv-1", default);

        await using (await tracer.CreateOrResumeConversationAsync("conv-1", null, default))
        {
        }

        ActivityTraceId secondTraceId = _activities[^1].TraceId;
        Assert.NotEqual(firstTraceId, secondTraceId);
    }
}
