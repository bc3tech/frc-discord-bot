namespace ChatBot.AgentIdentity;

using Azure.Core;

using ChatBot.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

internal sealed record AgentIdentityContext(string AgentIdentityClientId, string TenantId);

internal sealed class AgentIdentityContextProvider(
    IOptions<AiOptions> aiOptions,
    ILogger<AgentIdentityContextProvider> logger)
{
    private const string AgentIdentityGraphEndpoint = "https://graph.microsoft.com/beta/servicePrincipals/Microsoft.Graph.AgentIdentity";

    private readonly AiAgent365Settings _settings = aiOptions.Value.Agent365;
    private readonly ILogger<AgentIdentityContextProvider> _logger = logger;
    private readonly SemaphoreSlim _contextGate = new(1, 1);
    private AgentIdentityContext? _context;

    public bool IsEnabled => _settings.Enabled;

    public async ValueTask<AgentIdentityContext?> GetContextAsync(CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            return null;
        }

        if (_context is not null)
        {
            return _context;
        }

        await _contextGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _context ??= await CreateContextAsync(cancellationToken).ConfigureAwait(false);
            return _context;
        }
        finally
        {
            _ = _contextGate.Release();
        }
    }

    private async Task<AgentIdentityContext> CreateContextAsync(CancellationToken cancellationToken)
    {
        string agentIdentityClientId = _settings.AgentIdentityClientId;
        if (string.IsNullOrWhiteSpace(agentIdentityClientId))
        {
            if (!_settings.AutoCreateIdentity)
            {
                throw new InvalidOperationException("Agent365 is enabled but no agent identity id was configured and auto-creation is disabled.");
            }

            agentIdentityClientId = await CreateOrFindAgentIdentityAsync(cancellationToken).ConfigureAwait(false);
        }

        AgentIdentityCredential identityCredential = new(
            _settings.TenantId,
            _settings.BlueprintClientId,
            _settings.ManagedIdentityClientId,
            agentIdentityClientId,
            _settings.TokenExchangeAudience);

        AccessToken probeToken = await identityCredential
            .GetTokenAsync(new TokenRequestContext([_settings.ProbeScope]), cancellationToken)
            .ConfigureAwait(false);

        _logger.Agent365AgentIdentityReady(_settings.TenantId, agentIdentityClientId, _settings.ProbeScope, probeToken.ExpiresOn);
        return new(agentIdentityClientId, _settings.TenantId);
    }

    private async Task<string> CreateOrFindAgentIdentityAsync(CancellationToken cancellationToken)
    {
        AgentIdentityBlueprintCredential blueprintCredential = new(
            _settings.TenantId,
            _settings.BlueprintClientId,
            _settings.ManagedIdentityClientId,
            _settings.TokenExchangeAudience);
        using GraphServiceClient graphClient = new(blueprintCredential, [_settings.ProbeScope]);

        string? existingAgentIdentityClientId = await TryFindExistingAgentIdentityClientIdAsync(graphClient, cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(existingAgentIdentityClientId))
        {
            _logger.Agent365UsingExistingAgentIdentity(existingAgentIdentityClientId, _settings.AgentIdentityDisplayName);
            return existingAgentIdentityClientId;
        }

        _logger.Agent365CreatingAgentIdentity(_settings.AgentIdentityDisplayName);
        ServicePrincipal requestBody = new()
        {
            DisplayName = _settings.AgentIdentityDisplayName,
            AdditionalData = new Dictionary<string, object>
            {
                ["agentIdentityBlueprintId"] = _settings.BlueprintClientId,
                ["sponsors@odata.bind"] = _settings.Sponsors.ToArray(),
            },
        };

        ServicePrincipal? createdAgentIdentity = await graphClient.ServicePrincipals
            .WithUrl(AgentIdentityGraphEndpoint)
            .PostAsync(requestBody, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        string? createdAgentIdentityClientId = GetAgentIdentityClientId(createdAgentIdentity);
        if (string.IsNullOrWhiteSpace(createdAgentIdentityClientId))
        {
            throw new InvalidOperationException("Microsoft Graph created an agent identity but did not return either appId or id.");
        }

        _logger.Agent365CreatedAgentIdentity(createdAgentIdentityClientId, createdAgentIdentity?.Id ?? string.Empty, _settings.AgentIdentityDisplayName);
        return createdAgentIdentityClientId;
    }

    private async Task<string?> TryFindExistingAgentIdentityClientIdAsync(GraphServiceClient graphClient, CancellationToken cancellationToken)
    {
        string escapedDisplayName = _settings.AgentIdentityDisplayName.Replace("'", "''", StringComparison.Ordinal);
        ServicePrincipalCollectionResponse? existing = await graphClient.ServicePrincipals
            .WithUrl(AgentIdentityGraphEndpoint)
            .GetAsync(
                requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"displayName eq '{escapedDisplayName}'";
                    requestConfiguration.QueryParameters.Select = ["id", "appId", "displayName"];
                    requestConfiguration.QueryParameters.Top = 1;
                },
                cancellationToken)
            .ConfigureAwait(false);

        ServicePrincipal? existingIdentity = existing?.Value?.FirstOrDefault();
        return GetAgentIdentityClientId(existingIdentity);
    }

    private static string? GetAgentIdentityClientId(ServicePrincipal? servicePrincipal)
    {
        if (!string.IsNullOrWhiteSpace(servicePrincipal?.AppId))
        {
            return servicePrincipal.AppId;
        }

        if (!string.IsNullOrWhiteSpace(servicePrincipal?.Id))
        {
            return servicePrincipal.Id;
        }

        return null;
    }
}
