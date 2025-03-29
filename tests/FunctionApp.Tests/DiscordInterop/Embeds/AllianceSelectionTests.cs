namespace FunctionApp.Tests.DiscordInterop.Embeds;

using FunctionApp.DiscordInterop.Embeds;
using FunctionApp.TbaInterop.Models;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

public class AllianceSelectionTests : EmbeddingTest
{
    private readonly AllianceSelection _allianceSelection;

    public AllianceSelectionTests(ITestOutputHelper outputHelper) : base(typeof(AllianceSelection), outputHelper)
    {
        this.Mocker.WithSelfMock<IEventApi>();
        this.Mocker.WithSelfMock<IEventCache>();
        this.Mocker.WithSelfMock<ITeamCache>();

        _allianceSelection = this.Mocker.CreateInstance<AllianceSelection>();
        ((ConcurrentDictionary<string, bool>)typeof(AllianceSelection).GetField("ProcessedEvents", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.GetValue(null)!).Clear();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnAllianceSelectionEmbed()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var webhookMessageJson = """
        {"message_type": "alliance_selection", "message_data": {"event_key": "2025iscmp", "event_name": "FIRST Israel District Championship", "event": {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}}}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        var alliances = new List<EliminationAlliance>
        {
            new(["frc4"], ["frc1", "frc2", "frc3"])
        };

        var rankings = new Dictionary<string, int>
        {
            { "frc1", 1 },
            { "frc2", 2 },
            { "frc3", 3 },
            { "frc4", 4 }
        }.Select(i => new EventRankingRankingsInner(0, [], 1, 100, i.Value, new(1, 0, 0), [], i.Key));

        var eventClient = this.Mocker.GetMock<IEventApi>();
        eventClient.Setup(client => client.GetEventAlliancesAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. alliances]);
        eventClient.Setup(client => client.GetEventRankingsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new EventRanking([], [.. rankings], []));

        var teamCache = this.Mocker.GetMock<ITeamCache>();
        teamCache.SetupGet(client => client["frc1"]).Returns(new Team("address", "city", "country", null, null, "frc1", 0d, 0d, null, "Team 1", "t1", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc2"]).Returns(new Team("address", "city", "country", null, null, "frc2", 0d, 0d, null, "Team 2", "t2", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc3"]).Returns(new Team("address", "city", "country", null, null, "frc3", 0d, 0d, null, "Team 3", "t3", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc4"]).Returns(new Team("address", "city", "country", null, null, "frc4", 0d, 0d, null, "Team 4", "t4", "65498", 2000, "school", "state", 1));

        var eventCache = this.Mocker.GetMock<IEventCache>();
        eventCache.SetupGet(client => client[eventKey]).Returns(JsonSerializer.Deserialize<Event>("""
            {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}
            """)!);

        // Act
        var result = _allianceSelection.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        var response = result.Current;
        Assert.NotNull(response);
        Assert.Contains("FIRST Israel District Championship: Alliance Selection Complete", response.Content.Description);
        Assert.Contains("Alliance 1", response.Content.Description);
        Assert.Contains("t1", response.Content.Description);
        Assert.Contains("t2", response.Content.Description);
        Assert.Contains("t3", response.Content.Description);
        Assert.Contains("Declining Team", response.Content.Description);
        Assert.Contains("t4", response.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ShouldIgnoreDuplicateNotifications()
    {
        // Arrange
        var eventKey = "2025iscmp";
        var webhookMessageJson = """
        {"message_type": "alliance_selection", "message_data": {"event_key": "2025iscmp", "event_name": "FIRST Israel District Championship", "event": {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}}}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        var alliances = new List<EliminationAlliance>
        {
            new(["frc4"], ["frc1", "frc2", "frc3"])
        };

        var rankings = new Dictionary<string, int>
        {
            { "frc1", 1 },
            { "frc2", 2 },
            { "frc3", 3 },
            { "frc4", 4 }
        }.Select(i => new EventRankingRankingsInner(0, [], 1, 100, i.Value, new(1, 0, 0), [], i.Key));

        var eventClient = this.Mocker.GetMock<IEventApi>();
        eventClient.Setup(client => client.GetEventAlliancesAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. alliances]);
        eventClient.Setup(client => client.GetEventRankingsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new EventRanking([], [.. rankings], []));

        var teamCache = this.Mocker.GetMock<ITeamCache>();
        teamCache.SetupGet(client => client["frc1"]).Returns(new Team("address", "city", "country", null, null, "frc1", 0d, 0d, null, "Team 1", "t1", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc2"]).Returns(new Team("address", "city", "country", null, null, "frc2", 0d, 0d, null, "Team 2", "t2", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc3"]).Returns(new Team("address", "city", "country", null, null, "frc3", 0d, 0d, null, "Team 3", "t3", "65498", 2000, "school", "state", 1));
        teamCache.SetupGet(client => client["frc4"]).Returns(new Team("address", "city", "country", null, null, "frc4", 0d, 0d, null, "Team 4", "t4", "65498", 2000, "school", "state", 1));

        var eventCache = this.Mocker.GetMock<IEventCache>();
        eventCache.SetupGet(client => client[eventKey]).Returns(JsonSerializer.Deserialize<Event>("""
            {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}
            """)!);

        // Act
        _ = await _allianceSelection.CreateAsync(webhookMessage).ToArrayAsync();
        var result = _allianceSelection.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        var response = result.Current;
        Assert.Null(response);
        this.Logger.Verify(LogLevel.Trace, "Already processed alliance selection for event 2025iscmp");
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleEmptyAlliances()
    {
        // Arrange
        var eventKey = "2025test";
        var webhookMessageJson = """
        {"message_type": "alliance_selection", "message_data": {"event_key": "2025iscmp", "event_name": "FIRST Israel District Championship", "event": {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}}}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        var alliances = new List<EliminationAlliance>();
        var rankings = new List<EventRankingRankingsInner>();

        var eventClient = this.Mocker.GetMock<IEventApi>();
        eventClient.Setup(client => client.GetEventAlliancesAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. alliances]);
        eventClient.Setup(client => client.GetEventRankingsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new EventRanking([], [.. rankings], []));

        var eventCache = this.Mocker.GetMock<IEventCache>();
        eventCache.SetupGet(client => client[eventKey]).Returns(JsonSerializer.Deserialize<Event>("""
            {"key": "2025iscmp", "name": "FIRST Israel District Championship", "short_name": "Israel", "event_code": "iscmp", "event_type": 2, "event_type_string": "District Championship", "parent_event_key": null, "playoff_type": 10, "playoff_type_string": "Double Elimination Bracket (8 Alliances)", "district": {"key": "2025isr", "year": 2025, "abbreviation": "isr", "display_name": "FIRST Israel"}, "division_keys": [], "first_event_id": null, "first_event_code": "iscmp", "year": 2025, "timezone": "Asia/Jerusalem", "week": 4, "website": "http://firstisrael.org.il", "city": "Jerusalem", "state_prov": "JM", "country": "Israel", "postal_code": null, "lat": null, "lng": null, "location_name": null, "address": null, "gmaps_place_id": null, "gmaps_url": null, "start_date": "2025-03-25", "end_date": "2025-03-27", "webcasts": [{"type": "twitch", "channel": "firstisrael"}]}
            """)!);

        CancellationTokenSource cts = new();
        bool gotCancelled = false;

        // Act
        cts.CancelAfter(TimeSpan.FromSeconds(3));
        try
        {
            await _allianceSelection.CreateAsync(webhookMessage, cancellationToken: cts.Token).ToArrayAsync();
        }
        catch (TaskCanceledException)
        {
            gotCancelled = true;
        }

        // Assert
        Assert.True(cts.IsCancellationRequested);
        Assert.True(gotCancelled);
        this.Logger.Verify(LogLevel.Warning, "Failed to retrieve alliance selection data for 2025iscmp");
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleNullNotification()
    {
        // Arrange
        var webhookMessageJson = """
        {
            "message_type": "alliance_selection",
            "message_data": {}
        }
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        // Act
        var result = _allianceSelection.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        Assert.Null(result.Current);
        this.Logger.Verify(LogLevel.Warning, "Failed to deserialize notification data as alliance_selection");
    }
}
