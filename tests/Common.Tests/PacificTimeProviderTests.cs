namespace Common.Tests;

using System;
using System.Text.Json;

using Xunit;

public class PacificTimeProviderTests
{
    private static readonly PacificTimeProvider _timeProvider = new();

    [Fact]
    public void LocalTimeZone_ReturnsPacificTimeZone()
    {
        Assert.Equal("Pacific Standard Time", _timeProvider.LocalTimeZone.Id);
    }

    [Fact]
    public void DateTime_ToString_ShowsPacificTime()
    {
        // Arrange - Create a UTC time that will be 10 AM Pacific
        var utcTime = new DateTime(2024, 1, 15, 18, 0, 0, DateTimeKind.Utc); // 18:00 UTC = 10:00 PST

        // Act - Convert to local time using our provider
        var localTime = TimeZoneInfo.ConvertTime(utcTime, _timeProvider.LocalTimeZone);

        // Assert
        Assert.Equal("1/15/2024 10:00:00 AM", localTime.ToString());
        Assert.Equal(-8, _timeProvider.LocalTimeZone.GetUtcOffset(localTime).Hours); // PST is UTC-8 in January
    }

    [Fact]
    public void DateTime_ToString_HandlesDaylightSavings()
    {
        // Arrange - Create a UTC time during daylight savings
        var utcTime = new DateTime(2024, 7, 15, 17, 0, 0, DateTimeKind.Utc); // 17:00 UTC = 10:00 PDT

        // Act
        var localTime = TimeZoneInfo.ConvertTime(utcTime, _timeProvider.LocalTimeZone);

        // Assert
        Assert.Equal("7/15/2024 10:00:00 AM", localTime.ToString());
        Assert.Equal(-7, _timeProvider.LocalTimeZone.GetUtcOffset(localTime).Hours); // PDT is UTC-7 in July
    }

    [Fact]
    public void DateTimeOffset_ToString_ShowsPacificTime()
    {
        // Arrange
        var utcOffset = new DateTimeOffset(2024, 1, 15, 18, 0, 0, TimeSpan.Zero); // 18:00 UTC

        // Act
        var localTime = TimeZoneInfo.ConvertTime(utcOffset, _timeProvider.LocalTimeZone);

        // Assert
        Assert.Equal("1/15/2024 10:00:00 AM -08:00", localTime.ToString());
    }

    [Fact]
    public void JsonSerialization_MaintainsProperTimeConversion()
    {
        // Arrange
        var testTime = new DateTime(2024, 1, 15, 18, 0, 0, DateTimeKind.Utc);
        var testObject = new { Time = testTime };

        // Act
        var json = JsonSerializer.Serialize(testObject);
        var deserialized = JsonSerializer.Deserialize<dynamic>(json);
        Assert.NotNull(deserialized);

        var deserializedTime = DateTime.Parse(deserialized!.GetProperty("Time").ToString());
        var localTime = TimeZoneInfo.ConvertTime(deserializedTime, _timeProvider.LocalTimeZone);

        // Assert
        Assert.Equal("1/15/2024 10:00:00 AM", localTime.ToString());
    }

    [Fact]
    public void TimeProvider_UtcNow_ConvertsToLocalCorrectly()
    {
        // Arrange
        var utcNow = new DateTime(2024, 1, 15, 18, 0, 0, DateTimeKind.Utc);
        var expectedLocal = new DateTime(2024, 1, 15, 10, 0, 0); // 18:00 UTC = 10:00 PST

        // Act
        var localTime = TimeZoneInfo.ConvertTime(utcNow, _timeProvider.LocalTimeZone);

        // Assert
        Assert.Equal(expectedLocal.Hour, localTime.Hour);
        Assert.Equal(expectedLocal.Minute, localTime.Minute);
    }
}
