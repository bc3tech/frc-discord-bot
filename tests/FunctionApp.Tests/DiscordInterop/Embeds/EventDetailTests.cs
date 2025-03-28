using Discord;

using FunctionApp.Apis;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

namespace FunctionApp.Tests.DiscordInterop.Embeds
{
    public class EventDetailTests : TestWithLogger
    {
        private readonly Mock<IRESTCountries> _mockCountryCodeLookup;
        private readonly Mock<IEventCache> _mockEventsRepo;
        private readonly Mock<Statbotics.Api.IEventApi> _mockEventStats;
        private readonly EventDetail _eventDetail;

        public EventDetailTests(ITestOutputHelper outputHelper) : base(typeof(EventDetail), outputHelper)
        {
            _mockCountryCodeLookup = this.Mocker.GetMock<IRESTCountries>();
            _mockEventsRepo = this.Mocker.GetMock<IEventCache>();
            _mockEventStats = this.Mocker.GetMock<Statbotics.Api.IEventApi>();

            this.Mocker.Use(_mockCountryCodeLookup.Object);
            this.Mocker.Use(new EmbedBuilderFactory(new EmbeddingColorizer(this.Mocker.CreateSelfMock<FRCColors.IClient>(), null)));
            this.Mocker.Use(_mockEventsRepo.Object);
            this.Mocker.Use(_mockEventStats.Object);

            _eventDetail = this.Mocker.CreateInstance<EventDetail>();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnEventDetailsWhenNoStatsAvailable()
        {
            // Arrange
            var eventKey = "2025test";
            var eventDetailsJson = """
            {
                "key": "2025test",
                "name": "Test Event",
                "year": 2025,
                "week": 1,
                "event_type_string": "Regional",
                "location_name": "Test Location",
                "city": "Test City",
                "state_prov": "Test State",
                "country": "Test Country",
                "gmaps_url": "http://maps.google.com",
                "schedule_url": "http://schedule.com",
                "website": "http://event.com",
                "start_date": "2025-01-01",
                "end_date": "2025-01-03",
                "webcasts": [
                    {
                        "channel": "channel",
                        "type": "youtube"
                    }
                ],
                "address": "123 Test St",
                "district": null,
                "division_keys": [],
                "event_code": "TEST",
                "event_type": 0,
                "first_event_code": "test",
                "first_event_id": null,
                "gmaps_place_id": "place_id",
                "lat": 0.0,
                "lng": 0.0,
                "parent_event_key": null,
                "playoff_type": 0,
                "playoff_type_string": "Test Playoff",
                "postal_code": "12345",
                "short_name": "Test Event",
                "timezone": "America/New_York"
            }
            """;
            var eventDetails = JsonSerializer.Deserialize<Event>(eventDetailsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _mockEventsRepo.Setup(repo => repo[eventKey]).Returns(eventDetails);
            _mockCountryCodeLookup.Setup(c => c.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default)).ReturnsAsync("US");

            // Act
            var result = _eventDetail.CreateAsync(eventKey);

            // Assert
            var results = await result.ToArrayAsync();
            var response = results.FirstOrDefault();
            Assert.NotNull(response);
            Assert.True(response.Transient);
            Assert.Equal($"**{eventDetails.Name}**", response.Content.Title);
            Assert.Contains($"{eventDetails.LocationName}, {eventDetails.City}, {eventDetails.StateProv}, {eventDetails.Country}", response.Content.Description);
            Assert.Contains($"Schedule [here]({eventDetails.ScheduleUrl})", response.Content.Description);
            Assert.Contains("Where to watch", response.Content.Fields[1].Name);
            Assert.Contains("Event details on TBA", response.Content.Fields[2].Name);
            Assert.Contains("Stats", response.Content.Fields[3].Name);

            var statsUpdateEmbed = results[1];
            Assert.False(statsUpdateEmbed.Transient);
            Assert.Contains("No stats available", statsUpdateEmbed.Content.Fields[^1].Value);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnEventDetailsWhenStatsAvailable()
        {
            // Arrange
            var eventKey = "2025test";
            var eventDetailsJson = """
            {
                "key": "2025test",
                "name": "Test Event",
                "year": 2025,
                "week": 1,
                "event_type_string": "Regional",
                "location_name": "Test Location",
                "city": "Test City",
                "state_prov": "Test State",
                "country": "Test Country",
                "gmaps_url": "http://maps.google.com",
                "schedule_url": "http://schedule.com",
                "website": "http://event.com",
                "start_date": "2025-01-01",
                "end_date": "2025-01-03",
                "webcasts": [
                    {
                        "channel": "channel",
                        "type": "youtube"
                    }
                ],
                "address": "123 Test St",
                "district": null,
                "division_keys": [],
                "event_code": "TEST",
                "event_type": 0,
                "first_event_code": "test",
                "first_event_id": null,
                "gmaps_place_id": "place_id",
                "lat": 0.0,
                "lng": 0.0,
                "parent_event_key": null,
                "playoff_type": 0,
                "playoff_type_string": "Test Playoff",
                "postal_code": "12345",
                "short_name": "Test Event",
                "timezone": "America/New_York"
            }
            """;
            var eventDetails = JsonSerializer.Deserialize<Event>(eventDetailsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _mockEventsRepo.Setup(repo => repo[eventKey]).Returns(eventDetails);
            _mockCountryCodeLookup.Setup(c => c.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default)).ReturnsAsync("US");

            _mockEventStats.Setup(s => s.ReadEventV3EventEventGetAsync(eventKey, It.IsAny<CancellationToken>())).ReturnsAsync(new Statbotics.Model.Event { NumTeams = 1, Status = "Ongoing", EpaVal = new Statbotics.Model.Event.Epa { Max = 101, Mean = 85 } });

            // Act
            var result = _eventDetail.CreateAsync(eventKey);

            // Assert
            var results = await result.ToArrayAsync();
            var response = results.FirstOrDefault();
            Assert.NotNull(response);
            Assert.True(response.Transient);
            Assert.Equal($"**{eventDetails.Name}**", response.Content.Title);
            Assert.Contains($"{eventDetails.LocationName}, {eventDetails.City}, {eventDetails.StateProv}, {eventDetails.Country}", response.Content.Description);
            Assert.Contains($"Schedule [here]({eventDetails.ScheduleUrl})", response.Content.Description);
            Assert.Contains("Where to watch", response.Content.Fields[1].Name);
            Assert.Contains("Event details on TBA", response.Content.Fields[2].Name);
            Assert.Contains("Stats", response.Content.Fields[3].Name);

            var statsUpdateEmbed = results[1];
            Assert.False(statsUpdateEmbed.Transient);
            Assert.Contains("Max EPA", statsUpdateEmbed.Content.Fields[^1].Value);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleStatboticsException()
        {
            // Arrange
            var eventKey = "2025test";
            var eventDetailsJson = """
            {
                "key": "2025test",
                "name": "Test Event",
                "year": 2025,
                "week": 1,
                "event_type_string": "Regional",
                "location_name": "Test Location",
                "city": "Test City",
                "state_prov": "Test State",
                "country": "Test Country",
                "gmaps_url": "http://maps.google.com",
                "schedule_url": "http://schedule.com",
                "website": "http://event.com",
                "start_date": "2025-01-01",
                "end_date": "2025-01-03",
                "webcasts": [
                    {
                        "channel": "channel",
                        "type": "youtube"
                    }
                ],
                "address": "123 Test St",
                "district": null,
                "division_keys": [],
                "event_code": "TEST",
                "event_type": 0,
                "first_event_code": "test",
                "first_event_id": null,
                "gmaps_place_id": "place_id",
                "lat": 0.0,
                "lng": 0.0,
                "parent_event_key": null,
                "playoff_type": 0,
                "playoff_type_string": "Test Playoff",
                "postal_code": "12345",
                "short_name": "Test Event",
                "timezone": "America/New_York"
            }
            """;
            var eventDetails = JsonSerializer.Deserialize<Event>(eventDetailsJson);

            _mockEventsRepo.Setup(repo => repo[eventKey]).Returns(eventDetails);
            _mockCountryCodeLookup.Setup(c => c.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default)).ReturnsAsync("US");
            _mockEventStats.Setup(s => s.ReadEventV3EventEventGetAsync(eventKey, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Statbotics error"));

            // Act
            var result = _eventDetail.CreateAsync(eventKey).GetAsyncEnumerator();

            // Assert
            Assert.True(await result.MoveNextAsync());
            var response = result.Current;
            Assert.NotNull(response);
            Assert.True(response.Transient);
            Assert.Contains("Checking", response.Content.Fields[^1].Value);
            Assert.True(await result.MoveNextAsync());
            response = result.Current;
            Assert.Contains("No stats available.", response.Content.Fields[^1].Value); ;
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnEventDetailsWithDistrictField()
        {
            // Arrange
            var eventKey = "2025test";
            var eventDetailsJson = """
            {
                "key": "2025test",
                "name": "Test Event",
                "year": 2025,
                "week": 1,
                "event_type_string": "Regional",
                "location_name": "Test Location",
                "city": "Test City",
                "state_prov": "Test State",
                "country": "Test Country",
                "gmaps_url": "http://maps.google.com",
                "schedule_url": "http://schedule.com",
                "website": "http://event.com",
                "start_date": "2025-01-01",
                "end_date": "2025-01-03",
                "webcasts": [
                    {
                        "channel": "channel",
                        "type": "youtube"
                    }
                ],
                "address": "123 Test St",
                "district": {
                    "abbreviation": "test",
                    "key": "2025test",
                    "year": 2025,
                    "display_name": "Test District"
                },
                "division_keys": [],
                "event_code": "TEST",
                "event_type": 0,
                "first_event_code": "test",
                "first_event_id": null,
                "gmaps_place_id": "place_id",
                "lat": 0.0,
                "lng": 0.0,
                "parent_event_key": null,
                "playoff_type": 0,
                "playoff_type_string": "Test Playoff",
                "postal_code": "12345",
                "short_name": "Test Event",
                "timezone": "America/New_York"
            }           
            """;
            var eventDetails = JsonSerializer.Deserialize<Event>(eventDetailsJson)!;

            _mockEventsRepo.Setup(repo => repo[eventKey]).Returns(eventDetails);
            _mockCountryCodeLookup.Setup(c => c.GetCountryCodeForFlagLookupAsync(eventDetails.Country, default)).ReturnsAsync("US");

            // Act
            var result = _eventDetail.CreateAsync(eventKey);

            // Assert
            var results = await result.ToArrayAsync();
            var response = results.FirstOrDefault();
            Assert.NotNull(response);
            Assert.True(response.Transient);
            Assert.Equal($"**{eventDetails.Name}**", response.Content.Title);
            Assert.True(response.Content.Fields.Any(i => i.Name == "District" && i.Value == "Test District"));
        }
    }
}
