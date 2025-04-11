/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// MatchScoreBreakdown2018Alliance
/// </summary>

public partial record MatchScoreBreakdown2018Alliance
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2018Alliance" /> class.
    /// </summary>
    /// <param name="adjustPoints">adjustPoints.</param>
    /// <param name="autoOwnershipPoints">autoOwnershipPoints.</param>
    /// <param name="autoPoints">autoPoints.</param>
    /// <param name="autoQuestRankingPoint">autoQuestRankingPoint.</param>
    /// <param name="autoRobot1">autoRobot1.</param>
    /// <param name="autoRobot2">autoRobot2.</param>
    /// <param name="autoRobot3">autoRobot3.</param>
    /// <param name="autoRunPoints">autoRunPoints.</param>
    /// <param name="autoScaleOwnershipSec">autoScaleOwnershipSec.</param>
    /// <param name="autoSwitchAtZero">autoSwitchAtZero.</param>
    /// <param name="autoSwitchOwnershipSec">autoSwitchOwnershipSec.</param>
    /// <param name="endgamePoints">endgamePoints.</param>
    /// <param name="endgameRobot1">endgameRobot1.</param>
    /// <param name="endgameRobot2">endgameRobot2.</param>
    /// <param name="endgameRobot3">endgameRobot3.</param>
    /// <param name="faceTheBossRankingPoint">faceTheBossRankingPoint.</param>
    /// <param name="foulCount">foulCount.</param>
    /// <param name="foulPoints">foulPoints.</param>
    /// <param name="rp">rp.</param>
    /// <param name="tbaGameData">Unofficial TBA-computed value of the FMS provided GameData given to the alliance teams at the start of the match. 3 Character String containing &#x60;L&#x60; and &#x60;R&#x60; only. The first character represents the near switch, the 2nd the scale, and the 3rd the far, opposing, switch from the alliance&#39;s perspective. An &#x60;L&#x60; in a position indicates the platform on the left will be lit for the alliance while an &#x60;R&#x60; will indicate the right platform will be lit for the alliance. See also [WPI Screen Steps](https://wpilib.screenstepslive.com/s/currentCS/m/getting_started/l/826278-2018-game-data-details).</param>
    /// <param name="techFoulCount">techFoulCount.</param>
    /// <param name="teleopOwnershipPoints">teleopOwnershipPoints.</param>
    /// <param name="teleopPoints">teleopPoints.</param>
    /// <param name="teleopScaleBoostSec">teleopScaleBoostSec.</param>
    /// <param name="teleopScaleForceSec">teleopScaleForceSec.</param>
    /// <param name="teleopScaleOwnershipSec">teleopScaleOwnershipSec.</param>
    /// <param name="teleopSwitchBoostSec">teleopSwitchBoostSec.</param>
    /// <param name="teleopSwitchForceSec">teleopSwitchForceSec.</param>
    /// <param name="teleopSwitchOwnershipSec">teleopSwitchOwnershipSec.</param>
    /// <param name="totalPoints">totalPoints.</param>
    /// <param name="vaultBoostPlayed">vaultBoostPlayed.</param>
    /// <param name="vaultBoostTotal">vaultBoostTotal.</param>
    /// <param name="vaultForcePlayed">vaultForcePlayed.</param>
    /// <param name="vaultForceTotal">vaultForceTotal.</param>
    /// <param name="vaultLevitatePlayed">vaultLevitatePlayed.</param>
    /// <param name="vaultLevitateTotal">vaultLevitateTotal.</param>
    /// <param name="vaultPoints">vaultPoints.</param>
    public MatchScoreBreakdown2018Alliance(int? adjustPoints = default, int? autoOwnershipPoints = default, int? autoPoints = default, bool? autoQuestRankingPoint = default, string? autoRobot1 = default, string? autoRobot2 = default, string? autoRobot3 = default, int? autoRunPoints = default, int? autoScaleOwnershipSec = default, bool? autoSwitchAtZero = default, int? autoSwitchOwnershipSec = default, int? endgamePoints = default, string? endgameRobot1 = default, string? endgameRobot2 = default, string? endgameRobot3 = default, bool? faceTheBossRankingPoint = default, int? foulCount = default, int? foulPoints = default, int? rp = default, string? tbaGameData = default, int? techFoulCount = default, int? teleopOwnershipPoints = default, int? teleopPoints = default, int? teleopScaleBoostSec = default, int? teleopScaleForceSec = default, int? teleopScaleOwnershipSec = default, int? teleopSwitchBoostSec = default, int? teleopSwitchForceSec = default, int? teleopSwitchOwnershipSec = default, int? totalPoints = default, int? vaultBoostPlayed = default, int? vaultBoostTotal = default, int? vaultForcePlayed = default, int? vaultForceTotal = default, int? vaultLevitatePlayed = default, int? vaultLevitateTotal = default, int? vaultPoints = default)
    {
        this.AdjustPoints = adjustPoints;
        this.AutoOwnershipPoints = autoOwnershipPoints;
        this.AutoPoints = autoPoints;
        this.AutoQuestRankingPoint = autoQuestRankingPoint;
        this.AutoRobot1 = autoRobot1;
        this.AutoRobot2 = autoRobot2;
        this.AutoRobot3 = autoRobot3;
        this.AutoRunPoints = autoRunPoints;
        this.AutoScaleOwnershipSec = autoScaleOwnershipSec;
        this.AutoSwitchAtZero = autoSwitchAtZero;
        this.AutoSwitchOwnershipSec = autoSwitchOwnershipSec;
        this.EndgamePoints = endgamePoints;
        this.EndgameRobot1 = endgameRobot1;
        this.EndgameRobot2 = endgameRobot2;
        this.EndgameRobot3 = endgameRobot3;
        this.FaceTheBossRankingPoint = faceTheBossRankingPoint;
        this.FoulCount = foulCount;
        this.FoulPoints = foulPoints;
        this.Rp = rp;
        this.TbaGameData = tbaGameData;
        this.TechFoulCount = techFoulCount;
        this.TeleopOwnershipPoints = teleopOwnershipPoints;
        this.TeleopPoints = teleopPoints;
        this.TeleopScaleBoostSec = teleopScaleBoostSec;
        this.TeleopScaleForceSec = teleopScaleForceSec;
        this.TeleopScaleOwnershipSec = teleopScaleOwnershipSec;
        this.TeleopSwitchBoostSec = teleopSwitchBoostSec;
        this.TeleopSwitchForceSec = teleopSwitchForceSec;
        this.TeleopSwitchOwnershipSec = teleopSwitchOwnershipSec;
        this.TotalPoints = totalPoints;
        this.VaultBoostPlayed = vaultBoostPlayed;
        this.VaultBoostTotal = vaultBoostTotal;
        this.VaultForcePlayed = vaultForcePlayed;
        this.VaultForceTotal = vaultForceTotal;
        this.VaultLevitatePlayed = vaultLevitatePlayed;
        this.VaultLevitateTotal = vaultLevitateTotal;
        this.VaultPoints = vaultPoints;
    }

    /// <summary>
    /// Gets or Sets AdjustPoints
    /// </summary>

    [JsonPropertyName("adjustPoints")]
    public int? AdjustPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoOwnershipPoints
    /// </summary>

    [JsonPropertyName("autoOwnershipPoints")]
    public int? AutoOwnershipPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoPoints
    /// </summary>

    [JsonPropertyName("autoPoints")]
    public int? AutoPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoQuestRankingPoint
    /// </summary>

    [JsonPropertyName("autoQuestRankingPoint")]
    public bool? AutoQuestRankingPoint { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot1
    /// </summary>

    [JsonPropertyName("autoRobot1")]
    public string? AutoRobot1 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot2
    /// </summary>

    [JsonPropertyName("autoRobot2")]
    public string? AutoRobot2 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRobot3
    /// </summary>

    [JsonPropertyName("autoRobot3")]
    public string? AutoRobot3 { get; set; }

    /// <summary>
    /// Gets or Sets AutoRunPoints
    /// </summary>

    [JsonPropertyName("autoRunPoints")]
    public int? AutoRunPoints { get; set; }

    /// <summary>
    /// Gets or Sets AutoScaleOwnershipSec
    /// </summary>

    [JsonPropertyName("autoScaleOwnershipSec")]
    public int? AutoScaleOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets AutoSwitchAtZero
    /// </summary>

    [JsonPropertyName("autoSwitchAtZero")]
    public bool? AutoSwitchAtZero { get; set; }

    /// <summary>
    /// Gets or Sets AutoSwitchOwnershipSec
    /// </summary>

    [JsonPropertyName("autoSwitchOwnershipSec")]
    public int? AutoSwitchOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets EndgamePoints
    /// </summary>

    [JsonPropertyName("endgamePoints")]
    public int? EndgamePoints { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot1
    /// </summary>

    [JsonPropertyName("endgameRobot1")]
    public string? EndgameRobot1 { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot2
    /// </summary>

    [JsonPropertyName("endgameRobot2")]
    public string? EndgameRobot2 { get; set; }

    /// <summary>
    /// Gets or Sets EndgameRobot3
    /// </summary>

    [JsonPropertyName("endgameRobot3")]
    public string? EndgameRobot3 { get; set; }

    /// <summary>
    /// Gets or Sets FaceTheBossRankingPoint
    /// </summary>

    [JsonPropertyName("faceTheBossRankingPoint")]
    public bool? FaceTheBossRankingPoint { get; set; }

    /// <summary>
    /// Gets or Sets FoulCount
    /// </summary>

    [JsonPropertyName("foulCount")]
    public int? FoulCount { get; set; }

    /// <summary>
    /// Gets or Sets FoulPoints
    /// </summary>

    [JsonPropertyName("foulPoints")]
    public int? FoulPoints { get; set; }

    /// <summary>
    /// Gets or Sets Rp
    /// </summary>

    [JsonPropertyName("rp")]
    public int? Rp { get; set; }

    /// <summary>
    /// Unofficial TBA-computed value of the FMS provided GameData given to the alliance teams at the start of the match. 3 Character String containing &#x60;L&#x60; and &#x60;R&#x60; only. The first character represents the near switch, the 2nd the scale, and the 3rd the far, opposing, switch from the alliance&#39;s perspective. An &#x60;L&#x60; in a position indicates the platform on the left will be lit for the alliance while an &#x60;R&#x60; will indicate the right platform will be lit for the alliance. See also [WPI Screen Steps](https://wpilib.screenstepslive.com/s/currentCS/m/getting_started/l/826278-2018-game-data-details).
    /// </summary>
    /// <value>Unofficial TBA-computed value of the FMS provided GameData given to the alliance teams at the start of the match. 3 Character String containing &#x60;L&#x60; and &#x60;R&#x60; only. The first character represents the near switch, the 2nd the scale, and the 3rd the far, opposing, switch from the alliance&#39;s perspective. An &#x60;L&#x60; in a position indicates the platform on the left will be lit for the alliance while an &#x60;R&#x60; will indicate the right platform will be lit for the alliance. See also [WPI Screen Steps](https://wpilib.screenstepslive.com/s/currentCS/m/getting_started/l/826278-2018-game-data-details).</value>

    [JsonPropertyName("tba_gameData")]
    public string? TbaGameData { get; set; }

    /// <summary>
    /// Gets or Sets TechFoulCount
    /// </summary>

    [JsonPropertyName("techFoulCount")]
    public int? TechFoulCount { get; set; }

    /// <summary>
    /// Gets or Sets TeleopOwnershipPoints
    /// </summary>

    [JsonPropertyName("teleopOwnershipPoints")]
    public int? TeleopOwnershipPoints { get; set; }

    /// <summary>
    /// Gets or Sets TeleopPoints
    /// </summary>

    [JsonPropertyName("teleopPoints")]
    public int? TeleopPoints { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleBoostSec
    /// </summary>

    [JsonPropertyName("teleopScaleBoostSec")]
    public int? TeleopScaleBoostSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleForceSec
    /// </summary>

    [JsonPropertyName("teleopScaleForceSec")]
    public int? TeleopScaleForceSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopScaleOwnershipSec
    /// </summary>

    [JsonPropertyName("teleopScaleOwnershipSec")]
    public int? TeleopScaleOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchBoostSec
    /// </summary>

    [JsonPropertyName("teleopSwitchBoostSec")]
    public int? TeleopSwitchBoostSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchForceSec
    /// </summary>

    [JsonPropertyName("teleopSwitchForceSec")]
    public int? TeleopSwitchForceSec { get; set; }

    /// <summary>
    /// Gets or Sets TeleopSwitchOwnershipSec
    /// </summary>

    [JsonPropertyName("teleopSwitchOwnershipSec")]
    public int? TeleopSwitchOwnershipSec { get; set; }

    /// <summary>
    /// Gets or Sets TotalPoints
    /// </summary>

    [JsonPropertyName("totalPoints")]
    public int? TotalPoints { get; set; }

    /// <summary>
    /// Gets or Sets VaultBoostPlayed
    /// </summary>

    [JsonPropertyName("vaultBoostPlayed")]
    public int? VaultBoostPlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultBoostTotal
    /// </summary>

    [JsonPropertyName("vaultBoostTotal")]
    public int? VaultBoostTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultForcePlayed
    /// </summary>

    [JsonPropertyName("vaultForcePlayed")]
    public int? VaultForcePlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultForceTotal
    /// </summary>

    [JsonPropertyName("vaultForceTotal")]
    public int? VaultForceTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultLevitatePlayed
    /// </summary>

    [JsonPropertyName("vaultLevitatePlayed")]
    public int? VaultLevitatePlayed { get; set; }

    /// <summary>
    /// Gets or Sets VaultLevitateTotal
    /// </summary>

    [JsonPropertyName("vaultLevitateTotal")]
    public int? VaultLevitateTotal { get; set; }

    /// <summary>
    /// Gets or Sets VaultPoints
    /// </summary>

    [JsonPropertyName("vaultPoints")]
    public int? VaultPoints { get; set; }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}

