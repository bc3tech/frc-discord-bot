namespace TheBlueAlliance.Tests.Extensions;

using TheBlueAlliance.Extensions;
using TheBlueAlliance.Model;

public class CompLevelEnumExtensionsTests
{
    [Theory]
    [InlineData(Match.CompLevelEnum.Qm, "Quals")]
    [InlineData(Match.CompLevelEnum.Qf, "Quarters")]
    [InlineData(Match.CompLevelEnum.Sf, "Elims")]
    [InlineData(Match.CompLevelEnum.F, "Finals")]
    [InlineData((Match.CompLevelEnum)999, "999")]
    public void ToShortString_MatchCompLevelEnum_ReturnsExpectedString(Match.CompLevelEnum compLevel, string expected)
    {
        // Act
        var result = compLevel.ToShortString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(MatchSimple.CompLevelEnum.Qm, "Quals")]
    [InlineData(MatchSimple.CompLevelEnum.Qf, "Quarters")]
    [InlineData(MatchSimple.CompLevelEnum.Sf, "Elims")]
    [InlineData(MatchSimple.CompLevelEnum.F, "Finals")]
    [InlineData((MatchSimple.CompLevelEnum)999, "999")]
    public void ToShortString_MatchSimpleCompLevelEnum_ReturnsExpectedString(MatchSimple.CompLevelEnum compLevel, string expected)
    {
        // Act
        var result = compLevel.ToShortString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Match.CompLevelEnum.Qm, "Qualifications")]
    [InlineData(Match.CompLevelEnum.Qf, "Quarterfinals")]
    [InlineData(Match.CompLevelEnum.Sf, "Playoffs")]
    [InlineData(Match.CompLevelEnum.F, "Finals")]
    [InlineData((Match.CompLevelEnum)999, "999")]
    public void ToLongString_MatchCompLevelEnum_ReturnsExpectedString(Match.CompLevelEnum compLevel, string expected)
    {
        // Act
        var result = compLevel.ToLongString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(MatchSimple.CompLevelEnum.Qm, "Qualifications")]
    [InlineData(MatchSimple.CompLevelEnum.Qf, "Quarterfinals")]
    [InlineData(MatchSimple.CompLevelEnum.Sf, "Playoffs")]
    [InlineData(MatchSimple.CompLevelEnum.F, "Finals")]
    [InlineData((MatchSimple.CompLevelEnum)999, "999")]
    public void ToLongString_MatchSimpleCompLevelEnum_ReturnsExpectedString(MatchSimple.CompLevelEnum compLevel, string expected)
    {
        // Act
        var result = compLevel.ToLongString();

        // Assert
        Assert.Equal(expected, result);
    }
}
