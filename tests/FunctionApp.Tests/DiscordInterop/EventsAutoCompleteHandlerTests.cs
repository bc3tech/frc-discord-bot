namespace FunctionApp.Tests.DiscordInterop;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using Moq;
using Moq.AutoMock;

using System.Text.Json;

using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using static FunctionApp.DiscordInterop.AutoCompleteHandlers;

public class EventsAutoCompleteHandlerTests
{
    private static readonly AutoMocker _mocker = new();
    private static readonly Mock<ILogger<EventsAutoCompleteHandler>> _mockLogger = new();

    public EventsAutoCompleteHandlerTests()
    {
        _mockLogger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        var mockLogFactory = new Mock<ILoggerFactory>();
        mockLogFactory.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
        _mocker.Use(mockLogFactory);

        _mocker.Use(TimeProvider.System);
    }

    [Fact]
    public async Task GenerateSuggestionsAsync_ShouldReturnSuggestions()
    {
        // Arrange
        var eventsJson = """
            {
                "2025wabon":{
            	"address": "10920 199th Ave Ct E, Bonney Lake, WA 98391, USA",
            	"city": "Bonney Lake",
            	"country": "USA",
            	"district": {
            		"abbreviation": "pnw",
            		"display_name": "Pacific Northwest",
            		"key": "2025pnw",
            		"year": 2025
            	},
            	"division_keys": [],
            	"end_date": "2025-03-16",
            	"event_code": "wabon",
            	"event_type": 1,
            	"event_type_string": "District",
            	"first_event_code": "wabon",
            	"first_event_id": null,
            	"gmaps_place_id": "ChIJfU3qX9_6kFQR1QMqEr9wwYQ",
            	"gmaps_url": "https://maps.google.com/?q=10920+199th+Ave+Ct+E,+Bonney+Lake,+WA+98391,+USA&ftid=0x5490fadf5fea4d7d:0x84c170bf122a03d5",
            	"key": "2025wabon",
            	"lat": 47.1594537,
            	"lng": -122.1689707,
            	"location_name": "10920 199th Ave Ct E",
            	"name": "PNW District Bonney Lake Event",
            	"parent_event_key": null,
            	"playoff_type": 10,
            	"playoff_type_string": "Double Elimination Bracket (8 Alliances)",
            	"postal_code": "98391",
            	"short_name": "Bonney Lake",
            	"start_date": "2025-03-14",
            	"state_prov": "WA",
            	"timezone": "America/Los_Angeles",
            	"webcasts": [
            		{
            			"channel": "firstinspires15",
            			"type": "twitch"
            		}
            	],
            	"website": "http://www.firstwa.org",
            	"week": 2,
            	"year": 2025
            },
            "2025waahs":{
            	"address": "711 E Main St, Auburn, WA 98002, USA",
            	"city": "Auburn",
            	"country": "USA",
            	"district": {
            		"abbreviation": "pnw",
            		"display_name": "Pacific Northwest",
            		"key": "2025pnw",
            		"year": 2025
            	},
            	"division_keys": [],
            	"end_date": "2025-03-23",
            	"event_code": "waahs",
            	"event_type": 1,
            	"event_type_string": "District",
            	"first_event_code": "waahs",
            	"first_event_id": null,
            	"gmaps_place_id": "ChIJDzaxXXtYkFQR3eZfaNdUpzE",
            	"gmaps_url": "https://maps.google.com/?q=711+E+Main+St,+Auburn,+WA+98002,+USA&ftid=0x5490587b5db1360f:0x31a754d7685fe6dd",
            	"key": "2025waahs",
            	"lat": 47.3095644,
            	"lng": -122.220087,
            	"location_name": "711 E Main St",
            	"name": "PNW District Auburn Event",
            	"parent_event_key": null,
            	"playoff_type": 10,
            	"playoff_type_string": "Double Elimination Bracket (8 Alliances)",
            	"postal_code": "98002",
            	"short_name": "Auburn",
            	"start_date": "2025-03-21",
            	"state_prov": "WA",
            	"timezone": "America/Los_Angeles",
            	"webcasts": [
            		{
            			"channel": "firstinspires15",
            			"type": "twitch"
            		},
            		{
            			"channel": "firstinspires25",
            			"type": "twitch"
            		}
            	],
            	"website": "http://www.firstwa.org",
            	"week": 3,
            	"year": 2025
            }
            }
            """;

        var events = JsonSerializer.Deserialize<Dictionary<string, Event>>(eventsJson);
        Assert.NotNull(events);
        var eventsRepoMock = _mocker.GetMock<IEventCache>();
        eventsRepoMock.Setup(repo => repo.AllEvents).Returns(events.AsReadOnly());

        var autocompleteDataMock = _mocker.GetMock<IAutocompleteInteractionData>();
        autocompleteDataMock.SetupGet(ai => ai.Current).Returns(
            (AutocompleteOption)Activator.CreateInstance(typeof(AutocompleteOption),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
                [ApplicationCommandOptionType.User, "Event", null, true],
                null)!);

        var autocompleteInteractionMock = _mocker.GetMock<IAutocompleteInteraction>();
        autocompleteInteractionMock
            .SetupGet(ai => ai.Data)
            .Returns(autocompleteDataMock.Object);

        // Act
        var result = await _mocker.CreateInstance<EventsAutoCompleteHandler>()
            .GenerateSuggestionsAsync(_mocker.Get<IInteractionContext>(), autocompleteInteractionMock.Object, _mocker.Get<IParameterInfo>(), _mocker);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Suggestions.Count);
    }

    [Fact]
    public async Task GenerateSuggestionsAsync_ShouldHandleDiscordException()
    {
        // Arrange
        var eventsRepoMock = _mocker.GetMock<IEventCache>();
        eventsRepoMock.SetupGet(repo => repo.AllEvents).Throws(new Discord.Net.HttpException(System.Net.HttpStatusCode.InternalServerError, Mock.Of<Discord.Net.IRequest>(), DiscordErrorCode.UnknownInteraction));

        var autocompleteDataMock = _mocker.GetMock<IAutocompleteInteractionData>();
        autocompleteDataMock.SetupGet(ai => ai.Current).Returns(
            (AutocompleteOption)Activator.CreateInstance(typeof(AutocompleteOption),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
                [ApplicationCommandOptionType.User, "Event", null, true],
                null)!);

        var autocompleteInteractionMock = _mocker.GetMock<IAutocompleteInteraction>();
        autocompleteInteractionMock
            .SetupGet(ai => ai.Data)
            .Returns(autocompleteDataMock.Object);

        // Act
        var result = await _mocker.CreateInstance<EventsAutoCompleteHandler>()
            .GenerateSuggestionsAsync(_mocker.Get<IInteractionContext>(), autocompleteInteractionMock.Object, _mocker.Get<IParameterInfo>(), _mocker);

        // Assert
        _mockLogger.Verify(l => l.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
