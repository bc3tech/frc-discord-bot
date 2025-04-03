namespace Common.Tests.Extensions;
using Common.Extensions;

using System;

using Xunit;

public class ObjectExtensionsTests
{
    [Theory]
    [InlineData(null, "other", "other")]
    [InlineData("value", "other", "value")]
    public void Or_ShouldReturnExpectedResult(string? obj, string? other, string? expected)
    {
        // Act
        var result = obj.Or(other);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OrException_ShouldReturnObjectIfNotNull()
    {
        // Arrange
        string? obj = "value";
        string? other = "other";

        // Act
        var result = obj.OrException(other);

        // Assert
        Assert.Equal(obj, result);
    }

    [Fact]
    public void OrException_ShouldReturnOtherIfObjectIsNull()
    {
        // Arrange
        string? obj = null;
        string? other = "other";

        // Act
        var result = obj.OrException(other);

        // Assert
        Assert.Equal(other, result);
    }

    [Fact]
    public void OrException_ShouldThrowExceptionIfBothAreNull()
    {
        // Arrange
        string? obj = null;
        string? other = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => obj.OrException(other));
    }

    [Theory]
    [InlineData(null, true, "other")]
    [InlineData(null, false, null)]
    [InlineData("value", true, "other")]
    [InlineData("value", false, "value")]
    public void UnlessThen_ShouldReturnExpectedResult(string? value, bool conditionResult, string? expected)
    {
        // Act
        var result = value.UnlessThen(v => conditionResult, "other");

        // Assert
        Assert.Equal(expected, result);
    }
}
