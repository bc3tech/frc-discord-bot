namespace ChatBot.Agents;

using ChatBot;

using Microsoft.Extensions.Logging;

internal sealed class PromptCatalog(ILogger<PromptCatalog> logger)
{
    private static readonly string PromptRoot = Path.Combine(AppContext.BaseDirectory, "ChatBot", "Agents", "prompts");
    private readonly ILogger<PromptCatalog> _logger = logger;

    public string AnswerEvaluationRepairPrompt { get; } = LoadRequiredPrompt("answer-evaluation-repair-prompt.md");
    public string AnswerEvaluatorSystemPrompt { get; } = LoadRequiredPrompt("answer-evaluator-system-prompt.md");
    public string AnswerEvaluatorUserPrompt { get; } = LoadRequiredPrompt("answer-evaluator-user-prompt.md");
    public string AskUserDecisionEvaluatorSystemPrompt { get; } = LoadRequiredPrompt("ask-user-decision-evaluator-system-prompt.md");
    public string AskUserDecisionEvaluatorUserPrompt { get; } = LoadRequiredPrompt("ask-user-decision-evaluator-user-prompt.md");
    public string AskUserDecisionRepairPrompt { get; } = LoadRequiredPrompt("ask-user-decision-repair-prompt.md");
    public string FoundryContinuationMessage { get; } = LoadRequiredPrompt("foundry-continuation-message.md");
    public string FoundryTurnSystemMessage { get; } = LoadRequiredPrompt("foundry-turn-system-message.md");
    public string InvalidEvaluatorPayloadRetryPrompt { get; } = LoadRequiredPrompt("invalid-evaluator-payload-retry-prompt.md");
    public string InvalidJsonRetryPrompt { get; } = LoadRequiredPrompt("invalid-json-retry-prompt.md");
    public string LocalAgentContextMessage { get; } = LoadRequiredPrompt("local-agent-context-message.md");
    public string PrematureLocalLookupRetryPrompt { get; } = LoadRequiredPrompt("premature-local-lookup-retry-prompt.md");
    public string UserContextMessage { get; } = LoadRequiredPrompt("user-context-message.md");
    public string WorkflowSoftTimeoutPrompt { get; } = LoadRequiredPrompt("workflow-soft-timeout-prompt.md");

    public string Format(string template, params (string Token, string? Value)[] replacements)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(template);

        _logger.FormattingPromptTemplate(replacements.Length);

        string formatted = template;
        foreach ((string token, string? value) in replacements)
        {
            formatted = formatted.Replace($"[[{token}]]", value ?? string.Empty, StringComparison.Ordinal);
        }

        if (formatted.Contains("[[", StringComparison.Ordinal) && formatted.Contains("]]", StringComparison.Ordinal))
        {
            _logger.FormattedPromptTemplateStillContainsUnreplacedTokens(replacements.Length);
        }

        return formatted;
    }

    private static string LoadRequiredPrompt(string fileName)
    {
        string path = Path.Combine(PromptRoot, fileName);
        return File.Exists(path)
            ? File.ReadAllText(path).ReplaceLineEndings("\n").Trim()
            : throw new FileNotFoundException($"Required chatbot prompt file was not found: {path}", path);
    }
}
