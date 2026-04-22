namespace ChatBot.Copilot;

using Azure.Core;

using ChatBot.Configuration;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Options;

internal sealed class CopilotFoundryProviderFactory(IOptions<AiOptions> options, TokenCredential credential)
{
    private const string FoundryBearerScope = "https://ai.azure.com/.default";
    private static readonly TokenRequestContext FoundryTokenRequestContext = new([FoundryBearerScope]);

    private readonly AiOptions _options = options.Value;
    private readonly TokenCredential _credential = credential;

    public ProviderConfig CreateProviderConfig(CancellationToken cancellationToken = default)
        => new()
        {
            Azure = new AzureOptions
            {
                ApiVersion = _options.Foundry.OpenAIApiVersion,
            },
            BaseUrl = BuildOpenAiBaseUrl(_options.Foundry.Endpoint),
            BearerToken = _credential.GetToken(FoundryTokenRequestContext, cancellationToken).Token,
            Type = "azure",
            WireApi = GetWireApi(_options.Copilot.Model),
        };

    private static string BuildOpenAiBaseUrl(Uri projectEndpoint)
    {
        UriBuilder builder = new(projectEndpoint.GetLeftPart(UriPartial.Authority))
        {
            Path = "/openai/v1/",
        };
        return builder.Uri.AbsoluteUri;
    }

    private static string GetWireApi(string model)
        => model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
            ? "responses"
            : "chat-completions";
}
