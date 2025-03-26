namespace TheBlueAlliance.Tests.Model;

using System.Collections.ObjectModel;

using TheBlueAlliance.Model;


public class Event_GetLabelTests
{
    [Fact]
    public void GetLabel_DefaultParameters_ReturnsName()
    {
        // Arrange
        var eventInstance = CreateEventInstance();

        // Act
        var result = eventInstance.GetLabel();

        // Assert
        Assert.Equal("Test Event", result);
    }

    [Fact]
    public void GetLabel_ShortName_ReturnsShortName()
    {
        // Arrange
        var eventInstance = CreateEventInstance();

        // Act
        var result = eventInstance.GetLabel(shortName: true);

        // Assert
        Assert.Equal("Test Short Name", result);
    }

    [Fact]
    public void GetLabel_IncludeYear_ReturnsNameWithYear()
    {
        // Arrange
        var eventInstance = CreateEventInstance();

        // Act
        var result = eventInstance.GetLabel(includeYear: true);

        // Assert
        Assert.Equal("2023 Test Event", result);
    }

    [Fact]
    public void GetLabel_IncludeCityStateCountry_ReturnsNameWithLocation()
    {
        // Arrange
        var eventInstance = CreateEventInstance();

        // Act
        var result = eventInstance.GetLabel(includeCity: true, includeStateProv: true, includeCountry: true);

        // Assert
        Assert.Equal("Test Event - Test City, Test State, Test Country", result);
    }

    [Fact]
    public void GetLabel_AsMarkdownLink_ReturnsMarkdownLink()
    {
        // Arrange
        var eventInstance = CreateEventInstance();

        // Act
        var result = eventInstance.GetLabel(asMarkdownLink: true);

        // Assert
        Assert.Equal("[Test Event](https://frc.link/e/tba/testcode/2023)", result);
    }

    private static Event CreateEventInstance()
    {
        return new Event(
            address: "Test Address",
            city: "Test City",
            country: "Test Country",
            district: new DistrictList("Test Abbreviation", "Test Display Name", "Test Key", 2023),
            divisionKeys: new Collection<string> { "Test Division Key" },
            endDate: new DateOnly(2023, 12, 31),
            eventCode: "testcode",
            eventType: 1,
            eventTypeString: "Regional",
            firstEventCode: "testcode",
            firstEventId: "testid",
            gmapsPlaceId: "testplaceid",
            gmapsUrl: "https://maps.google.com",
            key: "2023testcode",
            lat: 0.0,
            lng: 0.0,
            locationName: "Test Location",
            name: "Test Event",
            parentEventKey: "parentkey",
            playoffType: 1,
            playoffTypeString: "Test Playoff",
            postalCode: "12345",
            shortName: "Test Short Name",
            startDate: new DateOnly(2023, 1, 1),
            stateProv: "Test State",
            timezone: "Test Timezone",
            webcasts: new Collection<Webcast> { new Webcast("Test Channel", Webcast.TypeEnum.Youtube) },
            website: "https://test.com",
            week: 1,
            year: 2023
        );
    }
}
