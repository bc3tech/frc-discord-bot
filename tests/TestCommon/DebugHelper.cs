namespace TestCommon;

using System.Collections.Concurrent;
using System.Diagnostics;

public class DebugHelper
{
    public static IDisposable IgnoreDebugAssertExceptions() => new DebugAssertWatcher();

    public static void AssertDebugException(Action action, string? messageContents = null)
    {
        using var i = new DebugAssertWatcher();

#if DEBUG
        try
        {
#endif
            action();
            Assert.Fail("DebugAssertException exception was not thrown.");
#if DEBUG
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }
#endif
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "Need this to be synchronously executed so the changes to the Trace listeners take effect")]
    public static void AssertDebugException(Task task, string? messageContents = null)
    {
#if DEBUG
        try
        {
#endif
            task.GetAwaiter().GetResult();
            if (task.Exception is not null)
            {
                throw task.Exception;
            }
#if DEBUG
            Assert.Fail("DebugAssertException exception was not thrown.");
        }
        catch (Exception ex)
        {
            Assert.Contains("DebugAssertException", ex.GetType().ToString());
            if (messageContents is not null)
            {
                Assert.Contains(messageContents, ex.Message);
            }
        }
#endif
    }

    public static T AssertDebugException<T>(Func<T> func, string? messageContents = null)
    {
        using var i = new DebugAssertWatcher();
        var retVal = func();
#if DEBUG
        Assert.NotEmpty(i.CapturedMessages);
        if (messageContents is not null)
        {
            Assert.Contains(i.CapturedMessages, j => j[6..].Contains(messageContents));
        }
#endif
        return retVal;
    }
    public static IEnumerable<T> AssertDebugException<T>(IAsyncEnumerable<T> func, string? messageContents = null)
    {
        using var i = new DebugAssertWatcher();
        var retVal = func.ToBlockingEnumerable().ToList();
#if DEBUG
        Assert.NotEmpty(i.CapturedMessages);
        if (messageContents is not null)
        {
            Assert.Contains(i.CapturedMessages, j => j[6..].Contains(messageContents));
        }
#endif
        return retVal;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method", Justification = "Need this to be synchronously executed so the changes to the Trace listeners take effect")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD104:Offer async methods", Justification = "<Pending>")]
    public static T AssertDebugException<T>(Task<T> task, string? messageContents = null)
    {
        using var i = new DebugAssertWatcher();
        var retVal = task.GetAwaiter().GetResult();
#if DEBUG
        Assert.NotEmpty(i.CapturedMessages);
        if (messageContents is not null)
        {
            Assert.Contains(i.CapturedMessages, j => j[6..].Contains(messageContents));
        }
#endif
        return retVal;
    }

    private sealed class DebugAssertWatcher : IDisposable
    {
        private static readonly AutoResetEvent _assertCaptureReady = new(true);
        private readonly IEnumerable<TraceListener> _listeners;
        private readonly IgnoreDebugAssertListener _ignoreDebugAssertListener = new();

        private bool disposedValue;

        public DebugAssertWatcher()
        {
#if DEBUG
            _assertCaptureReady.WaitOne();
            _listeners = Trace.Listeners.Cast<TraceListener?>().Where(i => i is not null).Select(i => i!);

            Trace.Listeners.Clear();
            Trace.Listeners.Add(_ignoreDebugAssertListener);
#endif
        }

        public IEnumerable<string> CapturedMessages => _ignoreDebugAssertListener.DebugFailMessages;

        public void Dispose(bool disposing)
        {
#if DEBUG
            if (!disposedValue)
            {
                if (disposing)
                {
                    Trace.Listeners.Clear();
                    foreach (var listener in _listeners)
                    {
                        Trace.Listeners.Add(listener);
                    }

                    _assertCaptureReady.Set();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
#endif
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DebugHelper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private sealed class IgnoreDebugAssertListener : TraceListener
        {
            private readonly ConcurrentBag<string> _debugFailMessages = [];
            public IEnumerable<string> DebugFailMessages => _debugFailMessages;

            public override void Write(string? message)
            {
                if (message?.StartsWith("Fail: ", StringComparison.Ordinal) is true)
                {
                    _debugFailMessages.Add(message);
                }
            }

            public override void WriteLine(string? message)
            {
                if (message?.StartsWith("Fail: ", StringComparison.Ordinal) is true)
                {
                    _debugFailMessages.Add(message);
                }
            }
        }
    }
}
