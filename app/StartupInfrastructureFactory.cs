namespace FunctionApp;

using Azure.Core;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;

internal static class StartupInfrastructureFactory
{
    public static TokenCredential CreateAzureCredential(IConfiguration configuration)
    {
        var clientId = configuration[Constants.Configuration.Azure.ClientId];
        return string.IsNullOrWhiteSpace(clientId)
            ? new ManagedIdentityCredential(new ManagedIdentityCredentialOptions())
            : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(clientId));
    }

    public static BlobServiceClient CreateBlobServiceClient(string? connectionString, Uri? serviceUri, TokenCredential credential)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return new BlobServiceClient(connectionString);
        }

        ArgumentNullException.ThrowIfNull(serviceUri);
        return new BlobServiceClient(serviceUri, credential);
    }

    public static TableServiceClient CreateTableServiceClient(string? connectionString, Uri? serviceUri, TokenCredential credential)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return new TableServiceClient(connectionString);
        }

        ArgumentNullException.ThrowIfNull(serviceUri);
        return new TableServiceClient(serviceUri, credential);
    }

    public static Uri? TryGetStorageServiceUri(IConfiguration configuration, string? storageService = null, params string[] configKeys)
    {
        foreach (var key in configKeys)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var serviceUri))
            {
                return serviceUri;
            }

            if (storageService is not null && !value.Contains(storageService, StringComparison.OrdinalIgnoreCase))
            {
                return new Uri($"https://{value}.{storageService}.core.windows.net/");
            }
        }

        return null;
    }
}
