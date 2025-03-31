namespace FunctionApp.Tests.TbaInterop.Models;

using FunctionApp.TbaInterop.Models;
using FunctionApp.TbaInterop.Models.Notifications;

using Moq;

using System.Text.Json;

using TestCommon;

using TheBlueAlliance.Api;

using Xunit;

using Match = TheBlueAlliance.Model.Match;

public class WebhookMessageTests : Test
{
    [Fact]
    public void GetDataAs_ShouldDeserializeJsonElement()
    {
        // Arrange
        var jsonString = "{\"key\":\"value\"}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.match_score,
            MessageData = jsonElement
        };

        // Act
        var result = webhookMessage.GetDataAs<JsonElement>();

        // Assert
        Assert.Equal(jsonElement.ToString(), result.ToString());
    }

    [Theory]
    [InlineData(nameof(NotificationType.schedule_updated), true)]
    [InlineData(nameof(NotificationType.starting_comp_level), true)]
    [InlineData(nameof(NotificationType.alliance_selection), true)]
    [InlineData(nameof(NotificationType.match_score), false)]
    public void IsBroadcast_ShouldReturnExpectedResult(string typeStr, bool expected)
    {
        var type = Enum.Parse<NotificationType>(typeStr);
        // Arrange
        var webhookMessage = new WebhookMessage
        {
            MessageType = type,
            MessageData = JsonDocument.Parse("{}").RootElement
        };

        // Act
        var result = webhookMessage.IsBroadcast;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetThreadDetails_ShouldReturnCorrectDetails_ForMatchVideo()
    {
        // Arrange
        var jsonString = "{\"event_key\": \"2025ncmec\", \"match_key\": \"2025ncmec_sf1m1\", \"event_name\": \"FNC District Mecklenburg County Event\", \"match\": {\"key\": \"2025ncmec_sf1m1\", \"event_key\": \"2025ncmec\", \"comp_level\": \"sf\", \"set_number\": 1, \"match_number\": 1, \"alliances\": {\"red\": {\"team_keys\": [\"frc9496\", \"frc9198\", \"frc8738\"], \"score\": 203, \"surrogate_team_keys\": [], \"dq_team_keys\": []}, \"blue\": {\"team_keys\": [\"frc6932\", \"frc10107\", \"frc7890\"], \"score\": 73, \"surrogate_team_keys\": [], \"dq_team_keys\": []}}, \"winning_alliance\": \"red\", \"score_breakdown\": {\"red\": {\"autoLineRobot1\": \"Yes\", \"endGameRobot1\": \"Parked\", \"autoLineRobot2\": \"Yes\", \"endGameRobot2\": \"Parked\", \"autoLineRobot3\": \"Yes\", \"endGameRobot3\": \"Parked\", \"autoReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": true, \"nodeK\": false, \"nodeL\": true}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 1, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 2}, \"autoCoralCount\": 3, \"autoMobilityPoints\": 9, \"autoPoints\": 26, \"autoCoralPoints\": 17, \"teleopReef\": {\"topRow\": {\"nodeA\": true, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": true, \"nodeF\": true, \"nodeG\": true, \"nodeH\": true, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"midRow\": {\"nodeA\": true, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": true, \"nodeF\": true, \"nodeG\": true, \"nodeH\": true, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"botRow\": {\"nodeA\": false, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"trough\": 1, \"tba_botRowCount\": 7, \"tba_midRowCount\": 12, \"tba_topRowCount\": 12}, \"teleopCoralCount\": 30, \"teleopPoints\": 177, \"teleopCoralPoints\": 121, \"algaePoints\": 50, \"netAlgaeCount\": 8, \"wallAlgaeCount\": 3, \"endGameBargePoints\": 6, \"autoBonusAchieved\": false, \"coralBonusAchieved\": false, \"bargeBonusAchieved\": false, \"coopertitionCriteriaMet\": false, \"foulCount\": 0, \"techFoulCount\": 0, \"g206Penalty\": false, \"g410Penalty\": false, \"g418Penalty\": false, \"g428Penalty\": false, \"adjustPoints\": 0, \"foulPoints\": 0, \"rp\": 0, \"totalPoints\": 203}, \"blue\": {\"autoLineRobot1\": \"Yes\", \"endGameRobot1\": \"Parked\", \"autoLineRobot2\": \"Yes\", \"endGameRobot2\": \"Parked\", \"autoLineRobot3\": \"Yes\", \"endGameRobot3\": \"None\", \"autoReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 2, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 0}, \"autoCoralCount\": 2, \"autoMobilityPoints\": 9, \"autoPoints\": 15, \"autoCoralPoints\": 6, \"teleopReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 7, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 0}, \"teleopCoralCount\": 7, \"teleopPoints\": 58, \"teleopCoralPoints\": 14, \"algaePoints\": 40, \"netAlgaeCount\": 4, \"wallAlgaeCount\": 4, \"endGameBargePoints\": 4, \"autoBonusAchieved\": false, \"coralBonusAchieved\": false, \"bargeBonusAchieved\": false, \"coopertitionCriteriaMet\": false, \"foulCount\": 0, \"techFoulCount\": 0, \"g206Penalty\": false, \"g410Penalty\": false, \"g418Penalty\": false, \"g428Penalty\": false, \"adjustPoints\": 0, \"foulPoints\": 0, \"rp\": 0, \"totalPoints\": 73}}, \"videos\": [{\"type\": \"youtube\", \"key\": \"ezKYM6QWzWI\"}], \"time\": 1742749200, \"actual_time\": 1742749972, \"predicted_time\": 1742748738, \"post_result_time\": 1742750178}}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.match_video,
            MessageData = jsonElement
        };

        var services = new Mock<IServiceProvider>();
        var matchApi = new Mock<IMatchApi>();
        services.Setup(s => s.GetService(typeof(IMatchApi))).Returns(matchApi.Object);

        // Act
        var result = webhookMessage.GetThreadDetails(services.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FNC District Mecklenburg County Event | Elims 1.1", result.Value.Title);
    }

    [Fact]
    public void GetThreadDetails_ShouldReturnCorrectDetails_ForMatchScore()
    {
        // Arrange
        var jsonString = "{\"event_key\":\"2025hiho\",\"match_key\":\"2025hiho_sf8m1\",\"event_name\":\"Hawaii Regional\",\"match\":{\"key\":\"2025hiho_sf8m1\",\"event_key\":\"2025hiho\",\"comp_level\":\"sf\",\"set_number\":8,\"match_number\":1,\"alliances\":{\"red\":{\"team_keys\":[\"frc4253\",\"frc3501\",\"frc10384\"],\"score\":117,\"surrogate_team_keys\":[],\"dq_team_keys\":[]},\"blue\":{\"team_keys\":[\"frc2438\",\"frc2477\",\"frc3882\"],\"score\":135,\"surrogate_team_keys\":[],\"dq_team_keys\":[]}},\"winning_alliance\":\"blue\",\"score_breakdown\":{\"red\":{\"autoLineRobot1\":\"Yes\",\"endGameRobot1\":\"DeepCage\",\"autoLineRobot2\":\"Yes\",\"endGameRobot2\":\"None\",\"autoLineRobot3\":\"Yes\",\"endGameRobot3\":\"Parked\",\"autoReef\":{\"topRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":true,\"nodeE\":true,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":true,\"nodeK\":false,\"nodeL\":false},\"midRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":0,\"tba_botRowCount\":0,\"tba_midRowCount\":0,\"tba_topRowCount\":3},\"autoCoralCount\":3,\"autoMobilityPoints\":9,\"autoPoints\":30,\"autoCoralPoints\":21,\"teleopReef\":{\"topRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":true,\"nodeF\":true,\"nodeG\":true,\"nodeH\":true,\"nodeI\":true,\"nodeJ\":true,\"nodeK\":true,\"nodeL\":true},\"midRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":0,\"tba_botRowCount\":0,\"tba_midRowCount\":7,\"tba_topRowCount\":12},\"teleopCoralCount\":16,\"teleopPoints\":87,\"teleopCoralPoints\":73,\"algaePoints\":0,\"netAlgaeCount\":0,\"wallAlgaeCount\":0,\"endGameBargePoints\":14,\"autoBonusAchieved\":false,\"coralBonusAchieved\":false,\"bargeBonusAchieved\":false,\"coopertitionCriteriaMet\":false,\"foulCount\":0,\"techFoulCount\":1,\"g206Penalty\":false,\"g410Penalty\":false,\"g418Penalty\":false,\"g428Penalty\":false,\"adjustPoints\":0,\"foulPoints\":0,\"rp\":0,\"totalPoints\":117},\"blue\":{\"autoLineRobot1\":\"Yes\",\"endGameRobot1\":\"None\",\"autoLineRobot2\":\"Yes\",\"endGameRobot2\":\"Parked\",\"autoLineRobot3\":\"Yes\",\"endGameRobot3\":\"DeepCage\",\"autoReef\":{\"topRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":true,\"nodeF\":false,\"nodeG\":false,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"midRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":1,\"tba_botRowCount\":0,\"tba_midRowCount\":0,\"tba_topRowCount\":2},\"autoCoralCount\":3,\"autoMobilityPoints\":9,\"autoPoints\":26,\"autoCoralPoints\":17,\"teleopReef\":{\"topRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":true,\"nodeF\":false,\"nodeG\":true,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":true,\"nodeK\":true,\"nodeL\":true},\"midRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":false,\"nodeF\":false,\"nodeG\":true,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"botRow\":{\"nodeA\":true,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"trough\":4,\"tba_botRowCount\":3,\"tba_midRowCount\":8,\"tba_topRowCount\":10},\"teleopCoralCount\":23,\"teleopPoints\":103,\"teleopCoralPoints\":89,\"algaePoints\":0,\"netAlgaeCount\":0,\"wallAlgaeCount\":0,\"endGameBargePoints\":14,\"autoBonusAchieved\":false,\"coralBonusAchieved\":false,\"bargeBonusAchieved\":false,\"coopertitionCriteriaMet\":false,\"foulCount\":0,\"techFoulCount\":0,\"g206Penalty\":false,\"g410Penalty\":false,\"g418Penalty\":false,\"g428Penalty\":false,\"adjustPoints\":0,\"foulPoints\":6,\"rp\":0,\"totalPoints\":135}},\"videos\":[],\"time\":1742776380,\"actual_time\":1742776993,\"predicted_time\":1742777045,\"post_result_time\":1742777192}}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.match_score,
            MessageData = jsonElement
        };

        var services = new Mock<IServiceProvider>();

        // Act
        var result = webhookMessage.GetThreadDetails(services.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hawaii Regional | Elims 8.1", result.Value.Title);
    }

    [Fact]
    public void GetThreadDetails_ShouldReturnCorrectDetails_ForUpcomingMatch()
    {
        // Arrange  
        var jsonString = "{\"event_key\":\"2025iscmp\",\"match_key\":\"2025iscmp_qm41\",\"event_name\":\"FIRST Israel District Championship\",\"team_keys\":[\"frc4586\",\"frc5951\",\"frc8175\",\"frc2630\",\"frc9740\",\"frc1937\"],\"scheduled_time\":1742998140,\"predicted_time\":1742998930,\"webcast\":{\"type\":\"twitch\",\"channel\":\"firstisrael\",\"status\":\"online\",\"stream_title\":\"🎗️ 2025 FIRST Israel District Championship | FIRST Robotics Competition\",\"viewer_count\":558}}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.upcoming_match,
            MessageData = jsonElement
        };

        var matchJsonString = "{\"key\":\"2025iscmp_qm41\",\"event_key\":\"2025iscmp\",\"comp_level\":\"qm\",\"set_number\":1,\"match_number\":41,\"alliances\":{\"red\":{\"team_keys\":[\"frc2630\",\"frc9740\",\"frc1937\"],\"score\":114,\"surrogate_team_keys\":[],\"dq_team_keys\":[]},\"blue\":{\"team_keys\":[\"frc4586\",\"frc5951\",\"frc8175\"],\"score\":123,\"surrogate_team_keys\":[],\"dq_team_keys\":[]}},\"winning_alliance\":\"blue\",\"score_breakdown\":null,\"videos\":[],\"time\":1742998140,\"actual_time\":1742998743,\"predicted_time\":1742998991,\"post_result_time\":1742999056}\r\n";
        var match = JsonSerializer.Deserialize<Match>(matchJsonString);

        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatch("2025iscmp_qm41", null)).Returns(match);

        // Act  
        var result = webhookMessage.GetThreadDetails(this.Mocker);

        // Assert  
        Assert.NotNull(result);
        Assert.Equal("FIRST Israel District Championship | Quals 1.41", result.Value.Title);
    }

    [Fact]
    public void GetThreadDetails_ShouldReturnNull_ForUnsupportedType()
    {
        // Arrange
        var jsonString = "{}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.awards_posted,
            MessageData = jsonElement
        };

        var services = new Mock<IServiceProvider>();

        // Act
        var result = webhookMessage.GetThreadDetails(services.Object);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetThreadDetails_ShouldThrowDebugAssertException_ForBadData_MatchVideo()
    {
        // Arrange
        var jsonString = "{\"event_key\": \"2025ncmec\", \"match_key\": \"2025ncmec_sf1m1\", \"event_name\": \"\", \"match\": {\"key\": \"2025ncmec_sf1m1\", \"event_key\": \"2025ncmec\", \"comp_level\": \"sf\", \"set_number\": 1, \"match_number\": 1, \"alliances\": {\"red\": {\"team_keys\": [\"frc9496\", \"frc9198\", \"frc8738\"], \"score\": 203, \"surrogate_team_keys\": [], \"dq_team_keys\": []}, \"blue\": {\"team_keys\": [\"frc6932\", \"frc10107\", \"frc7890\"], \"score\": 73, \"surrogate_team_keys\": [], \"dq_team_keys\": []}}, \"winning_alliance\": \"red\", \"score_breakdown\": {\"red\": {\"autoLineRobot1\": \"Yes\", \"endGameRobot1\": \"Parked\", \"autoLineRobot2\": \"Yes\", \"endGameRobot2\": \"Parked\", \"autoLineRobot3\": \"Yes\", \"endGameRobot3\": \"Parked\", \"autoReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": true, \"nodeK\": false, \"nodeL\": true}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 1, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 2}, \"autoCoralCount\": 3, \"autoMobilityPoints\": 9, \"autoPoints\": 26, \"autoCoralPoints\": 17, \"teleopReef\": {\"topRow\": {\"nodeA\": true, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": true, \"nodeF\": true, \"nodeG\": true, \"nodeH\": true, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"midRow\": {\"nodeA\": true, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": true, \"nodeF\": true, \"nodeG\": true, \"nodeH\": true, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"botRow\": {\"nodeA\": false, \"nodeB\": true, \"nodeC\": true, \"nodeD\": true, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": true, \"nodeJ\": true, \"nodeK\": true, \"nodeL\": true}, \"trough\": 1, \"tba_botRowCount\": 7, \"tba_midRowCount\": 12, \"tba_topRowCount\": 12}, \"teleopCoralCount\": 30, \"teleopPoints\": 177, \"teleopCoralPoints\": 121, \"algaePoints\": 50, \"netAlgaeCount\": 8, \"wallAlgaeCount\": 3, \"endGameBargePoints\": 6, \"autoBonusAchieved\": false, \"coralBonusAchieved\": false, \"bargeBonusAchieved\": false, \"coopertitionCriteriaMet\": false, \"foulCount\": 0, \"techFoulCount\": 0, \"g206Penalty\": false, \"g410Penalty\": false, \"g418Penalty\": false, \"g428Penalty\": false, \"adjustPoints\": 0, \"foulPoints\": 0, \"rp\": 0, \"totalPoints\": 203}, \"blue\": {\"autoLineRobot1\": \"Yes\", \"endGameRobot1\": \"Parked\", \"autoLineRobot2\": \"Yes\", \"endGameRobot2\": \"Parked\", \"autoLineRobot3\": \"Yes\", \"endGameRobot3\": \"None\", \"autoReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 2, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 0}, \"autoCoralCount\": 2, \"autoMobilityPoints\": 9, \"autoPoints\": 15, \"autoCoralPoints\": 6, \"teleopReef\": {\"topRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"midRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"botRow\": {\"nodeA\": false, \"nodeB\": false, \"nodeC\": false, \"nodeD\": false, \"nodeE\": false, \"nodeF\": false, \"nodeG\": false, \"nodeH\": false, \"nodeI\": false, \"nodeJ\": false, \"nodeK\": false, \"nodeL\": false}, \"trough\": 7, \"tba_botRowCount\": 0, \"tba_midRowCount\": 0, \"tba_topRowCount\": 0}, \"teleopCoralCount\": 7, \"teleopPoints\": 58, \"teleopCoralPoints\": 14, \"algaePoints\": 40, \"netAlgaeCount\": 4, \"wallAlgaeCount\": 4, \"endGameBargePoints\": 4, \"autoBonusAchieved\": false, \"coralBonusAchieved\": false, \"bargeBonusAchieved\": false, \"coopertitionCriteriaMet\": false, \"foulCount\": 0, \"techFoulCount\": 0, \"g206Penalty\": false, \"g410Penalty\": false, \"g418Penalty\": false, \"g428Penalty\": false, \"adjustPoints\": 0, \"foulPoints\": 0, \"rp\": 0, \"totalPoints\": 73}}, \"videos\": [{\"type\": \"youtube\", \"key\": \"ezKYM6QWzWI\"}], \"time\": 1742749200, \"actual_time\": 1742749972, \"predicted_time\": 1742748738, \"post_result_time\": 1742750178}}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.match_video,
            MessageData = jsonElement
        };

        // Act & Assert
        var threadDetails = DebugHelper.AssertDebugException(() => webhookMessage.GetThreadDetails(this.Mocker), "Bad data!");
        Assert.True(threadDetails.HasValue);
        Assert.Equal(" | Elims 1.1", threadDetails.Value.Title);
    }

    [Fact]
    public void GetThreadDetails_ShouldThrowDebugAssertException_ForBadData_MatchScore()
    {
        // Arrange
        var jsonString = "{\"event_key\":\"2025hiho\",\"match_key\":\"2025hiho_sf8m1\",\"event_name\":null,\"match\":{\"key\":\"2025hiho_sf8m1\",\"event_key\":\"2025hiho\",\"comp_level\":\"sf\",\"set_number\":8,\"match_number\":1,\"alliances\":{\"red\":{\"team_keys\":[\"frc4253\",\"frc3501\",\"frc10384\"],\"score\":117,\"surrogate_team_keys\":[],\"dq_team_keys\":[]},\"blue\":{\"team_keys\":[\"frc2438\",\"frc2477\",\"frc3882\"],\"score\":135,\"surrogate_team_keys\":[],\"dq_team_keys\":[]}},\"winning_alliance\":\"blue\",\"score_breakdown\":{\"red\":{\"autoLineRobot1\":\"Yes\",\"endGameRobot1\":\"DeepCage\",\"autoLineRobot2\":\"Yes\",\"endGameRobot2\":\"None\",\"autoLineRobot3\":\"Yes\",\"endGameRobot3\":\"Parked\",\"autoReef\":{\"topRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":true,\"nodeE\":true,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":true,\"nodeK\":false,\"nodeL\":false},\"midRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":0,\"tba_botRowCount\":0,\"tba_midRowCount\":0,\"tba_topRowCount\":3},\"autoCoralCount\":3,\"autoMobilityPoints\":9,\"autoPoints\":30,\"autoCoralPoints\":21,\"teleopReef\":{\"topRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":true,\"nodeF\":true,\"nodeG\":true,\"nodeH\":true,\"nodeI\":true,\"nodeJ\":true,\"nodeK\":true,\"nodeL\":true},\"midRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":0,\"tba_botRowCount\":0,\"tba_midRowCount\":7,\"tba_topRowCount\":12},\"teleopCoralCount\":16,\"teleopPoints\":87,\"teleopCoralPoints\":73,\"algaePoints\":0,\"netAlgaeCount\":0,\"wallAlgaeCount\":0,\"endGameBargePoints\":14,\"autoBonusAchieved\":false,\"coralBonusAchieved\":false,\"bargeBonusAchieved\":false,\"coopertitionCriteriaMet\":false,\"foulCount\":0,\"techFoulCount\":1,\"g206Penalty\":false,\"g410Penalty\":false,\"g418Penalty\":false,\"g428Penalty\":false,\"adjustPoints\":0,\"foulPoints\":0,\"rp\":0,\"totalPoints\":117},\"blue\":{\"autoLineRobot1\":\"Yes\",\"endGameRobot1\":\"None\",\"autoLineRobot2\":\"Yes\",\"endGameRobot2\":\"Parked\",\"autoLineRobot3\":\"Yes\",\"endGameRobot3\":\"DeepCage\",\"autoReef\":{\"topRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":true,\"nodeF\":false,\"nodeG\":false,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"midRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"botRow\":{\"nodeA\":false,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":false,\"nodeL\":false},\"trough\":1,\"tba_botRowCount\":0,\"tba_midRowCount\":0,\"tba_topRowCount\":2},\"autoCoralCount\":3,\"autoMobilityPoints\":9,\"autoPoints\":26,\"autoCoralPoints\":17,\"teleopReef\":{\"topRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":true,\"nodeF\":false,\"nodeG\":true,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":true,\"nodeK\":true,\"nodeL\":true},\"midRow\":{\"nodeA\":true,\"nodeB\":true,\"nodeC\":true,\"nodeD\":true,\"nodeE\":false,\"nodeF\":false,\"nodeG\":true,\"nodeH\":true,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"botRow\":{\"nodeA\":true,\"nodeB\":false,\"nodeC\":false,\"nodeD\":false,\"nodeE\":false,\"nodeF\":false,\"nodeG\":false,\"nodeH\":false,\"nodeI\":false,\"nodeJ\":false,\"nodeK\":true,\"nodeL\":true},\"trough\":4,\"tba_botRowCount\":3,\"tba_midRowCount\":8,\"tba_topRowCount\":10},\"teleopCoralCount\":23,\"teleopPoints\":103,\"teleopCoralPoints\":89,\"algaePoints\":0,\"netAlgaeCount\":0,\"wallAlgaeCount\":0,\"endGameBargePoints\":14,\"autoBonusAchieved\":false,\"coralBonusAchieved\":false,\"bargeBonusAchieved\":false,\"coopertitionCriteriaMet\":false,\"foulCount\":0,\"techFoulCount\":0,\"g206Penalty\":false,\"g410Penalty\":false,\"g418Penalty\":false,\"g428Penalty\":false,\"adjustPoints\":0,\"foulPoints\":6,\"rp\":0,\"totalPoints\":135}},\"videos\":[],\"time\":1742776380,\"actual_time\":1742776993,\"predicted_time\":1742777045,\"post_result_time\":1742777192}}";

        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.match_score,
            MessageData = jsonElement
        };

        var services = new Mock<IServiceProvider>();

        // Act & Assert
        DebugHelper.AssertDebugException(() => webhookMessage.GetThreadDetails(services.Object), "Bad data!");
    }

    [Fact]
    public void GetThreadDetails_ShouldThrowDebugAssertException_ForBadData_UpcomingMatch()
    {
        // Arrange
        var jsonString = "{\"event_key\":\"2025iscmp\",\"match_key\":\"2025iscmp_qm41\",\"event_name\":\"\",\"team_keys\":[\"frc4586\",\"frc5951\",\"frc8175\",\"frc2630\",\"frc9740\",\"frc1937\"],\"scheduled_time\":1742998140,\"predicted_time\":1742998930,\"webcast\":{\"type\":\"twitch\",\"channel\":\"firstisrael\",\"status\":\"online\",\"stream_title\":\"🎗️ 2025 FIRST Israel District Championship | FIRST Robotics Competition\",\"viewer_count\":558}}";
        var jsonElement = JsonDocument.Parse(jsonString).RootElement;
        var webhookMessage = new WebhookMessage
        {
            MessageType = NotificationType.upcoming_match,
            MessageData = jsonElement
        };

        var matchJsonString = "{\"key\":\"2025iscmp_qm41\",\"event_key\":\"2025iscmp\",\"comp_level\":\"qm\",\"set_number\":1,\"match_number\":41,\"alliances\":{\"red\":{\"team_keys\":[\"frc2630\",\"frc9740\",\"frc1937\"],\"score\":114,\"surrogate_team_keys\":[],\"dq_team_keys\":[]},\"blue\":{\"team_keys\":[\"frc4586\",\"frc5951\",\"frc8175\"],\"score\":123,\"surrogate_team_keys\":[],\"dq_team_keys\":[]}},\"winning_alliance\":\"blue\",\"score_breakdown\":null,\"videos\":[],\"time\":1742998140,\"actual_time\":1742998743,\"predicted_time\":1742998991,\"post_result_time\":1742999056}";
        var match = JsonSerializer.Deserialize<Match>(matchJsonString);

        this.Mocker.GetMock<IMatchApi>().Setup(api => api.GetMatch("2025iscmp_qm41", It.IsAny<string>())).Returns(match);

        // Act & Assert
        var thread = DebugHelper.AssertDebugException(() => webhookMessage.GetThreadDetails(this.Mocker), "Bad data!");
        Assert.True(thread.HasValue);
        Assert.Equal(" | Quals 1.41", thread.Value.Title);
    }
}