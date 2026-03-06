namespace FunctionApp.Tests.Subscription;

using Azure.Data.Tables;

using FunctionApp.Subscription;

internal sealed class InMemorySubscriptionTableClient : ISubscriptionTableClient
{
    private readonly Dictionary<(string PartitionKey, string RowKey), ITableEntity> entities = [];
    public int UpsertCount { get; private set; }

    public IAsyncEnumerable<T> QueryAsync<T>(CancellationToken cancellationToken) where T : class, ITableEntity
        => queryAsync<T>(cancellationToken);

    private async IAsyncEnumerable<T> queryAsync<T>([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        where T : class, ITableEntity
    {
        foreach (var entity in entities.Values.OfType<T>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return entity;
            await Task.Yield();
        }
    }

    public Task<T?> GetEntityIfExistsAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(entities.TryGetValue((partitionKey, rowKey), out var entity) ? entity as T : null);
    }

    public Task<TableWriteResult> UpsertEntityAsync<T>(T entity, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        entities[(entity.PartitionKey, entity.RowKey)] = entity;
        UpsertCount++;
        return Task.FromResult(new TableWriteResult(false, 204, null));
    }

    public void Seed(ITableEntity entity) => entities[(entity.PartitionKey, entity.RowKey)] = entity;
}
