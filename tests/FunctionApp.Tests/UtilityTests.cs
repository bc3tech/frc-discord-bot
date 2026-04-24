namespace FunctionApp.Tests;

using Discord;

using FunctionApp.DiscordInterop;

public sealed class UtilityTests
{
    [Fact]
    public void CreateCountryFlagEmojiRefWithValidCountryCodeReturnsDiscordEmojiCode()
    {
        string emoji = Utility.CreateCountryFlagEmojiRef("US");

        Assert.Equal(":flag_us:", emoji);
    }

    [Theory]
    [InlineData("")]
    [InlineData("U")]
    [InlineData("USA")]
    public void CreateCountryFlagEmojiRefWithInvalidCountryCodeThrowsArgumentException(string countryCode)
    {
        Assert.Throws<ArgumentException>(() => Utility.CreateCountryFlagEmojiRef(countryCode));
    }

    [Fact]
    public void CreateCountryFlagUrlWithValidInputsComposesExpectedUrl()
    {
        Uri uri = Utility.CreateCountryFlagUrl("US", 64);

        Assert.Equal(new Uri("https://flagsapi.com/US/shiny/64.png"), uri);
    }

    [Fact]
    public void GetLightestColorOfWithNoColorsReturnsNull()
    {
        Color? color = Utility.GetLightestColorOf([]);

        Assert.Null(color);
    }

    [Fact]
    public void GetLightestColorOfWithColorsReturnsHighestLuminanceColor()
    {
        Color? color = Utility.GetLightestColorOf(
            new Color(10, 10, 10),
            new Color(250, 250, 250),
            new Color(50, 50, 50));

        Assert.Equal(new Color(250, 250, 250), color);
    }
}
