namespace ChatBot.Agents.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed record FoundryAgentResult(
    [property: JsonPropertyName("next_step")] string NextStep,
    [property: JsonPropertyName("reason")] string? Reason = null,
    [property: JsonPropertyName("target_input")] string? TargetInput = null,
    [property: JsonPropertyName("question")] string? Question = null,
    [property: JsonPropertyName("answer")] string? Answer = null,
    [property: JsonPropertyName("messageToUser")] string? MessageToUser = null,
    [property: JsonPropertyName("hosted_source")] string? HostedSource = null,
    [property: JsonPropertyName("skip_evaluator")] bool? SkipEvaluator = null);

internal static class FoundryAgentResultParser
{
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);

    public static bool TryParse(
        string payload,
        out FoundryAgentResult? result,
        out JsonException? exception,
        out bool recoveredMalformedJson)
        => JsonPayloadParser.TryParse(payload, s_jsonOptions, out result, out exception, out recoveredMalformedJson);
}
