namespace FunctionApp.Tests.DiscordInterop.Embeds;

using Discord;

using FunctionApp.TbaInterop.Models;

using Microsoft.Extensions.Logging;

using Moq;

using System.Text.Json;

using TheBlueAlliance.Api;
using TheBlueAlliance.Model;

using Xunit.Abstractions;

using Match = TheBlueAlliance.Model.Match;
using MatchVideo = FunctionApp.DiscordInterop.Embeds.MatchVideo;

public class MatchVideoTests : EmbeddingTest
{
    private readonly MatchVideo _matchVideo;

    public MatchVideoTests(ITestOutputHelper outputHelper) : base(typeof(MatchVideo), outputHelper) => _matchVideo = this.Mocker.CreateInstance<MatchVideo>();

    [Fact]
    public async Task CreateAsync_ShouldHandleNullVideoData()
    {
        // Arrange
        var webhookMessageJson = """
        {
            "message_type": "match_video",
            "message_data": null
        }
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        // Act
        var result = await _matchVideo.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.Null(result[0]);
        this.Logger.Verify(LogLevel.Warning, "Failed to deserialize notification data as match_video");
    }

    [Fact]
    public async Task CreateAsync_ShouldHandleNoVideoUrls()
    {
        // Arrange
        var webhookMessageJson = """
        {"message_type":"match_video","message_data":{"event_key":"2025nytr","match_key":"2025nytr_qm50","event_name":"New York Tech Valley Regional","match":{"key":"2025nytr_qm50","event_key":"2025nytr","comp_level":"qm","set_number":1,"match_number":50,"alliances":{"red":{"team_keys":["frc2053","frc8067","frc334"],"score":64,"surrogate_team_keys":[],"dq_team_keys":[]},"blue":{"team_keys":["frc578","frc1591","frc4458"],"score":141,"surrogate_team_keys":[],"dq_team_keys":[]}},"winning_alliance":"blue","score_breakdown":{"red":{"autoLineRobot1":"Yes","endGameRobot1":"Parked","autoLineRobot2":"Yes","endGameRobot2":"None","autoLineRobot3":"No","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":false},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":1},"autoCoralCount":1,"autoMobilityPoints":6,"autoPoints":13,"autoCoralPoints":7,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":1,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":6},"teleopCoralCount":6,"teleopPoints":49,"teleopCoralPoints":27,"algaePoints":20,"netAlgaeCount":2,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":false,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":0,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":2,"rp":1,"totalPoints":64},"blue":{"autoLineRobot1":"Yes","endGameRobot1":"None","autoLineRobot2":"Yes","endGameRobot2":"Parked","autoLineRobot3":"Yes","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":3},"autoCoralCount":3,"autoMobilityPoints":9,"autoPoints":30,"autoCoralPoints":21,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":true,"nodeI":true,"nodeJ":true,"nodeK":true,"nodeL":true},"midRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":true,"nodeK":true,"nodeL":false},"botRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"trough":0,"tba_botRowCount":6,"tba_midRowCount":7,"tba_topRowCount":10},"teleopCoralCount":20,"teleopPoints":111,"teleopCoralPoints":81,"algaePoints":28,"netAlgaeCount":4,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":true,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":1,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":0,"rp":5,"totalPoints":141}},"videos":null,"time":1743193200,"actual_time":1743193361,"predicted_time":1743193158,"post_result_time":1743193555}},"IsBroadcast":false}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        // Act
        var result = await _matchVideo.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnVideoUrls()
    {
        // Arrange
        var webhookMessageJson = """
        {"message_type":"match_video","message_data":{"event_key":"2025nytr","match_key":"2025nytr_qm50","event_name":"New York Tech Valley Regional","match":{"key":"2025nytr_qm50","event_key":"2025nytr","comp_level":"qm","set_number":1,"match_number":50,"alliances":{"red":{"team_keys":["frc2053","frc8067","frc334"],"score":64,"surrogate_team_keys":[],"dq_team_keys":[]},"blue":{"team_keys":["frc578","frc1591","frc4458"],"score":141,"surrogate_team_keys":[],"dq_team_keys":[]}},"winning_alliance":"blue","score_breakdown":{"red":{"autoLineRobot1":"Yes","endGameRobot1":"Parked","autoLineRobot2":"Yes","endGameRobot2":"None","autoLineRobot3":"No","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":false},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":1},"autoCoralCount":1,"autoMobilityPoints":6,"autoPoints":13,"autoCoralPoints":7,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":1,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":6},"teleopCoralCount":6,"teleopPoints":49,"teleopCoralPoints":27,"algaePoints":20,"netAlgaeCount":2,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":false,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":0,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":2,"rp":1,"totalPoints":64},"blue":{"autoLineRobot1":"Yes","endGameRobot1":"None","autoLineRobot2":"Yes","endGameRobot2":"Parked","autoLineRobot3":"Yes","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":3},"autoCoralCount":3,"autoMobilityPoints":9,"autoPoints":30,"autoCoralPoints":21,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":true,"nodeI":true,"nodeJ":true,"nodeK":true,"nodeL":true},"midRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":true,"nodeK":true,"nodeL":false},"botRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"trough":0,"tba_botRowCount":6,"tba_midRowCount":7,"tba_topRowCount":10},"teleopCoralCount":20,"teleopPoints":111,"teleopCoralPoints":81,"algaePoints":28,"netAlgaeCount":4,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":true,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":1,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":0,"rp":5,"totalPoints":141}},"videos":[{"type":"youtube","key":"0kbP6GFJVIE"}],"time":1743193200,"actual_time":1743193361,"predicted_time":1743193158,"post_result_time":1743193555}},"IsBroadcast":false}
        """;
        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(webhookMessageJson)!;

        // Act
        var result = await _matchVideo.CreateAsync(webhookMessage).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[0]);
        Assert.Contains("New York Tech Valley Regional", result[0]!.Content.Title);

        var actionButton = result[0]!.Actions?.SingleOrDefault() as ButtonComponent;
        Assert.NotNull(actionButton);
        Assert.Equal(Discord.ComponentType.Button, actionButton.Type);
        Assert.Equal("🎞️YouTube", actionButton.Label);
        Assert.Equal("https://youtube.com/watch?v=0kbP6GFJVIE", actionButton.Url);
        Assert.Equal(ButtonStyle.Link, actionButton.Style);
    }

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

    [Fact]
    public async Task CreateAsync_WithMatchKey_ShouldReturnVideoUrls()
    {
        // Arrange
        var matchJson = """
        {"key":"2025nytr_qm50","event_key":"2025nytr","comp_level":"qm","set_number":1,"match_number":50,"alliances":{"red":{"team_keys":["frc2053","frc8067","frc334"],"score":64,"surrogate_team_keys":[],"dq_team_keys":[]},"blue":{"team_keys":["frc578","frc1591","frc4458"],"score":141,"surrogate_team_keys":[],"dq_team_keys":[]}},"winning_alliance":"blue","score_breakdown":{"red":{"autoLineRobot1":"Yes","endGameRobot1":"Parked","autoLineRobot2":"Yes","endGameRobot2":"None","autoLineRobot3":"No","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":false},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":1},"autoCoralCount":1,"autoMobilityPoints":6,"autoPoints":13,"autoCoralPoints":7,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":true,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":1,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":6},"teleopCoralCount":6,"teleopPoints":49,"teleopCoralPoints":27,"algaePoints":20,"netAlgaeCount":2,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":false,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":0,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":2,"rp":1,"totalPoints":64},"blue":{"autoLineRobot1":"Yes","endGameRobot1":"None","autoLineRobot2":"Yes","endGameRobot2":"Parked","autoLineRobot3":"Yes","endGameRobot3":"None","autoReef":{"topRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"midRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"botRow":{"nodeA":false,"nodeB":false,"nodeC":false,"nodeD":false,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":false,"nodeK":false,"nodeL":false},"trough":0,"tba_botRowCount":0,"tba_midRowCount":0,"tba_topRowCount":3},"autoCoralCount":3,"autoMobilityPoints":9,"autoPoints":30,"autoCoralPoints":21,"teleopReef":{"topRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":true,"nodeI":true,"nodeJ":true,"nodeK":true,"nodeL":true},"midRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":true,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":false,"nodeJ":true,"nodeK":true,"nodeL":false},"botRow":{"nodeA":true,"nodeB":true,"nodeC":true,"nodeD":true,"nodeE":false,"nodeF":false,"nodeG":false,"nodeH":false,"nodeI":true,"nodeJ":false,"nodeK":false,"nodeL":true},"trough":0,"tba_botRowCount":6,"tba_midRowCount":7,"tba_topRowCount":10},"teleopCoralCount":20,"teleopPoints":111,"teleopCoralPoints":81,"algaePoints":28,"netAlgaeCount":4,"wallAlgaeCount":2,"endGameBargePoints":2,"autoBonusAchieved":true,"coralBonusAchieved":true,"bargeBonusAchieved":false,"coopertitionCriteriaMet":true,"foulCount":1,"techFoulCount":0,"g206Penalty":false,"g410Penalty":false,"g418Penalty":false,"g428Penalty":false,"adjustPoints":0,"foulPoints":0,"rp":5,"totalPoints":141}},"videos":[{"type":"youtube","key":"0kbP6GFJVIE"}],"time":1743193200,"actual_time":1743193361,"predicted_time":1743193158,"post_result_time":1743193555}
        """;
        var match = JsonSerializer.Deserialize<Match>(matchJson)!;
        var matchKey = "2022miket_qm1";
        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(match);
        this.Mocker.GetMock<IEventApi>()
            .Setup(cache => cache.GetEvent(match.EventKey, It.IsAny<string>()))
            .Returns(_testEvent);

        // Act
        var result = await _matchVideo.CreateAsync(matchKey).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[0]);
        Assert.Equal("Videos for Phantom Lakes Regional | Quals 1.50", result[0]!.Content.Title);
        Assert.Contains("[YouTube](https://youtube.com/watch?v=0kbP6GFJVIE)", result[0]!.Content.Description);
    }

    [Fact]
    public async Task CreateAsync_WithMatchKey_ShouldHandleNoMatch()
    {
        // Arrange
        var matchKey = "2022miket_qm1";
        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatchAsync(matchKey, It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Match?)null);

        // Act
        var result = await _matchVideo.CreateAsync(matchKey).ToListAsync();

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[0]);
        Assert.Equal("No videos found", result[0]!.Content.Description);
    }
}
