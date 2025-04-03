namespace Common.Extensions;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

public static class MeterExtensions
{
    public static void LogMetric<T>(this Meter meter, string name, T value, params KeyValuePair<string, object?>[] tags) where T : struct => meter.CreateHistogram<T>(name).Record(value, tags);
    public static void LogMetric<T>(this Histogram<T> meter, T value, params KeyValuePair<string, object?>[] tags) where T : struct => meter.Record(value, tags);
    public static void LogMetric<T>(this Counter<T> meter, T value, params KeyValuePair<string, object?>[] tags) where T : struct => meter.Add(value, tags);
}
