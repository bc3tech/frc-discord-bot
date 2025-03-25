namespace Common.Tests.JsonConverters;

using Common.JsonConverters;

using System;
using System.Text.Json;

using Xunit;

public class UnixEpochDateTimeConverterTests
{
    private readonly UnixEpochDateTimeConverter _converter = new();
    private readonly JsonSerializerOptions _options = new();

    [Fact]
    public void Read_ValidUnixTimestamp_ReturnsCorrectDateTime()
    {
        // Arrange
        var json = "1609459200000"; // 2021-01-01T00:00:00 in Unix milliseconds
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read(); // Advance to the first token

        // Act
        var result = _converter.Read(ref reader, typeof(DateTime), _options);

        // Assert
        var expected = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected.ToUniversalTime().Date, result.ToUniversalTime().Date);
    }

    [Fact]
    public void Read_ZeroUnixTimestamp_ReturnsEpochStart()
    {
        // Arrange
        var json = "0"; // 1970-01-01T00:00:00 (Unix epoch)
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read(); // Advance to the first token

        // Act
        var result = _converter.Read(ref reader, typeof(DateTime), _options);

        // Assert
        var expected = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Write_NormalDateTime_WritesCorrectUnixTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedTimestamp = 1609459200000; // 2021-01-01T00:00:00 in Unix milliseconds
        
        using var stream = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        _converter.Write(writer, dateTime, _options);
        writer.Flush();
        
        // Reset stream position to read the written value
        stream.Position = 0;
        var reader = new Utf8JsonReader(stream.ToArray());
        reader.Read(); // Advance to the first token
        var result = reader.GetInt64();

        // Assert
        Assert.Equal(expectedTimestamp, result);
    }

    [Fact]
    public void Write_MinValue_WritesCorrectUnixTimestamp()
    {
        // Arrange
        // Using a reasonably old date rather than DateTime.MinValue which would cause overflow
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expectedTimestamp = 0; // Unix epoch start
        
        using var stream = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        _converter.Write(writer, dateTime, _options);
        writer.Flush();
        
        // Reset stream position to read the written value
        stream.Position = 0;
        var reader = new Utf8JsonReader(stream.ToArray());
        reader.Read(); // Advance to the first token
        var result = reader.GetInt64();

        // Assert
        Assert.Equal(expectedTimestamp, result);
    }

    [Fact]
    public void RoundTrip_DateTime_PreservesValue()
    {
        // Arrange
        var original = new DateTime(2023, 6, 15, 12, 30, 45, DateTimeKind.Utc);

        using var stream = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act - Write
        _converter.Write(writer, original, _options);
        writer.Flush();

        // Act - Read
        stream.Position = 0;
        var reader = new Utf8JsonReader(stream.ToArray());
        reader.Read(); // Advance to the first token
        var result = _converter.Read(ref reader, typeof(DateTime), _options);

        // Assert
        Assert.Equal(original, result);
    }
}
