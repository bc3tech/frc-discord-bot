namespace ChatBot.Agents.Models;

using Microsoft.Extensions.AI;

using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed record DecisionEvaluationResult(
    [property: JsonPropertyName("decision")] string Decision,
    [property: JsonPropertyName("feedback")] string? Feedback = null)
{
    public bool RequiresRepair => string.Equals(Decision, "repair", StringComparison.OrdinalIgnoreCase);
}

internal static class DecisionEvaluationResultParser
{
    private static readonly JsonSerializerOptions s_jsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonElement s_jsonSchema = JsonDocument.Parse(
        """
        {
          "type": "object",
          "additionalProperties": false,
          "properties": {
            "decision": {
              "type": "string",
              "enum": ["accept", "repair"]
            },
            "feedback": {
              "anyOf": [
                {
                  "type": "string",
                  "maxLength": 160
                },
                {
                  "type": "null"
                }
              ]
            }
          },
          "required": ["decision", "feedback"]
        }
        """).RootElement.Clone();

    public static ChatResponseFormat ResponseFormat { get; } = ChatResponseFormat.ForJsonSchema(
        s_jsonSchema,
        schemaName: "decision_evaluation_result",
        schemaDescription: "Workflow decision evaluator result with an accept or repair decision and optional repair feedback.");

    public static bool TryParse(
        string payload,
        out DecisionEvaluationResult? result,
        out JsonException? exception,
        out bool recoveredMalformedJson)
        => JsonPayloadParser.TryParse(payload, s_jsonOptions, out result, out exception, out recoveredMalformedJson);
}
