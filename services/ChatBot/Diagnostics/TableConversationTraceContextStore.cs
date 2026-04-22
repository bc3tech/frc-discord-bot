namespace ChatBot.Diagnostics;

using System.Globalization;

using Azure;
using Azure.Data.Tables;

using CopilotSdk.OpenTelemetry;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Azure Table Storage backed <see cref="IConversationTraceContextStore"/>.
/// Persists conversation root <see cref="ConversationTraceContext"/> values across Function invocations
/// so multi-turn conversations show up as a single Trace in Application Insights.
/// </summary>
public sealed partial class TableConversationTraceContextStore(
    TableServiceClient tableServiceClient,
    IOptions<TableConversationTraceContextStoreOptions> options,
    ILogger<TableConversationTraceContextStore> logger) : IConversationTraceContextStore
{
    private readonly TableServiceClient _tableServiceClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
    private readonly TableConversationTraceContextStoreOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<TableConversationTraceContextStore> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private TableClient? _tableClient;

    /// <inheritdoc />
    public async Task<ConversationTraceContext?> TryGetAsync(string conversationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);

        (string? partitionKey, string? rowKey) = SplitKey(conversationId);
        TableClient tableClient = await GetTableClientAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            Response<TraceContextEntity> response = await tableClient
                .GetEntityAsync<TraceContextEntity>(partitionKey, rowKey, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            TraceContextEntity entity = response.Value;
            return string.IsNullOrEmpty(entity.TraceId) || string.IsNullOrEmpty(entity.SpanId)
                ? null
                : new ConversationTraceContext(entity.TraceId, entity.SpanId, (byte)entity.TraceFlags);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync(string conversationId, ConversationTraceContext context, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);
        ArgumentNullException.ThrowIfNull(context);

        (string? partitionKey, string? rowKey) = SplitKey(conversationId);
        TableClient tableClient = await GetTableClientAsync(cancellationToken).ConfigureAwait(false);

        var entity = new TraceContextEntity
        {
            PartitionKey = partitionKey,
            RowKey = rowKey,
            TraceId = context.TraceId,
            SpanId = context.SpanId,
            TraceFlags = context.TraceFlags,
            CreatedUtc = DateTimeOffset.UtcNow,
        };

        await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken).ConfigureAwait(false);
        LogStored(_logger, conversationId);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string conversationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(conversationId);

        (string? partitionKey, string? rowKey) = SplitKey(conversationId);
        TableClient tableClient = await GetTableClientAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
            LogRemoved(_logger, conversationId);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // Already gone — nothing to do.
        }
    }

    private async Task<TableClient> GetTableClientAsync(CancellationToken cancellationToken)
    {
        if (_tableClient is not null)
        {
            return _tableClient;
        }

        TableClient client = _tableServiceClient.GetTableClient(_options.TableName);
        if (_options.CreateTableIfNotExists)
        {
            await client.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
        }

        _tableClient = client;
        return client;
    }

    private static (string PartitionKey, string RowKey) SplitKey(string conversationId)
    {
        // Discord ConversationKey.ToStorageKey() format: "{(int)Scope}:{Id}"
        var separator = conversationId.IndexOf(':');
        if (separator > 0 && separator < conversationId.Length - 1)
        {
            var scope = conversationId[..separator];
            var id = SanitizeKey(conversationId[(separator + 1)..]);
            return (scope, id);
        }

        return ("0", SanitizeKey(conversationId));
    }

    private static string SanitizeKey(string key)
    {
        // Azure Table keys disallow / \ # ? and control characters.
        const int maxLength = 512;
        if (key.Length > maxLength)
        {
            key = key[..maxLength];
        }

        Span<char> buffer = stackalloc char[key.Length];
        for (var i = 0; i < key.Length; i++)
        {
            var c = key[i];
            buffer[i] = c switch
            {
                '/' or '\\' or '#' or '?' => '-',
                _ when char.IsControl(c) => '-',
                _ => c,
            };
        }

        return new string(buffer);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Stored conversation trace context for {ConversationId}")]
    private static partial void LogStored(ILogger logger, string conversationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Removed conversation trace context for {ConversationId}")]
    private static partial void LogRemoved(ILogger logger, string conversationId);

    private sealed class TraceContextEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public string SpanId { get; set; } = string.Empty;
        public int TraceFlags { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
    }
}

/// <summary>Options for <see cref="TableConversationTraceContextStore"/>.</summary>
public sealed class TableConversationTraceContextStoreOptions
{
    /// <summary>Azure Table name to store trace contexts in.</summary>
    public string TableName { get; set; } = "conversationtraces";

    /// <summary>Whether to create the table on first use if it does not exist.</summary>
    public bool CreateTableIfNotExists { get; set; } = true;
}
