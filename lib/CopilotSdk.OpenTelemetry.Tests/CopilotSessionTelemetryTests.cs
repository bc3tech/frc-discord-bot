namespace CopilotSdk.OpenTelemetry.Tests;

using System.Diagnostics;

using GitHub.Copilot.SDK;

using Xunit;

[Collection("ActivityListener")]
public class CopilotSessionTelemetryTests : IDisposable
{
    private readonly List<Activity> _stopped = new();
    private readonly ActivityListener _listener;
    private readonly ActivitySource _parentSource = new("CopilotSdk.OpenTelemetry.Tests.Parent");

    public CopilotSessionTelemetryTests()
    {
        this._listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => this._stopped.Add(activity),
        };
        ActivitySource.AddActivityListener(this._listener);
    }

    public void Dispose()
    {
        this._listener.Dispose();
        this._parentSource.Dispose();
    }

    [Fact]
    public void ToolStartThenComplete_EmitsExecuteToolSpan_ChildOfParent()
    {
        using var parent = this._parentSource.StartActivity("parent")!;
        var state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "call-1", ToolName = "TbaApi.GetTeam" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "call-1", Success = true },
        });

        var toolSpan = this._stopped.Single(a => a.OperationName == "execute_tool TbaApi.GetTeam");
        Assert.Equal(parent.TraceId, toolSpan.TraceId);
        Assert.Equal(parent.SpanId, toolSpan.ParentSpanId);
        Assert.Equal(ActivityStatusCode.Ok, toolSpan.Status);
        Assert.Equal("TbaApi.GetTeam", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolName));
        Assert.Equal("call-1", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolCallId));
    }

    [Fact]
    public void ToolComplete_OnFailure_SetsErrorStatusAndMessage()
    {
        using var parent = this._parentSource.StartActivity("parent")!;
        var state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "c1", ToolName = "BoomTool" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData
            {
                ToolCallId = "c1",
                Success = false,
                Error = new ToolExecutionCompleteDataError { Message = "kaboom" },
            },
        });

        var toolSpan = this._stopped.Single(a => a.OperationName == "execute_tool BoomTool");
        Assert.Equal(ActivityStatusCode.Error, toolSpan.Status);
        Assert.Equal("kaboom", toolSpan.StatusDescription);
    }

    [Fact]
    public void AssistantUsage_TagsParentTurn()
    {
        using var parent = this._parentSource.StartActivity("turn")!;
        var state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new AssistantUsageEvent
        {
            Data = new AssistantUsageData { Model = "gpt-5", InputTokens = 123, OutputTokens = 456 },
        });

        Assert.Equal("gpt-5", parent.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ResponseModel));
        Assert.Equal(123, Convert.ToInt64(parent.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageInputTokens)));
        Assert.Equal(456, Convert.ToInt64(parent.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageOutputTokens)));
    }

    [Fact]
    public void SessionError_SetsParentTurnError()
    {
        using var parent = this._parentSource.StartActivity("turn")!;
        var state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new SessionErrorEvent
        {
            Data = new SessionErrorData { ErrorType = "RateLimited", Message = "slow down" },
        });

        Assert.Equal(ActivityStatusCode.Error, parent.Status);
        Assert.Equal("slow down", parent.StatusDescription);
        Assert.Equal("RateLimited", parent.GetTagItem("error.type"));
    }

    [Fact]
    public void DisposeAll_ClosesInflightToolSpans()
    {
        using var parent = this._parentSource.StartActivity("turn")!;
        var state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "leaked", ToolName = "Leaky" },
        });

        state.DisposeAll();

        Assert.Contains(this._stopped, a => a.OperationName == "execute_tool Leaky");
    }
}
