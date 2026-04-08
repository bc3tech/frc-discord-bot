namespace ChatBot.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;

internal sealed class AiOptions
{
    [Required]
    public required Uri ProjectEndpoint { get; set; } = null!;

    [Required]
    public required string AgentId { get; set; } = string.Empty;

    [Required]
    public required string MealSignupGeniusId { get; set; } = string.Empty;

    [Required]
    public required string LocalAgentModel { get; set; } = string.Empty;

    [Required]
    public required string OpenAIApiVersion { get; set; } = "2025-06-01";

    [Range(1, int.MaxValue)]
    public int MaxLocalAgentHandoffs { get; set; } = 6;

    [Range(1, int.MaxValue)]
    public int MaxWorkflowSteps { get; set; } = 24;

    [Range(1, int.MaxValue)]
    public int WorkflowSoftTimeoutSeconds { get; set; } = 15;

    [Range(1, int.MaxValue)]
    public int WorkflowHardTimeoutSeconds { get; set; } = 20;

    [Range(0, int.MaxValue)]
    public int MaxPrematureLocalLookupRetries { get; set; } = 1;

    public AiEvaluationSettings EvaluationSettings { get; init; } = new();

    [Range(1, int.MaxValue)]
    public required int DefaultTeamNumber { get; set; }
}

internal sealed class AiEvaluationSettings
{
    public string? Model { get; set; }

    [Range(0, int.MaxValue)]
    public int MaxAnswerEvaluationRetries { get; set; } = 1;

    [Range(1, int.MaxValue)]
    public int TimeoutSeconds { get; set; } = 8;
}

internal sealed class ConfigureAiOptions(IConfiguration configuration) : IConfigureOptions<AiOptions>
{
    public void Configure(AiOptions options)
    {
        options.ProjectEndpoint = GetAbsoluteUri(configuration, ChatBotConstants.Configuration.AI.Azure.ProjectEndpoint);
        options.AgentId = GetConfiguredValue(configuration, ChatBotConstants.Configuration.AI.Azure.AgentId);
        options.MealSignupGeniusId = GetConfiguredValue(configuration, ChatBotConstants.Configuration.AI.Azure.MealSignupGeniusId);
        options.LocalAgentModel = GetConfiguredValue(configuration, ChatBotConstants.Configuration.AI.Azure.LocalAgentModel);
        options.OpenAIApiVersion = GetConfiguredValue(configuration, ChatBotConstants.Configuration.AI.Azure.OpenAIApiVersion);

        options.MaxLocalAgentHandoffs = GetConfiguredIntOrDefault(
            configuration,
            options.MaxLocalAgentHandoffs,
            ChatBotConstants.Configuration.AI.Azure.MaxLocalAgentHandoffs);
        options.MaxWorkflowSteps = GetConfiguredIntOrDefault(
            configuration,
            options.MaxWorkflowSteps,
            ChatBotConstants.Configuration.AI.Azure.MaxWorkflowSteps);
        options.WorkflowSoftTimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.WorkflowSoftTimeoutSeconds,
            ChatBotConstants.Configuration.AI.Azure.WorkflowSoftTimeoutSeconds);
        options.WorkflowHardTimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.WorkflowHardTimeoutSeconds,
            ChatBotConstants.Configuration.AI.Azure.WorkflowHardTimeoutSeconds);
        options.MaxPrematureLocalLookupRetries = GetConfiguredIntOrDefault(
            configuration,
            options.MaxPrematureLocalLookupRetries,
            ChatBotConstants.Configuration.AI.Azure.MaxPrematureLocalLookupRetries);
        options.EvaluationSettings.Model = GetConfiguredValue(configuration, ChatBotConstants.Configuration.AI.Azure.EvaluationSettings.Model);
        options.EvaluationSettings.MaxAnswerEvaluationRetries = GetConfiguredIntOrDefault(
            configuration,
            options.EvaluationSettings.MaxAnswerEvaluationRetries,
            ChatBotConstants.Configuration.AI.Azure.EvaluationSettings.MaxAnswerEvaluationRetries);
        options.EvaluationSettings.TimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.EvaluationSettings.TimeoutSeconds,
            ChatBotConstants.Configuration.AI.Azure.EvaluationSettings.TimeoutSeconds);

        options.DefaultTeamNumber = GetConfiguredInt(configuration, ChatBotConstants.Configuration.DefaultTeamNumber);
    }

    private static Uri GetAbsoluteUri(IConfiguration configuration, params string[] keys)
    {
        string value = GetConfiguredValue(configuration, keys);
        return Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
            ? uri
            : null!;
    }

    private static string GetConfiguredValue(IConfiguration configuration, params string[] keys)
    {
        foreach (string key in keys)
        {
            string? value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return string.Empty;
    }

    private static int GetConfiguredInt(IConfiguration configuration, params string[] keys)
    {
        foreach (string key in keys)
        {
            if (int.TryParse(configuration[key], out int value))
            {
                return value;
            }
        }

        return 0;
    }

    private static int GetConfiguredIntOrDefault(IConfiguration configuration, int fallbackValue, params string[] keys)
    {
        foreach (string key in keys)
        {
            if (int.TryParse(configuration[key], out int value))
            {
                return value;
            }
        }

        return fallbackValue;
    }
}

internal sealed class ValidateAiOptions : IValidateOptions<AiOptions>
{
    public ValidateOptionsResult Validate(string? name, AiOptions options)
    {
        var retVal = new ValidateOptionsResultBuilder();

        if (options.ProjectEndpoint is null || string.IsNullOrWhiteSpace(options.ProjectEndpoint.AbsoluteUri))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.ProjectEndpoint)}' is missing or empty."));
        }
        else if (!options.ProjectEndpoint.IsAbsoluteUri)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.ProjectEndpoint)}' must be an absolute URI"));
        }

        if (string.IsNullOrWhiteSpace(options.AgentId))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.AgentId)}' is missing or empty."));
        }

        if (string.IsNullOrWhiteSpace(options.LocalAgentModel))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.LocalAgentModel)}' is missing or empty."));
        }

        if (string.IsNullOrWhiteSpace(options.OpenAIApiVersion))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.OpenAIApiVersion)}' is missing or empty."));
        }

        if (options.MaxLocalAgentHandoffs <= 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.MaxLocalAgentHandoffs)}' must be greater than zero."));
        }

        if (options.MaxWorkflowSteps <= 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.MaxWorkflowSteps)}' must be greater than zero."));
        }

        if (options.WorkflowSoftTimeoutSeconds <= 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.WorkflowSoftTimeoutSeconds)}' must be greater than zero."));
        }

        if (options.WorkflowHardTimeoutSeconds <= options.WorkflowSoftTimeoutSeconds)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.WorkflowHardTimeoutSeconds)}' must be greater than '{nameof(AiOptions.WorkflowSoftTimeoutSeconds)}'."));
        }

        if (options.MaxPrematureLocalLookupRetries < 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.MaxPrematureLocalLookupRetries)}' must be zero or greater."));
        }

        if (options.EvaluationSettings.MaxAnswerEvaluationRetries < 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.EvaluationSettings)}.{nameof(AiEvaluationSettings.MaxAnswerEvaluationRetries)}' must be zero or greater."));
        }

        if (options.EvaluationSettings.TimeoutSeconds <= 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.EvaluationSettings)}.{nameof(AiEvaluationSettings.TimeoutSeconds)}' must be greater than zero."));
        }

        if (options.DefaultTeamNumber <= 0)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.DefaultTeamNumber)}' must be a positive team number."));
        }

        return retVal.Build();
    }
}
