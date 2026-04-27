namespace CopilotSdk.OpenTelemetry;

/// <summary>
/// Configuration options for the CopilotSdk.OpenTelemetry library.
/// Register via <c>services.AddCopilotSdkOpenTelemetry(opts => opts.RecordSensitiveData = true)</c>.
/// </summary>
public sealed class CopilotSdkOpenTelemetryOptions
{
    /// <summary>
    /// When <c>true</c>, prompt and completion content are recorded as span attributes
    /// (<c>gen_ai.content.prompt</c>, <c>gen_ai.content.completion</c>).
    /// Default is <c>false</c> — sensitive data is never emitted unless explicitly opted in.
    /// </summary>
    public bool RecordSensitiveData { get; set; }
}
