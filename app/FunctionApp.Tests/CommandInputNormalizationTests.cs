namespace FunctionApp.Tests;

using FunctionApp.DiscordInterop.CommandModules;

using TheBlueAlliance.Model;

public sealed class CommandInputNormalizationTests
{
    [Theory]
    [InlineData("2046", "frc2046")]
    [InlineData("frc1114", "frc1114")]
    [InlineData("abc", "abc")]
    public void NormalizeTeamKeyWithAnyInputReturnsExpectedTeamKey(string teamKey, string expected)
        => Assert.Equal(expected, CommandInputNormalization.NormalizeTeamKey(teamKey));

    [Theory]
    [InlineData((int)CompLevel.Qm, 7u, "2026cabl_qm7")]
    [InlineData((int)CompLevel.Sf, 2u, "2026cabl_sf2m1")]
    [InlineData((int)CompLevel.F, 3u, "2026cabl_f1m3")]
    public void BuildMatchKeyWithKnownCompLevelReturnsExpectedFormat(int compLevel, uint matchNumber, string expected)
        => Assert.Equal(expected, CommandInputNormalization.BuildMatchKey("2026cabl", compLevel, matchNumber));

    [Fact]
    public void BuildMatchKeyWithUnknownCompLevelThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CommandInputNormalization.BuildMatchKey("2026cabl", compLevel: -1, matchNumber: 1));
    }
}
