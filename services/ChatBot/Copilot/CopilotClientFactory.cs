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
    private readonly ILogger _sdkLogger = loggerFactory.CreateLogger<CopilotClient>();
    private readonly SemaphoreSlim _sync = new(1, 1);

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
                await client.StartAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not OperationCanceledException and not TaskCanceledException)
            {
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
    }
}
