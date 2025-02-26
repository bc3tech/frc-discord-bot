/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// MatchScoreBreakdown2020Alliance
/// </summary>

  public partial record MatchScoreBreakdown2020Alliance
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2020Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoCellPoints">autoCellPoints.</param>
            /// <param name="autoCellsBottom">autoCellsBottom.</param>
            /// <param name="autoCellsInner">autoCellsInner.</param>
            /// <param name="autoCellsOuter">autoCellsOuter.</param>
            /// <param name="autoInitLinePoints">autoInitLinePoints.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="controlPanelPoints">controlPanelPoints.</param>
            /// <param name="endgamePoints">endgamePoints.</param>
            /// <param name="endgameRobot1">endgameRobot1.</param>
            /// <param name="endgameRobot2">endgameRobot2.</param>
            /// <param name="endgameRobot3">endgameRobot3.</param>
            /// <param name="endgameRungIsLevel">endgameRungIsLevel.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="initLineRobot1">initLineRobot1.</param>
            /// <param name="initLineRobot2">initLineRobot2.</param>
            /// <param name="initLineRobot3">initLineRobot3.</param>
            /// <param name="rp">rp.</param>
            /// <param name="shieldEnergizedRankingPoint">shieldEnergizedRankingPoint.</param>
            /// <param name="shieldOperationalRankingPoint">shieldOperationalRankingPoint.</param>
            /// <param name="stage1Activated">stage1Activated.</param>
            /// <param name="stage2Activated">stage2Activated.</param>
            /// <param name="stage3Activated">stage3Activated.</param>
            /// <param name="stage3TargetColor">stage3TargetColor.</param>
            /// <param name="tbaNumRobotsHanging">Unofficial TBA-computed value that counts the number of robots who were hanging at the end of the match.</param>
            /// <param name="tbaShieldEnergizedRankingPointFromFoul">Unofficial TBA-computed value that indicates whether the shieldEnergizedRankingPoint was earned normally or awarded due to a foul.</param>
            /// <param name="techFoulCount">techFoulCount.</param>
            /// <param name="teleopCellPoints">teleopCellPoints.</param>
            /// <param name="teleopCellsBottom">teleopCellsBottom.</param>
            /// <param name="teleopCellsInner">teleopCellsInner.</param>
            /// <param name="teleopCellsOuter">teleopCellsOuter.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="totalPoints">totalPoints.</param>
        public MatchScoreBreakdown2020Alliance(int? adjustPoints = default, int? autoCellPoints = default, int? autoCellsBottom = default, int? autoCellsInner = default, int? autoCellsOuter = default, int? autoInitLinePoints = default, int? autoPoints = default, int? controlPanelPoints = default, int? endgamePoints = default, string? endgameRobot1 = default, string? endgameRobot2 = default, string? endgameRobot3 = default, string? endgameRungIsLevel = default, int? foulCount = default, int? foulPoints = default, string? initLineRobot1 = default, string? initLineRobot2 = default, string? initLineRobot3 = default, int? rp = default, bool? shieldEnergizedRankingPoint = default, bool? shieldOperationalRankingPoint = default, bool? stage1Activated = default, bool? stage2Activated = default, bool? stage3Activated = default, string? stage3TargetColor = default, int? tbaNumRobotsHanging = default, bool? tbaShieldEnergizedRankingPointFromFoul = default, int? techFoulCount = default, int? teleopCellPoints = default, int? teleopCellsBottom = default, int? teleopCellsInner = default, int? teleopCellsOuter = default, int? teleopPoints = default, int? totalPoints = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoCellPoints = autoCellPoints;
                      this.AutoCellsBottom = autoCellsBottom;
                      this.AutoCellsInner = autoCellsInner;
                      this.AutoCellsOuter = autoCellsOuter;
                      this.AutoInitLinePoints = autoInitLinePoints;
                      this.AutoPoints = autoPoints;
                      this.ControlPanelPoints = controlPanelPoints;
                      this.EndgamePoints = endgamePoints;
                      this.EndgameRobot1 = endgameRobot1;
                      this.EndgameRobot2 = endgameRobot2;
                      this.EndgameRobot3 = endgameRobot3;
                      this.EndgameRungIsLevel = endgameRungIsLevel;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.InitLineRobot1 = initLineRobot1;
                      this.InitLineRobot2 = initLineRobot2;
                      this.InitLineRobot3 = initLineRobot3;
                      this.Rp = rp;
                      this.ShieldEnergizedRankingPoint = shieldEnergizedRankingPoint;
                      this.ShieldOperationalRankingPoint = shieldOperationalRankingPoint;
                      this.Stage1Activated = stage1Activated;
                      this.Stage2Activated = stage2Activated;
                      this.Stage3Activated = stage3Activated;
                      this.Stage3TargetColor = stage3TargetColor;
                      this.TbaNumRobotsHanging = tbaNumRobotsHanging;
                      this.TbaShieldEnergizedRankingPointFromFoul = tbaShieldEnergizedRankingPointFromFoul;
                      this.TechFoulCount = techFoulCount;
                      this.TeleopCellPoints = teleopCellPoints;
                      this.TeleopCellsBottom = teleopCellsBottom;
                      this.TeleopCellsInner = teleopCellsInner;
                      this.TeleopCellsOuter = teleopCellsOuter;
                      this.TeleopPoints = teleopPoints;
                      this.TotalPoints = totalPoints;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjustPoints")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoCellPoints
              /// </summary>
                
                  [JsonPropertyName("autoCellPoints")]
                  public int? AutoCellPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoCellsBottom
              /// </summary>
                
                  [JsonPropertyName("autoCellsBottom")]
                  public int? AutoCellsBottom { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoCellsInner
              /// </summary>
                
                  [JsonPropertyName("autoCellsInner")]
                  public int? AutoCellsInner { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoCellsOuter
              /// </summary>
                
                  [JsonPropertyName("autoCellsOuter")]
                  public int? AutoCellsOuter { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoInitLinePoints
              /// </summary>
                
                  [JsonPropertyName("autoInitLinePoints")]
                  public int? AutoInitLinePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("autoPoints")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets ControlPanelPoints
              /// </summary>
                
                  [JsonPropertyName("controlPanelPoints")]
                  public int? ControlPanelPoints { get; set; }
                  
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
              /// Gets or Sets EndgameRungIsLevel
              /// </summary>
                
                  [JsonPropertyName("endgameRungIsLevel")]
                  public string? EndgameRungIsLevel { get; set; }
                  
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
              /// Gets or Sets InitLineRobot1
              /// </summary>
                
                  [JsonPropertyName("initLineRobot1")]
                  public string? InitLineRobot1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets InitLineRobot2
              /// </summary>
                
                  [JsonPropertyName("initLineRobot2")]
                  public string? InitLineRobot2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets InitLineRobot3
              /// </summary>
                
                  [JsonPropertyName("initLineRobot3")]
                  public string? InitLineRobot3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rp
              /// </summary>
                
                  [JsonPropertyName("rp")]
                  public int? Rp { get; set; }
                  
              /// <summary>
              /// Gets or Sets ShieldEnergizedRankingPoint
              /// </summary>
                
                  [JsonPropertyName("shieldEnergizedRankingPoint")]
                  public bool? ShieldEnergizedRankingPoint { get; set; }
                  
              /// <summary>
              /// Gets or Sets ShieldOperationalRankingPoint
              /// </summary>
                
                  [JsonPropertyName("shieldOperationalRankingPoint")]
                  public bool? ShieldOperationalRankingPoint { get; set; }
                  
              /// <summary>
              /// Gets or Sets Stage1Activated
              /// </summary>
                
                  [JsonPropertyName("stage1Activated")]
                  public bool? Stage1Activated { get; set; }
                  
              /// <summary>
              /// Gets or Sets Stage2Activated
              /// </summary>
                
                  [JsonPropertyName("stage2Activated")]
                  public bool? Stage2Activated { get; set; }
                  
              /// <summary>
              /// Gets or Sets Stage3Activated
              /// </summary>
                
                  [JsonPropertyName("stage3Activated")]
                  public bool? Stage3Activated { get; set; }
                  
              /// <summary>
              /// Gets or Sets Stage3TargetColor
              /// </summary>
                
                  [JsonPropertyName("stage3TargetColor")]
                  public string? Stage3TargetColor { get; set; }
                  
              /// <summary>
              /// Unofficial TBA-computed value that counts the number of robots who were hanging at the end of the match.
              /// </summary>
              /// <value>Unofficial TBA-computed value that counts the number of robots who were hanging at the end of the match.</value>
                
                  [JsonPropertyName("tba_numRobotsHanging")]
                  public int? TbaNumRobotsHanging { get; set; }
                  
              /// <summary>
              /// Unofficial TBA-computed value that indicates whether the shieldEnergizedRankingPoint was earned normally or awarded due to a foul.
              /// </summary>
              /// <value>Unofficial TBA-computed value that indicates whether the shieldEnergizedRankingPoint was earned normally or awarded due to a foul.</value>
                
                  [JsonPropertyName("tba_shieldEnergizedRankingPointFromFoul")]
                  public bool? TbaShieldEnergizedRankingPointFromFoul { get; set; }
                  
              /// <summary>
              /// Gets or Sets TechFoulCount
              /// </summary>
                
                  [JsonPropertyName("techFoulCount")]
                  public int? TechFoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopCellPoints
              /// </summary>
                
                  [JsonPropertyName("teleopCellPoints")]
                  public int? TeleopCellPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopCellsBottom
              /// </summary>
                
                  [JsonPropertyName("teleopCellsBottom")]
                  public int? TeleopCellsBottom { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopCellsInner
              /// </summary>
                
                  [JsonPropertyName("teleopCellsInner")]
                  public int? TeleopCellsInner { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopCellsOuter
              /// </summary>
                
                  [JsonPropertyName("teleopCellsOuter")]
                  public int? TeleopCellsOuter { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleopPoints")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotalPoints
              /// </summary>
                
                  [JsonPropertyName("totalPoints")]
                  public int? TotalPoints { get; set; }
                  
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            
