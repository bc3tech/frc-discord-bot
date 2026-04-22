namespace FunctionApp.DiscordInterop;

using Azure;
using Azure.Data.Tables;

using FunctionApp.Storage.TableEntities;

internal interface IWebhookSubscriptionStore<T> where T : class, ISubscriptionEntity
{
    string Name { get; }

    Task<T?> GetEntityIfExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
}

internal sealed class TableWebhookSubscriptionStore<T>(TableClient tableClient) : IWebhookSubscriptionStore<T>
    where T : class, ISubscriptionEntity
{
    public string Name => tableClient.Name;

    public async Task<T?> GetEntityIfExistsAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
    {
        NullableResponse<T> response = await tableClient.GetEntityIfExistsAsync<T>(partitionKey, rowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.HasValue ? response.Value : null;
    }
}
