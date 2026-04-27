namespace CopilotSdk.OpenTelemetry.Tests;

using GitHub.Copilot.SDK;

using System.Diagnostics;

using Xunit;

[Collection("ActivityListener")]
public class TelemetrySessionSubscriberTests : IDisposable
{
    private readonly List<Activity> _stopped = [];
    private readonly ActivityListener _listener;

    public TelemetrySessionSubscriberTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => _stopped.Add(activity),
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose() => _listener.Dispose();

    [Fact]
    public void ToolStart_CreatesExecuteToolSpan_WithCorrectAttributes()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "call-1", ToolName = "TbaApi.GetTeam" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "call-1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool TbaApi.GetTeam");
        Assert.Equal(turn!.TraceId, toolSpan.TraceId);
        Assert.Equal(turn.SpanId, toolSpan.ParentSpanId);
        Assert.Equal(ActivityStatusCode.Ok, toolSpan.Status);
        Assert.Equal("TbaApi.GetTeam", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolName));
        Assert.Equal("call-1", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ToolCallId));
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.ExecuteTool, toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName));
        Assert.Equal(CopilotSdkOpenTelemetry.GenAiProviderValue, toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName));
    }

    [Fact]
    public void ToolComplete_Success_SetsOkStatus()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "c1", ToolName = "SomeTool" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "c1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool SomeTool");
        Assert.Equal(ActivityStatusCode.Ok, toolSpan.Status);
    }

    [Fact]
    public void ToolComplete_Failure_SetsErrorStatusAndMessage()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

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
    public void ToolSpan_ParentsUnderAgentSpan_WhenAgentActive()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        // Start an agent
        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "test-agent", AgentDisplayName = "test-agent", AgentDescription = "", ToolCallId = "agent-1" },
        });

        // Now start a tool — should parent under the agent, not the turn
        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "tool-1", ToolName = "MyTool" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "tool-1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool MyTool");

        // Clean up agent so it lands in _stopped
        state.Handle(new SubagentCompletedEvent
        {
            Data = new SubagentCompletedData { AgentName = "test-agent", AgentDisplayName = "test-agent", ToolCallId = "agent-1" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent test-agent");
        Assert.Equal(agentSpan.SpanId, toolSpan.ParentSpanId);
    }

    [Fact]
    public void ToolSpan_ParentsUnderTurn_WhenNoAgentActive()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "tool-1", ToolName = "DirectTool" },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "tool-1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool DirectTool");
        Assert.Equal(turn!.SpanId, toolSpan.ParentSpanId);
    }

    [Fact]
    public void SubagentStarted_CreatesInvokeAgentSpan()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "explore", AgentDisplayName = "explore", AgentDescription = "", ToolCallId = "a-1" },
        });

        // Must still be running — stop it so we can inspect
        state.Handle(new SubagentCompletedEvent
        {
            Data = new SubagentCompletedData { AgentName = "explore", AgentDisplayName = "explore", ToolCallId = "a-1" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent explore");
        Assert.Equal(CopilotSdkOpenTelemetry.Operations.InvokeAgent, agentSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.OperationName));
        Assert.Equal(CopilotSdkOpenTelemetry.GenAiProviderValue, agentSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ProviderName));
        Assert.Equal("explore", agentSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.AgentName));
        Assert.Equal("a-1", agentSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.AgentId));
    }

    [Fact]
    public void SubagentSelected_BufferedData_AppliedOnStart()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SubagentSelectedEvent
        {
            Data = new SubagentSelectedData
            {
                AgentName = "explore",
                AgentDisplayName = "Explorer Agent",
                Tools = ["grep", "glob", "view"],
            },
        });
        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "explore", AgentDisplayName = "explore", AgentDescription = "", ToolCallId = "a-1" },
        });
        state.Handle(new SubagentCompletedEvent
        {
            Data = new SubagentCompletedData { AgentName = "explore", AgentDisplayName = "explore", ToolCallId = "a-1" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent explore");
        ActivityEvent selectedEvent = agentSpan.Events.Single(e => e.Name == "agent.selected");
        Assert.Equal("Explorer Agent", selectedEvent.Tags.Single(t => t.Key == "agent.display_name").Value);
        Assert.Equal("grep,glob,view", selectedEvent.Tags.Single(t => t.Key == "agent.tools").Value);
    }

    [Fact]
    public void SubagentCompleted_SetsOkStatus()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "task", AgentDisplayName = "task", AgentDescription = "", ToolCallId = "a-1" },
        });
        state.Handle(new SubagentCompletedEvent
        {
            Data = new SubagentCompletedData { AgentName = "task", AgentDisplayName = "task", ToolCallId = "a-1" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent task");
        Assert.Equal(ActivityStatusCode.Ok, agentSpan.Status);
    }

    [Fact]
    public void SubagentFailed_SetsErrorStatus()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "failing-agent", AgentDisplayName = "failing-agent", AgentDescription = "", ToolCallId = "a-1" },
        });
        state.Handle(new SubagentFailedEvent
        {
            Data = new SubagentFailedData { AgentName = "failing-agent", AgentDisplayName = "failing-agent", ToolCallId = "a-1", Error = "something broke" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent failing-agent");
        Assert.Equal(ActivityStatusCode.Error, agentSpan.Status);
        Assert.Equal("something broke", agentSpan.StatusDescription);
    }

    [Fact]
    public void McpTool_HasMcpServerNameAttribute()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData
            {
                ToolCallId = "m1",
                ToolName = "read_file",
                McpServerName = "filesystem",
            },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "m1", Success = true },
        });

        Activity toolSpan = _stopped.Single(a => a.OperationName == "execute_tool read_file");
        Assert.Equal("filesystem", toolSpan.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.McpServerName));
    }

    [Fact]
    public void MultipleConcurrentTools_TrackedIndependently()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "t1", ToolName = "ToolA" },
        });
        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "t2", ToolName = "ToolB" },
        });

        // Complete in reverse order
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "t2", Success = true },
        });
        state.Handle(new ToolExecutionCompleteEvent
        {
            Data = new ToolExecutionCompleteData { ToolCallId = "t1", Success = false, Error = new ToolExecutionCompleteDataError { Message = "oops" } },
        });

        Activity toolA = _stopped.Single(a => a.OperationName == "execute_tool ToolA");
        Activity toolB = _stopped.Single(a => a.OperationName == "execute_tool ToolB");
        Assert.Equal(ActivityStatusCode.Error, toolA.Status);
        Assert.Equal(ActivityStatusCode.Ok, toolB.Status);
    }

    [Fact]
    public void AssistantUsage_AddsTokenAttributes_ToCurrentActivity()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new AssistantUsageEvent
        {
            Data = new AssistantUsageData { Model = "gpt-5", InputTokens = 123, OutputTokens = 456 },
        });

        Assert.Equal("gpt-5", turn!.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.RequestModel));
        Assert.Equal("gpt-5", turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ResponseModel));
        Assert.Equal(123, Convert.ToInt64(turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageInputTokens)));
        Assert.Equal(456, Convert.ToInt64(turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageOutputTokens)));
        Assert.Equal("chat gpt-5", turn.DisplayName);
    }

    [Fact]
    public void SkillInvoked_AddsEventToAgentSpan()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "agent-1", AgentDisplayName = "agent-1", AgentDescription = "", ToolCallId = "a-1" },
        });

        state.Handle(new SkillInvokedEvent
        {
            Data = new SkillInvokedData { Name = "pdf", PluginName = "document", Path = "", Content = "", AllowedTools = ["view", "grep"] },
        });

        state.Handle(new SubagentCompletedEvent
        {
            Data = new SubagentCompletedData { AgentName = "agent-1", AgentDisplayName = "agent-1", ToolCallId = "a-1" },
        });

        Activity agentSpan = _stopped.Single(a => a.OperationName == "invoke_agent agent-1");
        ActivityEvent skillEvent = agentSpan.Events.Single(e => e.Name == "gen_ai.skill.invoked");
        Assert.Equal("pdf", skillEvent.Tags.Single(t => t.Key == "skill.name").Value);
        Assert.Equal("document", skillEvent.Tags.Single(t => t.Key == "skill.plugin_name").Value);
        Assert.Equal("view,grep", skillEvent.Tags.Single(t => t.Key == "skill.allowed_tools").Value);
    }

    [Fact]
    public void SessionError_SetsErrorStatus_OnCurrentActivity()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new SessionErrorEvent
        {
            Data = new SessionErrorData { ErrorType = "RateLimited", Message = "slow down" },
        });

        Assert.Equal(ActivityStatusCode.Error, turn!.Status);
        Assert.Equal("slow down", turn.StatusDescription);
        Assert.Equal("RateLimited", turn.GetTagItem("error.type"));
    }

    [Fact]
    public void DisposeAll_ClosesInflightSpans()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "leaked", ToolName = "Leaky" },
        });
        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "dangling", AgentDisplayName = "dangling", AgentDescription = "", ToolCallId = "a-1" },
        });

        state.DisposeAll();

        Assert.Contains(_stopped, a => a.OperationName == "execute_tool Leaky");
        Assert.Contains(_stopped, a => a.OperationName == "invoke_agent dangling");
    }

    [Fact]
    public void RecordSensitiveData_False_DoesNotRecordContent()
    {
        // Default options have RecordSensitiveData=false — verify no content attributes are set
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting(new CopilotSdkOpenTelemetryOptions { RecordSensitiveData = false });

        state.Handle(new UserMessageEvent { Data = new UserMessageData { Content = "secret prompt" } });
        state.Handle(new AssistantMessageEvent { Data = new AssistantMessageData { MessageId = "m1", Content = "secret reply" } });

        Assert.Null(turn!.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ContentPrompt));
        Assert.Null(turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ContentCompletion));
    }

    [Fact]
    public void RecordSensitiveData_True_RecordsPromptAndCompletion()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting(new CopilotSdkOpenTelemetryOptions { RecordSensitiveData = true });

        state.Handle(new UserMessageEvent { Data = new UserMessageData { Content = "Hello, world" } });
        state.Handle(new AssistantMessageEvent { Data = new AssistantMessageData { MessageId = "m1", Content = "Hi there!" } });

        Assert.Equal("Hello, world", turn!.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ContentPrompt));
        Assert.Equal("Hi there!", turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.ContentCompletion));
    }

    [Fact]
    public void Usage_RecordsCachedTokens()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new AssistantUsageEvent
        {
            Data = new AssistantUsageData
            {
                Model = "gpt-5",
                InputTokens = 100,
                OutputTokens = 50,
                CacheReadTokens = 80,
                CacheWriteTokens = 20,
            },
        });

        Assert.Equal(100u, turn!.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageInputTokens));
        Assert.Equal(50u, turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageOutputTokens));
        Assert.Equal(80u, turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageCacheReadTokens));
        Assert.Equal(20u, turn.GetTagItem(CopilotSdkOpenTelemetry.GenAiAttributes.UsageCacheWriteTokens));
    }

    [Fact]
    public void ParallelToolStarts_AreSiblings_NotCascading()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "a", ToolName = "ToolA" },
        });
        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "b", ToolName = "ToolB" },
        });
        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "c", ToolName = "ToolC" },
        });

        state.Handle(new ToolExecutionCompleteEvent { Data = new ToolExecutionCompleteData { ToolCallId = "a", Success = true } });
        state.Handle(new ToolExecutionCompleteEvent { Data = new ToolExecutionCompleteData { ToolCallId = "b", Success = true } });
        state.Handle(new ToolExecutionCompleteEvent { Data = new ToolExecutionCompleteData { ToolCallId = "c", Success = true } });

        Activity toolA = _stopped.Single(a => a.OperationName == "execute_tool ToolA");
        Activity toolB = _stopped.Single(a => a.OperationName == "execute_tool ToolB");
        Activity toolC = _stopped.Single(a => a.OperationName == "execute_tool ToolC");

        // All three should be siblings under the turn span, not cascading
        Assert.Equal(turn!.SpanId, toolA.ParentSpanId);
        Assert.Equal(turn.SpanId, toolB.ParentSpanId);
        Assert.Equal(turn.SpanId, toolC.ParentSpanId);
    }

    [Fact]
    public void OnToolStart_DoesNotLeakActivityCurrent()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        Activity? before = Activity.Current;

        state.Handle(new ToolExecutionStartEvent
        {
            Data = new ToolExecutionStartData { ToolCallId = "x", ToolName = "SomeTool" },
        });

        Assert.Same(before, Activity.Current);

        state.Handle(new ToolExecutionCompleteEvent { Data = new ToolExecutionCompleteData { ToolCallId = "x", Success = true } });
    }

    [Fact]
    public void OnSubagentStarted_DoesNotLeakActivityCurrent()
    {
        using Activity? turn = CopilotSdkOpenTelemetry.ActivitySource.StartActivity("chat", ActivityKind.Client);
        var state = TelemetrySessionSubscriber.CreateStateForTesting();

        Activity? before = Activity.Current;

        state.Handle(new SubagentStartedEvent
        {
            Data = new SubagentStartedData { AgentName = "sub", AgentDisplayName = "Sub Agent", AgentDescription = "", ToolCallId = "s-1" },
        });

        Assert.Same(before, Activity.Current);

        state.Handle(new SubagentCompletedEvent { Data = new SubagentCompletedData() });
    }
}
