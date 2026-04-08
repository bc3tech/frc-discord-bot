namespace Common.Discord;

using global::Discord;

using Microsoft.Extensions.Logging;

using System;

public static class Helpers
{
    internal const int DiscordWriteMaxRetryAttempts = 5;

    public static async Task<T?> ExecuteDiscordWriteWithRetryAsync<T>(
        string operation,
        Func<RequestOptions, Task<T>> action,
        bool required, TimeProvider time,
        ILogger? logger, CancellationToken cancellationToken)
    {
        Exception? lastException = null;
        for (int attempt = 1; attempt <= DiscordWriteMaxRetryAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await action(cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.IsRetriableDiscordWrite(attempt, out TimeSpan delay))
            {
                lastException = ex;
                logger?.DiscordWriteRetrying(ex, operation, attempt, DiscordWriteMaxRetryAttempts, delay);
                await Task.Delay(delay, time, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!required)
            {
                logger?.DiscordWriteDeferred(ex, operation);
                return default;
            }
            catch (Exception ex)
            {
                throw new DiscordDeliveryException($"Discord write failed during {operation}.", ex);
            }
        }

        if (!required)
        {
            logger?.DiscordWriteDeferredAfterRetries(lastException, operation, DiscordWriteMaxRetryAttempts);
            return default;
        }

        throw new DiscordDeliveryException($"Discord write failed during {operation} after {DiscordWriteMaxRetryAttempts} attempts.", lastException);
    }
}
