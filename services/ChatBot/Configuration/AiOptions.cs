namespace ChatBot.Configuration;

using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;

internal sealed class AiOptions
{
    public AiCopilotSettings Copilot { get; init; } = new();

    public AiFoundrySettings Foundry { get; init; } = new();

    [Required]
    public required Uri FoundryEndpoint
    {
        get => this.Foundry.Endpoint;
        set => this.Foundry.Endpoint = value;
    }

    [Required]
    public required string AgentId
    {
        get => this.Foundry.AgentId;
        set => this.Foundry.AgentId = value;
    }

    [Required]
    public required string MealSignupGeniusId
    {
        get => this.Foundry.MealSignupGeniusId;
        set => this.Foundry.MealSignupGeniusId = value;
    }

    [Required]
    public required string LocalAgentModel
    {
        get => this.Foundry.LocalAgentModel;
        set => this.Foundry.LocalAgentModel = value;
    }

    [Required]
    public required string OpenAIApiVersion
    {
        get => this.Foundry.OpenAIApiVersion;
        set => this.Foundry.OpenAIApiVersion = value;
    }

    [Range(1, int.MaxValue)]
    public int MaxLocalAgentHandoffs
    {
        get => this.Foundry.MaxLocalAgentHandoffs;
        set => this.Foundry.MaxLocalAgentHandoffs = value;
    }

    [Range(1, int.MaxValue)]
    public int MaxWorkflowSteps
    {
        get => this.Foundry.MaxWorkflowSteps;
        set => this.Foundry.MaxWorkflowSteps = value;
    }

    [Range(1, int.MaxValue)]
    public int WorkflowSoftTimeoutSeconds
    {
        get => this.Foundry.WorkflowSoftTimeoutSeconds;
        set => this.Foundry.WorkflowSoftTimeoutSeconds = value;
    }

    [Range(1, int.MaxValue)]
    public int WorkflowHardTimeoutSeconds
    {
        get => this.Foundry.WorkflowHardTimeoutSeconds;
        set => this.Foundry.WorkflowHardTimeoutSeconds = value;
    }

    [Range(0, int.MaxValue)]
    public int MaxPrematureLocalLookupRetries
    {
        get => this.Foundry.MaxPrematureLocalLookupRetries;
        set => this.Foundry.MaxPrematureLocalLookupRetries = value;
    }

    public AiEvaluationSettings EvaluationSettings => this.Foundry.EvaluationSettings;

    [Range(1, int.MaxValue)]
    public required int DefaultTeamNumber { get; set; }
}

internal sealed class AiCopilotSettings
{
    [Required]
    public string Model { get; set; } = "gpt-5.4-mini";

    public string? ReasoningEffort { get; set; }

    public string? LogLevel { get; set; }
}

internal sealed class AiFoundrySettings
{
    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public string AgentId { get; set; } = string.Empty;

    [Required]
    public string MealSignupGeniusId { get; set; } = string.Empty;

    [Required]
    public string LocalAgentModel { get; set; } = string.Empty;

    [Required]
    public string OpenAIApiVersion { get; set; } = "2025-06-01";

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
        options.Copilot.Model = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Copilot.Model).UnlessNullOrWhitespaceThen(options.Copilot.Model);
        options.Copilot.ReasoningEffort = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Copilot.ReasoningEffort);
        options.Copilot.LogLevel = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Copilot.LogLevel);

        options.Foundry.Endpoint = GetAbsoluteUri(configuration, ChatBotConstants.Configuration.Foundry.Endpoint);
        options.Foundry.AgentId = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Foundry.AgentId);
        options.Foundry.MealSignupGeniusId = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Foundry.MealSignupGeniusId);
        options.Foundry.LocalAgentModel = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Foundry.LocalAgentModel);
        options.Foundry.OpenAIApiVersion = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Foundry.OpenAIApiVersion);

        options.Foundry.MaxLocalAgentHandoffs = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.MaxLocalAgentHandoffs,
            ChatBotConstants.Configuration.Foundry.MaxLocalAgentHandoffs);
        options.Foundry.MaxWorkflowSteps = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.MaxWorkflowSteps,
            ChatBotConstants.Configuration.Foundry.MaxWorkflowSteps);
        options.Foundry.WorkflowSoftTimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.WorkflowSoftTimeoutSeconds,
            ChatBotConstants.Configuration.Foundry.WorkflowSoftTimeoutSeconds);
        options.Foundry.WorkflowHardTimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.WorkflowHardTimeoutSeconds,
            ChatBotConstants.Configuration.Foundry.WorkflowHardTimeoutSeconds);
        options.Foundry.MaxPrematureLocalLookupRetries = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.MaxPrematureLocalLookupRetries,
            ChatBotConstants.Configuration.Foundry.MaxPrematureLocalLookupRetries);
        options.Foundry.EvaluationSettings.Model = GetConfiguredValue(configuration, ChatBotConstants.Configuration.Foundry.EvaluationSettings.Model);
        options.Foundry.EvaluationSettings.MaxAnswerEvaluationRetries = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.EvaluationSettings.MaxAnswerEvaluationRetries,
            ChatBotConstants.Configuration.Foundry.EvaluationSettings.MaxAnswerEvaluationRetries);
        options.Foundry.EvaluationSettings.TimeoutSeconds = GetConfiguredIntOrDefault(
            configuration,
            options.Foundry.EvaluationSettings.TimeoutSeconds,
            ChatBotConstants.Configuration.Foundry.EvaluationSettings.TimeoutSeconds);

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

        if (string.IsNullOrWhiteSpace(options.Copilot.Model))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.Copilot)}.{nameof(AiCopilotSettings.Model)}' is missing or empty."));
        }

        if (options.FoundryEndpoint is null || string.IsNullOrWhiteSpace(options.FoundryEndpoint.AbsoluteUri))
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"Required configuration value '{nameof(AiOptions.FoundryEndpoint)}' is missing or empty."));
        }
        else if (!options.FoundryEndpoint.IsAbsoluteUri)
        {
            retVal.AddResult(ValidateOptionsResult.Fail($"'{nameof(AiOptions.FoundryEndpoint)}' must be an absolute URI"));
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
