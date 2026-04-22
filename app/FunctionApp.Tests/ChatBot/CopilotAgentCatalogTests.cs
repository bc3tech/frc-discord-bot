namespace FunctionApp.Tests;

using global::ChatBot.Agents;
using global::ChatBot.Configuration;
using global::ChatBot.Copilot;
using global::ChatBot.Tools;

using Azure.Core;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

public sealed class CopilotAgentCatalogTests
{
    [Fact]
    public void CreateSessionConfigBuildsParentAndSpecialistAgents()
    {
        AiOptions options = new()
        {
            FoundryEndpoint = new Uri("https://example.services.ai.azure.com/api/projects/test"),
            AgentId = "discord-bot",
            MealSignupGeniusId = "signup-board",
            LocalAgentModel = "gpt-5.4-mini",
            OpenAIApiVersion = "2025-06-01",
            DefaultTeamNumber = 2046,
            Copilot =
            {
                Model = "gpt-5.4-mini",
                ReasoningEffort = "high",
            },
        };

        PromptCatalog promptCatalog = new(NullLogger<PromptCatalog>.Instance);
        StubToolProvider toolProvider = new(["statbotics_api", "tba_api", "tba_last_comp"]);
        CopilotFoundryProviderFactory providerFactory = new(Options.Create(options), new StubTokenCredential("token-value"));
        CopilotAgentCatalog catalog = new(Options.Create(options), promptCatalog, [toolProvider], providerFactory);
        IReadOnlyList<AIFunction> tools = [toolProvider.Functions[0]];

        SessionConfig config = catalog.CreateSessionConfig(tools, cancellationToken: TestContext.Current.CancellationToken);
        IReadOnlyList<string> skillDirectories = Assert.IsAssignableFrom<IReadOnlyList<string>>(config.SkillDirectories);
        IReadOnlyList<AIFunction> configuredTools = Assert.IsAssignableFrom<IReadOnlyList<AIFunction>>(config.Tools);
        IReadOnlyList<CustomAgentConfig> customAgents = Assert.IsAssignableFrom<IReadOnlyList<CustomAgentConfig>>(config.CustomAgents);
        ProviderConfig provider = Assert.IsType<ProviderConfig>(config.Provider);

        Assert.Equal(CopilotAgentCatalog.ParentAgentName, config.Agent);
        Assert.Equal("gpt-5.4-mini", config.Model);
        Assert.Equal("high", config.ReasoningEffort);
        Assert.True(config.Streaming);
        Assert.Equal(Directory.GetCurrentDirectory(), config.WorkingDirectory);
        Assert.Single(skillDirectories);
        Assert.EndsWith(Path.Combine("ChatBot", "Copilot", "Skills"), skillDirectories[0], StringComparison.Ordinal);
        Assert.Same(tools[0], Assert.Single(configuredTools));
        Assert.Equal("azure", provider.Type);
        Assert.Equal("https://example.services.ai.azure.com/openai/v1/", provider.BaseUrl);
        Assert.Equal("token-value", provider.BearerToken);
        Assert.Equal("responses", provider.WireApi);
        Assert.Equal("2025-06-01", Assert.IsType<AzureOptions>(provider.Azure).ApiVersion);

        Assert.Collection(
            customAgents,
            parentAgent =>
            {
                Assert.Equal(CopilotAgentCatalog.ParentAgentName, parentAgent.Name);
                Assert.Equal("Bear Metal Assistant", parentAgent.DisplayName);
                Assert.Equal([FoundrySpecialistTool.ToolName], parentAgent.Tools);
                Assert.DoesNotContain("[[DEFAULT_TEAM_NUMBER]]", parentAgent.Prompt, StringComparison.Ordinal);
                Assert.Contains("2046", parentAgent.Prompt, StringComparison.Ordinal);
            },
            frcAgent =>
            {
                Assert.Equal(CopilotAgentCatalog.FrcDataSpecialistAgentName, frcAgent.Name);
                Assert.Equal("FRC Data Specialist", frcAgent.DisplayName);
                Assert.True(frcAgent.Infer);
                Assert.Equal(["statbotics_api", "tba_api", "tba_last_comp"], frcAgent.Tools);
                Assert.DoesNotContain("[[DEFAULT_TEAM_NUMBER]]", frcAgent.Prompt, StringComparison.Ordinal);
                Assert.Contains("2046", frcAgent.Prompt, StringComparison.Ordinal);
            });
    }

    private sealed class StubToolProvider(IReadOnlyList<string> toolNames) : IProvideFunctionTools
    {
        public FunctionToolScope Scope => FunctionToolScope.LocalFrcData;

        public IReadOnlyList<AIFunction> Functions { get; } =
        [
            AIFunctionFactory.Create(
                static () => "ok",
                new AIFunctionFactoryOptions
                {
                    Name = toolNames[0],
                    Description = "test",
                }),
        ];

        public IReadOnlyList<string> ToolNames => toolNames;
    }

    private sealed class StubTokenCredential(string tokenValue) : TokenCredential
    {
        private readonly AccessToken _token = new(tokenValue, DateTimeOffset.UtcNow.AddHours(1));

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => _token;

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(_token);
    }
}
