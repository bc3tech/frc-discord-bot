namespace FunctionApp.Tests;

using FunctionApp.TbaInterop;

public sealed class AwardTypeEnumExtensionsTests
{
    [Fact]
    public void IsBlueBannerWhenAwardIsBlueBannerTypeReturnsTrue()
    {
        Assert.True(AwardType.Chairmans.IsBlueBanner());
    }

    [Fact]
    public void IsBlueBannerWhenAwardIsNotBlueBannerTypeReturnsFalse()
    {
        Assert.False(AwardType.Quality.IsBlueBanner());
    }

    [Fact]
    public void IndividualAwardsIncludesExpectedAwardTypes()
    {
        Assert.Contains(AwardType.DeansList, AwardTypeEnumExtensions.IndividualAwards);
        Assert.Contains(AwardType.Volunteer, AwardTypeEnumExtensions.IndividualAwards);
    }
}
