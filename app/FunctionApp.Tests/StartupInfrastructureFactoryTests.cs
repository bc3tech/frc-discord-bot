namespace FunctionApp.Tests;

using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;

public sealed class StartupInfrastructureFactoryTests
{
    [Fact]
    public void CreateAzureCredentialWhenClientIdMissingUsesManagedIdentityCredential()
    {
        IConfiguration configuration = BuildConfiguration();
        TokenCredential credential = StartupInfrastructureFactory.CreateAzureCredential(configuration);

        Assert.IsType<ManagedIdentityCredential>(credential);
    }

    [Fact]
    public void TryGetStorageServiceUriWhenConfiguredWithAbsoluteUriReturnsConfiguredValue()
    {
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>(Constants.Configuration.Azure.Storage.TableEndpoint, "https://storageacct.table.core.windows.net/"));

        Uri? uri = StartupInfrastructureFactory.TryGetStorageServiceUri(
            configuration,
            storageService: "table",
            Constants.Configuration.Azure.Storage.TableEndpoint);

        Assert.Equal(new Uri("https://storageacct.table.core.windows.net/"), uri);
    }

    [Fact]
    public void TryGetStorageServiceUriWhenConfiguredWithAccountNameComposesServiceUri()
    {
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>(ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"), "storageacct"));

        Uri? uri = StartupInfrastructureFactory.TryGetStorageServiceUri(
            configuration,
            storageService: "blob",
            ConfigurationPath.Combine("AzureWebJobsStorage", "accountName"));

        Assert.Equal(new Uri("https://storageacct.blob.core.windows.net/"), uri);
    }

    [Fact]
    public void TryGetStorageServiceUriWhenNoKeysMatchReturnsNull()
    {
        IConfiguration configuration = BuildConfiguration();

        Uri? uri = StartupInfrastructureFactory.TryGetStorageServiceUri(configuration, "blob", "Missing:Key");

        Assert.Null(uri);
    }

    [Fact]
    public void CreateBlobServiceClientWhenUsingConnectionStringReturnsClient()
    {
        BlobServiceClient client = StartupInfrastructureFactory.CreateBlobServiceClient(
            connectionString: "UseDevelopmentStorage=true",
            serviceUri: null,
            credential: new ManagedIdentityCredential(new ManagedIdentityCredentialOptions()));

        Assert.NotNull(client);
    }

    [Fact]
    public void CreateBlobServiceClientWhenConnectionStringAndServiceUriMissingThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StartupInfrastructureFactory.CreateBlobServiceClient(
            connectionString: null,
            serviceUri: null,
            credential: new ManagedIdentityCredential(new ManagedIdentityCredentialOptions())));
    }

    [Fact]
    public void CreateTableServiceClientWhenConnectionStringAndServiceUriMissingThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StartupInfrastructureFactory.CreateTableServiceClient(
            connectionString: null,
            serviceUri: null,
            credential: new ManagedIdentityCredential(new ManagedIdentityCredentialOptions())));
    }

    [Fact]
    public void HasFoundryChatConfigurationWhenUsingFoundryKeysReturnsTrue()
    {
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>(Constants.Configuration.AI.Foundry.Endpoint, "https://example.services.ai.azure.com/api/projects/test"),
            new KeyValuePair<string, string?>(Constants.Configuration.AI.Foundry.AgentId, "discord-bot"),
            new KeyValuePair<string, string?>(Constants.Configuration.AI.Foundry.LocalAgentModel, "gpt-5.4-mini"));

        Assert.True(HasAllRequiredFoundryChatConfiguration(configuration));
    }

    [Fact]
    public void HasFoundryChatConfigurationWhenKeysAreIncompleteReturnsFalse()
    {
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>(Constants.Configuration.AI.Foundry.Endpoint, "https://example.services.ai.azure.com/api/projects/test"),
            new KeyValuePair<string, string?>(Constants.Configuration.AI.Foundry.AgentId, "discord-bot"));

        Assert.False(HasAllRequiredFoundryChatConfiguration(configuration));
    }

    private static IConfiguration BuildConfiguration(params KeyValuePair<string, string?>[] values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private static bool HasAllRequiredFoundryChatConfiguration(IConfiguration configuration)
        => HasAnyConfiguredValue(configuration, Constants.Configuration.AI.Foundry.Endpoint)
        && HasAnyConfiguredValue(configuration, Constants.Configuration.AI.Foundry.AgentId)
        && HasAnyConfiguredValue(configuration, Constants.Configuration.AI.Foundry.LocalAgentModel);

    private static bool HasAnyConfiguredValue(IConfiguration configuration, params string[] keys)
    {
        foreach (string key in keys)
        {
            if (!string.IsNullOrWhiteSpace(configuration[key]))
            {
                return true;
            }
        }

        return false;
    }
}
