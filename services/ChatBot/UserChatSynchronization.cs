namespace ChatBot;

using System.Collections.Concurrent;

public sealed class UserChatSynchronization
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _userLocks = new(StringComparer.Ordinal);

    public async ValueTask<IAsyncDisposable> AcquireAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        SemaphoreSlim gate = _userLocks.GetOrAdd(userId, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Releaser(gate);
    }

    private sealed class Releaser(SemaphoreSlim gate) : IAsyncDisposable
    {
        private readonly SemaphoreSlim _gate = gate;
        private int _disposed;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _gate.Release();
            }

            return ValueTask.CompletedTask;
        }
    }
}
