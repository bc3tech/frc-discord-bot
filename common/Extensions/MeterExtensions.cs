namespace Common.Extensions;

using System.Collections.Generic;
using System.Diagnostics.Metrics;

public static class MeterExtensions
{
    public static void LogMetric<T>(this Meter meter, string name, T value, Dictionary<string, object?> tags) where T : struct => meter.LogMetric(name, value, [.. tags]);
    public static void LogMetric<T>(this Meter meter, string name, T value, params KeyValuePair<string, object?>[] tags) where T : struct => meter.CreateHistogram<T>(name).Record(value, tags);
    public static void LogMetric<T>(this Histogram<T> instrument, T value, params KeyValuePair<string, object?>[] tags) where T : struct => instrument.Record(value, tags);
    public static void LogMetric<T>(this Counter<T> instrument, T value, params KeyValuePair<string, object?>[] tags) where T : struct => instrument.Add(value, tags);
}
