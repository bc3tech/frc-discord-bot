namespace ChatBot;

using global::OpenTelemetry;

using System.Diagnostics;

public sealed class AzureIdentityActivityFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity) => Suppress(activity, clearRecordedFlag: false);

    public override void OnEnd(Activity activity) => Suppress(activity, clearRecordedFlag: true);

    private static void Suppress(Activity activity, bool clearRecordedFlag)
    {
        if (!ShouldSuppress(activity))
        {
            return;
        }

        activity.IsAllDataRequested = false;
        if (clearRecordedFlag)
        {
            activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
        }
    }

    private static bool ShouldSuppress(Activity activity)
        => activity.Source.Name.StartsWith("Azure.Identity", StringComparison.Ordinal)
            || string.Equals(activity.DisplayName, "DefaultAzureCredential.GetToken", StringComparison.Ordinal);
}
