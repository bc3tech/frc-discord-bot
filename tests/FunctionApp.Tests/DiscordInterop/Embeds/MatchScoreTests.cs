namespace FunctionApp.Tests.DiscordInterop.Embeds;

using FIRST.Api;
using FIRST.Model;

using FunctionApp.ChatBot;
using FunctionApp.TbaInterop.Models;

using Microsoft.Extensions.Logging;

using Moq;

using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using TheBlueAlliance.Api;
using TheBlueAlliance.Caching;
using TheBlueAlliance.Model;

using Xunit;
using Xunit.Abstractions;

using Match = TheBlueAlliance.Model.Match;
using MatchScore = FunctionApp.DiscordInterop.Embeds.MatchScore;

public class MatchScoreTests : EmbeddingTest
{
    private readonly MatchScore _matchScore;

    public MatchScoreTests(ITestOutputHelper outputHelper) : base(typeof(MatchScore), outputHelper)
    {
        this.Mocker.WithSelfMock<IEventApi>();
        this.Mocker.WithSelfMock<IMatchApi>();
        this.Mocker.WithSelfMock<IDistrictApi>();
        this.Mocker.WithSelfMock<FIRST.Api.IScheduleApi>();
        this.Mocker.WithSelfMock<IChatWithLLMs>();

        this.Mocker.With<EventCache>();
        this.Mocker.With<TeamCache>();

        _matchScore = this.Mocker.CreateInstance<MatchScore>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnMatchScoreEmbed()
    {
        // Arrange
        var eventKey = "2025wimu";
        var matchKey = "2025wimu_qm24";
        var webhookMessageJson = """
        {"message_type": "match_score", "message_data": {"event_key": "2025wimu", "match_key": "2025wimu_qm24", "event_name": "Phantom Lakes Regional", "match": {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}}}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        var tbaMatchJson = """
        {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}
        """;
        var tbaMatch = JsonSerializer.Deserialize<Match>(tbaMatchJson)!;

        this.Mocker.GetMock<IMatchApi>()
            .Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tbaMatch);
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventMatchesSimpleAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([JsonSerializer.Deserialize<MatchSimple>(tbaMatchJson)]);
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventRankingsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testRankings);
        this.Mocker.GetMock<IDistrictApi>()
            .Setup(api => api.GetEventDistrictPointsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testDistrictPoints);

        this.Mocker.GetMock<IEventApi>()
            .Setup(i => i.GetEvent(eventKey, It.IsAny<string>())).Returns(_testEvent);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(i => i.GetTeam(It.IsAny<string>(), It.IsAny<string>())).Returns(_testTeam);

        this.Mocker.GetMock<IScheduleApi>()
            .Setup(api => api.SeasonScheduleEventCodeGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TournamentLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testSchedule);

        // Act
        var result = _matchScore.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        var response = result.Current;
        Assert.NotNull(response);
        Assert.Contains("Scores are in!", response.Content.Description);
        Assert.Contains("Phantom Lakes Regional: Quals 1 - Match 2", response.Content.Title);
        Assert.Contains("Red Alliance - 87", response.Content.Description);
        Assert.Contains("Blue Alliance - 82", response.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleMissingMatchData()
    {
        // Arrange
        var matchKey = "2025wimu_qm24";
        var webhookMessageJson = """
        {"message_type": "match_score", "message_data": {"event_key": "2025wimu", "match_key": "2025wimu_qm24", "event_name": "Phantom Lakes Regional", "match": {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}}}
        
        
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        this.Mocker.GetMock<IMatchApi>()
            .Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Match?)null);

        // Act
        var result = _matchScore.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        Assert.Null(result.Current);
        this.Logger.Verify(Microsoft.Extensions.Logging.LogLevel.Warning, "Failed to retrieve detailed match data for 2025wimu_qm24");
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleInvalidScores()
    {
        // Arrange
        var matchKey = "2025gaalb_qm25";
        var webhookMessageJson = """
        {"message_type": "match_score", "message_data": {"event_key": "2025gaalb", "match_key": "2025gaalb_qm25", "event_name": "PCH District Albany Event presented by Procter & Gamble", "match": {"key": "2025gaalb_qm25", "event_key": "2025gaalb", "comp_level": "qm", "set_number": 1, "match_number": 25, "alliances": {"red": {"team_keys": ["frc4459", "frc5074", "frc4509"], "score": -1, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc7514", "frc10482", "frc3329"], "score": -1, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "blue", "score_breakdown": null, "videos": [], "time": 1743191280, "actual_time": 1743191103, "predicted_time": 1743191239, "post_result_time": null}}}
        
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        var tbaMatchJson = """
        {"key": "2025gaalb_qm25", "event_key": "2025gaalb", "comp_level": "qm", "set_number": 1, "match_number": 25, "alliances": {"red": {"team_keys": ["frc4459", "frc5074", "frc4509"], "score": null, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc7514", "frc10482", "frc3329"], "score": null, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "blue", "score_breakdown": null, "videos": [], "time": 1743191280, "actual_time": 1743191103, "predicted_time": 1743191239, "post_result_time": null}
        """;
        var tbaMatch = JsonSerializer.Deserialize<Match>(tbaMatchJson)!;

        this.Mocker.GetMock<IMatchApi>()
            .Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tbaMatch);

        // Act
        var result = _matchScore.CreateAsync(webhookMessage).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        Assert.Null(result.Current);
        this.Logger.Verify(Microsoft.Extensions.Logging.LogLevel.Warning);
    }

    #region Test Objects
    private static readonly EventRanking _testRankings = JsonSerializer.Deserialize<EventRanking>("""
                                {
                	"extra_stats_info": [
                		{
                			"name": "Total Ranking Points",
                			"precision": 0
                		}
                	],
                	"rankings": [
                		{
                			"dq": 0,
                			"extra_stats": [
                				20
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 1,
                			"record": {
                				"losses": 0,
                				"ties": 0,
                				"wins": 4
                			},
                			"sort_orders": [
                				5,
                				0.25,
                				109.5,
                				18.75,
                				20.5,
                				0
                			],
                			"team_key": "frc6574"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				18
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 2,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				4.5,
                				0,
                				148.25,
                				33.5,
                				17.5,
                				0
                			],
                			"team_key": "frc2530"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				18
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 3,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				4.5,
                				0,
                				131.5,
                				24.75,
                				23,
                				0
                			],
                			"team_key": "frc5934"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				13
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 4,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				4.33,
                				0,
                				101.67,
                				26.33,
                				15.33,
                				0
                			],
                			"team_key": "frc2077"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				17
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 5,
                			"record": {
                				"losses": 0,
                				"ties": 0,
                				"wins": 4
                			},
                			"sort_orders": [
                				4.25,
                				0,
                				126,
                				30,
                				13,
                				0
                			],
                			"team_key": "frc4607"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				17
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 6,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				4.25,
                				0,
                				123.5,
                				24.5,
                				15,
                				0
                			],
                			"team_key": "frc8701"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				16
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 7,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				4,
                				0.25,
                				128.25,
                				26.25,
                				14.5,
                				0
                			],
                			"team_key": "frc4635"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				15
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 8,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3.75,
                				0.25,
                				118.75,
                				19.25,
                				15.5,
                				0
                			],
                			"team_key": "frc6223"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				15
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 9,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				3.75,
                				0,
                				118.25,
                				25.75,
                				12.5,
                				0
                			],
                			"team_key": "frc1792"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				15
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 10,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				3.75,
                				0,
                				116.75,
                				25,
                				18,
                				0
                			],
                			"team_key": "frc930"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 11,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3.67,
                				0,
                				108.33,
                				27.67,
                				16,
                				0
                			],
                			"team_key": "frc9401"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				14
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 12,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				3.5,
                				0.25,
                				89.5,
                				15,
                				7.5,
                				0
                			],
                			"team_key": "frc3184"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				14
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 13,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3.5,
                				0,
                				106.25,
                				26.5,
                				13,
                				0
                			],
                			"team_key": "frc4786"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				14
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 14,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3.5,
                				0,
                				105,
                				21,
                				21,
                				0
                			],
                			"team_key": "frc6381"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				14
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 15,
                			"record": {
                				"losses": 1,
                				"ties": 0,
                				"wins": 3
                			},
                			"sort_orders": [
                				3.5,
                				0,
                				104,
                				18.75,
                				15.5,
                				0
                			],
                			"team_key": "frc6317"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				13
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 16,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3.25,
                				0.25,
                				107,
                				24.5,
                				9.5,
                				0
                			],
                			"team_key": "frc2202"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				12
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 17,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3,
                				0.25,
                				113.75,
                				17.5,
                				9,
                				0
                			],
                			"team_key": "frc1306"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				12
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 18,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3,
                				0,
                				104,
                				23,
                				18,
                				0
                			],
                			"team_key": "frc1220"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				12
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 19,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				3,
                				0,
                				87.5,
                				15,
                				10,
                				0
                			],
                			"team_key": "frc6643"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 20,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.75,
                				0.25,
                				117,
                				32.5,
                				8,
                				0
                			],
                			"team_key": "frc1732"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 21,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.75,
                				0,
                				102.75,
                				24.75,
                				6.5,
                				0
                			],
                			"team_key": "frc3354"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 22,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.75,
                				0,
                				100.5,
                				22,
                				10.5,
                				0
                			],
                			"team_key": "frc5148"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 23,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.75,
                				0,
                				89.25,
                				15,
                				13,
                				0
                			],
                			"team_key": "frc7900"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				11
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 24,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.75,
                				0,
                				86.75,
                				24.75,
                				4.5,
                				0
                			],
                			"team_key": "frc2290"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				10
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 25,
                			"record": {
                				"losses": 3,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				2.5,
                				0.25,
                				118.25,
                				21.25,
                				11.5,
                				0
                			],
                			"team_key": "frc6318"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				10
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 26,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.5,
                				0.25,
                				94.5,
                				17.5,
                				7,
                				0
                			],
                			"team_key": "frc6421"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				10
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 27,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.5,
                				0,
                				98.25,
                				24.75,
                				13.5,
                				0
                			],
                			"team_key": "frc6947"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				7
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 28,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				2.33,
                				0.33,
                				100.33,
                				21.67,
                				4.67,
                				0
                			],
                			"team_key": "frc7417"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				9
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 29,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 2
                			},
                			"sort_orders": [
                				2.25,
                				0,
                				69.5,
                				18.75,
                				6,
                				0
                			],
                			"team_key": "frc10264"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				6
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 30,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				2,
                				0,
                				100,
                				20.67,
                				6.67,
                				0
                			],
                			"team_key": "frc1714"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				6
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 31,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				2,
                				0,
                				80.33,
                				22,
                				8,
                				0
                			],
                			"team_key": "frc10553"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				7
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 32,
                			"record": {
                				"losses": 3,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				1.75,
                				0.25,
                				92.5,
                				19.25,
                				7.5,
                				0
                			],
                			"team_key": "frc1091"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				7
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 33,
                			"record": {
                				"losses": 3,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				1.75,
                				0,
                				103,
                				28,
                				5,
                				0
                			],
                			"team_key": "frc6024"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				5
                			],
                			"matches_played": 3,
                			"qual_average": null,
                			"rank": 34,
                			"record": {
                				"losses": 2,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				1.67,
                				0,
                				81.67,
                				15,
                				7.33,
                				0
                			],
                			"team_key": "frc6166"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				6
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 35,
                			"record": {
                				"losses": 3,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				1.5,
                				0,
                				99.5,
                				14.25,
                				2.5,
                				0
                			],
                			"team_key": "frc4693"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				5
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 36,
                			"record": {
                				"losses": 3,
                				"ties": 0,
                				"wins": 1
                			},
                			"sort_orders": [
                				1.25,
                				0,
                				89,
                				15,
                				5.5,
                				0
                			],
                			"team_key": "frc2062"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				4
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 37,
                			"record": {
                				"losses": 4,
                				"ties": 0,
                				"wins": 0
                			},
                			"sort_orders": [
                				1,
                				0.25,
                				88,
                				13.25,
                				6.5,
                				0
                			],
                			"team_key": "frc8700"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				3
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 38,
                			"record": {
                				"losses": 4,
                				"ties": 0,
                				"wins": 0
                			},
                			"sort_orders": [
                				0.75,
                				0,
                				81.75,
                				13.5,
                				3,
                				0
                			],
                			"team_key": "frc3692"
                		},
                		{
                			"dq": 0,
                			"extra_stats": [
                				2
                			],
                			"matches_played": 4,
                			"qual_average": null,
                			"rank": 39,
                			"record": {
                				"losses": 4,
                				"ties": 0,
                				"wins": 0
                			},
                			"sort_orders": [
                				0.5,
                				0,
                				77,
                				14.25,
                				7,
                				0
                			],
                			"team_key": "frc5993"
                		}
                	],
                	"sort_order_info": [
                		{
                			"name": "Ranking Score",
                			"precision": 2
                		},
                		{
                			"name": "Avg Coop",
                			"precision": 2
                		},
                		{
                			"name": "Avg Match",
                			"precision": 2
                		},
                		{
                			"name": "Avg Auto",
                			"precision": 2
                		},
                		{
                			"name": "Avg Barge",
                			"precision": 2
                		}
                	]
                }
                """)!;
    private static readonly EventDistrictPoints _testDistrictPoints = JsonSerializer.Deserialize<EventDistrictPoints>("""
                                {
                	"points": {
                		"frc10264": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 10,
                			"total": 10
                		},
                		"frc10553": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 9,
                			"total": 9
                		},
                		"frc1091": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 8,
                			"total": 8
                		},
                		"frc1220": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 13,
                			"total": 13
                		},
                		"frc1306": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 14,
                			"total": 14
                		},
                		"frc1714": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 9,
                			"total": 9
                		},
                		"frc1732": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 13,
                			"total": 13
                		},
                		"frc1792": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 17,
                			"total": 17
                		},
                		"frc2062": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 6,
                			"total": 6
                		},
                		"frc2077": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 19,
                			"total": 19
                		},
                		"frc2202": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 14,
                			"total": 14
                		},
                		"frc2290": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 11,
                			"total": 11
                		},
                		"frc2530": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 21,
                			"total": 21
                		},
                		"frc3184": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 15,
                			"total": 15
                		},
                		"frc3354": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 12,
                			"total": 12
                		},
                		"frc3692": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 5,
                			"total": 5
                		},
                		"frc4607": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 19,
                			"total": 19
                		},
                		"frc4635": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 18,
                			"total": 18
                		},
                		"frc4693": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 7,
                			"total": 7
                		},
                		"frc4786": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 15,
                			"total": 15
                		},
                		"frc5148": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 12,
                			"total": 12
                		},
                		"frc5934": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 20,
                			"total": 20
                		},
                		"frc5993": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 4,
                			"total": 4
                		},
                		"frc6024": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 8,
                			"total": 8
                		},
                		"frc6166": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 7,
                			"total": 7
                		},
                		"frc6223": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 17,
                			"total": 17
                		},
                		"frc6317": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 14,
                			"total": 14
                		},
                		"frc6318": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 11,
                			"total": 11
                		},
                		"frc6381": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 15,
                			"total": 15
                		},
                		"frc6421": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 11,
                			"total": 11
                		},
                		"frc6574": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 22,
                			"total": 22
                		},
                		"frc6643": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 13,
                			"total": 13
                		},
                		"frc6947": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 10,
                			"total": 10
                		},
                		"frc7417": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 10,
                			"total": 10
                		},
                		"frc7900": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 12,
                			"total": 12
                		},
                		"frc8700": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 6,
                			"total": 6
                		},
                		"frc8701": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 18,
                			"total": 18
                		},
                		"frc930": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 16,
                			"total": 16
                		},
                		"frc9401": {
                			"alliance_points": 0,
                			"award_points": 0,
                			"elim_points": 0,
                			"qual_points": 16,
                			"total": 16
                		}
                	},
                	"tiebreakers": {
                		"frc10264": {
                			"highest_qual_scores": [
                				104,
                				87,
                				80
                			],
                			"qual_wins": 0
                		},
                		"frc10553": {
                			"highest_qual_scores": [
                				153,
                				79,
                				15
                			],
                			"qual_wins": 0
                		},
                		"frc1091": {
                			"highest_qual_scores": [
                				132,
                				105,
                				104
                			],
                			"qual_wins": 0
                		},
                		"frc1220": {
                			"highest_qual_scores": [
                				118,
                				113,
                				94
                			],
                			"qual_wins": 0
                		},
                		"frc1306": {
                			"highest_qual_scores": [
                				144,
                				109,
                				105
                			],
                			"qual_wins": 0
                		},
                		"frc1714": {
                			"highest_qual_scores": [
                				115,
                				105,
                				80
                			],
                			"qual_wins": 0
                		},
                		"frc1732": {
                			"highest_qual_scores": [
                				137,
                				123,
                				109
                			],
                			"qual_wins": 0
                		},
                		"frc1792": {
                			"highest_qual_scores": [
                				153,
                				119,
                				116
                			],
                			"qual_wins": 0
                		},
                		"frc2062": {
                			"highest_qual_scores": [
                				107,
                				98,
                				88
                			],
                			"qual_wins": 0
                		},
                		"frc2077": {
                			"highest_qual_scores": [
                				130,
                				102,
                				75
                			],
                			"qual_wins": 0
                		},
                		"frc2202": {
                			"highest_qual_scores": [
                				137,
                				115,
                				105
                			],
                			"qual_wins": 0
                		},
                		"frc2290": {
                			"highest_qual_scores": [
                				108,
                				101,
                				91
                			],
                			"qual_wins": 0
                		},
                		"frc2530": {
                			"highest_qual_scores": [
                				176,
                				153,
                				140
                			],
                			"qual_wins": 0
                		},
                		"frc3184": {
                			"highest_qual_scores": [
                				133,
                				101,
                				98
                			],
                			"qual_wins": 0
                		},
                		"frc3354": {
                			"highest_qual_scores": [
                				120,
                				114,
                				90
                			],
                			"qual_wins": 0
                		},
                		"frc3692": {
                			"highest_qual_scores": [
                				114,
                				109,
                				89
                			],
                			"qual_wins": 0
                		},
                		"frc4607": {
                			"highest_qual_scores": [
                				144,
                				123,
                				122
                			],
                			"qual_wins": 0
                		},
                		"frc4635": {
                			"highest_qual_scores": [
                				175,
                				138,
                				132
                			],
                			"qual_wins": 0
                		},
                		"frc4693": {
                			"highest_qual_scores": [
                				109,
                				105,
                				98
                			],
                			"qual_wins": 0
                		},
                		"frc4786": {
                			"highest_qual_scores": [
                				130,
                				114,
                				101
                			],
                			"qual_wins": 0
                		},
                		"frc5148": {
                			"highest_qual_scores": [
                				144,
                				120,
                				89
                			],
                			"qual_wins": 0
                		},
                		"frc5934": {
                			"highest_qual_scores": [
                				176,
                				124,
                				115
                			],
                			"qual_wins": 0
                		},
                		"frc5993": {
                			"highest_qual_scores": [
                				94,
                				77,
                				72
                			],
                			"qual_wins": 0
                		},
                		"frc6024": {
                			"highest_qual_scores": [
                				132,
                				107,
                				91
                			],
                			"qual_wins": 0
                		},
                		"frc6166": {
                			"highest_qual_scores": [
                				90,
                				80,
                				75
                			],
                			"qual_wins": 0
                		},
                		"frc6223": {
                			"highest_qual_scores": [
                				175,
                				116,
                				104
                			],
                			"qual_wins": 0
                		},
                		"frc6317": {
                			"highest_qual_scores": [
                				123,
                				118,
                				108
                			],
                			"qual_wins": 0
                		},
                		"frc6318": {
                			"highest_qual_scores": [
                				175,
                				140,
                				96
                			],
                			"qual_wins": 0
                		},
                		"frc6381": {
                			"highest_qual_scores": [
                				124,
                				118,
                				102
                			],
                			"qual_wins": 0
                		},
                		"frc6421": {
                			"highest_qual_scores": [
                				133,
                				88,
                				87
                			],
                			"qual_wins": 0
                		},
                		"frc6574": {
                			"highest_qual_scores": [
                				133,
                				123,
                				113
                			],
                			"qual_wins": 0
                		},
                		"frc6643": {
                			"highest_qual_scores": [
                				116,
                				105,
                				75
                			],
                			"qual_wins": 0
                		},
                		"frc6947": {
                			"highest_qual_scores": [
                				124,
                				123,
                				79
                			],
                			"qual_wins": 0
                		},
                		"frc7417": {
                			"highest_qual_scores": [
                				109,
                				108,
                				88
                			],
                			"qual_wins": 0
                		},
                		"frc7900": {
                			"highest_qual_scores": [
                				138,
                				122,
                				54
                			],
                			"qual_wins": 0
                		},
                		"frc8700": {
                			"highest_qual_scores": [
                				109,
                				96,
                				75
                			],
                			"qual_wins": 0
                		},
                		"frc8701": {
                			"highest_qual_scores": [
                				176,
                				137,
                				122
                			],
                			"qual_wins": 0
                		},
                		"frc930": {
                			"highest_qual_scores": [
                				140,
                				123,
                				119
                			],
                			"qual_wins": 0
                		},
                		"frc9401": {
                			"highest_qual_scores": [
                				138,
                				120,
                				79
                			],
                			"qual_wins": 0
                		}
                	}
                }
                """)!;
    private static readonly Event _testEvent = JsonSerializer.Deserialize<Event>("""
                                {
                	"address": "605 W Veterans Way, Mukwonago, WI 53149, USA",
                	"city": "Mukwonago",
                	"country": "USA",
                	"district": null,
                	"division_keys": [],
                	"end_date": "2025-03-29",
                	"event_code": "wimu",
                	"event_type": 0,
                	"event_type_string": "Regional",
                	"first_event_code": "wimu",
                	"first_event_id": null,
                	"gmaps_place_id": "ChIJNaenHxO9BYgRmpwJPYDQdlw",
                	"gmaps_url": "https://maps.google.com/?q=605+W+Veterans+Way,+Mukwonago,+WI+53149,+USA&ftid=0x8805bd131fa7a735:0x5c76d0803d099c9a",
                	"key": "2025wimu",
                	"lat": 42.8726377,
                	"lng": -88.3494328,
                	"location_name": "605 W Veterans Way",
                	"name": "Phantom Lakes Regional",
                	"parent_event_key": null,
                	"playoff_type": 10,
                	"playoff_type_string": "Double Elimination Bracket (8 Alliances)",
                	"postal_code": "53149",
                	"short_name": "Phantom Lakes",
                	"start_date": "2025-03-27",
                	"state_prov": "WI",
                	"timezone": "America/Chicago",
                	"webcasts": [
                		{
                			"channel": "firstinspires11",
                			"type": "twitch"
                		}
                	],
                	"website": null,
                	"week": 4,
                	"year": 2025
                }
                """)!;
    private static readonly Team _testTeam = JsonSerializer.Deserialize<Team>("""
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
    private static readonly EventSchedule _testSchedule = JsonSerializer.Deserialize<EventSchedule>("""
                                {
                	"Schedule": [
                		{
                			"description": "Qualification 1",
                			"startTime": "2025-03-28T10:50:00",
                			"matchNumber": 1,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6317,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 2",
                			"startTime": "2025-03-28T10:58:00",
                			"matchNumber": 2,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5934,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 3",
                			"startTime": "2025-03-28T11:06:00",
                			"matchNumber": 3,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 7417,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 4",
                			"startTime": "2025-03-28T11:14:00",
                			"matchNumber": 4,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2077,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 5",
                			"startTime": "2025-03-28T11:22:00",
                			"matchNumber": 5,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 10264,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 6",
                			"startTime": "2025-03-28T11:30:00",
                			"matchNumber": 6,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1306,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 7",
                			"startTime": "2025-03-28T11:38:00",
                			"matchNumber": 7,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2062,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 8",
                			"startTime": "2025-03-28T11:46:00",
                			"matchNumber": 8,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8700,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 9",
                			"startTime": "2025-03-28T11:54:00",
                			"matchNumber": 9,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6381,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 10",
                			"startTime": "2025-03-28T12:02:00",
                			"matchNumber": 10,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6024,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 11",
                			"startTime": "2025-03-28T12:10:00",
                			"matchNumber": 11,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4607,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 12",
                			"startTime": "2025-03-28T12:18:00",
                			"matchNumber": 12,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4786,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 13",
                			"startTime": "2025-03-28T12:26:00",
                			"matchNumber": 13,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6166,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 14",
                			"startTime": "2025-03-28T12:34:00",
                			"matchNumber": 14,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6947,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 15",
                			"startTime": "2025-03-28T12:42:00",
                			"matchNumber": 15,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6024,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 16",
                			"startTime": "2025-03-28T12:50:00",
                			"matchNumber": 16,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6318,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 17",
                			"startTime": "2025-03-28T14:00:00",
                			"matchNumber": 17,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6574,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 18",
                			"startTime": "2025-03-28T14:08:00",
                			"matchNumber": 18,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 3354,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 19",
                			"startTime": "2025-03-28T14:16:00",
                			"matchNumber": 19,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 10264,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 20",
                			"startTime": "2025-03-28T14:24:00",
                			"matchNumber": 20,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 7900,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 21",
                			"startTime": "2025-03-28T14:32:00",
                			"matchNumber": 21,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6574,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 22",
                			"startTime": "2025-03-28T14:40:00",
                			"matchNumber": 22,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2202,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 23",
                			"startTime": "2025-03-28T14:48:00",
                			"matchNumber": 23,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 3692,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 24",
                			"startTime": "2025-03-28T14:56:00",
                			"matchNumber": 24,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6421,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 25",
                			"startTime": "2025-03-28T15:04:00",
                			"matchNumber": 25,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5934,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 26",
                			"startTime": "2025-03-28T15:12:00",
                			"matchNumber": 26,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6166,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 27",
                			"startTime": "2025-03-28T15:20:00",
                			"matchNumber": 27,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1306,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 28",
                			"startTime": "2025-03-28T15:28:00",
                			"matchNumber": 28,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5993,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 29",
                			"startTime": "2025-03-28T15:36:00",
                			"matchNumber": 29,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 3354,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 30",
                			"startTime": "2025-03-28T15:44:00",
                			"matchNumber": 30,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6223,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 31",
                			"startTime": "2025-03-28T15:52:00",
                			"matchNumber": 31,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4786,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 32",
                			"startTime": "2025-03-28T16:00:00",
                			"matchNumber": 32,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2530,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 33",
                			"startTime": "2025-03-28T16:08:00",
                			"matchNumber": 33,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4607,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 34",
                			"startTime": "2025-03-28T16:16:00",
                			"matchNumber": 34,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5148,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 35",
                			"startTime": "2025-03-28T16:24:00",
                			"matchNumber": 35,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 930,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 36",
                			"startTime": "2025-03-28T16:32:00",
                			"matchNumber": 36,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 3354,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 37",
                			"startTime": "2025-03-28T16:40:00",
                			"matchNumber": 37,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4607,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 38",
                			"startTime": "2025-03-28T16:48:00",
                			"matchNumber": 38,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6574,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 39",
                			"startTime": "2025-03-28T16:56:00",
                			"matchNumber": 39,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2530,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 40",
                			"startTime": "2025-03-28T17:04:00",
                			"matchNumber": 40,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6947,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 41",
                			"startTime": "2025-03-28T17:12:00",
                			"matchNumber": 41,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6024,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 42",
                			"startTime": "2025-03-28T17:20:00",
                			"matchNumber": 42,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8700,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 43",
                			"startTime": "2025-03-28T17:28:00",
                			"matchNumber": 43,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 3184,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 44",
                			"startTime": "2025-03-28T17:36:00",
                			"matchNumber": 44,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1732,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 45",
                			"startTime": "2025-03-28T17:44:00",
                			"matchNumber": 45,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6317,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 46",
                			"startTime": "2025-03-28T17:52:00",
                			"matchNumber": 46,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1091,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 47",
                			"startTime": "2025-03-28T18:00:00",
                			"matchNumber": 47,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6947,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 48",
                			"startTime": "2025-03-28T18:08:00",
                			"matchNumber": 48,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 9401,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 49",
                			"startTime": "2025-03-28T18:16:00",
                			"matchNumber": 49,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6421,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 50",
                			"startTime": "2025-03-28T18:24:00",
                			"matchNumber": 50,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4786,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 51",
                			"startTime": "2025-03-28T18:32:00",
                			"matchNumber": 51,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4693,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 52",
                			"startTime": "2025-03-28T18:40:00",
                			"matchNumber": 52,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6317,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 53",
                			"startTime": "2025-03-28T18:48:00",
                			"matchNumber": 53,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1220,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 54",
                			"startTime": "2025-03-29T09:00:00",
                			"matchNumber": 54,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5148,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 55",
                			"startTime": "2025-03-29T09:08:00",
                			"matchNumber": 55,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6318,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 56",
                			"startTime": "2025-03-29T09:16:00",
                			"matchNumber": 56,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6643,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 57",
                			"startTime": "2025-03-29T09:24:00",
                			"matchNumber": 57,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2077,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 58",
                			"startTime": "2025-03-29T09:32:00",
                			"matchNumber": 58,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4635,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 59",
                			"startTime": "2025-03-29T09:40:00",
                			"matchNumber": 59,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5993,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 60",
                			"startTime": "2025-03-29T09:48:00",
                			"matchNumber": 60,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2202,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 61",
                			"startTime": "2025-03-29T09:56:00",
                			"matchNumber": 61,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1714,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 62",
                			"startTime": "2025-03-29T10:04:00",
                			"matchNumber": 62,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8700,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 63",
                			"startTime": "2025-03-29T10:12:00",
                			"matchNumber": 63,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6024,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1792,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 64",
                			"startTime": "2025-03-29T10:20:00",
                			"matchNumber": 64,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8701,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 65",
                			"startTime": "2025-03-29T10:28:00",
                			"matchNumber": 65,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 6317,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 66",
                			"startTime": "2025-03-29T10:36:00",
                			"matchNumber": 66,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 10264,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8700,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 67",
                			"startTime": "2025-03-29T10:44:00",
                			"matchNumber": 67,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4607,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10553,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 68",
                			"startTime": "2025-03-29T10:52:00",
                			"matchNumber": 68,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 5148,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 69",
                			"startTime": "2025-03-29T11:00:00",
                			"matchNumber": 69,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1091,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 70",
                			"startTime": "2025-03-29T11:08:00",
                			"matchNumber": 70,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 4635,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2290,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2062,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 71",
                			"startTime": "2025-03-29T11:16:00",
                			"matchNumber": 71,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1792,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1714,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 72",
                			"startTime": "2025-03-29T11:24:00",
                			"matchNumber": 72,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8701,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6574,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6223,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6947,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 73",
                			"startTime": "2025-03-29T11:32:00",
                			"matchNumber": 73,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 10553,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1306,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7900,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5993,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5148,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2202,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 74",
                			"startTime": "2025-03-29T11:40:00",
                			"matchNumber": 74,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2290,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4607,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4786,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3692,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2077,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6317,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 75",
                			"startTime": "2025-03-29T11:48:00",
                			"matchNumber": 75,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1714,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 930,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1732,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6643,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4635,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 10264,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 76",
                			"startTime": "2025-03-29T11:56:00",
                			"matchNumber": 76,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 1792,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1091,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 9401,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6381,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 8701,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6421,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 77",
                			"startTime": "2025-03-29T12:04:00",
                			"matchNumber": 77,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 8700,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3184,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6024,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 4693,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 7417,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 5934,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		},
                		{
                			"description": "Qualification 78",
                			"startTime": "2025-03-29T12:12:00",
                			"matchNumber": 78,
                			"field": "Primary",
                			"tournamentLevel": "Qualification",
                			"teams": [
                				{
                					"teamNumber": 2062,
                					"station": "Red1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6166,
                					"station": "Red2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 2530,
                					"station": "Red3",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 1220,
                					"station": "Blue1",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 3354,
                					"station": "Blue2",
                					"surrogate": false
                				},
                				{
                					"teamNumber": 6318,
                					"station": "Blue3",
                					"surrogate": false
                				}
                			]
                		}
                	]
                }
                """)!;
    private static readonly Match _testMatch = JsonSerializer.Deserialize<Match>("""
        {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}
        """)!;
    #endregion

    [Fact]
    public async Task CreateAsync_ShouldReturnMatchSummary()
    {
        // Arrange
        var eventKey = "2025wimu";
        var matchKey = "2025wimu_qm24";
        var input = (matchKey, summarize: true);
        var detailedMatchJson = """
        {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": 87, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": 82, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": {"red": {"autoLineRobot1": "Yes", "endGameRobot1": "None", "autoLineRobot2": "Yes", "endGameRobot2": "None", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": true, "nodeE": false, "nodeF": true, "nodeG": false, "nodeH": true, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": true, "nodeF": true, "nodeG": true, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": true, "nodeB": true, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 1, "tba_botRowCount": 0, "tba_midRowCount": 2, "tba_topRowCount": 12}, "teleopCoralCount": 12, "teleopPoints": 57, "teleopCoralPoints": 55, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 2, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": false, "coopertitionCriteriaMet": false, "foulCount": 1, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 0, "rp": 4, "totalPoints": 87}, "blue": {"autoLineRobot1": "Yes", "endGameRobot1": "DeepCage", "autoLineRobot2": "Yes", "endGameRobot2": "Parked", "autoLineRobot3": "Yes", "endGameRobot3": "Parked", "autoReef": {"topRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 0, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 3}, "autoCoralCount": 3, "autoMobilityPoints": 9, "autoPoints": 30, "autoCoralPoints": 21, "teleopReef": {"topRow": {"nodeA": true, "nodeB": true, "nodeC": true, "nodeD": true, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": true, "nodeI": true, "nodeJ": true, "nodeK": true, "nodeL": true}, "midRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "botRow": {"nodeA": false, "nodeB": false, "nodeC": false, "nodeD": false, "nodeE": false, "nodeF": false, "nodeG": false, "nodeH": false, "nodeI": false, "nodeJ": false, "nodeK": false, "nodeL": false}, "trough": 2, "tba_botRowCount": 0, "tba_midRowCount": 0, "tba_topRowCount": 9}, "teleopCoralCount": 8, "teleopPoints": 50, "teleopCoralPoints": 34, "algaePoints": 0, "netAlgaeCount": 0, "wallAlgaeCount": 0, "endGameBargePoints": 16, "autoBonusAchieved": true, "coralBonusAchieved": false, "bargeBonusAchieved": true, "coopertitionCriteriaMet": false, "foulCount": 0, "techFoulCount": 0, "g206Penalty": false, "g410Penalty": false, "g418Penalty": false, "g428Penalty": false, "adjustPoints": 0, "foulPoints": 2, "rp": 2, "totalPoints": 82}}, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}
        """;
        var detailedMatch = JsonSerializer.Deserialize<Match>(detailedMatchJson)!;

        this.Mocker.GetMock<IMatchApi>()
            .Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(detailedMatch);
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventMatchesSimpleAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([JsonSerializer.Deserialize<MatchSimple>(detailedMatchJson)]);
        this.Mocker.GetMock<IEventApi>()
            .Setup(api => api.GetEventRankingsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testRankings);
        this.Mocker.GetMock<IDistrictApi>()
            .Setup(api => api.GetEventDistrictPointsAsync(eventKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testDistrictPoints);

        this.Mocker.GetMock<IEventApi>()
            .Setup(i => i.GetEvent(eventKey, It.IsAny<string>())).Returns(_testEvent);
        this.Mocker.GetMock<ITeamApi>()
            .Setup(i => i.GetTeam(It.IsAny<string>(), It.IsAny<string>())).Returns(_testTeam);

        this.Mocker.GetMock<IScheduleApi>()
            .Setup(api => api.SeasonScheduleEventCodeGetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TournamentLevel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_testSchedule);

        var completions = new List<string> { "This is a match summary." };
        this.Mocker.GetMock<IChatWithLLMs>()
            .Setup(gpt => gpt.GetCompletionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(AsyncEnumerable.ToAsyncEnumerable(completions));

        // Act
        var result = _matchScore.CreateAsync(input).GetAsyncEnumerator();

        // Assert
        Assert.True(await result.MoveNextAsync());
        var response = result.Current;
        Assert.NotNull(response);
        Assert.Contains("Match Result", response.Content.Description);

        Assert.True(await result.MoveNextAsync());
        response = result.Current;
        Assert.NotNull(response);
        Assert.Equal("Generating match summary... 🤖", response.Content.Description);

        Assert.True(await result.MoveNextAsync());
        response = result.Current;
        Assert.NotNull(response);
        Assert.Equal("This is a match summary.", response.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleInvalidNotificationData()
    {
        // Arrange
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>("""
            { "message_type": "match_score", "message_data": null }
            """)!;

        // Act
        var result = await _matchScore.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Null(result[0]);
        this.Logger.Verify(LogLevel.Warning, "Failed to deserialize notification data as match_score");
    }

    [Fact]
    public async Task CreateAsync_WithMatchKey_ShouldHandleMissingMatchData()
    {
        // Arrange
        var matchKey = "2022miket_qm1";
        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Match?)null);

        // Act
        var result = await _matchScore.CreateAsync((matchKey, false)).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[0]);
        this.Logger.Verify(LogLevel.Warning, "Failed to retrieve detailed match data for 2022miket_qm1");
    }

    [Fact]
    public async Task CreateAsync_ShouldLogBadDataForMatch()
    {
        // Arrange
        var matchKey = "2025wimu_qm24";
        var detailedMatchJson = """
        {"key": "2025wimu_qm24", "event_key": "2025wimu", "comp_level": "qm", "set_number": 1, "match_number": 24, "alliances": {"red": {"team_keys": ["frc6421", "frc10264", "frc3354"], "score": -1, "surrogate_team_keys": [], "dq_team_keys": []}, "blue": {"team_keys": ["frc6381", "frc4786", "frc6318"], "score": -1, "surrogate_team_keys": [], "dq_team_keys": []}}, "winning_alliance": "red", "score_breakdown": null, "videos": [], "time": 1743191760, "actual_time": 1743191817, "predicted_time": 1743191747, "post_result_time": 1743192191}
        """;
        var detailedMatch = JsonSerializer.Deserialize<Match>(detailedMatchJson)!;

        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(detailedMatch);

        // Act
        var results = await _matchScore.CreateAsync((matchKey, false)).ToArrayAsync();

        // Assert
        Assert.Single(results);
        Assert.Null(results[0]);
        this.Logger.Verify(LogLevel.Warning, "Bad data for match 2025wimu_qm24");
    }
}
