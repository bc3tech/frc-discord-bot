namespace FunctionApp.Subscription;

using Azure.Data.Tables;

internal readonly record struct TableWriteResult(bool IsError, int Status, string? ReasonPhrase);

internal interface ISubscriptionTableClient
{
    IAsyncEnumerable<T> QueryAsync<T>(CancellationToken cancellationToken) where T : class, ITableEntity;
    Task<T?> GetEntityIfExistsAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken) where T : class, ITableEntity;
    Task<TableWriteResult> UpsertEntityAsync<T>(T entity, CancellationToken cancellationToken) where T : class, ITableEntity;
}

internal sealed class AzureSubscriptionTableClient(TableClient tableClient) : ISubscriptionTableClient
{
    public IAsyncEnumerable<T> QueryAsync<T>(CancellationToken cancellationToken) where T : class, ITableEntity
        => tableClient.QueryAsync<T>(cancellationToken: cancellationToken);

    public async Task<T?> GetEntityIfExistsAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var result = await tableClient.GetEntityIfExistsAsync<T>(partitionKey, rowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.HasValue ? result.Value : null;
    }

    public async Task<TableWriteResult> UpsertEntityAsync<T>(T entity, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var result = await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken: cancellationToken).ConfigureAwait(false);
        return new(result.IsError, result.Status, result.ReasonPhrase);
    }
}
