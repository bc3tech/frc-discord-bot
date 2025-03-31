namespace Common.Tests.Extensions;
using Common.Extensions;

using System.Threading.Tasks;

using TestCommon;

using Xunit;

public class StringExtensionsTests : Test
{
    [Theory]
    [InlineData("frc1234", (ushort)1234)]
    [InlineData("FRC5678", (ushort)5678)]
    [InlineData("team9999", (ushort)9999)]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData(" ", null)]
    public void TeamKeyToTeamNumber_ShouldReturnExpectedResult(string? input, ushort? expected)
    {
        // Act
        var result = input.TeamKeyToTeamNumber();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("invalid", null)]
    public void TeamKeyToTeamNumber_ShouldDebugAssert(string input, ushort? expected)
    {
        // Act & Assert
        var result = DebugHelper.AssertDebugException(input.TeamKeyToTeamNumber);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "replacement", "replacement")]
    [InlineData("", "replacement", "replacement")]
    [InlineData(" ", "replacement", "replacement")]
    [InlineData("value", "replacement", "value")]
    public void UnlessNullOrWhitespaceThen_ShouldReturnExpectedResult(string? input, string replacement, string expected)
    {
        // Act
        var result = input.UnlessNullOrWhitespaceThen(replacement);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Compress_Decompress_ShouldReturnOriginalString()
    {
        // Arrange
        var originalString = "This is a test string to compress and decompress.";

        // Act
        var compressedString = originalString.Compress();
        var decompressedString = compressedString.Decompress();

        // Assert
        Assert.Equal(originalString, decompressedString);
    }

    [Fact]
    public void Compress_ShouldReturnNonEmptyString()
    {
        // Arrange
        var originalString = "This is a test string to compress.";

        // Act
        var compressedString = originalString.Compress();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(compressedString));
    }

    [Fact]
    public void Decompress_InvalidString_ShouldThrowException()
    {
        // Arrange
        var invalidCompressedString = "InvalidCompressedString";

        // Act & Assert
        Assert.Throws<FormatException>(invalidCompressedString.Decompress);
    }
}
