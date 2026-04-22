namespace CopilotSdk.OpenTelemetry.Spike;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using GitHub.Copilot.SDK;

using Xunit;

/// <summary>
/// Throwaway spike — answers the four decision-gate questions G1-G4 from
/// <c>docs/plans/2026-04-22-001-feat-copilot-tool-call-sub-spans-plan.md</c> against
/// the real <see cref="CopilotClient"/> using a live GitHub Models PAT.
/// </summary>
/// <remarks>
/// <para>
/// <b>G1.</b> Does <see cref="CopilotClient.On(Action{SessionLifecycleEvent})"/> fire
/// for sessions created via <see cref="CopilotClient.CreateSessionAsync"/> on the same
/// client instance?
/// </para>
/// <para>
/// <b>G2.</b> Does the <see cref="CopilotSession"/> returned by
/// <see cref="CopilotClient.ResumeSessionAsync"/> deliver the live event stream
/// (including events that occur *after* the resume call)?
/// </para>
/// <para>
/// <b>G3.</b> Does the lifecycle callback fire early enough to subscribe before the
/// first <see cref="ToolExecutionStartEvent"/>?
/// </para>
/// <para>
/// <b>G4.</b> Is <see cref="Activity.Current"/> in the lifecycle callback the in-flight
/// per-turn span (i.e., on the calling async context)?
/// </para>
/// <para>
/// Skipped unless <c>GITHUB_TOKEN</c> is set. Findings are written to
/// <c>spike-findings.md</c> in the test working directory and printed to xUnit output.
/// </para>
/// </remarks>
[Trait("Category", "live")]
public sealed class LifecycleResumeSpike
{
    private static readonly ActivitySource SpikeSource = new("CopilotSdk.OpenTelemetry.Spike");

    [Fact]
    public async Task RunSpike()
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            Assert.Skip("GITHUB_TOKEN not set; live spike skipped.");
            return;
        }

        var observations = new List<string>();
        var lifecycleLog = new ConcurrentQueue<LifecycleSample>();
        var resumedSessionLog = new ConcurrentQueue<EventSample>();
        var primaryEventLog = new ConcurrentQueue<EventSample>();

        var spikeStart = DateTimeOffset.UtcNow;
        var swStart = Stopwatch.GetTimestamp();

        long ElapsedMs() => (long)((Stopwatch.GetTimestamp() - swStart) * 1000.0 / Stopwatch.Frequency);

        observations.Add($"Spike started at {spikeStart:o}");

        await using var client = new CopilotClient(new CopilotClientOptions
        {
            AutoStart = true,
            UseStdio = true,
            GitHubToken = token,
        });

        // Listen with an ActivityListener so we can confirm Activity propagation.
        var allActivities = new ConcurrentQueue<(string Op, string? Id, string? ParentId)>();
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = a => allActivities.Enqueue((a.OperationName, a.Id, a.ParentId)),
        };
        ActivitySource.AddActivityListener(activityListener);

        // Open a parent span BEFORE wiring lifecycle subscription so we can observe
        // whether Activity.Current flows through to the lifecycle callback.
        using Activity? turnSpan = SpikeSource.StartActivity("spike.turn", ActivityKind.Internal);
        observations.Add($"Opened spike.turn activity: id={turnSpan?.Id ?? "<null>"} traceId={turnSpan?.TraceId.ToString() ?? "<null>"}");

        IDisposable? resumeSubscription = null;
        var lifecycleHandlerInvocations = 0;

        using IDisposable lifecycleSubscription = client.On((SessionLifecycleEvent evt) =>
        {
            Interlocked.Increment(ref lifecycleHandlerInvocations);
            var sample = new LifecycleSample(
                ElapsedMs(),
                Environment.CurrentManagedThreadId,
                Activity.Current?.Id,
                Activity.Current?.OperationName,
                evt.Type,
                evt.SessionId,
                evt.Metadata?.StartTime,
                evt.Metadata?.ModifiedTime,
                evt.Metadata?.Summary);
            lifecycleLog.Enqueue(sample);

            // For session.created on the *new* session, try to resume and subscribe.
            if (string.Equals(evt.Type, SessionLifecycleEventTypes.Created, StringComparison.Ordinal))
            {
                try
                {
                    // Fire-and-forget resume so we don't block the lifecycle dispatcher.
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            CopilotSession resumed = await client.ResumeSessionAsync(
                                evt.SessionId,
                                new ResumeSessionConfig
                                {
                                    OnPermissionRequest = PermissionHandler.ApproveAll,
                                    Provider = new ProviderConfig
                                    {
                                        Type = "openai",
                                        WireApi = "chat-completions",
                                        BaseUrl = "https://models.github.ai/inference",
                                        ApiKey = token,
                                    },
                                    Model = "openai/gpt-4o-mini",
                                    OnEvent = re => resumedSessionLog.Enqueue(new EventSample(
                                        ElapsedMs(),
                                        Environment.CurrentManagedThreadId,
                                        Activity.Current?.Id,
                                        re.GetType().Name,
                                        SummarizeEvent(re))),
                                },
                                CancellationToken.None);

                            // Belt-and-suspenders: also subscribe via .On() to compare
                            // OnEvent vs explicit subscription delivery.
                            resumeSubscription = resumed.On(re =>
                            {
                                resumedSessionLog.Enqueue(new EventSample(
                                    ElapsedMs(),
                                    Environment.CurrentManagedThreadId,
                                    Activity.Current?.Id,
                                    "[via .On()] " + re.GetType().Name,
                                    SummarizeEvent(re)));
                            });
                        }
                        catch (Exception ex)
                        {
                            observations.Add($"ResumeSessionAsync threw: {ex.GetType().Name}: {ex.Message}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    observations.Add($"Lifecycle resume scheduling threw: {ex.GetType().Name}: {ex.Message}");
                }
            }
        });

        // Build a session that uses GitHub Models with the supplied PAT.
        var sessionConfig = new SessionConfig
        {
            Provider = new ProviderConfig
            {
                Type = "openai",
                WireApi = "chat-completions",
                BaseUrl = "https://models.github.ai/inference",
                ApiKey = token,
            },
            Model = "openai/gpt-4o-mini",
            OnPermissionRequest = PermissionHandler.ApproveAll,
        };

        observations.Add($"[t+{ElapsedMs()}ms] Calling CreateSessionAsync (Activity.Current={Activity.Current?.Id ?? "<null>"})");

        CopilotSession session;
        try
        {
            session = await client.CreateSessionAsync(sessionConfig, CancellationToken.None);
        }
        catch (Exception ex)
        {
            observations.Add($"CreateSessionAsync threw: {ex.GetType().Name}: {ex.Message}");
            await WriteFindingsAsync(observations, lifecycleLog, primaryEventLog, resumedSessionLog, allActivities, fatal: true);
            throw;
        }

        observations.Add($"[t+{ElapsedMs()}ms] CreateSessionAsync returned. SessionId={session.SessionId}");

        await using var sessionDisposal = session.ConfigureAwait(false);

        // Subscribe to the primary handle (control sample for G2 — what events does the
        // primary handle deliver vs. the resumed handle?).
        using IDisposable primarySub = session.On(e =>
        {
            primaryEventLog.Enqueue(new EventSample(
                ElapsedMs(),
                Environment.CurrentManagedThreadId,
                Activity.Current?.Id,
                e.GetType().Name,
                SummarizeEvent(e)));
        });

        // Drive a prompt that exercises the event stream. We avoid tool registration to
        // keep the spike self-contained; assistant deltas + usage events are sufficient
        // signal for G2/G3.
        var messageOptions = new MessageOptions
        {
            Prompt = "Reply with the single word 'hello' and nothing else.",
        };

        try
        {
            observations.Add($"[t+{ElapsedMs()}ms] Calling SendAndWaitAsync");
            AssistantMessageEvent? reply = await session.SendAndWaitAsync(
                messageOptions,
                TimeSpan.FromSeconds(60),
                CancellationToken.None);
            observations.Add($"[t+{ElapsedMs()}ms] SendAndWaitAsync returned. Reply: {reply?.Data?.Content?.Trim() ?? "<null>"}");
        }
        catch (Exception ex)
        {
            observations.Add($"SendAndWaitAsync threw: {ex.GetType().Name}: {ex.Message}");
        }

        // Give the resume subscription a moment to receive any final events.
        await Task.Delay(500);

        resumeSubscription?.Dispose();

        await WriteFindingsAsync(observations, lifecycleLog, primaryEventLog, resumedSessionLog, allActivities, fatal: false);

        // Sanity: at minimum we want to have invoked the lifecycle handler at least once.
        Assert.True(lifecycleHandlerInvocations > 0, "Lifecycle handler was never invoked. See spike-findings.md for details.");
    }

    private static string SummarizeEvent(SessionEvent e) => e switch
    {
        ToolExecutionStartEvent t => $"toolName={t.Data?.ToolName} callId={t.Data?.ToolCallId}",
        ToolExecutionCompleteEvent t => $"callId={t.Data?.ToolCallId} success={t.Data?.Success}",
        AssistantUsageEvent u => $"model={u.Data?.Model} in={u.Data?.InputTokens} out={u.Data?.OutputTokens}",
        AssistantMessageEvent a => $"len={a.Data?.Content?.Length ?? 0}",
        AssistantMessageDeltaEvent d => $"deltaLen={d.Data?.DeltaContent?.Length ?? 0}",
        SessionErrorEvent err => $"errType={err.Data?.ErrorType} msg={err.Data?.Message}",
        _ => string.Empty,
    };

    private static async Task WriteFindingsAsync(
        IReadOnlyList<string> observations,
        IReadOnlyCollection<LifecycleSample> lifecycle,
        IReadOnlyCollection<EventSample> primary,
        IReadOnlyCollection<EventSample> resumed,
        IReadOnlyCollection<(string Op, string? Id, string? ParentId)> activities,
        bool fatal)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Spike Findings — lifecycle/resume validation").AppendLine();
        sb.AppendLine($"Run at: {DateTimeOffset.UtcNow:o}").AppendLine();
        sb.AppendLine($"Fatal error during run: {(fatal ? "YES — see Observations" : "no")}").AppendLine();

        sb.AppendLine("## Observations").AppendLine();
        foreach (var line in observations)
        {
            sb.Append("- ").AppendLine(line);
        }

        sb.AppendLine().AppendLine("## Lifecycle events").AppendLine();
        sb.AppendLine("| t+ms | thread | Activity.Current.Id | Activity.OperationName | Type | SessionId |");
        sb.AppendLine("|------|--------|---------------------|------------------------|------|-----------|");
        foreach (var s in lifecycle)
        {
            sb.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "| {0} | {1} | `{2}` | `{3}` | `{4}` | `{5}` |",
                s.ElapsedMs, s.ThreadId, s.ActivityId ?? "<null>", s.ActivityOperationName ?? "<null>", s.Type, s.SessionId));
        }

        sb.AppendLine().AppendLine("## Events on PRIMARY session handle").AppendLine();
        AppendEventTable(sb, primary);

        sb.AppendLine().AppendLine("## Events on RESUMED session handle").AppendLine();
        AppendEventTable(sb, resumed);

        sb.AppendLine().AppendLine("## All Activities observed").AppendLine();
        sb.AppendLine("| Op | Id | ParentId |");
        sb.AppendLine("|----|----|----------|");
        foreach (var (op, id, parentId) in activities)
        {
            sb.AppendLine($"| {op} | `{id ?? "<null>"}` | `{parentId ?? "<null>"}` |");
        }

        sb.AppendLine().AppendLine("## Decision Gate Answers").AppendLine();
        var g1 = lifecycle.Any(l => string.Equals(l.Type, SessionLifecycleEventTypes.Created, StringComparison.Ordinal));
        var g2 = resumed.Count > 0;
        var firstToolEventOnPrimary = primary
            .Where(e => string.Equals(e.EventType, nameof(ToolExecutionStartEvent), StringComparison.Ordinal))
            .Select(e => (long?)e.ElapsedMs)
            .FirstOrDefault();
        var firstLifecycleCreated = lifecycle
            .Where(l => string.Equals(l.Type, SessionLifecycleEventTypes.Created, StringComparison.Ordinal))
            .Select(l => (long?)l.ElapsedMs)
            .FirstOrDefault();
        var g3 = firstToolEventOnPrimary is null
            || (firstLifecycleCreated is not null && firstLifecycleCreated <= firstToolEventOnPrimary);
        var g4 = lifecycle.Any(l => l.ActivityId is not null);

        sb.AppendLine($"- **G1 (lifecycle fires for own client's CreateSessionAsync):** {YesNo(g1)}");
        sb.AppendLine($"- **G2 (resumed handle delivers events):** {YesNo(g2)}");
        sb.AppendLine($"- **G3 (lifecycle fires before first tool event):** {YesNo(g3)} (first lifecycle.created={firstLifecycleCreated?.ToString(CultureInfo.InvariantCulture) ?? "n/a"}ms, first tool start={firstToolEventOnPrimary?.ToString(CultureInfo.InvariantCulture) ?? "n/a"}ms)");
        sb.AppendLine($"- **G4 (Activity.Current set in lifecycle callback):** {YesNo(g4)}");

        sb.AppendLine().AppendLine("## Recommended Path").AppendLine();
        if (g1 && g2 && g3)
        {
            if (g4)
            {
                sb.AppendLine("**Path A** — lifecycle/resume path is fully viable. Activity.Current flows through.");
            }
            else
            {
                sb.AppendLine("**Path A with AsyncLocal correlation** — lifecycle path works but Activity.Current does not flow; capture per-turn Activity in an AsyncLocal at the BeginTurnAsync boundary and read it from the listener.");
            }
        }
        else
        {
            sb.AppendLine("**Path B** — fall back to the generic `OnSessionCreated` hook in `gpt/`.");
        }

        var path = Path.Combine(Environment.CurrentDirectory, "spike-findings.md");
        await File.WriteAllTextAsync(path, sb.ToString());
        Console.WriteLine($"Spike findings written to: {path}");
        Console.WriteLine(sb.ToString());
    }

    private static void AppendEventTable(StringBuilder sb, IReadOnlyCollection<EventSample> events)
    {
        sb.AppendLine("| t+ms | thread | Activity.Current.Id | EventType | Detail |");
        sb.AppendLine("|------|--------|---------------------|-----------|--------|");
        foreach (var e in events)
        {
            sb.AppendLine(string.Format(
                CultureInfo.InvariantCulture,
                "| {0} | {1} | `{2}` | `{3}` | {4} |",
                e.ElapsedMs, e.ThreadId, e.ActivityId ?? "<null>", e.EventType, e.Detail));
        }
    }

    private static string YesNo(bool b) => b ? "✅ YES" : "❌ NO";

    private sealed record LifecycleSample(
        long ElapsedMs,
        int ThreadId,
        string? ActivityId,
        string? ActivityOperationName,
        string Type,
        string SessionId,
        string? StartTime,
        string? ModifiedTime,
        string? Summary);

    private sealed record EventSample(
        long ElapsedMs,
        int ThreadId,
        string? ActivityId,
        string EventType,
        string Detail);
}
