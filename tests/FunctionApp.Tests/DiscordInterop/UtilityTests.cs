namespace FunctionApp.Tests.DiscordInterop;

using FunctionApp.DiscordInterop;

using System;

using Xunit;

public class UtilityTests
{
    [Fact]
    public void ToLocalTime_ShouldConvertToLocalTime()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2023, 10, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new TimeProvider(TimeZoneInfo.Local);

        // Act
        var result = dateTimeOffset.ToLocalTime(timeProvider);

        // Assert
        var expected = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeOffset, timeProvider.LocalTimeZone.Id);
        Assert.Equal(expected, result);
    }

    sealed class TimeProvider(TimeZoneInfo localTimeZone) : System.TimeProvider()
    {
        public override TimeZoneInfo LocalTimeZone { get; } = localTimeZone;
    }
}
