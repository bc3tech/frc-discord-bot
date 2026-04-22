namespace ChatBot.Copilot;

using ChatBot.Configuration;

using GitHub.Copilot.SDK;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed partial class CopilotClientFactory(
    IOptions<AiOptions> options,
    ILoggerFactory loggerFactory)
    : IAsyncDisposable
{
    private readonly AiOptions _options = options.Value;
    private readonly ILogger _logger = loggerFactory.CreateLogger<CopilotClientFactory>();
    private readonly ILogger _sdkLogger = new CopilotSdkRedactingLogger(loggerFactory.CreateLogger<CopilotClient>());
    private readonly SemaphoreSlim _sync = new(1, 1);
    private static readonly TimeSpan ClientStartupTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ClientDisposeTimeout = TimeSpan.FromSeconds(10);

    private CopilotClient? _client;

    public async ValueTask<CopilotClient> GetStartedClientAsync(CancellationToken cancellationToken = default)
    {
        if (_client is not null)
        {
            return _client;
        }

        await _sync.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_client is not null)
            {
                return _client;
            }

            CopilotClient client = new(CreateClientOptions());
            Log.StartingCopilotClient(_logger, _options.Copilot.Model, _options.Foundry.Endpoint, useLoggedInUser: false);
            try
            {
                await client.StartAsync(cancellationToken).WaitAsync(ClientStartupTimeout, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                await TryDisposeClientAsync(client, _logger).ConfigureAwait(false);
                Log.CopilotClientStartupFailed(_logger, e, _options.Copilot.Model, _options.Foundry.Endpoint, useLoggedInUser: false);
                throw;
            }

            Log.CopilotClientStarted(_logger, _options.Copilot.Model, _options.Foundry.Endpoint);
            _client = client;
            return client;
        }
        finally
        {
            _ = _sync.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync().ConfigureAwait(false);
            _client = null;
        }

        _sync.Dispose();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Resilience", "EA0014:The async method doesn't support cancellation", Justification = "Not valid on Dispose semantics")]
    private static async Task TryDisposeClientAsync(CopilotClient client, ILogger logger)
    {
        try
        {
            await client.DisposeAsync().ConfigureAwait(false);
        }
        catch (TimeoutException timeoutException)
        {
            Log.CopilotClientCleanupFailed(logger, timeoutException);
        }
        catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
        {
            Log.CopilotClientCleanupFailed(logger, e);
        }
    }

    private CopilotClientOptions CreateClientOptions()
    {
        CopilotClientOptions options = new()
        {
            AutoStart = false,
            Cwd = Directory.GetCurrentDirectory(),
            Logger = _sdkLogger,
            Port = 0,
            UseStdio = false,
            UseLoggedInUser = false,
        };

        if (!string.IsNullOrWhiteSpace(_options.Copilot.LogLevel))
        {
            options.LogLevel = _options.Copilot.LogLevel;
        }

        return options;
    }

    private static partial class Log
    {
        [LoggerMessage(EventId = 1199, Level = LogLevel.Information, Message = "Starting GitHub Copilot SDK client for Foundry-backed model {Model} at {ProjectEndpoint} with UseLoggedInUser={UseLoggedInUser}.")]
        public static partial void StartingCopilotClient(ILogger logger, string model, Uri projectEndpoint, bool useLoggedInUser);

        [LoggerMessage(EventId = 1200, Level = LogLevel.Information, Message = "Started GitHub Copilot SDK client for Foundry-backed model {Model} at {ProjectEndpoint}.")]
        public static partial void CopilotClientStarted(ILogger logger, string model, Uri projectEndpoint);

        [LoggerMessage(EventId = 1201, Level = LogLevel.Error, Message = "GitHub Copilot SDK client startup failed for Foundry-backed model {Model} at {ProjectEndpoint} with UseLoggedInUser={UseLoggedInUser}.")]
        public static partial void CopilotClientStartupFailed(ILogger logger, Exception exception, string model, Uri projectEndpoint, bool useLoggedInUser);

        [LoggerMessage(EventId = 1202, Level = LogLevel.Warning, Message = "GitHub Copilot SDK client cleanup failed after startup did not complete.")]
        public static partial void CopilotClientCleanupFailed(ILogger logger, Exception exception);
    }
}
