namespace ChatBot.AgentIdentity;

using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;

internal sealed class AgentIdentityBlueprintCredential(
    string tenantId,
    string agentIdentityBlueprintId,
    string managedIdentityClientId,
    string tokenExchangeAudience,
    ClientAssertionCredentialOptions? options = null)
    : TokenCredential
{
    private readonly ClientAssertionCredential _innerCredential = CreateCredential(
        tenantId,
        agentIdentityBlueprintId,
        managedIdentityClientId,
        tokenExchangeAudience,
        options);

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _innerCredential.GetToken(requestContext, cancellationToken);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _innerCredential.GetTokenAsync(requestContext, cancellationToken);

    private static ClientAssertionCredential CreateCredential(
        string tenantId,
        string agentIdentityBlueprintId,
        string managedIdentityClientId,
        string tokenExchangeAudience,
        ClientAssertionCredentialOptions? options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(agentIdentityBlueprintId);
        ArgumentException.ThrowIfNullOrWhiteSpace(managedIdentityClientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenExchangeAudience);

        ManagedIdentityCredential managedIdentityCredential = new(managedIdentityClientId);
        string publicTokenExchangeScope = $"{tokenExchangeAudience}/.default";
        Func<CancellationToken, Task<string>> clientAssertionCallback = async cancellationToken =>
            (await managedIdentityCredential
                .GetTokenAsync(new TokenRequestContext([publicTokenExchangeScope]), cancellationToken)
                .ConfigureAwait(false))
                .Token;

        return options is null
            ? new ClientAssertionCredential(tenantId, agentIdentityBlueprintId, clientAssertionCallback)
            : new ClientAssertionCredential(tenantId, agentIdentityBlueprintId, clientAssertionCallback, options);
    }
}

internal sealed class AgentIdentityCredential(
    string tenantId,
    string agentIdentityBlueprintId,
    string managedIdentityClientId,
    string agentIdentityClientId,
    string tokenExchangeAudience)
    : TokenCredential
{
    private readonly ClientAssertionCredential _innerCredential = CreateCredential(
        tenantId,
        agentIdentityBlueprintId,
        managedIdentityClientId,
        agentIdentityClientId,
        tokenExchangeAudience);

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _innerCredential.GetToken(requestContext, cancellationToken);

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        => _innerCredential.GetTokenAsync(requestContext, cancellationToken);

    private static ClientAssertionCredential CreateCredential(
        string tenantId,
        string agentIdentityBlueprintId,
        string managedIdentityClientId,
        string agentIdentityClientId,
        string tokenExchangeAudience)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(agentIdentityBlueprintId);
        ArgumentException.ThrowIfNullOrWhiteSpace(managedIdentityClientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(agentIdentityClientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenExchangeAudience);

        ClientAssertionCredentialOptions blueprintOptions = new()
        {
            Transport = new FmiTransport(agentIdentityClientId),
        };
        AgentIdentityBlueprintCredential blueprintCredential = new(
            tenantId,
            agentIdentityBlueprintId,
            managedIdentityClientId,
            tokenExchangeAudience,
            blueprintOptions);

        string publicTokenExchangeScope = $"{tokenExchangeAudience}/.default";
        Func<CancellationToken, Task<string>> clientAssertionCallback = async cancellationToken =>
            (await blueprintCredential
                .GetTokenAsync(new TokenRequestContext([publicTokenExchangeScope]), cancellationToken)
                .ConfigureAwait(false))
                .Token;

        return new ClientAssertionCredential(tenantId, agentIdentityClientId, clientAssertionCallback);
    }
}

internal sealed class FmiTransport(string agentIdentityClientId) : HttpClientTransport
{
    private readonly string _agentIdentityClientId = string.IsNullOrWhiteSpace(agentIdentityClientId)
        ? throw new ArgumentException("Value cannot be null or whitespace.", nameof(agentIdentityClientId))
        : agentIdentityClientId;

    public override void Process(HttpMessage message)
    {
        message.Request.Uri.AppendQuery("fmi_path", _agentIdentityClientId);
        base.Process(message);
    }

    public override ValueTask ProcessAsync(HttpMessage message)
    {
        message.Request.Uri.AppendQuery("fmi_path", _agentIdentityClientId);
        return base.ProcessAsync(message);
    }
}
