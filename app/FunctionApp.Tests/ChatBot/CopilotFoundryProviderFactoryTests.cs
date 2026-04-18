namespace FunctionApp.Tests;

using global::ChatBot.Configuration;
using global::ChatBot.Copilot;

using Azure.Core;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Options;

public sealed class CopilotFoundryProviderFactoryTests
{
    [Theory]
    [InlineData("gpt-5", "responses")]
    [InlineData("gpt-4.1", "chat-completions")]
    public void CreateProviderConfigUsesFoundryProjectOpenAiRoute(string model, string expectedWireApi)
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
                Model = model,
            },
        };

        CopilotFoundryProviderFactory factory = new(Options.Create(options), new StubTokenCredential("token-value"));

        ProviderConfig provider = factory.CreateProviderConfig(TestContext.Current.CancellationToken);

        Assert.Equal("openai", provider.Type);
        Assert.Equal("https://example.services.ai.azure.com/api/projects/test/openai/v1/", provider.BaseUrl);
        Assert.Equal("token-value", provider.BearerToken);
        Assert.Equal(expectedWireApi, provider.WireApi);
        Assert.Equal("2025-06-01", Assert.IsType<AzureOptions>(provider.Azure).ApiVersion);
    }

    private sealed class StubTokenCredential(string tokenValue) : TokenCredential
    {
        private readonly AccessToken _token = new(tokenValue, DateTimeOffset.UtcNow.AddHours(1));

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            Assert.Equal(["https://ai.azure.com/.default"], requestContext.Scopes);
            return _token;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => ValueTask.FromResult(GetToken(requestContext, cancellationToken));
    }
}
