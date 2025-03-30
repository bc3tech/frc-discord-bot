namespace TestCommon;
using System.Diagnostics;

public static class DebugHelper
{
    public static void IgnoreDebugAsserts()
    {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new IgnoreDebugAssertListener());
    }

    private class IgnoreDebugAssertListener : TraceListener
    {
        public override void Write(string message) { }

        public override void WriteLine(string message) { }
    }
}
