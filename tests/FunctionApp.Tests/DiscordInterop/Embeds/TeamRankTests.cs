namespace FunctionApp.Tests.DiscordInterop.Embeds;

using Common.Extensions;

using FIRST.Api;
using FIRST.Model;

using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TestCommon;

using TheBlueAlliance.Api;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

using Event = TheBlueAlliance.Model.Event;
using StatboticsTeam = Statbotics.Model.Team;
using Team = TheBlueAlliance.Model.Team;

public class TeamRankTests : EmbeddingTest
{
    private readonly TeamRank _teamRank;

    public TeamRankTests(ITestOutputHelper outputHelper) : base(typeof(TeamRank), outputHelper)
    {
        this.Mocker.CreateSelfMock<ITeamCache>();
        this.Mocker.GetMock<ITeamCache>().Setup(t => t[It.IsAny<string>()]).Returns((string key) => key is "invalid" ? throw new KeyNotFoundException() : _utTeam);
        this.Mocker.CreateSelfMock<IEventCache>();
        this.Mocker.GetMock<IEventCache>().Setup(i => i[It.IsAny<string>()]).Returns((string key) => key is "invalid" ? throw new KeyNotFoundException() : _utEvent);
        this.Mocker.CreateSelfMock<IDistrictApi>();
        this.Mocker.CreateSelfMock<Statbotics.Api.ITeamYearApi>();
        this.Mocker.CreateSelfMock<IRankingsApi>();

        _teamRank = this.Mocker.CreateInstance<TeamRank>();
    }

    private static readonly Team _utTeam = JsonSerializer.Deserialize<Team>("""
                {
        	"address": null,
        	"city": "Maple Valley",
        	"country": "USA",
        	"gmaps_place_id": null,
        	"gmaps_url": null,
        	"key": "frc2046",
        	"lat": null,
        	"lng": null,
        	"location_name": null,
        	"motto": null,
        	"name": "Washington State OSPI/The Truck Shop/1-800-Got-Junk/West Coast Products&Tahoma Senior High School",
        	"nickname": "Bear Metal",
        	"postal_code": "98038",
        	"rookie_year": 2007,
        	"school_name": "Tahoma Senior High School",
        	"state_prov": "Washington",
        	"team_number": 2046,
        	"website": "http://tahomarobotics.org/"
        }
        """)!;
    private static readonly Statbotics.Model.TeamYear _utTeamYear = JsonSerializer.Deserialize<Statbotics.Model.TeamYear>("""
        {
        	"team": 2046,
        	"year": 2025,
        	"name": "Bear Metal",
        	"country": "USA",
        	"state": "WA",
        	"district": "pnw",
        	"epa": {
        		"total_points": {
        			"mean": 64.11,
        			"sd": 8.71
        		},
        		"unitless": 1747,
        		"norm": 1732,
        		"conf": [
        			-0.81,
        			0.94
        		],
        		"breakdown": {
        			"total_points": 64.11,
        			"auto_points": 16.54,
        			"teleop_points": 41.02,
        			"endgame_points": 6.55,
        			"auto_rp": 0.591,
        			"coral_rp": 0.3081,
        			"barge_rp": 0.19619,
        			"tiebreaker_points": 0.02,
        			"auto_leave_points": 3.45,
        			"auto_coral": 1.94,
        			"auto_coral_points": 13.09,
        			"teleop_coral": 10.15,
        			"teleop_coral_points": 38.53,
        			"coral_l1": 1.41,
        			"coral_l2": 2.17,
        			"coral_l3": 4.26,
        			"coral_l4": 4.26,
        			"total_coral_points": 51.62,
        			"processor_algae": 0.13,
        			"processor_algae_points": 0.39,
        			"net_algae": 0.52,
        			"net_algae_points": 2.1,
        			"total_algae_points": 2.49,
        			"total_game_pieces": 12.95,
        			"barge_points": 6.55,
        			"rp_1": 0.591,
        			"rp_2": 0.3081,
        			"rp_3": 0.19619
        		},
        		"stats": {
        			"start": 48.35,
        			"pre_champs": 64.11,
        			"max": 67.65
        		},
        		"ranks": {
        			"total": {
        				"rank": 100,
        				"percentile": 0.9729,
        				"team_count": 3695
        			},
        			"country": {
        				"rank": 77,
        				"percentile": 0.9737,
        				"team_count": 2930
        			},
        			"state": {
        				"rank": 4,
        				"percentile": 0.9565,
        				"team_count": 92
        			},
        			"district": {
        				"rank": 5,
        				"percentile": 0.9621,
        				"team_count": 132
        			}
        		}
        	},
        	"record": {
        		"wins": 36,
        		"losses": 15,
        		"ties": 1,
        		"count": 52,
        		"winrate": 0.7019
        	},
        	"district_points": 142,
        	"district_rank": 4,
        	"competing": {
        		"this_week": false,
        		"next_event_key": "2025pncmp",
        		"next_event_name": "Pacific Northwest FIRST District Championship",
        		"next_event_week": 6
        	}
        }
        """)!;
    private static readonly Collection<DistrictList> _utTeamDistricts = [.. JsonSerializer.Deserialize<List<DistrictList>>("""
        [
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2014pnw",
        		"year": 2014
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2015pnw",
        		"year": 2015
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2016pnw",
        		"year": 2016
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2017pnw",
        		"year": 2017
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2018pnw",
        		"year": 2018
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2019pnw",
        		"year": 2019
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2020pnw",
        		"year": 2020
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2021pnw",
        		"year": 2021
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2022pnw",
        		"year": 2022
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2023pnw",
        		"year": 2023
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2024pnw",
        		"year": 2024
        	},
        	{
        		"abbreviation": "pnw",
        		"display_name": "Pacific Northwest",
        		"key": "2025pnw",
        		"year": 2025
        	}
        ]
        """)!];
    private static readonly SeasonRankingsDistrict _utDistrictRankings = JsonSerializer.Deserialize<SeasonRankingsDistrict>("""
        {
        	"districtRanks": [
        		{
        			"districtCode": "PNW",
        			"teamNumber": 2046,
        			"rank": 3,
        			"totalPoints": 69,
        			"event1Code": "WASNO",
        			"event1Points": 69,
        			"event2Code": "WABON",
        			"event2Points": 0,
        			"districtCmpCode": null,
        			"districtCmpPoints": 0,
        			"teamAgePoints": 0,
        			"adjustmentPoints": 0,
        			"qualifiedDistrictCmp": false,
        			"qualifiedFirstCmp": false
        		}
        	],
        	"rankingCountTotal": 1,
        	"rankingCountPage": 1,
        	"pageCurrent": 1,
        	"pageTotal": 1
        }
        """)!;

    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsEmbedding()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYear(input.Year.Value, It.IsAny<string>())).Returns(_utTeamDistricts);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var embedding = result[0];
        Assert.NotNull(embedding);
        var content = embedding.Content;
        Assert.NotNull(content);
        Assert.Equal("2025 Ranking detail for 2046 Bear Metal", content.Title);
        Assert.Equal(_utTeam.TbaUrl, content.Url);

        embedding = result[1];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Equal("Getting district-wide data...", embedding.Content.Title);

        embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_InvalidTeamKey_LogsAndReturnsNull()
    {
        var input = (Year: (int?)2022, TeamKey: "invalid", EventKey: (string?)null);
        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Throws(new KeyNotFoundException());

        DebugHelper.IgnoreDebugAsserts();
        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains("Invalid team key", result[0]!.Content.Title);

        this.Logger.Verify(LogLevel.Error, "Unable to get team number from key invalid");
    }

    [Fact]
    public async Task CreateAsync_NoDistrictRankingData_ReturnsNoDataMessage()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((SeasonRankingsDistrict?)null);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains("No district ranking data found", result[^1]!.Content.Description);
    }

    private static readonly Event _utEvent = JsonSerializer.Deserialize<Event>("""
        {
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
        }
        """)!;
    private static readonly SeasonRankingsEvent _utEventRankings = JsonSerializer.Deserialize<SeasonRankingsEvent>("""
        {
        	"Rankings": [
        		{
        			"rank": 1,
        			"teamNumber": 2046,
        			"sortOrder1": 4.5,
        			"sortOrder2": 0.17,
        			"sortOrder3": 131.92,
        			"sortOrder4": 28.83,
        			"sortOrder5": 12.5,
        			"sortOrder6": 0,
        			"wins": 9,
        			"losses": 2,
        			"ties": 1,
        			"qualAverage": 134.41,
        			"dq": 0,
        			"matchesPlayed": 12
        		}
        	]
        }
        """)!;

    [Fact]
    public async Task CreateAsync_ValidInputWithNoDistrictData_ReturnsNoDistrictDataMessage()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYear(input.Year.Value, It.IsAny<string>())).Returns((Collection<DistrictList>?)null);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((SeasonRankingsDistrict?)null);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains("No district ranking data found", result[^1]!.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithDistrictData_ReturnsDistrictData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYear(input.Year.Value, It.IsAny<string>())).Returns(_utTeamDistricts);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        var embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithEventKey_ReturnsEventData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetEventDistrictPointsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventDistrictPoints);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventRankings);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District Bonney Lake Event", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithEventKeyNoDistrictPoints_LogsNoDistrictPointsDataFound()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        var pointsWithoutEvent = new Dictionary<string, EventDistrictPointsPointsValue>(_utEventDistrictPoints.Points)
        {
            [input.TeamKey] = null
        };
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetEventDistrictPointsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventDistrictPoints with { Points = pointsWithoutEvent });
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventRankings);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District Bonney Lake Event", embedding.Content.Description);

        this.Logger.Verify(LogLevel.Warning, "No district points data found for team frc2046 at event 2025wabon");
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithEventKeyNoDistrictData_LogsNoDistrictDataFound()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetEventDistrictPointsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(default(EventDistrictPoints));
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventRankings);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District Bonney Lake Event", embedding.Content.Description);

        this.Logger.Verify(LogLevel.Warning, "No district data found for team frc2046");
    }
    [Fact]
    public async Task CreateAsync_ValidEventKey_ReturnsEventData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventRankings);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetEventDistrictPointsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventDistrictPoints);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[0];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        var content = embedding.Content;
        Assert.NotNull(content);
        Assert.Equal("2025 Ranking detail for 2046 Bear Metal", content.Title);
        Assert.Equal(_utTeam.TbaUrl, content.Url);

        embedding = result[1];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Equal("Getting district-wide data...", embedding.Content.Title);

        embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);

        embedding = result[3];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Contains("Getting event data for PNW District Bonney Lake Event...", embedding.Content.Title);

        embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District Bonney Lake Event", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_InvalidEventKey_ReturnsNoEventData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "invalid");

        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventRankings);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        DebugHelper.IgnoreDebugAsserts();
        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        var embedding = result[0];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        var content = embedding.Content;
        Assert.NotNull(content);
        Assert.Equal("2025 Ranking detail for 2046 Bear Metal", content.Title);
        Assert.Equal(_utTeam.TbaUrl, content.Url);

        embedding = result[1];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Equal("Getting district-wide data...", embedding.Content.Title);

        embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);

        embedding = result[3];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("No data found for", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_EventKeyNoRankings_ReturnsNoEventData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(default(SeasonRankingsEvent));
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        DebugHelper.IgnoreDebugAsserts();
        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[0];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        var content = embedding.Content;
        Assert.NotNull(content);
        Assert.Equal("2025 Ranking detail for 2046 Bear Metal", content.Title);
        Assert.Equal(_utTeam.TbaUrl, content.Url);

        embedding = result[1];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Equal("Getting district-wide data...", embedding.Content.Title);

        embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);

        embedding = result[3];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Contains("Getting event data for PNW District Bonney Lake Event...", embedding.Content.Title);

        embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("No data found for", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_TeamWithNoRankings_ReturnsNoEventData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: "2025wabon");

        var eventRankings = _utEventRankings with { Rankings = [] };
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsEventCodeGetAsync(_utEvent.EventCode.ToUpperInvariant(), input.Year.Value.ToString(), input.TeamKey.TeamKeyToTeamNumber()!.ToString()!, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(eventRankings);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        DebugHelper.IgnoreDebugAsserts();
        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        var embedding = result[0];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        var content = embedding.Content;
        Assert.NotNull(content);
        Assert.Equal("2025 Ranking detail for 2046 Bear Metal", content.Title);
        Assert.Equal(_utTeam.TbaUrl, content.Url);

        embedding = result[1];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Equal("Getting district-wide data...", embedding.Content.Title);

        embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("PNW District rank", embedding.Content.Description);

        embedding = result[3];
        Assert.NotNull(embedding);
        Assert.True(embedding.Transient);
        Assert.Contains("Getting event data for PNW District Bonney Lake Event...", embedding.Content.Title);

        embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("No data found for", embedding.Content.Description);
    }

    private static readonly Collection<DistrictRanking> _utTbaDistrictRankings = [.. JsonSerializer.Deserialize<List<DistrictRanking>>("""
        [
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wasam",
        				"qual_points": 22,
        				"total": 73
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025waahs",
        				"qual_points": 22,
        				"total": 73
        			}
        		],
        		"point_total": 146,
        		"rank": 1,
        		"rookie_bonus": 0,
        		"team_key": "frc2910"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wasno",
        				"qual_points": 22,
        				"total": 73
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wabon",
        				"qual_points": 20,
        				"total": 71
        			}
        		],
        		"point_total": 144,
        		"rank": 2,
        		"rookie_bonus": 0,
        		"team_key": "frc1778"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasno",
        				"qual_points": 21,
        				"total": 61
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wayak",
        				"qual_points": 22,
        				"total": 78
        			}
        		],
        		"point_total": 144,
        		"rank": 3,
        		"rookie_bonus": 5,
        		"team_key": "frc9450"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wasno",
        				"qual_points": 18,
        				"total": 69
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wabon",
        				"qual_points": 22,
        				"total": 73
        			}
        		],
        		"point_total": 142,
        		"rank": 4,
        		"rookie_bonus": 0,
        		"team_key": "frc2046"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orore",
        				"qual_points": 21,
        				"total": 67
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025waahs",
        				"qual_points": 21,
        				"total": 72
        			}
        		],
        		"point_total": 139,
        		"rank": 5,
        		"rookie_bonus": 0,
        		"team_key": "frc3663"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wayak",
        				"qual_points": 21,
        				"total": 61
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orwil",
        				"qual_points": 21,
        				"total": 72
        			}
        		],
        		"point_total": 133,
        		"rank": 6,
        		"rookie_bonus": 0,
        		"team_key": "frc5468"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasno",
        				"qual_points": 20,
        				"total": 60
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wayak",
        				"qual_points": 18,
        				"total": 69
        			}
        		],
        		"point_total": 129,
        		"rank": 7,
        		"rookie_bonus": 0,
        		"team_key": "frc2930"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orore",
        				"qual_points": 20,
        				"total": 60
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orwil",
        				"qual_points": 22,
        				"total": 68
        			}
        		],
        		"point_total": 128,
        		"rank": 8,
        		"rookie_bonus": 0,
        		"team_key": "frc957"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasno",
        				"qual_points": 15,
        				"total": 47
        			},
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wasam",
        				"qual_points": 21,
        				"total": 72
        			}
        		],
        		"point_total": 124,
        		"rank": 9,
        		"rookie_bonus": 5,
        		"team_key": "frc9442"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orsal",
        				"qual_points": 22,
        				"total": 73
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wayak",
        				"qual_points": 16,
        				"total": 47
        			}
        		],
        		"point_total": 120,
        		"rank": 10,
        		"rookie_bonus": 0,
        		"team_key": "frc955"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orsal",
        				"qual_points": 20,
        				"total": 66
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orwil",
        				"qual_points": 19,
        				"total": 46
        			}
        		],
        		"point_total": 112,
        		"rank": 11,
        		"rookie_bonus": 0,
        		"team_key": "frc3674"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasno",
        				"qual_points": 19,
        				"total": 46
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wayak",
        				"qual_points": 20,
        				"total": 60
        			}
        		],
        		"point_total": 106,
        		"rank": 12,
        		"rookie_bonus": 0,
        		"team_key": "frc1318"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 16,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orore",
        				"qual_points": 22,
        				"total": 68
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 17,
        				"total": 30
        			}
        		],
        		"point_total": 98,
        		"rank": 13,
        		"rookie_bonus": 0,
        		"team_key": "frc3636"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasno",
        				"qual_points": 17,
        				"total": 42
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025waahs",
        				"qual_points": 20,
        				"total": 55
        			}
        		],
        		"point_total": 97,
        		"rank": 14,
        		"rookie_bonus": 0,
        		"team_key": "frc360"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orore",
        				"qual_points": 17,
        				"total": 44
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orwil",
        				"qual_points": 17,
        				"total": 52
        			}
        		],
        		"point_total": 96,
        		"rank": 15,
        		"rookie_bonus": 0,
        		"team_key": "frc1540"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 17,
        				"total": 35
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orwil",
        				"qual_points": 20,
        				"total": 60
        			}
        		],
        		"point_total": 95,
        		"rank": 16,
        		"rookie_bonus": 0,
        		"team_key": "frc1425"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasam",
        				"qual_points": 20,
        				"total": 65
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 12,
        				"total": 28
        			}
        		],
        		"point_total": 93,
        		"rank": 17,
        		"rookie_bonus": 0,
        		"team_key": "frc2412"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasam",
        				"qual_points": 19,
        				"total": 54
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025waahs",
        				"qual_points": 13,
        				"total": 38
        			}
        		],
        		"point_total": 92,
        		"rank": 18,
        		"rookie_bonus": 0,
        		"team_key": "frc488"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orsal",
        				"qual_points": 19,
        				"total": 58
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 15,
        				"total": 34
        			}
        		],
        		"point_total": 92,
        		"rank": 19,
        		"rookie_bonus": 0,
        		"team_key": "frc4061"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orsal",
        				"qual_points": 17,
        				"total": 56
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wayak",
        				"qual_points": 12,
        				"total": 33
        			}
        		],
        		"point_total": 89,
        		"rank": 20,
        		"rookie_bonus": 0,
        		"team_key": "frc2147"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasam",
        				"qual_points": 15,
        				"total": 46
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wabon",
        				"qual_points": 16,
        				"total": 42
        			}
        		],
        		"point_total": 88,
        		"rank": 21,
        		"rookie_bonus": 0,
        		"team_key": "frc948"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orore",
        				"qual_points": 19,
        				"total": 46
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orwil",
        				"qual_points": 14,
        				"total": 37
        			}
        		],
        		"point_total": 83,
        		"rank": 22,
        		"rookie_bonus": 0,
        		"team_key": "frc6696"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 14,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 17,
        				"total": 36
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025waahs",
        				"qual_points": 19,
        				"total": 46
        			}
        		],
        		"point_total": 82,
        		"rank": 23,
        		"rookie_bonus": 0,
        		"team_key": "frc4915"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 4,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orsal",
        				"qual_points": 12,
        				"total": 29
        			},
        			{
        				"alliance_points": 1,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orwil",
        				"qual_points": 14,
        				"total": 45
        			}
        		],
        		"point_total": 79,
        		"rank": 24,
        		"rookie_bonus": 5,
        		"team_key": "frc9567"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 16,
        				"total": 36
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025waahs",
        				"qual_points": 18,
        				"total": 43
        			}
        		],
        		"point_total": 79,
        		"rank": 25,
        		"rookie_bonus": 0,
        		"team_key": "frc9023"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 1,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wasno",
        				"qual_points": 12,
        				"total": 48
        			},
        			{
        				"alliance_points": 4,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wayak",
        				"qual_points": 11,
        				"total": 28
        			}
        		],
        		"point_total": 76,
        		"rank": 26,
        		"rookie_bonus": 0,
        		"team_key": "frc3826"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orsal",
        				"qual_points": 10,
        				"total": 36
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orore",
        				"qual_points": 16,
        				"total": 40
        			}
        		],
        		"point_total": 76,
        		"rank": 27,
        		"rookie_bonus": 0,
        		"team_key": "frc7034"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 21,
        				"total": 36
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wayak",
        				"qual_points": 19,
        				"total": 40
        			}
        		],
        		"point_total": 76,
        		"rank": 28,
        		"rookie_bonus": 0,
        		"team_key": "frc4513"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 12,
        				"total": 26
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wabon",
        				"qual_points": 14,
        				"total": 48
        			}
        		],
        		"point_total": 74,
        		"rank": 29,
        		"rookie_bonus": 0,
        		"team_key": "frc9036"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasno",
        				"qual_points": 18,
        				"total": 38
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 21,
        				"total": 36
        			}
        		],
        		"point_total": 74,
        		"rank": 30,
        		"rookie_bonus": 0,
        		"team_key": "frc2522"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 1,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orore",
        				"qual_points": 9,
        				"total": 45
        			},
        			{
        				"alliance_points": 4,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025waahs",
        				"qual_points": 11,
        				"total": 27
        			}
        		],
        		"point_total": 72,
        		"rank": 31,
        		"rookie_bonus": 0,
        		"team_key": "frc3574"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orore",
        				"qual_points": 18,
        				"total": 53
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 7,
        				"total": 19
        			}
        		],
        		"point_total": 72,
        		"rank": 32,
        		"rookie_bonus": 0,
        		"team_key": "frc4043"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 8,
        				"total": 23
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wabon",
        				"qual_points": 15,
        				"total": 49
        			}
        		],
        		"point_total": 72,
        		"rank": 33,
        		"rookie_bonus": 0,
        		"team_key": "frc5937"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 16,
        				"total": 28
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wabon",
        				"qual_points": 17,
        				"total": 44
        			}
        		],
        		"point_total": 72,
        		"rank": 34,
        		"rookie_bonus": 0,
        		"team_key": "frc4469"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orsal",
        				"qual_points": 18,
        				"total": 49
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 14,
        				"total": 23
        			}
        		],
        		"point_total": 72,
        		"rank": 35,
        		"rookie_bonus": 0,
        		"team_key": "frc6343"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 1,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025orsal",
        				"qual_points": 12,
        				"total": 43
        			},
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orwil",
        				"qual_points": 12,
        				"total": 28
        			}
        		],
        		"point_total": 71,
        		"rank": 36,
        		"rookie_bonus": 0,
        		"team_key": "frc2550"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 16,
        				"total": 27
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 15,
        				"total": 33
        			}
        		],
        		"point_total": 70,
        		"rank": 37,
        		"rookie_bonus": 10,
        		"team_key": "frc10416"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 5,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orore",
        				"qual_points": 9,
        				"total": 26
        			},
        			{
        				"alliance_points": 1,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025wabon",
        				"qual_points": 7,
        				"total": 38
        			}
        		],
        		"point_total": 69,
        		"rank": 38,
        		"rookie_bonus": 5,
        		"team_key": "frc9446"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 15,
        				"total": 35
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 17,
        				"total": 34
        			}
        		],
        		"point_total": 69,
        		"rank": 39,
        		"rookie_bonus": 0,
        		"team_key": "frc4131"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 15,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 18,
        				"total": 33
        			},
        			{
        				"alliance_points": 2,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orwil",
        				"qual_points": 8,
        				"total": 35
        			}
        		],
        		"point_total": 68,
        		"rank": 40,
        		"rookie_bonus": 0,
        		"team_key": "frc997"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 16,
        				"total": 33
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 16,
        				"total": 34
        			}
        		],
        		"point_total": 67,
        		"rank": 41,
        		"rookie_bonus": 0,
        		"team_key": "frc3218"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 14,
        				"total": 30
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 18,
        				"total": 36
        			}
        		],
        		"point_total": 66,
        		"rank": 42,
        		"rookie_bonus": 0,
        		"team_key": "frc2471"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orore",
        				"qual_points": 16,
        				"total": 40
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 11,
        				"total": 25
        			}
        		],
        		"point_total": 65,
        		"rank": 43,
        		"rookie_bonus": 0,
        		"team_key": "frc6443"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 2,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasno",
        				"qual_points": 12,
        				"total": 39
        			},
        			{
        				"alliance_points": 4,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasam",
        				"qual_points": 8,
        				"total": 25
        			}
        		],
        		"point_total": 64,
        		"rank": 44,
        		"rookie_bonus": 0,
        		"team_key": "frc4918"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 8,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 13,
        				"total": 29
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 13,
        				"total": 25
        			}
        		],
        		"point_total": 64,
        		"rank": 45,
        		"rookie_bonus": 10,
        		"team_key": "frc10079"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 14,
        				"total": 28
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 17,
        				"total": 36
        			}
        		],
        		"point_total": 64,
        		"rank": 46,
        		"rookie_bonus": 0,
        		"team_key": "frc4450"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 4,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasno",
        				"qual_points": 6,
        				"total": 17
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wabon",
        				"qual_points": 19,
        				"total": 46
        			}
        		],
        		"point_total": 63,
        		"rank": 47,
        		"rookie_bonus": 0,
        		"team_key": "frc4512"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasam",
        				"qual_points": 17,
        				"total": 43
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 10,
        				"total": 20
        			}
        		],
        		"point_total": 63,
        		"rank": 48,
        		"rookie_bonus": 0,
        		"team_key": "frc492"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasam",
        				"qual_points": 15,
        				"total": 38
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 13,
        				"total": 25
        			}
        		],
        		"point_total": 63,
        		"rank": 49,
        		"rookie_bonus": 0,
        		"team_key": "frc1899"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 7,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 9,
        				"total": 16
        			},
        			{
        				"alliance_points": 8,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wabon",
        				"qual_points": 13,
        				"total": 46
        			}
        		],
        		"point_total": 62,
        		"rank": 50,
        		"rookie_bonus": 0,
        		"team_key": "frc4911"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 15,
        				"total": 29
        			},
        			{
        				"alliance_points": 3,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025waahs",
        				"qual_points": 11,
        				"total": 32
        			}
        		],
        		"point_total": 61,
        		"rank": 51,
        		"rookie_bonus": 0,
        		"team_key": "frc5683"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orsal",
        				"qual_points": 16,
        				"total": 34
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 16,
        				"total": 27
        			}
        		],
        		"point_total": 61,
        		"rank": 52,
        		"rookie_bonus": 0,
        		"team_key": "frc5920"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 9,
        				"total": 11
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wayak",
        				"qual_points": 18,
        				"total": 49
        			}
        		],
        		"point_total": 60,
        		"rank": 53,
        		"rookie_bonus": 0,
        		"team_key": "frc2811"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 16,
        				"total": 26
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orwil",
        				"qual_points": 16,
        				"total": 34
        			}
        		],
        		"point_total": 60,
        		"rank": 54,
        		"rookie_bonus": 0,
        		"team_key": "frc2521"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 8,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 13,
        				"total": 26
        			},
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wabon",
        				"qual_points": 11,
        				"total": 34
        			}
        		],
        		"point_total": 60,
        		"rank": 55,
        		"rookie_bonus": 0,
        		"team_key": "frc8032"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wabon",
        				"qual_points": 12,
        				"total": 28
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 16,
        				"total": 31
        			}
        		],
        		"point_total": 59,
        		"rank": 56,
        		"rookie_bonus": 0,
        		"team_key": "frc8051"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 15,
        				"total": 31
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 16,
        				"total": 28
        			}
        		],
        		"point_total": 59,
        		"rank": 57,
        		"rookie_bonus": 0,
        		"team_key": "frc4488"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 14,
        				"total": 23
        			},
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 18,
        				"total": 36
        			}
        		],
        		"point_total": 59,
        		"rank": 58,
        		"rookie_bonus": 0,
        		"team_key": "frc4089"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 10,
        				"total": 25
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 17,
        				"total": 34
        			}
        		],
        		"point_total": 59,
        		"rank": 59,
        		"rookie_bonus": 0,
        		"team_key": "frc7627"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 6,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasam",
        				"qual_points": 11,
        				"total": 29
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 15,
        				"total": 29
        			}
        		],
        		"point_total": 58,
        		"rank": 60,
        		"rookie_bonus": 0,
        		"team_key": "frc1294"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orsal",
        				"qual_points": 11,
        				"total": 34
        			},
        			{
        				"alliance_points": 6,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 12,
        				"total": 23
        			}
        		],
        		"point_total": 57,
        		"rank": 61,
        		"rookie_bonus": 0,
        		"team_key": "frc2926"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wayak",
        				"qual_points": 5,
        				"total": 27
        			},
        			{
        				"alliance_points": 6,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orwil",
        				"qual_points": 11,
        				"total": 29
        			}
        		],
        		"point_total": 56,
        		"rank": 62,
        		"rookie_bonus": 0,
        		"team_key": "frc1595"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 13,
        				"total": 24
        			},
        			{
        				"alliance_points": 14,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 18,
        				"total": 32
        			}
        		],
        		"point_total": 56,
        		"rank": 63,
        		"rookie_bonus": 0,
        		"team_key": "frc5827"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 14,
        				"total": 31
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 15,
        				"total": 25
        			}
        		],
        		"point_total": 56,
        		"rank": 64,
        		"rookie_bonus": 0,
        		"team_key": "frc2374"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 6,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025orsal",
        				"qual_points": 12,
        				"total": 25
        			},
        			{
        				"alliance_points": 4,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 12,
        				"total": 24
        			}
        		],
        		"point_total": 54,
        		"rank": 65,
        		"rookie_bonus": 5,
        		"team_key": "frc9600"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 16,
        				"total": 28
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 16,
        				"total": 26
        			}
        		],
        		"point_total": 54,
        		"rank": 66,
        		"rookie_bonus": 0,
        		"team_key": "frc3681"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 12,
        				"total": 26
        			},
        			{
        				"alliance_points": 8,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 14,
        				"total": 22
        			}
        		],
        		"point_total": 53,
        		"rank": 67,
        		"rookie_bonus": 5,
        		"team_key": "frc9430"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 3,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orsal",
        				"qual_points": 7,
        				"total": 35
        			},
        			{
        				"alliance_points": 5,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 7,
        				"total": 17
        			}
        		],
        		"point_total": 52,
        		"rank": 68,
        		"rookie_bonus": 0,
        		"team_key": "frc4692"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 11,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasam",
        				"qual_points": 13,
        				"total": 31
        			},
        			{
        				"alliance_points": 8,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 12,
        				"total": 20
        			}
        		],
        		"point_total": 51,
        		"rank": 69,
        		"rookie_bonus": 0,
        		"team_key": "frc3070"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 14,
        				"total": 24
        			},
        			{
        				"alliance_points": 7,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 15,
        				"total": 27
        			}
        		],
        		"point_total": 51,
        		"rank": 70,
        		"rookie_bonus": 0,
        		"team_key": "frc3219"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 8,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 10,
        				"total": 28
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 14,
        				"total": 23
        			}
        		],
        		"point_total": 51,
        		"rank": 71,
        		"rookie_bonus": 0,
        		"team_key": "frc2557"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 8,
        				"total": 8
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 15,
        				"total": 33
        			}
        		],
        		"point_total": 51,
        		"rank": 72,
        		"rookie_bonus": 10,
        		"team_key": "frc10455"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 11,
        				"total": 26
        			},
        			{
        				"alliance_points": 7,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 11,
        				"total": 23
        			}
        		],
        		"point_total": 49,
        		"rank": 73,
        		"rookie_bonus": 0,
        		"team_key": "frc4980"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 8,
        				"total": 8
        			},
        			{
        				"alliance_points": 1,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 30,
        				"event_key": "2025waahs",
        				"qual_points": 9,
        				"total": 40
        			}
        		],
        		"point_total": 48,
        		"rank": 74,
        		"rookie_bonus": 0,
        		"team_key": "frc8896"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 7,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 11,
        				"total": 23
        			},
        			{
        				"alliance_points": 4,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 11,
        				"total": 25
        			}
        		],
        		"point_total": 48,
        		"rank": 75,
        		"rookie_bonus": 0,
        		"team_key": "frc2976"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 1,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 24,
        				"event_key": "2025wasam",
        				"qual_points": 14,
        				"total": 39
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 8,
        				"total": 8
        			}
        		],
        		"point_total": 47,
        		"rank": 76,
        		"rookie_bonus": 0,
        		"team_key": "frc2906"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 9,
        				"total": 9
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 24,
        				"event_key": "2025wayak",
        				"qual_points": 14,
        				"total": 38
        			}
        		],
        		"point_total": 47,
        		"rank": 77,
        		"rookie_bonus": 0,
        		"team_key": "frc749"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 8,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 14,
        				"total": 22
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 15,
        				"total": 24
        			}
        		],
        		"point_total": 46,
        		"rank": 78,
        		"rookie_bonus": 0,
        		"team_key": "frc753"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 15,
        				"total": 24
        			},
        			{
        				"alliance_points": 8,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 13,
        				"total": 21
        			}
        		],
        		"point_total": 45,
        		"rank": 79,
        		"rookie_bonus": 0,
        		"team_key": "frc2990"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 7,
        				"total": 7
        			},
        			{
        				"alliance_points": 15,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 18,
        				"total": 38
        			}
        		],
        		"point_total": 45,
        		"rank": 80,
        		"rookie_bonus": 0,
        		"team_key": "frc4682"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 11,
        				"total": 16
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 17,
        				"total": 29
        			}
        		],
        		"point_total": 45,
        		"rank": 81,
        		"rookie_bonus": 0,
        		"team_key": "frc7461"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 10,
        				"total": 18
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 13,
        				"total": 27
        			}
        		],
        		"point_total": 45,
        		"rank": 82,
        		"rookie_bonus": 0,
        		"team_key": "frc2635"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025wasno",
        				"qual_points": 4,
        				"total": 20
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 14,
        				"total": 24
        			}
        		],
        		"point_total": 44,
        		"rank": 83,
        		"rookie_bonus": 0,
        		"team_key": "frc2929"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 13,
        				"total": 23
        			},
        			{
        				"alliance_points": 1,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wayak",
        				"qual_points": 8,
        				"total": 21
        			}
        		],
        		"point_total": 44,
        		"rank": 84,
        		"rookie_bonus": 0,
        		"team_key": "frc4125"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 7,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 14,
        				"total": 21
        			},
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 14,
        				"total": 23
        			}
        		],
        		"point_total": 44,
        		"rank": 85,
        		"rookie_bonus": 0,
        		"team_key": "frc6465"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 4,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 12,
        				"total": 24
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 10,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 9,
        				"total": 19
        			}
        		],
        		"point_total": 43,
        		"rank": 86,
        		"rookie_bonus": 0,
        		"team_key": "frc6831"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wasam",
        				"qual_points": 12,
        				"total": 19
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 4,
        				"event_key": "2025wabon",
        				"qual_points": 14,
        				"total": 23
        			}
        		],
        		"point_total": 42,
        		"rank": 87,
        		"rookie_bonus": 0,
        		"team_key": "frc6350"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 9,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 14,
        				"total": 23
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 14,
        				"total": 19
        			}
        		],
        		"point_total": 42,
        		"rank": 88,
        		"rookie_bonus": 0,
        		"team_key": "frc8248"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 8,
        				"total": 13
        			},
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025orore",
        				"qual_points": 6,
        				"total": 28
        			}
        		],
        		"point_total": 41,
        		"rank": 89,
        		"rookie_bonus": 0,
        		"team_key": "frc2898"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025wasam",
        				"qual_points": 4,
        				"total": 26
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 10,
        				"total": 15
        			}
        		],
        		"point_total": 41,
        		"rank": 90,
        		"rookie_bonus": 0,
        		"team_key": "frc949"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 5,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 9,
        				"total": 19
        			},
        			{
        				"alliance_points": 6,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 4,
        				"event_key": "2025wabon",
        				"qual_points": 7,
        				"total": 22
        			}
        		],
        		"point_total": 41,
        		"rank": 91,
        		"rookie_bonus": 0,
        		"team_key": "frc1983"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 13,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 13,
        				"total": 31
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 10,
        				"total": 10
        			}
        		],
        		"point_total": 41,
        		"rank": 92,
        		"rookie_bonus": 0,
        		"team_key": "frc3673"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 12,
        				"total": 12
        			},
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 17,
        				"total": 29
        			}
        		],
        		"point_total": 41,
        		"rank": 93,
        		"rookie_bonus": 0,
        		"team_key": "frc8302"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 14,
        				"total": 24
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 12,
        				"total": 17
        			}
        		],
        		"point_total": 41,
        		"rank": 94,
        		"rookie_bonus": 0,
        		"team_key": "frc4662"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 12,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 17,
        				"total": 29
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 10,
        				"total": 10
        			}
        		],
        		"point_total": 39,
        		"rank": 95,
        		"rookie_bonus": 0,
        		"team_key": "frc4060"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 15,
        				"total": 15
        			},
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 7,
        				"event_key": "2025wayak",
        				"qual_points": 13,
        				"total": 23
        			}
        		],
        		"point_total": 38,
        		"rank": 96,
        		"rookie_bonus": 0,
        		"team_key": "frc8532"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 11,
        				"total": 19
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 8,
        				"total": 8
        			}
        		],
        		"point_total": 37,
        		"rank": 97,
        		"rookie_bonus": 10,
        		"team_key": "frc10444"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 7,
        				"total": 7
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 11,
        				"total": 19
        			}
        		],
        		"point_total": 36,
        		"rank": 98,
        		"rookie_bonus": 10,
        		"team_key": "frc10423"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 5,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 13,
        				"total": 18
        			},
        			{
        				"alliance_points": 7,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 9,
        				"total": 16
        			}
        		],
        		"point_total": 34,
        		"rank": 99,
        		"rookie_bonus": 0,
        		"team_key": "frc2903"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 5,
        				"total": 5
        			},
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 20,
        				"event_key": "2025waahs",
        				"qual_points": 5,
        				"total": 27
        			}
        		],
        		"point_total": 32,
        		"rank": 100,
        		"rookie_bonus": 0,
        		"team_key": "frc3049"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 13,
        				"event_key": "2025orore",
        				"qual_points": 10,
        				"total": 26
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 6,
        				"total": 6
        			}
        		],
        		"point_total": 32,
        		"rank": 101,
        		"rookie_bonus": 0,
        		"team_key": "frc2915"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 5,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 11,
        				"total": 16
        			},
        			{
        				"alliance_points": 6,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 10,
        				"total": 16
        			}
        		],
        		"point_total": 32,
        		"rank": 102,
        		"rookie_bonus": 0,
        		"team_key": "frc3786"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 7,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 11,
        				"total": 23
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 9,
        				"total": 9
        			}
        		],
        		"point_total": 32,
        		"rank": 103,
        		"rookie_bonus": 0,
        		"team_key": "frc5970"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 6,
        				"total": 6
        			},
        			{
        				"alliance_points": 10,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 15,
        				"total": 25
        			}
        		],
        		"point_total": 31,
        		"rank": 104,
        		"rookie_bonus": 0,
        		"team_key": "frc847"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 11,
        				"total": 16
        			},
        			{
        				"alliance_points": 5,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 4,
        				"total": 14
        			}
        		],
        		"point_total": 30,
        		"rank": 105,
        		"rookie_bonus": 0,
        		"team_key": "frc3024"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 6,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 5,
        				"total": 11
        			},
        			{
        				"alliance_points": 5,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 12,
        				"total": 17
        			}
        		],
        		"point_total": 28,
        		"rank": 106,
        		"rookie_bonus": 0,
        		"team_key": "frc5941"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 3,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 10,
        				"total": 13
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 15,
        				"total": 15
        			}
        		],
        		"point_total": 28,
        		"rank": 107,
        		"rookie_bonus": 0,
        		"team_key": "frc4180"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 10,
        				"total": 10
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 8,
        				"total": 8
        			}
        		],
        		"point_total": 28,
        		"rank": 108,
        		"rookie_bonus": 10,
        		"team_key": "frc10498"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 5,
        				"total": 10
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 8,
        				"total": 13
        			}
        		],
        		"point_total": 28,
        		"rank": 109,
        		"rookie_bonus": 5,
        		"team_key": "frc9613"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 11,
        				"total": 11
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 11,
        				"total": 11
        			}
        		],
        		"point_total": 27,
        		"rank": 110,
        		"rookie_bonus": 5,
        		"team_key": "frc9680"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 10,
        				"total": 10
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 12,
        				"total": 12
        			}
        		],
        		"point_total": 27,
        		"rank": 111,
        		"rookie_bonus": 5,
        		"team_key": "frc9438"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 6,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 8,
        				"total": 14
        			},
        			{
        				"alliance_points": 7,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 5,
        				"total": 12
        			}
        		],
        		"point_total": 26,
        		"rank": 112,
        		"rookie_bonus": 0,
        		"team_key": "frc2733"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 9,
        				"total": 14
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 9,
        				"total": 9
        			}
        		],
        		"point_total": 23,
        		"rank": 113,
        		"rookie_bonus": 0,
        		"team_key": "frc2927"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 8,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 7,
        				"total": 15
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 7,
        				"total": 7
        			}
        		],
        		"point_total": 22,
        		"rank": 114,
        		"rookie_bonus": 0,
        		"team_key": "frc2980"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 12,
        				"total": 12
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 9,
        				"total": 9
        			}
        		],
        		"point_total": 21,
        		"rank": 115,
        		"rookie_bonus": 0,
        		"team_key": "frc6076"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 12,
        				"total": 12
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 9,
        				"total": 9
        			}
        		],
        		"point_total": 21,
        		"rank": 116,
        		"rookie_bonus": 0,
        		"team_key": "frc5295"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 7,
        				"total": 7
        			},
        			{
        				"alliance_points": 5,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 8,
        				"total": 13
        			}
        		],
        		"point_total": 20,
        		"rank": 117,
        		"rookie_bonus": 0,
        		"team_key": "frc2928"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 8,
        				"total": 8
        			},
        			{
        				"alliance_points": 2,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 10,
        				"total": 12
        			}
        		],
        		"point_total": 20,
        		"rank": 118,
        		"rookie_bonus": 0,
        		"team_key": "frc5588"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 9,
        				"total": 9
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 11,
        				"total": 11
        			}
        		],
        		"point_total": 20,
        		"rank": 119,
        		"rookie_bonus": 0,
        		"team_key": "frc3393"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 13,
        				"total": 13
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 6,
        				"total": 6
        			}
        		],
        		"point_total": 19,
        		"rank": 120,
        		"rookie_bonus": 0,
        		"team_key": "frc3712"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasno",
        				"qual_points": 9,
        				"total": 9
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 10,
        				"total": 10
        			}
        		],
        		"point_total": 19,
        		"rank": 121,
        		"rookie_bonus": 0,
        		"team_key": "frc4173"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 4,
        				"total": 4
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 13,
        				"total": 13
        			}
        		],
        		"point_total": 17,
        		"rank": 122,
        		"rookie_bonus": 0,
        		"team_key": "frc5975"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 8,
        				"total": 8
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 9,
        				"total": 9
        			}
        		],
        		"point_total": 17,
        		"rank": 123,
        		"rookie_bonus": 0,
        		"team_key": "frc3812"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 5,
        				"total": 5
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 11,
        				"total": 11
        			}
        		],
        		"point_total": 16,
        		"rank": 124,
        		"rookie_bonus": 0,
        		"team_key": "frc1844"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 4,
        				"total": 4
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 12,
        				"total": 12
        			}
        		],
        		"point_total": 16,
        		"rank": 125,
        		"rookie_bonus": 0,
        		"team_key": "frc4579"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 4,
        				"total": 4
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 5,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orwil",
        				"qual_points": 7,
        				"total": 12
        			}
        		],
        		"point_total": 16,
        		"rank": 126,
        		"rookie_bonus": 0,
        		"team_key": "frc1359"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 10,
        				"total": 10
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 4,
        				"total": 4
        			}
        		],
        		"point_total": 14,
        		"rank": 127,
        		"rookie_bonus": 0,
        		"team_key": "frc3588"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orsal",
        				"qual_points": 7,
        				"total": 7
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025orore",
        				"qual_points": 7,
        				"total": 7
        			}
        		],
        		"point_total": 14,
        		"rank": 128,
        		"rookie_bonus": 0,
        		"team_key": "frc6845"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 6,
        				"total": 6
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 7,
        				"total": 7
        			}
        		],
        		"point_total": 13,
        		"rank": 129,
        		"rookie_bonus": 0,
        		"team_key": "frc8303"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wasam",
        				"qual_points": 6,
        				"total": 6
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wabon",
        				"qual_points": 5,
        				"total": 5
        			}
        		],
        		"point_total": 11,
        		"rank": 130,
        		"rookie_bonus": 0,
        		"team_key": "frc3268"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025wayak",
        				"qual_points": 4,
        				"total": 4
        			},
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 6,
        				"total": 6
        			}
        		],
        		"point_total": 10,
        		"rank": 131,
        		"rookie_bonus": 0,
        		"team_key": "frc3876"
        	},
        	{
        		"event_points": [
        			{
        				"alliance_points": 0,
        				"award_points": 0,
        				"district_cmp": false,
        				"elim_points": 0,
        				"event_key": "2025waahs",
        				"qual_points": 10,
        				"total": 10
        			}
        		],
        		"point_total": 10,
        		"rank": 132,
        		"rookie_bonus": 0,
        		"team_key": "frc2907"
        	}
        ]
        """)!];
    private static readonly EventDistrictPoints _utEventDistrictPoints = JsonSerializer.Deserialize<EventDistrictPoints>("""
        {
        	"points": {
        		"frc10079": {
        			"alliance_points": 12,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 13,
        			"total": 25
        		},
        		"frc10455": {
        			"alliance_points": 10,
        			"award_points": 8,
        			"elim_points": 0,
        			"qual_points": 15,
        			"total": 33
        		},
        		"frc1778": {
        			"alliance_points": 16,
        			"award_points": 5,
        			"elim_points": 30,
        			"qual_points": 20,
        			"total": 71
        		},
        		"frc1983": {
        			"alliance_points": 6,
        			"award_points": 5,
        			"elim_points": 4,
        			"qual_points": 7,
        			"total": 22
        		},
        		"frc2046": {
        			"alliance_points": 16,
        			"award_points": 5,
        			"elim_points": 30,
        			"qual_points": 22,
        			"total": 73
        		},
        		"frc2522": {
        			"alliance_points": 15,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 21,
        			"total": 36
        		},
        		"frc2903": {
        			"alliance_points": 7,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 9,
        			"total": 16
        		},
        		"frc2906": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 8,
        			"total": 8
        		},
        		"frc2927": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 9,
        			"total": 9
        		},
        		"frc2929": {
        			"alliance_points": 10,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 14,
        			"total": 24
        		},
        		"frc2976": {
        			"alliance_points": 4,
        			"award_points": 10,
        			"elim_points": 0,
        			"qual_points": 11,
        			"total": 25
        		},
        		"frc3218": {
        			"alliance_points": 13,
        			"award_points": 5,
        			"elim_points": 0,
        			"qual_points": 16,
        			"total": 34
        		},
        		"frc3268": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 5,
        			"total": 5
        		},
        		"frc3393": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 11,
        			"total": 11
        		},
        		"frc4089": {
        			"alliance_points": 13,
        			"award_points": 5,
        			"elim_points": 0,
        			"qual_points": 18,
        			"total": 36
        		},
        		"frc4469": {
        			"alliance_points": 14,
        			"award_points": 0,
        			"elim_points": 13,
        			"qual_points": 17,
        			"total": 44
        		},
        		"frc4512": {
        			"alliance_points": 14,
        			"award_points": 0,
        			"elim_points": 13,
        			"qual_points": 19,
        			"total": 46
        		},
        		"frc4579": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 4,
        			"total": 4
        		},
        		"frc4682": {
        			"alliance_points": 15,
        			"award_points": 5,
        			"elim_points": 0,
        			"qual_points": 18,
        			"total": 38
        		},
        		"frc4911": {
        			"alliance_points": 8,
        			"award_points": 5,
        			"elim_points": 20,
        			"qual_points": 13,
        			"total": 46
        		},
        		"frc5588": {
        			"alliance_points": 2,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 10,
        			"total": 12
        		},
        		"frc5937": {
        			"alliance_points": 9,
        			"award_points": 5,
        			"elim_points": 20,
        			"qual_points": 15,
        			"total": 49
        		},
        		"frc5941": {
        			"alliance_points": 5,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 12,
        			"total": 17
        		},
        		"frc6350": {
        			"alliance_points": 0,
        			"award_points": 5,
        			"elim_points": 4,
        			"qual_points": 14,
        			"total": 23
        		},
        		"frc7461": {
        			"alliance_points": 12,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 17,
        			"total": 29
        		},
        		"frc8032": {
        			"alliance_points": 11,
        			"award_points": 5,
        			"elim_points": 7,
        			"qual_points": 11,
        			"total": 34
        		},
        		"frc8051": {
        			"alliance_points": 3,
        			"award_points": 0,
        			"elim_points": 13,
        			"qual_points": 12,
        			"total": 28
        		},
        		"frc8302": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 12,
        			"total": 12
        		},
        		"frc8303": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 6,
        			"total": 6
        		},
        		"frc8896": {
        			"alliance_points": 0,
        			"award_points": 0,
        			"elim_points": 0,
        			"qual_points": 8,
        			"total": 8
        		},
        		"frc9036": {
        			"alliance_points": 9,
        			"award_points": 5,
        			"elim_points": 20,
        			"qual_points": 14,
        			"total": 48
        		},
        		"frc9446": {
        			"alliance_points": 1,
        			"award_points": 0,
        			"elim_points": 30,
        			"qual_points": 7,
        			"total": 38
        		},
        		"frc948": {
        			"alliance_points": 11,
        			"award_points": 8,
        			"elim_points": 7,
        			"qual_points": 16,
        			"total": 42
        		},
        		"frc949": {
        			"alliance_points": 0,
        			"award_points": 5,
        			"elim_points": 0,
        			"qual_points": 10,
        			"total": 15
        		}
        	},
        	"tiebreakers": {
        		"frc10079": {
        			"highest_qual_scores": [
        				190,
        				147,
        				146
        			],
        			"qual_wins": 0
        		},
        		"frc10455": {
        			"highest_qual_scores": [
        				172,
        				119,
        				105
        			],
        			"qual_wins": 0
        		},
        		"frc1778": {
        			"highest_qual_scores": [
        				190,
        				180,
        				172
        			],
        			"qual_wins": 0
        		},
        		"frc1983": {
        			"highest_qual_scores": [
        				172,
        				147,
        				108
        			],
        			"qual_wins": 0
        		},
        		"frc2046": {
        			"highest_qual_scores": [
        				190,
        				167,
        				161
        			],
        			"qual_wins": 0
        		},
        		"frc2522": {
        			"highest_qual_scores": [
        				169,
        				154,
        				147
        			],
        			"qual_wins": 0
        		},
        		"frc2903": {
        			"highest_qual_scores": [
        				125,
        				124,
        				120
        			],
        			"qual_wins": 0
        		},
        		"frc2906": {
        			"highest_qual_scores": [
        				143,
        				113,
        				112
        			],
        			"qual_wins": 0
        		},
        		"frc2927": {
        			"highest_qual_scores": [
        				161,
        				113,
        				111
        			],
        			"qual_wins": 0
        		},
        		"frc2929": {
        			"highest_qual_scores": [
        				159,
        				132,
        				98
        			],
        			"qual_wins": 0
        		},
        		"frc2976": {
        			"highest_qual_scores": [
        				153,
        				123,
        				105
        			],
        			"qual_wins": 0
        		},
        		"frc3218": {
        			"highest_qual_scores": [
        				167,
        				140,
        				128
        			],
        			"qual_wins": 0
        		},
        		"frc3268": {
        			"highest_qual_scores": [
        				155,
        				147,
        				125
        			],
        			"qual_wins": 0
        		},
        		"frc3393": {
        			"highest_qual_scores": [
        				107,
        				106,
        				100
        			],
        			"qual_wins": 0
        		},
        		"frc4089": {
        			"highest_qual_scores": [
        				169,
        				167,
        				131
        			],
        			"qual_wins": 0
        		},
        		"frc4469": {
        			"highest_qual_scores": [
        				140,
        				128,
        				119
        			],
        			"qual_wins": 0
        		},
        		"frc4512": {
        			"highest_qual_scores": [
        				155,
        				147,
        				146
        			],
        			"qual_wins": 0
        		},
        		"frc4579": {
        			"highest_qual_scores": [
        				113,
        				108,
        				96
        			],
        			"qual_wins": 0
        		},
        		"frc4682": {
        			"highest_qual_scores": [
        				147,
        				143,
        				140
        			],
        			"qual_wins": 0
        		},
        		"frc4911": {
        			"highest_qual_scores": [
        				124,
        				118,
        				114
        			],
        			"qual_wins": 0
        		},
        		"frc5588": {
        			"highest_qual_scores": [
        				153,
        				147,
        				140
        			],
        			"qual_wins": 0
        		},
        		"frc5937": {
        			"highest_qual_scores": [
        				161,
        				159,
        				147
        			],
        			"qual_wins": 0
        		},
        		"frc5941": {
        			"highest_qual_scores": [
        				138,
        				124,
        				113
        			],
        			"qual_wins": 0
        		},
        		"frc6350": {
        			"highest_qual_scores": [
        				147,
        				125,
        				116
        			],
        			"qual_wins": 0
        		},
        		"frc7461": {
        			"highest_qual_scores": [
        				147,
        				140,
        				123
        			],
        			"qual_wins": 0
        		},
        		"frc8032": {
        			"highest_qual_scores": [
        				180,
        				169,
        				120
        			],
        			"qual_wins": 0
        		},
        		"frc8051": {
        			"highest_qual_scores": [
        				140,
        				112,
        				103
        			],
        			"qual_wins": 0
        		},
        		"frc8302": {
        			"highest_qual_scores": [
        				180,
        				154,
        				109
        			],
        			"qual_wins": 0
        		},
        		"frc8303": {
        			"highest_qual_scores": [
        				127,
        				100,
        				97
        			],
        			"qual_wins": 0
        		},
        		"frc8896": {
        			"highest_qual_scores": [
        				118,
        				105,
        				102
        			],
        			"qual_wins": 0
        		},
        		"frc9036": {
        			"highest_qual_scores": [
        				146,
        				132,
        				125
        			],
        			"qual_wins": 0
        		},
        		"frc9446": {
        			"highest_qual_scores": [
        				118,
        				109,
        				97
        			],
        			"qual_wins": 0
        		},
        		"frc948": {
        			"highest_qual_scores": [
        				147,
        				140,
        				131
        			],
        			"qual_wins": 0
        		},
        		"frc949": {
        			"highest_qual_scores": [
        				125,
        				114,
        				102
        			],
        			"qual_wins": 0
        		}
        	}
        }
        """)!;

    [Fact]
    public async Task CreateAsync_ValidInputWithEventPoints_ReturnsEventPoints()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYearAsync(input.Year.Value, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetEventDistrictPointsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utEventDistrictPoints);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictRankingsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTbaDistrictRankings);

        DebugHelper.IgnoreDebugAsserts();
        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(6, result.Count);
        var embedding = result[3];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("District Point breakdown by Event", embedding.Content.Description);
        Assert.Contains("**PNW District Bonney Lake Event**", embedding.Content.Description);
        Assert.Contains("- Alliance: 16", embedding.Content.Description);
        Assert.Contains("- Quals: 18", embedding.Content.Description);
        Assert.Contains("- Elims: 30", embedding.Content.Description);
        Assert.Contains("- Awards: 5", embedding.Content.Description);
        embedding = result[4];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("**PNW District Bonney Lake Event**", embedding.Content.Description);
        Assert.Contains("- Alliance: 16", embedding.Content.Description);
        Assert.Contains("- Quals: 22", embedding.Content.Description);
        Assert.Contains("- Elims: 30", embedding.Content.Description);
        Assert.Contains("- Awards: 5", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithEpaData_ReturnsEpaData()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey.TeamKeyToTeamNumber().ToString()!, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYearAsync(input.Year.Value, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        var embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("EPA (64.11) rank", embedding.Content.Description);
        Assert.Contains("State (WA): 4/92 (95.65%ile)", embedding.Content.Description);
        Assert.Contains("District: 5 / 132 (96.21%ile)", embedding.Content.Description);
        Assert.Contains("Country (USA): 77 / 2930 (97.37%ile)", embedding.Content.Description);
        Assert.Contains("World: 100 / 3695 (97.29%ile)", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithEpaRank_ReturnsEpaRank()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        var teamYear = _utTeamYear with { Epa = _utTeamYear.Epa! with { TotalPoints = null } };
        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey.TeamKeyToTeamNumber().ToString()!, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(teamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYear(input.Year.Value, It.IsAny<string>())).Returns(_utTeamDistricts);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        var embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("EPA rank", embedding.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ValidInputWithDistrictRank_ReturnsDistrictRank()
    {
        var input = (Year: (int?)2025, TeamKey: "frc2046", EventKey: (string?)null);

        _utTeamDistricts[0] = _utTeamDistricts[0] with { DisplayName = string.Empty };
        this.Mocker.GetMock<ITeamCache>().Setup(t => t[input.TeamKey]).Returns(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamYearApi>().Setup(t => t.ReadTeamYearV3TeamYearTeamYearGetAsync(input.TeamKey.TeamKeyToTeamNumber().ToString()!, input.Year.Value, It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamYear);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetDistrictsByYearAsync(input.Year.Value, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);
        this.Mocker.GetMock<IRankingsApi>().Setup(r => r.SeasonRankingsDistrictGetAsync(input.Year.Value.ToString(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), input.TeamKey.TeamKeyToTeamNumber()!.Value.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utDistrictRankings);

        var result = await _teamRank.CreateAsync(input).ToListAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        var embedding = result[2];
        Assert.NotNull(embedding);
        Assert.False(embedding.Transient);
        Assert.Contains("District: 5 / 132 (96.21%ile)", embedding.Content.Description);
    }
}