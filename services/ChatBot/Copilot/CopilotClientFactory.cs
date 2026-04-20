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
        if (this._client is not null)
        {
            return this._client;
        }

        await this._sync.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (this._client is not null)
            {
                return this._client;
            }

            CopilotClient client = new(this.CreateClientOptions());
            Log.StartingCopilotClient(this._logger, this._options.Copilot.Model, this._options.Foundry.Endpoint, useLoggedInUser: false);
            try
            {
                await client.StartAsync(cancellationToken).WaitAsync(ClientStartupTimeout, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
                await TryDisposeClientAsync(client, this._logger).ConfigureAwait(false);
                Log.CopilotClientStartupFailed(this._logger, e, this._options.Copilot.Model, this._options.Foundry.Endpoint, useLoggedInUser: false);
                throw;
            }

            Log.CopilotClientStarted(this._logger, this._options.Copilot.Model, this._options.Foundry.Endpoint);
            this._client = client;
            return client;
        }
        finally
        {
            _ = this._sync.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (this._client is not null)
        {
            await this._client.DisposeAsync().ConfigureAwait(false);
            this._client = null;
        }

        this._sync.Dispose();
    }

    private static async Task TryDisposeClientAsync(CopilotClient client, ILogger logger)
    {
        try
        {
            await client.DisposeAsync().AsTask().WaitAsync(ClientDisposeTimeout).ConfigureAwait(false);
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
            Logger = this._sdkLogger,
            Port = 0,
            UseStdio = false,
            UseLoggedInUser = false,
        };

        if (!string.IsNullOrWhiteSpace(this._options.Copilot.LogLevel))
        {
            options.LogLevel = this._options.Copilot.LogLevel;
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
