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
/// MatchScoreBreakdown2024Alliance
/// </summary>

  public partial class MatchScoreBreakdown2024Alliance
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2024Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoAmpNoteCount">autoAmpNoteCount.</param>
            /// <param name="autoAmpNotePoints">autoAmpNotePoints.</param>
            /// <param name="autoLeavePoints">autoLeavePoints.</param>
            /// <param name="autoLineRobot1">autoLineRobot1.</param>
            /// <param name="autoLineRobot2">autoLineRobot2.</param>
            /// <param name="autoLineRobot3">autoLineRobot3.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="autoSpeakerNoteCount">autoSpeakerNoteCount.</param>
            /// <param name="autoSpeakerNotePoints">autoSpeakerNotePoints.</param>
            /// <param name="autoTotalNotePoints">autoTotalNotePoints.</param>
            /// <param name="coopNotePlayed">coopNotePlayed.</param>
            /// <param name="coopertitionBonusAchieved">coopertitionBonusAchieved.</param>
            /// <param name="coopertitionCriteriaMet">coopertitionCriteriaMet.</param>
            /// <param name="endGameHarmonyPoints">endGameHarmonyPoints.</param>
            /// <param name="endGameNoteInTrapPoints">endGameNoteInTrapPoints.</param>
            /// <param name="endGameOnStagePoints">endGameOnStagePoints.</param>
            /// <param name="endGameParkPoints">endGameParkPoints.</param>
            /// <param name="endGameRobot1">endGameRobot1.</param>
            /// <param name="endGameRobot2">endGameRobot2.</param>
            /// <param name="endGameRobot3">endGameRobot3.</param>
            /// <param name="endGameSpotLightBonusPoints">endGameSpotLightBonusPoints.</param>
            /// <param name="endGameTotalStagePoints">endGameTotalStagePoints.</param>
            /// <param name="ensembleBonusAchieved">ensembleBonusAchieved.</param>
            /// <param name="ensembleBonusOnStageRobotsThreshold">ensembleBonusOnStageRobotsThreshold.</param>
            /// <param name="ensembleBonusStagePointsThreshold">ensembleBonusStagePointsThreshold.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="g206Penalty">g206Penalty.</param>
            /// <param name="g408Penalty">g408Penalty.</param>
            /// <param name="g424Penalty">g424Penalty.</param>
            /// <param name="melodyBonusAchieved">melodyBonusAchieved.</param>
            /// <param name="melodyBonusThreshold">melodyBonusThreshold.</param>
            /// <param name="melodyBonusThresholdCoop">melodyBonusThresholdCoop.</param>
            /// <param name="melodyBonusThresholdNonCoop">melodyBonusThresholdNonCoop.</param>
            /// <param name="micCenterStage">micCenterStage.</param>
            /// <param name="micStageLeft">micStageLeft.</param>
            /// <param name="micStageRight">micStageRight.</param>
            /// <param name="rp">rp.</param>
            /// <param name="techFoulCount">techFoulCount.</param>
            /// <param name="teleopAmpNoteCount">teleopAmpNoteCount.</param>
            /// <param name="teleopAmpNotePoints">teleopAmpNotePoints.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="teleopSpeakerNoteAmplifiedCount">teleopSpeakerNoteAmplifiedCount.</param>
            /// <param name="teleopSpeakerNoteAmplifiedPoints">teleopSpeakerNoteAmplifiedPoints.</param>
            /// <param name="teleopSpeakerNoteCount">teleopSpeakerNoteCount.</param>
            /// <param name="teleopSpeakerNotePoints">teleopSpeakerNotePoints.</param>
            /// <param name="teleopTotalNotePoints">teleopTotalNotePoints.</param>
            /// <param name="totalPoints">totalPoints.</param>
            /// <param name="trapCenterStage">trapCenterStage.</param>
            /// <param name="trapStageLeft">trapStageLeft.</param>
            /// <param name="trapStageRight">trapStageRight.</param>
        public MatchScoreBreakdown2024Alliance(int? adjustPoints = default, int? autoAmpNoteCount = default, int? autoAmpNotePoints = default, int? autoLeavePoints = default, string? autoLineRobot1 = default, string? autoLineRobot2 = default, string? autoLineRobot3 = default, int? autoPoints = default, int? autoSpeakerNoteCount = default, int? autoSpeakerNotePoints = default, int? autoTotalNotePoints = default, bool? coopNotePlayed = default, bool? coopertitionBonusAchieved = default, bool? coopertitionCriteriaMet = default, int? endGameHarmonyPoints = default, int? endGameNoteInTrapPoints = default, int? endGameOnStagePoints = default, int? endGameParkPoints = default, string? endGameRobot1 = default, string? endGameRobot2 = default, string? endGameRobot3 = default, int? endGameSpotLightBonusPoints = default, int? endGameTotalStagePoints = default, bool? ensembleBonusAchieved = default, int? ensembleBonusOnStageRobotsThreshold = default, int? ensembleBonusStagePointsThreshold = default, int? foulCount = default, int? foulPoints = default, bool? g206Penalty = default, bool? g408Penalty = default, bool? g424Penalty = default, bool? melodyBonusAchieved = default, int? melodyBonusThreshold = default, int? melodyBonusThresholdCoop = default, int? melodyBonusThresholdNonCoop = default, bool? micCenterStage = default, bool? micStageLeft = default, bool? micStageRight = default, int? rp = default, int? techFoulCount = default, int? teleopAmpNoteCount = default, int? teleopAmpNotePoints = default, int? teleopPoints = default, int? teleopSpeakerNoteAmplifiedCount = default, int? teleopSpeakerNoteAmplifiedPoints = default, int? teleopSpeakerNoteCount = default, int? teleopSpeakerNotePoints = default, int? teleopTotalNotePoints = default, int? totalPoints = default, bool? trapCenterStage = default, bool? trapStageLeft = default, bool? trapStageRight = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoAmpNoteCount = autoAmpNoteCount;
                      this.AutoAmpNotePoints = autoAmpNotePoints;
                      this.AutoLeavePoints = autoLeavePoints;
                      this.AutoLineRobot1 = autoLineRobot1;
                      this.AutoLineRobot2 = autoLineRobot2;
                      this.AutoLineRobot3 = autoLineRobot3;
                      this.AutoPoints = autoPoints;
                      this.AutoSpeakerNoteCount = autoSpeakerNoteCount;
                      this.AutoSpeakerNotePoints = autoSpeakerNotePoints;
                      this.AutoTotalNotePoints = autoTotalNotePoints;
                      this.CoopNotePlayed = coopNotePlayed;
                      this.CoopertitionBonusAchieved = coopertitionBonusAchieved;
                      this.CoopertitionCriteriaMet = coopertitionCriteriaMet;
                      this.EndGameHarmonyPoints = endGameHarmonyPoints;
                      this.EndGameNoteInTrapPoints = endGameNoteInTrapPoints;
                      this.EndGameOnStagePoints = endGameOnStagePoints;
                      this.EndGameParkPoints = endGameParkPoints;
                      this.EndGameRobot1 = endGameRobot1;
                      this.EndGameRobot2 = endGameRobot2;
                      this.EndGameRobot3 = endGameRobot3;
                      this.EndGameSpotLightBonusPoints = endGameSpotLightBonusPoints;
                      this.EndGameTotalStagePoints = endGameTotalStagePoints;
                      this.EnsembleBonusAchieved = ensembleBonusAchieved;
                      this.EnsembleBonusOnStageRobotsThreshold = ensembleBonusOnStageRobotsThreshold;
                      this.EnsembleBonusStagePointsThreshold = ensembleBonusStagePointsThreshold;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.G206Penalty = g206Penalty;
                      this.G408Penalty = g408Penalty;
                      this.G424Penalty = g424Penalty;
                      this.MelodyBonusAchieved = melodyBonusAchieved;
                      this.MelodyBonusThreshold = melodyBonusThreshold;
                      this.MelodyBonusThresholdCoop = melodyBonusThresholdCoop;
                      this.MelodyBonusThresholdNonCoop = melodyBonusThresholdNonCoop;
                      this.MicCenterStage = micCenterStage;
                      this.MicStageLeft = micStageLeft;
                      this.MicStageRight = micStageRight;
                      this.Rp = rp;
                      this.TechFoulCount = techFoulCount;
                      this.TeleopAmpNoteCount = teleopAmpNoteCount;
                      this.TeleopAmpNotePoints = teleopAmpNotePoints;
                      this.TeleopPoints = teleopPoints;
                      this.TeleopSpeakerNoteAmplifiedCount = teleopSpeakerNoteAmplifiedCount;
                      this.TeleopSpeakerNoteAmplifiedPoints = teleopSpeakerNoteAmplifiedPoints;
                      this.TeleopSpeakerNoteCount = teleopSpeakerNoteCount;
                      this.TeleopSpeakerNotePoints = teleopSpeakerNotePoints;
                      this.TeleopTotalNotePoints = teleopTotalNotePoints;
                      this.TotalPoints = totalPoints;
                      this.TrapCenterStage = trapCenterStage;
                      this.TrapStageLeft = trapStageLeft;
                      this.TrapStageRight = trapStageRight;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjustPoints")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoAmpNoteCount
              /// </summary>
                
                  [JsonPropertyName("autoAmpNoteCount")]
                  public int? AutoAmpNoteCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoAmpNotePoints
              /// </summary>
                
                  [JsonPropertyName("autoAmpNotePoints")]
                  public int? AutoAmpNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoLeavePoints
              /// </summary>
                
                  [JsonPropertyName("autoLeavePoints")]
                  public int? AutoLeavePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoLineRobot1
              /// </summary>
                
                  [JsonPropertyName("autoLineRobot1")]
                  public string? AutoLineRobot1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoLineRobot2
              /// </summary>
                
                  [JsonPropertyName("autoLineRobot2")]
                  public string? AutoLineRobot2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoLineRobot3
              /// </summary>
                
                  [JsonPropertyName("autoLineRobot3")]
                  public string? AutoLineRobot3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("autoPoints")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoSpeakerNoteCount
              /// </summary>
                
                  [JsonPropertyName("autoSpeakerNoteCount")]
                  public int? AutoSpeakerNoteCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoSpeakerNotePoints
              /// </summary>
                
                  [JsonPropertyName("autoSpeakerNotePoints")]
                  public int? AutoSpeakerNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoTotalNotePoints
              /// </summary>
                
                  [JsonPropertyName("autoTotalNotePoints")]
                  public int? AutoTotalNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets CoopNotePlayed
              /// </summary>
                
                  [JsonPropertyName("coopNotePlayed")]
                  public bool? CoopNotePlayed { get; set; }
                  
              /// <summary>
              /// Gets or Sets CoopertitionBonusAchieved
              /// </summary>
                
                  [JsonPropertyName("coopertitionBonusAchieved")]
                  public bool? CoopertitionBonusAchieved { get; set; }
                  
              /// <summary>
              /// Gets or Sets CoopertitionCriteriaMet
              /// </summary>
                
                  [JsonPropertyName("coopertitionCriteriaMet")]
                  public bool? CoopertitionCriteriaMet { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameHarmonyPoints
              /// </summary>
                
                  [JsonPropertyName("endGameHarmonyPoints")]
                  public int? EndGameHarmonyPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameNoteInTrapPoints
              /// </summary>
                
                  [JsonPropertyName("endGameNoteInTrapPoints")]
                  public int? EndGameNoteInTrapPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameOnStagePoints
              /// </summary>
                
                  [JsonPropertyName("endGameOnStagePoints")]
                  public int? EndGameOnStagePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameParkPoints
              /// </summary>
                
                  [JsonPropertyName("endGameParkPoints")]
                  public int? EndGameParkPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameRobot1
              /// </summary>
                
                  [JsonPropertyName("endGameRobot1")]
                  public string? EndGameRobot1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameRobot2
              /// </summary>
                
                  [JsonPropertyName("endGameRobot2")]
                  public string? EndGameRobot2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameRobot3
              /// </summary>
                
                  [JsonPropertyName("endGameRobot3")]
                  public string? EndGameRobot3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameSpotLightBonusPoints
              /// </summary>
                
                  [JsonPropertyName("endGameSpotLightBonusPoints")]
                  public int? EndGameSpotLightBonusPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EndGameTotalStagePoints
              /// </summary>
                
                  [JsonPropertyName("endGameTotalStagePoints")]
                  public int? EndGameTotalStagePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets EnsembleBonusAchieved
              /// </summary>
                
                  [JsonPropertyName("ensembleBonusAchieved")]
                  public bool? EnsembleBonusAchieved { get; set; }
                  
              /// <summary>
              /// Gets or Sets EnsembleBonusOnStageRobotsThreshold
              /// </summary>
                
                  [JsonPropertyName("ensembleBonusOnStageRobotsThreshold")]
                  public int? EnsembleBonusOnStageRobotsThreshold { get; set; }
                  
              /// <summary>
              /// Gets or Sets EnsembleBonusStagePointsThreshold
              /// </summary>
                
                  [JsonPropertyName("ensembleBonusStagePointsThreshold")]
                  public int? EnsembleBonusStagePointsThreshold { get; set; }
                  
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
              /// Gets or Sets G206Penalty
              /// </summary>
                
                  [JsonPropertyName("g206Penalty")]
                  public bool? G206Penalty { get; set; }
                  
              /// <summary>
              /// Gets or Sets G408Penalty
              /// </summary>
                
                  [JsonPropertyName("g408Penalty")]
                  public bool? G408Penalty { get; set; }
                  
              /// <summary>
              /// Gets or Sets G424Penalty
              /// </summary>
                
                  [JsonPropertyName("g424Penalty")]
                  public bool? G424Penalty { get; set; }
                  
              /// <summary>
              /// Gets or Sets MelodyBonusAchieved
              /// </summary>
                
                  [JsonPropertyName("melodyBonusAchieved")]
                  public bool? MelodyBonusAchieved { get; set; }
                  
              /// <summary>
              /// Gets or Sets MelodyBonusThreshold
              /// </summary>
                
                  [JsonPropertyName("melodyBonusThreshold")]
                  public int? MelodyBonusThreshold { get; set; }
                  
              /// <summary>
              /// Gets or Sets MelodyBonusThresholdCoop
              /// </summary>
                
                  [JsonPropertyName("melodyBonusThresholdCoop")]
                  public int? MelodyBonusThresholdCoop { get; set; }
                  
              /// <summary>
              /// Gets or Sets MelodyBonusThresholdNonCoop
              /// </summary>
                
                  [JsonPropertyName("melodyBonusThresholdNonCoop")]
                  public int? MelodyBonusThresholdNonCoop { get; set; }
                  
              /// <summary>
              /// Gets or Sets MicCenterStage
              /// </summary>
                
                  [JsonPropertyName("micCenterStage")]
                  public bool? MicCenterStage { get; set; }
                  
              /// <summary>
              /// Gets or Sets MicStageLeft
              /// </summary>
                
                  [JsonPropertyName("micStageLeft")]
                  public bool? MicStageLeft { get; set; }
                  
              /// <summary>
              /// Gets or Sets MicStageRight
              /// </summary>
                
                  [JsonPropertyName("micStageRight")]
                  public bool? MicStageRight { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rp
              /// </summary>
                
                  [JsonPropertyName("rp")]
                  public int? Rp { get; set; }
                  
              /// <summary>
              /// Gets or Sets TechFoulCount
              /// </summary>
                
                  [JsonPropertyName("techFoulCount")]
                  public int? TechFoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopAmpNoteCount
              /// </summary>
                
                  [JsonPropertyName("teleopAmpNoteCount")]
                  public int? TeleopAmpNoteCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopAmpNotePoints
              /// </summary>
                
                  [JsonPropertyName("teleopAmpNotePoints")]
                  public int? TeleopAmpNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleopPoints")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopSpeakerNoteAmplifiedCount
              /// </summary>
                
                  [JsonPropertyName("teleopSpeakerNoteAmplifiedCount")]
                  public int? TeleopSpeakerNoteAmplifiedCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopSpeakerNoteAmplifiedPoints
              /// </summary>
                
                  [JsonPropertyName("teleopSpeakerNoteAmplifiedPoints")]
                  public int? TeleopSpeakerNoteAmplifiedPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopSpeakerNoteCount
              /// </summary>
                
                  [JsonPropertyName("teleopSpeakerNoteCount")]
                  public int? TeleopSpeakerNoteCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopSpeakerNotePoints
              /// </summary>
                
                  [JsonPropertyName("teleopSpeakerNotePoints")]
                  public int? TeleopSpeakerNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopTotalNotePoints
              /// </summary>
                
                  [JsonPropertyName("teleopTotalNotePoints")]
                  public int? TeleopTotalNotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotalPoints
              /// </summary>
                
                  [JsonPropertyName("totalPoints")]
                  public int? TotalPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TrapCenterStage
              /// </summary>
                
                  [JsonPropertyName("trapCenterStage")]
                  public bool? TrapCenterStage { get; set; }
                  
              /// <summary>
              /// Gets or Sets TrapStageLeft
              /// </summary>
                
                  [JsonPropertyName("trapStageLeft")]
                  public bool? TrapStageLeft { get; set; }
                  
              /// <summary>
              /// Gets or Sets TrapStageRight
              /// </summary>
                
                  [JsonPropertyName("trapStageRight")]
                  public bool? TrapStageRight { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class MatchScoreBreakdown2024Alliance {");
                    sb.Append("  AdjustPoints: ").AppendLine($"{ AdjustPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoAmpNoteCount: ").AppendLine($"{ AutoAmpNoteCount?.ToString() ?? "[null]" }");
                    sb.Append("  AutoAmpNotePoints: ").AppendLine($"{ AutoAmpNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoLeavePoints: ").AppendLine($"{ AutoLeavePoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoLineRobot1: ").AppendLine($"{ AutoLineRobot1?.ToString() ?? "[null]" }");
                    sb.Append("  AutoLineRobot2: ").AppendLine($"{ AutoLineRobot2?.ToString() ?? "[null]" }");
                    sb.Append("  AutoLineRobot3: ").AppendLine($"{ AutoLineRobot3?.ToString() ?? "[null]" }");
                    sb.Append("  AutoPoints: ").AppendLine($"{ AutoPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoSpeakerNoteCount: ").AppendLine($"{ AutoSpeakerNoteCount?.ToString() ?? "[null]" }");
                    sb.Append("  AutoSpeakerNotePoints: ").AppendLine($"{ AutoSpeakerNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoTotalNotePoints: ").AppendLine($"{ AutoTotalNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  CoopNotePlayed: ").AppendLine($"{ CoopNotePlayed?.ToString() ?? "[null]" }");
                    sb.Append("  CoopertitionBonusAchieved: ").AppendLine($"{ CoopertitionBonusAchieved?.ToString() ?? "[null]" }");
                    sb.Append("  CoopertitionCriteriaMet: ").AppendLine($"{ CoopertitionCriteriaMet?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameHarmonyPoints: ").AppendLine($"{ EndGameHarmonyPoints?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameNoteInTrapPoints: ").AppendLine($"{ EndGameNoteInTrapPoints?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameOnStagePoints: ").AppendLine($"{ EndGameOnStagePoints?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameParkPoints: ").AppendLine($"{ EndGameParkPoints?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameRobot1: ").AppendLine($"{ EndGameRobot1?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameRobot2: ").AppendLine($"{ EndGameRobot2?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameRobot3: ").AppendLine($"{ EndGameRobot3?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameSpotLightBonusPoints: ").AppendLine($"{ EndGameSpotLightBonusPoints?.ToString() ?? "[null]" }");
                    sb.Append("  EndGameTotalStagePoints: ").AppendLine($"{ EndGameTotalStagePoints?.ToString() ?? "[null]" }");
                    sb.Append("  EnsembleBonusAchieved: ").AppendLine($"{ EnsembleBonusAchieved?.ToString() ?? "[null]" }");
                    sb.Append("  EnsembleBonusOnStageRobotsThreshold: ").AppendLine($"{ EnsembleBonusOnStageRobotsThreshold?.ToString() ?? "[null]" }");
                    sb.Append("  EnsembleBonusStagePointsThreshold: ").AppendLine($"{ EnsembleBonusStagePointsThreshold?.ToString() ?? "[null]" }");
                    sb.Append("  FoulCount: ").AppendLine($"{ FoulCount?.ToString() ?? "[null]" }");
                    sb.Append("  FoulPoints: ").AppendLine($"{ FoulPoints?.ToString() ?? "[null]" }");
                    sb.Append("  G206Penalty: ").AppendLine($"{ G206Penalty?.ToString() ?? "[null]" }");
                    sb.Append("  G408Penalty: ").AppendLine($"{ G408Penalty?.ToString() ?? "[null]" }");
                    sb.Append("  G424Penalty: ").AppendLine($"{ G424Penalty?.ToString() ?? "[null]" }");
                    sb.Append("  MelodyBonusAchieved: ").AppendLine($"{ MelodyBonusAchieved?.ToString() ?? "[null]" }");
                    sb.Append("  MelodyBonusThreshold: ").AppendLine($"{ MelodyBonusThreshold?.ToString() ?? "[null]" }");
                    sb.Append("  MelodyBonusThresholdCoop: ").AppendLine($"{ MelodyBonusThresholdCoop?.ToString() ?? "[null]" }");
                    sb.Append("  MelodyBonusThresholdNonCoop: ").AppendLine($"{ MelodyBonusThresholdNonCoop?.ToString() ?? "[null]" }");
                    sb.Append("  MicCenterStage: ").AppendLine($"{ MicCenterStage?.ToString() ?? "[null]" }");
                    sb.Append("  MicStageLeft: ").AppendLine($"{ MicStageLeft?.ToString() ?? "[null]" }");
                    sb.Append("  MicStageRight: ").AppendLine($"{ MicStageRight?.ToString() ?? "[null]" }");
                    sb.Append("  Rp: ").AppendLine($"{ Rp?.ToString() ?? "[null]" }");
                    sb.Append("  TechFoulCount: ").AppendLine($"{ TechFoulCount?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopAmpNoteCount: ").AppendLine($"{ TeleopAmpNoteCount?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopAmpNotePoints: ").AppendLine($"{ TeleopAmpNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopPoints: ").AppendLine($"{ TeleopPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopSpeakerNoteAmplifiedCount: ").AppendLine($"{ TeleopSpeakerNoteAmplifiedCount?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopSpeakerNoteAmplifiedPoints: ").AppendLine($"{ TeleopSpeakerNoteAmplifiedPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopSpeakerNoteCount: ").AppendLine($"{ TeleopSpeakerNoteCount?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopSpeakerNotePoints: ").AppendLine($"{ TeleopSpeakerNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopTotalNotePoints: ").AppendLine($"{ TeleopTotalNotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  TotalPoints: ").AppendLine($"{ TotalPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TrapCenterStage: ").AppendLine($"{ TrapCenterStage?.ToString() ?? "[null]" }");
                    sb.Append("  TrapStageLeft: ").AppendLine($"{ TrapStageLeft?.ToString() ?? "[null]" }");
                    sb.Append("  TrapStageRight: ").AppendLine($"{ TrapStageRight?.ToString() ?? "[null]" }");
                sb.AppendLine("}");
                return sb.ToString();
              }
              
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            
