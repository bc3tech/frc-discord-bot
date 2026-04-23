namespace CopilotSdk.OpenTelemetry.Tests;

using GitHub.Copilot.SDK;

using System.Diagnostics;

using Xunit;

[Collection("ActivityListener")]
public class CopilotSessionTelemetryTests : IDisposable
{
    private readonly List<Activity> _stopped = [];
    private readonly ActivityListener _listener;
    private readonly ActivitySource _parentSource = new("CopilotSdk.OpenTelemetry.Tests.Parent");

    public CopilotSessionTelemetryTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => _stopped.Add(activity),
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        _listener.Dispose();
        _parentSource.Dispose();
    }

    [Fact]
    public void ToolStartThenComplete_EmitsExecuteToolSpan_ChildOfParent()
    {
        using Activity parent = _parentSource.StartActivity("parent")!;
        CopilotSessionTelemetry.TelemetryState state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "call-1", ToolName = "TbaApi.GetTeam" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "call-1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool TbaApi.GetTeam");
        Assert.Equal(parent.TraceId, toolSpan.TraceId);
        Assert.Equal(parent.SpanId, toolSpan.ParentSpanId);
        Assert.Equal(ActivityStatusCode.Ok, toolSpan.Status);
        Assert.Equal("TbaApi.GetTeam", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolName));
        Assert.Equal("call-1", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolCallId));
    }

    [Fact]
    public void ToolComplete_OnFailure_SetsErrorStatusAndMessage()
    {
        using Activity parent = _parentSource.StartActivity("parent")!;
        CopilotSessionTelemetry.TelemetryState state = CopilotSessionTelemetry.CreateForTesting(parent);

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

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool BoomTool");
        Assert.Equal(ActivityStatusCode.Error, toolSpan.Status);
        Assert.Equal("kaboom", toolSpan.StatusDescription);
    }

    [Fact]
    public void AssistantUsage_TagsParentTurn()
    {
        using Activity parent = _parentSource.StartActivity("turn")!;
        CopilotSessionTelemetry.TelemetryState state = CopilotSessionTelemetry.CreateForTesting(parent);

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
        using Activity parent = _parentSource.StartActivity("turn")!;
        CopilotSessionTelemetry.TelemetryState state = CopilotSessionTelemetry.CreateForTesting(parent);

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
        using Activity parent = _parentSource.StartActivity("turn")!;
        CopilotSessionTelemetry.TelemetryState state = CopilotSessionTelemetry.CreateForTesting(parent);

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "leaked", ToolName = "Leaky" },
        });

        state.DisposeAll();

        Assert.Contains(_stopped, a => a.OperationName == "execute_tool Leaky");
    }
}
