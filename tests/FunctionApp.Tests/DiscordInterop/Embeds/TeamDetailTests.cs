namespace FunctionApp.Tests.DiscordInterop.Embeds;

using FunctionApp.Apis;
using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

public class TeamDetailTests : EmbeddingTest
{
    private readonly TeamDetail _teamDetail;

    public TeamDetailTests(ITestOutputHelper outputHelper) : base(typeof(TeamDetail), outputHelper)
    {
        this.Mocker.CreateSelfMock<IRESTCountries>();
        this.Mocker.CreateSelfMock<ITeamApi>();
        this.Mocker.CreateSelfMock<Statbotics.Api.ITeamApi>();
        this.Mocker.CreateSelfMock<IDistrictApi>();

        this.Mocker.With<TeamCache>();
        _teamDetail = this.Mocker.CreateInstance<TeamDetail>();
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
    private static readonly Statbotics.Model.Team _utTeamStats = JsonSerializer.Deserialize<Statbotics.Model.Team>("""
                {
        	"team": 2046,
        	"name": "Bear Metal",
        	"country": "USA",
        	"state": "WA",
        	"district": "pnw",
        	"rookie_year": 2007,
        	"active": true,
        	"record": {
        		"wins": 677,
        		"losses": 272,
        		"ties": 1,
        		"count": 950,
        		"winrate": 0.7132
        	},
        	"norm_epa": {
        		"current": 1732,
        		"recent": 1791,
        		"mean": 1697,
        		"max": 1865
        	}
        }
        """)!;
    private static readonly Collection<Media> _utTeamMedia = [.. JsonSerializer.Deserialize<List<Media>>("""
        [
        	{
        		"details": {
        			"base64Image": "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"
        		},
        		"direct_url": "",
        		"foreign_key": "avatar_2025_frc2046",
        		"preferred": false,
        		"team_keys": [
        			"frc2046"
        		],
        		"type": "avatar",
        		"view_url": ""
        	},
        	{
        		"details": {},
        		"direct_url": "https://i.imgur.com/l3EJ2V2.jpeg",
        		"foreign_key": "l3EJ2V2",
        		"preferred": false,
        		"team_keys": [
        			"frc2046"
        		],
        		"type": "imgur",
        		"view_url": "https://imgur.com/l3EJ2V2"
        	}
        ]
        """)!];
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

    [Fact]
    public async Task CreateAsync_ValidTeamKey_ReturnsEmbedding()
    {
        var teamStats = _utTeamStats with { Record = null };
        this.Mocker.GetMock<ITeamApi>()
            .Setup(t => t.GetTeamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(t => t.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamStats);
        this.Mocker.GetMock<IRESTCountries>().Setup(c => c.GetCountryCodeForFlagLookupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("US");
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamMediaByYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamMedia);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetTeamDistrictsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);

        var result = await _teamDetail.CreateAsync("frc2046").ToListAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        var embedding = result.First();
        Assert.NotNull(embedding);
        Assert.Contains(_utTeam.Nickname, embedding.Content.Title);
        Assert.Equal(_utTeam.Name, embedding.Content.Description);
        Assert.Equal(_utTeam.Website, embedding.Content.Url);
    }

    [Fact]
    public async Task CreateAsync_TeamWithFullRecord_ReturnsCorrectAllTimeRecord()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(t => t.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamStats);
        this.Mocker.GetMock<IRESTCountries>().Setup(c => c.GetCountryCodeForFlagLookupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("US");
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamMediaByYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamMedia);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetTeamDistrictsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(d => d.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_utTeamStats);

        // Act
        var result = await _teamDetail.CreateAsync("frc2046").ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var embedding = result.First();
        Assert.NotNull(embedding);
        Assert.Contains(_utTeam.Nickname, embedding.Content.Title);
        Assert.Equal(_utTeam.Name, embedding.Content.Description);
        Assert.Equal(_utTeam.Website, embedding.Content.Url);
        Assert.True(embedding.Content.Fields.Any(f => f.Name == "All-time Record"));
    }
    [Fact]
    public async Task CreateAsync_TeamResultIsNull_LogsWarning()
    {
        // Arrange
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(t => t.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Statbotics.Model.Team?)null);
        this.Mocker.GetMock<IRESTCountries>().Setup(c => c.GetCountryCodeForFlagLookupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("US");
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamMediaByYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamMedia);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetTeamDistrictsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);

        // Act
        var result = await _teamDetail.CreateAsync("frc2046").ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var embedding = result.First();
        Assert.NotNull(embedding);
        this.Logger.Verify(LogLevel.Warning, "Unable to get stats for frc2046 from Statbotics");
    }

    [Fact]
    public async Task CreateAsync_TeamWithColors_SetsLightestColor()
    {
        // Arrange
        var teamStats = _utTeamStats with { Colors = new Statbotics.Model.Colors { Primary = "#FF0000", Secondary = "#00FF00" } };
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeam);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(t => t.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamStats);
        this.Mocker.GetMock<IRESTCountries>().Setup(c => c.GetCountryCodeForFlagLookupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("US");
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamMediaByYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamMedia);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetTeamDistrictsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);

        // Act
        var result = await _teamDetail.CreateAsync("frc2046").ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var embedding = result.First();
        Assert.NotNull(embedding);
        Assert.Equal(Discord.Color.Parse("#00FF00"), embedding.Content.Color);
    }

    [Fact]
    public async Task CreateAsync_TeamWithLocationName_AddsLocationNameToLocationString()
    {
        // Arrange
        var teamWithLocationName = _utTeam with { LocationName = "Test Location" };
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamWithLocationName);
        this.Mocker.GetMock<Statbotics.Api.ITeamApi>().Setup(t => t.ReadTeamV3TeamTeamGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamStats);
        this.Mocker.GetMock<IRESTCountries>().Setup(c => c.GetCountryCodeForFlagLookupAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("US");
        this.Mocker.GetMock<ITeamApi>().Setup(t => t.GetTeamMediaByYearAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamMedia);
        this.Mocker.GetMock<IDistrictApi>().Setup(d => d.GetTeamDistrictsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(_utTeamDistricts);

        // Act
        var result = await _teamDetail.CreateAsync("frc2046").ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var embedding = result.First();
        Assert.NotNull(embedding);
        Assert.Contains("Test Location", embedding.Content.Fields.First(f => f.Name == "Location").Value);
    }
}
