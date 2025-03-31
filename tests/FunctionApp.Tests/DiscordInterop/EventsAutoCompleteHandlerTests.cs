namespace FunctionApp.Tests.DiscordInterop;

using Discord;
using Discord.Net;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Concurrent;
using System.Text.Json;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit.Abstractions;

using static FunctionApp.DiscordInterop.AutoCompleteHandlers;

using IParameterInfo = Discord.Interactions.IParameterInfo;

public class EventsAutoCompleteHandlerTests : TestWithLogger, IDisposable
{
    private readonly IDisposable _eventCacheAccessor = RequireClearedEventCache();

    public EventsAutoCompleteHandlerTests(ITestOutputHelper outputHelper) : base(typeof(EventsAutoCompleteHandler), outputHelper)
    {
        this.Mocker.With<EventCache>();
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
        var eventsRepoMock = this.Mocker.GetMock<IEventApi>();
        eventsRepoMock
            .Setup(repo => repo.GetEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string key, string _) => events[key]);

        // Load up the EventCache's static dictionary w/ the ones for this test, via reflection
        // because suggestions pull from `AllEvents` property of the cache
        var eventCacheType = typeof(EventCache);
        var eventCacheField = eventCacheType.GetField("_events", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        var eventCache = (ConcurrentDictionary<string, Event>)eventCacheField!.GetValue(null)!;
        foreach (var kvp in events)
        {
            eventCache.TryAdd(kvp.Key, kvp.Value);
        }

        var autocompleteDataMock = this.Mocker.GetMock<IAutocompleteInteractionData>();
        autocompleteDataMock.SetupGet(ai => ai.Current).Returns(
            (AutocompleteOption)Activator.CreateInstance(typeof(AutocompleteOption),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null,
                [ApplicationCommandOptionType.User, "Event", null, true],
                null)!);

        var autocompleteInteractionMock = this.Mocker.GetMock<IAutocompleteInteraction>();
        autocompleteInteractionMock
            .SetupGet(ai => ai.Data)
            .Returns(autocompleteDataMock.Object);

        // Act
        var result = await this.Mocker.CreateInstance<EventsAutoCompleteHandler>()
            .GenerateSuggestionsAsync(this.Mocker.CreateSelfMock<IInteractionContext>(), autocompleteInteractionMock.Object, this.Mocker.CreateSelfMock<IParameterInfo>(), this.Mocker);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Suggestions.Count);
    }

    [Fact]
    public async Task GenerateSuggestionsAsync_ShouldHandleHttpException()
    {
        // Arrange
        var autocompleteInteractionMock = this.Mocker.GetMock<IAutocompleteInteraction>();
        autocompleteInteractionMock
            .SetupGet(ai => ai.Data)
            .Throws(new HttpException(System.Net.HttpStatusCode.InternalServerError, this.Mocker.CreateSelfMock<IRequest>(), DiscordErrorCode.UnknownInteraction));

        // Act
        var result = await this.Mocker.CreateInstance<EventsAutoCompleteHandler>()
            .GenerateSuggestionsAsync(this.Mocker.Get<IInteractionContext>(), autocompleteInteractionMock.Object, this.Mocker.Get<IParameterInfo>(), this.Mocker);

        // Assert
        this.Logger.Verify(LogLevel.Debug);
    }

    public void Dispose()
    {
        _eventCacheAccessor.Dispose();
    }
}
