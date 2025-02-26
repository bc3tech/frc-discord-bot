/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// MatchScoreBreakdown2019Alliance
/// </summary>

  public partial record MatchScoreBreakdown2019Alliance
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2019Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="bay1">bay1.</param>
            /// <param name="bay2">bay2.</param>
            /// <param name="bay3">bay3.</param>
            /// <param name="bay4">bay4.</param>
            /// <param name="bay5">bay5.</param>
            /// <param name="bay6">bay6.</param>
            /// <param name="bay7">bay7.</param>
            /// <param name="bay8">bay8.</param>
            /// <param name="cargoPoints">cargoPoints.</param>
            /// <param name="completeRocketRankingPoint">completeRocketRankingPoint.</param>
            /// <param name="completedRocketFar">completedRocketFar.</param>
            /// <param name="completedRocketNear">completedRocketNear.</param>
            /// <param name="endgameRobot1">endgameRobot1.</param>
            /// <param name="endgameRobot2">endgameRobot2.</param>
            /// <param name="endgameRobot3">endgameRobot3.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="habClimbPoints">habClimbPoints.</param>
            /// <param name="habDockingRankingPoint">habDockingRankingPoint.</param>
            /// <param name="habLineRobot1">habLineRobot1.</param>
            /// <param name="habLineRobot2">habLineRobot2.</param>
            /// <param name="habLineRobot3">habLineRobot3.</param>
            /// <param name="hatchPanelPoints">hatchPanelPoints.</param>
            /// <param name="lowLeftRocketFar">lowLeftRocketFar.</param>
            /// <param name="lowLeftRocketNear">lowLeftRocketNear.</param>
            /// <param name="lowRightRocketFar">lowRightRocketFar.</param>
            /// <param name="lowRightRocketNear">lowRightRocketNear.</param>
            /// <param name="midLeftRocketFar">midLeftRocketFar.</param>
            /// <param name="midLeftRocketNear">midLeftRocketNear.</param>
            /// <param name="midRightRocketFar">midRightRocketFar.</param>
            /// <param name="midRightRocketNear">midRightRocketNear.</param>
            /// <param name="preMatchBay1">preMatchBay1.</param>
            /// <param name="preMatchBay2">preMatchBay2.</param>
            /// <param name="preMatchBay3">preMatchBay3.</param>
            /// <param name="preMatchBay6">preMatchBay6.</param>
            /// <param name="preMatchBay7">preMatchBay7.</param>
            /// <param name="preMatchBay8">preMatchBay8.</param>
            /// <param name="preMatchLevelRobot1">preMatchLevelRobot1.</param>
            /// <param name="preMatchLevelRobot2">preMatchLevelRobot2.</param>
            /// <param name="preMatchLevelRobot3">preMatchLevelRobot3.</param>
            /// <param name="rp">rp.</param>
            /// <param name="sandStormBonusPoints">sandStormBonusPoints.</param>
            /// <param name="techFoulCount">techFoulCount.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="topLeftRocketFar">topLeftRocketFar.</param>
            /// <param name="topLeftRocketNear">topLeftRocketNear.</param>
            /// <param name="topRightRocketFar">topRightRocketFar.</param>
            /// <param name="topRightRocketNear">topRightRocketNear.</param>
            /// <param name="totalPoints">totalPoints.</param>
        public MatchScoreBreakdown2019Alliance(int? adjustPoints = default, int? autoPoints = default, string? bay1 = default, string? bay2 = default, string? bay3 = default, string? bay4 = default, string? bay5 = default, string? bay6 = default, string? bay7 = default, string? bay8 = default, int? cargoPoints = default, bool? completeRocketRankingPoint = default, bool? completedRocketFar = default, bool? completedRocketNear = default, string? endgameRobot1 = default, string? endgameRobot2 = default, string? endgameRobot3 = default, int? foulCount = default, int? foulPoints = default, int? habClimbPoints = default, bool? habDockingRankingPoint = default, string? habLineRobot1 = default, string? habLineRobot2 = default, string? habLineRobot3 = default, int? hatchPanelPoints = default, string? lowLeftRocketFar = default, string? lowLeftRocketNear = default, string? lowRightRocketFar = default, string? lowRightRocketNear = default, string? midLeftRocketFar = default, string? midLeftRocketNear = default, string? midRightRocketFar = default, string? midRightRocketNear = default, string? preMatchBay1 = default, string? preMatchBay2 = default, string? preMatchBay3 = default, string? preMatchBay6 = default, string? preMatchBay7 = default, string? preMatchBay8 = default, string? preMatchLevelRobot1 = default, string? preMatchLevelRobot2 = default, string? preMatchLevelRobot3 = default, int? rp = default, int? sandStormBonusPoints = default, int? techFoulCount = default, int? teleopPoints = default, string? topLeftRocketFar = default, string? topLeftRocketNear = default, string? topRightRocketFar = default, string? topRightRocketNear = default, int? totalPoints = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoPoints = autoPoints;
                      this.Bay1 = bay1;
                      this.Bay2 = bay2;
                      this.Bay3 = bay3;
                      this.Bay4 = bay4;
                      this.Bay5 = bay5;
                      this.Bay6 = bay6;
                      this.Bay7 = bay7;
                      this.Bay8 = bay8;
                      this.CargoPoints = cargoPoints;
                      this.CompleteRocketRankingPoint = completeRocketRankingPoint;
                      this.CompletedRocketFar = completedRocketFar;
                      this.CompletedRocketNear = completedRocketNear;
                      this.EndgameRobot1 = endgameRobot1;
                      this.EndgameRobot2 = endgameRobot2;
                      this.EndgameRobot3 = endgameRobot3;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.HabClimbPoints = habClimbPoints;
                      this.HabDockingRankingPoint = habDockingRankingPoint;
                      this.HabLineRobot1 = habLineRobot1;
                      this.HabLineRobot2 = habLineRobot2;
                      this.HabLineRobot3 = habLineRobot3;
                      this.HatchPanelPoints = hatchPanelPoints;
                      this.LowLeftRocketFar = lowLeftRocketFar;
                      this.LowLeftRocketNear = lowLeftRocketNear;
                      this.LowRightRocketFar = lowRightRocketFar;
                      this.LowRightRocketNear = lowRightRocketNear;
                      this.MidLeftRocketFar = midLeftRocketFar;
                      this.MidLeftRocketNear = midLeftRocketNear;
                      this.MidRightRocketFar = midRightRocketFar;
                      this.MidRightRocketNear = midRightRocketNear;
                      this.PreMatchBay1 = preMatchBay1;
                      this.PreMatchBay2 = preMatchBay2;
                      this.PreMatchBay3 = preMatchBay3;
                      this.PreMatchBay6 = preMatchBay6;
                      this.PreMatchBay7 = preMatchBay7;
                      this.PreMatchBay8 = preMatchBay8;
                      this.PreMatchLevelRobot1 = preMatchLevelRobot1;
                      this.PreMatchLevelRobot2 = preMatchLevelRobot2;
                      this.PreMatchLevelRobot3 = preMatchLevelRobot3;
                      this.Rp = rp;
                      this.SandStormBonusPoints = sandStormBonusPoints;
                      this.TechFoulCount = techFoulCount;
                      this.TeleopPoints = teleopPoints;
                      this.TopLeftRocketFar = topLeftRocketFar;
                      this.TopLeftRocketNear = topLeftRocketNear;
                      this.TopRightRocketFar = topRightRocketFar;
                      this.TopRightRocketNear = topRightRocketNear;
                      this.TotalPoints = totalPoints;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjustPoints")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("autoPoints")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay1
              /// </summary>
                
                  [JsonPropertyName("bay1")]
                  public string? Bay1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay2
              /// </summary>
                
                  [JsonPropertyName("bay2")]
                  public string? Bay2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay3
              /// </summary>
                
                  [JsonPropertyName("bay3")]
                  public string? Bay3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay4
              /// </summary>
                
                  [JsonPropertyName("bay4")]
                  public string? Bay4 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay5
              /// </summary>
                
                  [JsonPropertyName("bay5")]
                  public string? Bay5 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay6
              /// </summary>
                
                  [JsonPropertyName("bay6")]
                  public string? Bay6 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay7
              /// </summary>
                
                  [JsonPropertyName("bay7")]
                  public string? Bay7 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Bay8
              /// </summary>
                
                  [JsonPropertyName("bay8")]
                  public string? Bay8 { get; set; }
                  
              /// <summary>
              /// Gets or Sets CargoPoints
              /// </summary>
                
                  [JsonPropertyName("cargoPoints")]
                  public int? CargoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets CompleteRocketRankingPoint
              /// </summary>
                
                  [JsonPropertyName("completeRocketRankingPoint")]
                  public bool? CompleteRocketRankingPoint { get; set; }
                  
              /// <summary>
              /// Gets or Sets CompletedRocketFar
              /// </summary>
                
                  [JsonPropertyName("completedRocketFar")]
                  public bool? CompletedRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets CompletedRocketNear
              /// </summary>
                
                  [JsonPropertyName("completedRocketNear")]
                  public bool? CompletedRocketNear { get; set; }
                  
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
              /// Gets or Sets HabClimbPoints
              /// </summary>
                
                  [JsonPropertyName("habClimbPoints")]
                  public int? HabClimbPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets HabDockingRankingPoint
              /// </summary>
                
                  [JsonPropertyName("habDockingRankingPoint")]
                  public bool? HabDockingRankingPoint { get; set; }
                  
              /// <summary>
              /// Gets or Sets HabLineRobot1
              /// </summary>
                
                  [JsonPropertyName("habLineRobot1")]
                  public string? HabLineRobot1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets HabLineRobot2
              /// </summary>
                
                  [JsonPropertyName("habLineRobot2")]
                  public string? HabLineRobot2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets HabLineRobot3
              /// </summary>
                
                  [JsonPropertyName("habLineRobot3")]
                  public string? HabLineRobot3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets HatchPanelPoints
              /// </summary>
                
                  [JsonPropertyName("hatchPanelPoints")]
                  public int? HatchPanelPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets LowLeftRocketFar
              /// </summary>
                
                  [JsonPropertyName("lowLeftRocketFar")]
                  public string? LowLeftRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets LowLeftRocketNear
              /// </summary>
                
                  [JsonPropertyName("lowLeftRocketNear")]
                  public string? LowLeftRocketNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets LowRightRocketFar
              /// </summary>
                
                  [JsonPropertyName("lowRightRocketFar")]
                  public string? LowRightRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets LowRightRocketNear
              /// </summary>
                
                  [JsonPropertyName("lowRightRocketNear")]
                  public string? LowRightRocketNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets MidLeftRocketFar
              /// </summary>
                
                  [JsonPropertyName("midLeftRocketFar")]
                  public string? MidLeftRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets MidLeftRocketNear
              /// </summary>
                
                  [JsonPropertyName("midLeftRocketNear")]
                  public string? MidLeftRocketNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets MidRightRocketFar
              /// </summary>
                
                  [JsonPropertyName("midRightRocketFar")]
                  public string? MidRightRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets MidRightRocketNear
              /// </summary>
                
                  [JsonPropertyName("midRightRocketNear")]
                  public string? MidRightRocketNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay1
              /// </summary>
                
                  [JsonPropertyName("preMatchBay1")]
                  public string? PreMatchBay1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay2
              /// </summary>
                
                  [JsonPropertyName("preMatchBay2")]
                  public string? PreMatchBay2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay3
              /// </summary>
                
                  [JsonPropertyName("preMatchBay3")]
                  public string? PreMatchBay3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay6
              /// </summary>
                
                  [JsonPropertyName("preMatchBay6")]
                  public string? PreMatchBay6 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay7
              /// </summary>
                
                  [JsonPropertyName("preMatchBay7")]
                  public string? PreMatchBay7 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchBay8
              /// </summary>
                
                  [JsonPropertyName("preMatchBay8")]
                  public string? PreMatchBay8 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchLevelRobot1
              /// </summary>
                
                  [JsonPropertyName("preMatchLevelRobot1")]
                  public string? PreMatchLevelRobot1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchLevelRobot2
              /// </summary>
                
                  [JsonPropertyName("preMatchLevelRobot2")]
                  public string? PreMatchLevelRobot2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets PreMatchLevelRobot3
              /// </summary>
                
                  [JsonPropertyName("preMatchLevelRobot3")]
                  public string? PreMatchLevelRobot3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rp
              /// </summary>
                
                  [JsonPropertyName("rp")]
                  public int? Rp { get; set; }
                  
              /// <summary>
              /// Gets or Sets SandStormBonusPoints
              /// </summary>
                
                  [JsonPropertyName("sandStormBonusPoints")]
                  public int? SandStormBonusPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TechFoulCount
              /// </summary>
                
                  [JsonPropertyName("techFoulCount")]
                  public int? TechFoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleopPoints")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TopLeftRocketFar
              /// </summary>
                
                  [JsonPropertyName("topLeftRocketFar")]
                  public string? TopLeftRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets TopLeftRocketNear
              /// </summary>
                
                  [JsonPropertyName("topLeftRocketNear")]
                  public string? TopLeftRocketNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets TopRightRocketFar
              /// </summary>
                
                  [JsonPropertyName("topRightRocketFar")]
                  public string? TopRightRocketFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets TopRightRocketNear
              /// </summary>
                
                  [JsonPropertyName("topRightRocketNear")]
                  public string? TopRightRocketNear { get; set; }
                  
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
            
