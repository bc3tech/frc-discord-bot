namespace FunctionApp.Tests;

using global::ChatBot;
using global::ChatBot.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public sealed class CopilotConfigurationTests
{
    [Fact]
    public void ConfigureAiOptionsBindsCopilotAndFoundrySettings()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("AI:Copilot:Model", "gpt-5"),
                new KeyValuePair<string, string?>("AI:Copilot:ReasoningEffort", "high"),
                new KeyValuePair<string, string?>("AI:Copilot:LogLevel", "debug"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.Foundry.Endpoint, "https://example.services.ai.azure.com/api/projects/test"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.Foundry.AgentId, "discord-bot"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.Foundry.MealSignupGeniusId, "signup-board"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.Foundry.LocalAgentModel, "gpt-5.4-mini"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.Foundry.OpenAIApiVersion, "2025-06-01"),
                new KeyValuePair<string, string?>(ChatBotConstants.Configuration.DefaultTeamNumber, "2046"),
            ])
            .Build();

        AiOptions options = new()
        {
            FoundryEndpoint = new Uri("https://bootstrap.invalid"),
            AgentId = "bootstrap",
            MealSignupGeniusId = "bootstrap",
            LocalAgentModel = "bootstrap",
            OpenAIApiVersion = "2025-06-01",
            DefaultTeamNumber = 1,
        };

        new ConfigureAiOptions(configuration).Configure(options);

        Assert.Equal("gpt-5", options.Copilot.Model);
        Assert.Equal("high", options.Copilot.ReasoningEffort);
        Assert.Equal("debug", options.Copilot.LogLevel);
        Assert.Equal(new Uri("https://example.services.ai.azure.com/api/projects/test"), options.Foundry.Endpoint);
        Assert.Equal("discord-bot", options.AgentId);
    }

    [Fact]
    public void ValidateAiOptionsWhenCopilotModelMissingReturnsFailure()
    {
        AiOptions options = new()
        {
            FoundryEndpoint = new Uri("https://example.services.ai.azure.com/api/projects/test"),
            AgentId = "discord-bot",
            MealSignupGeniusId = "signup-board",
            LocalAgentModel = "gpt-5.4-mini",
            OpenAIApiVersion = "2025-06-01",
            DefaultTeamNumber = 2046,
        };
        options.Copilot.Model = string.Empty;

        ValidateOptionsResult result = new ValidateAiOptions().Validate(name: null, options);

        Assert.False(result.Succeeded);
        Assert.Contains("Copilot.Model", string.Join(Environment.NewLine, result.Failures ?? []));
    }

    [Fact]
    public void HasValidChatBotConfigurationWhenRequiredAiSettingsPresentReturnsTrue()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("AI:Copilot:Model", "gpt-5"),
                new KeyValuePair<string, string?>("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/test"),
                new KeyValuePair<string, string?>("AI:Foundry:AgentId", "discord-bot"),
                new KeyValuePair<string, string?>("AI:Foundry:LocalAgentModel", "gpt-5.4-mini"),
                new KeyValuePair<string, string?>("AI:Foundry:OpenAIApiVersion", "2025-06-01"),
                new KeyValuePair<string, string?>("DefaultTeamNumber", "2046"),
            ])
            .Build();

        bool isValid = configuration.HasValidChatBotConfiguration(out string[] validationFailures);

        Assert.True(isValid);
        Assert.Empty(validationFailures);
    }

    [Fact]
    public void HasValidChatBotConfigurationWhenRequiredAiSettingsMissingReturnsFalse()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("AI:Copilot:Model", "gpt-5"),
                new KeyValuePair<string, string?>("AI:Foundry:Endpoint", "https://example.services.ai.azure.com/api/projects/test"),
                new KeyValuePair<string, string?>("DefaultTeamNumber", "2046"),
            ])
            .Build();

        bool isValid = configuration.HasValidChatBotConfiguration(out string[] validationFailures);

        Assert.False(isValid);
        Assert.Contains(validationFailures, failure => failure.Contains(nameof(AiOptions.AgentId), StringComparison.Ordinal));
        Assert.Contains(validationFailures, failure => failure.Contains(nameof(AiOptions.LocalAgentModel), StringComparison.Ordinal));
    }
}
