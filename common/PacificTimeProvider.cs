namespace Common;
using System;

public class PacificTimeProvider : TimeProvider
{
    public override TimeZoneInfo LocalTimeZone { get; } = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
}
