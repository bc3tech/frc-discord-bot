namespace FunctionApp.Tests.DiscordInterop.Embeds;

using FunctionApp.DiscordInterop.Embeds;

using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Interfaces.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

using Match = TheBlueAlliance.Model.Match;

public class ScheduleTests : EmbeddingTest
{
    private readonly Schedule _schedule;

    public ScheduleTests(ITestOutputHelper outputHelper) : base(typeof(Schedule), outputHelper)
    {
        this.Mocker.CreateSelfMock<IEventCache>();
        this.Mocker.CreateSelfMock<ITeamCache>();
        this.Mocker.CreateSelfMock<IMatchApi>();
        _schedule = this.Mocker.CreateInstance<Schedule>();
    }

    private static Match DeserializeMatch(string json) => JsonSerializer.Deserialize<Match>(json)!;

    [Fact]
    public async Task CreateAsync_NoMatchesRequested_LogsAndReturns()
    {
        var input = (eventKey: (string?)null, numMatches: (ushort)0);
        var result = _schedule.CreateAsync(input);

        await foreach (var embedding in result)
        {
            Assert.Null(embedding);
        }

        //_loggerMock.Verify(logger => logger.InvalidNumberOfMatchesRequestedNumMatches(input.numMatches), Times.Once);
    }

    private static readonly Team _utTeam = new Team(
        address: "1234 Robotics Ave",
        city: "Roboville",
        country: "USA",
        gmapsPlaceId: null,
        gmapsUrl: null,
        key: "frc6421",
        lat: 40.7128,
        lng: -74.0060,
        locationName: null,
        name: "Robo Warriors",
        nickname: "Warriors",
        postalCode: "12345",
        rookieYear: 2005,
        schoolName: "Robo High School",
        stateProv: "NY",
        teamNumber: 6421,
        website: "http://www.robowarriors.com"
    );

    [Fact]
    public async Task CreateAsync_HighlightTeamNoEventKey_ReturnsScheduleForTeam()
    {
        var input = (eventKey: (string?)null, numMatches: (ushort)5);
        var detailedMatchJson = """
        {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}
        """;
        var matches = new List<Match>
        {
            DeserializeMatch(detailedMatchJson)
        };

        this.Mocker.GetMock<ITeamCache>().SetupGet(t => t[_utTeam.Key]).Returns(_utTeam);
        this.Mocker.GetMock<ITeamCache>().SetupGet(t => t[(ushort)_utTeam.TeamNumber]).Returns(_utTeam);
        this.Mocker.GetMock<IMatchApi>().Setup(m => m.GetTeamMatchesByYearAsync(_utTeam.Key, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. matches]);

        var result = _schedule.CreateAsync(input, (ushort)_utTeam.TeamNumber);

        await foreach (var embedding in result)
        {
            Assert.NotNull(embedding);
            Assert.Contains(_utTeam.GetLabel(), embedding!.Content.Description);
        }
    }

    [Fact]
    public async Task CreateAsync_HighlightTeamWithEventKey_ReturnsScheduleForTeamAtEvent()
    {
        var input = (eventKey: "2022miket", numMatches: (ushort)5);
        var highlightTeam = (ushort)254;
        var teamLabel = "Team 254";
        var eventLabel = "Event 2022miket";
        var matches = new List<Match>
        {
            DeserializeMatch("{\"comp_level\": \"qm\", \"set_number\": 1, \"match_number\": 1, \"predicted_time\": " + this.TimeMock.Object.GetUtcNow().ToUnixTimeSeconds() + "}")
        };

        this.Mocker.GetMock<ITeamCache>().Setup(t => t[highlightTeam].GetLabel(false, false, It.IsAny<bool>(), It.IsAny<bool>())).Returns(teamLabel);
        this.Mocker.GetMock<IEventCache>().Setup(e => e[input.eventKey].GetLabel(true, true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(eventLabel);
        this.Mocker.GetMock<IMatchApi>().Setup(m => m.GetTeamEventMatchesAsync(input.eventKey, $"frc{highlightTeam}", It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. matches]);

        var result = _schedule.CreateAsync(input, highlightTeam);

        await foreach (var embedding in result)
        {
            Assert.NotNull(embedding);
            Assert.Contains(teamLabel, embedding!.Content.Description);
            Assert.Contains(eventLabel, embedding.Content.Description);
        }
    }

    [Fact]
    public async Task CreateAsync_NoHighlightTeam_ReturnsScheduleForEvent()
    {
        var input = (eventKey: "2022miket", numMatches: (ushort)5);
        var eventLabel = "Event 2022miket";
        var matches = new List<Match>
        {
            DeserializeMatch("{\"comp_level\": \"qm\", \"set_number\": 1, \"match_number\": 1, \"predicted_time\": " + this.TimeMock.Object.GetUtcNow().ToUnixTimeSeconds() + "}")
        };

        this.Mocker.GetMock<IEventCache>().Setup(e => e[input.eventKey!].GetLabel(true, true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(eventLabel);
        this.Mocker.GetMock<IMatchApi>().Setup(m => m.GetEventMatchesAsync(input.eventKey!, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. matches]);

        var result = _schedule.CreateAsync(input);

        await foreach (var embedding in result)
        {
            Assert.NotNull(embedding);
            Assert.Contains(eventLabel, embedding!.Content.Description);
        }
    }

    [Fact]
    public async Task CreateAsync_NoMatchesToPost_ReturnsNoMatchesScheduled()
    {
        var input = (eventKey: "2022miket", numMatches: (ushort)5);
        var eventLabel = "Event 2022miket";
        var matches = new List<Match>();

        this.Mocker.GetMock<IEventCache>().Setup(e => e[input.eventKey!].GetLabel(true, true, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(eventLabel);
        this.Mocker.GetMock<IMatchApi>().Setup(m => m.GetEventMatchesAsync(input.eventKey!, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync([.. matches]);

        var result = _schedule.CreateAsync(input);

        await foreach (var embedding in result)
        {
            Assert.NotNull(embedding);
            Assert.Equal("No matches scheduled yet.", embedding!.Content.Description);
        }
    }
}