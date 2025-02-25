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
/// MatchScoreBreakdown2015Alliance
/// </summary>

  public partial class MatchScoreBreakdown2015Alliance
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2015Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="containerCountLevel1">containerCountLevel1.</param>
            /// <param name="containerCountLevel2">containerCountLevel2.</param>
            /// <param name="containerCountLevel3">containerCountLevel3.</param>
            /// <param name="containerCountLevel4">containerCountLevel4.</param>
            /// <param name="containerCountLevel5">containerCountLevel5.</param>
            /// <param name="containerCountLevel6">containerCountLevel6.</param>
            /// <param name="containerPoints">containerPoints.</param>
            /// <param name="containerSet">containerSet.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="litterCountContainer">litterCountContainer.</param>
            /// <param name="litterCountLandfill">litterCountLandfill.</param>
            /// <param name="litterCountUnprocessed">litterCountUnprocessed.</param>
            /// <param name="litterPoints">litterPoints.</param>
            /// <param name="robotSet">robotSet.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="totalPoints">totalPoints.</param>
            /// <param name="toteCountFar">toteCountFar.</param>
            /// <param name="toteCountNear">toteCountNear.</param>
            /// <param name="totePoints">totePoints.</param>
            /// <param name="toteSet">toteSet.</param>
            /// <param name="toteStack">toteStack.</param>
        public MatchScoreBreakdown2015Alliance(int? adjustPoints = default, int? autoPoints = default, int? containerCountLevel1 = default, int? containerCountLevel2 = default, int? containerCountLevel3 = default, int? containerCountLevel4 = default, int? containerCountLevel5 = default, int? containerCountLevel6 = default, int? containerPoints = default, bool? containerSet = default, int? foulCount = default, int? foulPoints = default, int? litterCountContainer = default, int? litterCountLandfill = default, int? litterCountUnprocessed = default, int? litterPoints = default, bool? robotSet = default, int? teleopPoints = default, int? totalPoints = default, int? toteCountFar = default, int? toteCountNear = default, int? totePoints = default, bool? toteSet = default, bool? toteStack = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoPoints = autoPoints;
                      this.ContainerCountLevel1 = containerCountLevel1;
                      this.ContainerCountLevel2 = containerCountLevel2;
                      this.ContainerCountLevel3 = containerCountLevel3;
                      this.ContainerCountLevel4 = containerCountLevel4;
                      this.ContainerCountLevel5 = containerCountLevel5;
                      this.ContainerCountLevel6 = containerCountLevel6;
                      this.ContainerPoints = containerPoints;
                      this.ContainerSet = containerSet;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.LitterCountContainer = litterCountContainer;
                      this.LitterCountLandfill = litterCountLandfill;
                      this.LitterCountUnprocessed = litterCountUnprocessed;
                      this.LitterPoints = litterPoints;
                      this.RobotSet = robotSet;
                      this.TeleopPoints = teleopPoints;
                      this.TotalPoints = totalPoints;
                      this.ToteCountFar = toteCountFar;
                      this.ToteCountNear = toteCountNear;
                      this.TotePoints = totePoints;
                      this.ToteSet = toteSet;
                      this.ToteStack = toteStack;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjust_points")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("auto_points")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel1
              /// </summary>
                
                  [JsonPropertyName("container_count_level1")]
                  public int? ContainerCountLevel1 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel2
              /// </summary>
                
                  [JsonPropertyName("container_count_level2")]
                  public int? ContainerCountLevel2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel3
              /// </summary>
                
                  [JsonPropertyName("container_count_level3")]
                  public int? ContainerCountLevel3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel4
              /// </summary>
                
                  [JsonPropertyName("container_count_level4")]
                  public int? ContainerCountLevel4 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel5
              /// </summary>
                
                  [JsonPropertyName("container_count_level5")]
                  public int? ContainerCountLevel5 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerCountLevel6
              /// </summary>
                
                  [JsonPropertyName("container_count_level6")]
                  public int? ContainerCountLevel6 { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerPoints
              /// </summary>
                
                  [JsonPropertyName("container_points")]
                  public int? ContainerPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets ContainerSet
              /// </summary>
                
                  [JsonPropertyName("container_set")]
                  public bool? ContainerSet { get; set; }
                  
              /// <summary>
              /// Gets or Sets FoulCount
              /// </summary>
                
                  [JsonPropertyName("foul_count")]
                  public int? FoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets FoulPoints
              /// </summary>
                
                  [JsonPropertyName("foul_points")]
                  public int? FoulPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets LitterCountContainer
              /// </summary>
                
                  [JsonPropertyName("litter_count_container")]
                  public int? LitterCountContainer { get; set; }
                  
              /// <summary>
              /// Gets or Sets LitterCountLandfill
              /// </summary>
                
                  [JsonPropertyName("litter_count_landfill")]
                  public int? LitterCountLandfill { get; set; }
                  
              /// <summary>
              /// Gets or Sets LitterCountUnprocessed
              /// </summary>
                
                  [JsonPropertyName("litter_count_unprocessed")]
                  public int? LitterCountUnprocessed { get; set; }
                  
              /// <summary>
              /// Gets or Sets LitterPoints
              /// </summary>
                
                  [JsonPropertyName("litter_points")]
                  public int? LitterPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets RobotSet
              /// </summary>
                
                  [JsonPropertyName("robot_set")]
                  public bool? RobotSet { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleop_points")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotalPoints
              /// </summary>
                
                  [JsonPropertyName("total_points")]
                  public int? TotalPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets ToteCountFar
              /// </summary>
                
                  [JsonPropertyName("tote_count_far")]
                  public int? ToteCountFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets ToteCountNear
              /// </summary>
                
                  [JsonPropertyName("tote_count_near")]
                  public int? ToteCountNear { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotePoints
              /// </summary>
                
                  [JsonPropertyName("tote_points")]
                  public int? TotePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets ToteSet
              /// </summary>
                
                  [JsonPropertyName("tote_set")]
                  public bool? ToteSet { get; set; }
                  
              /// <summary>
              /// Gets or Sets ToteStack
              /// </summary>
                
                  [JsonPropertyName("tote_stack")]
                  public bool? ToteStack { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class MatchScoreBreakdown2015Alliance {");
                    sb.Append("  AdjustPoints: ").AppendLine($"{ AdjustPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoPoints: ").AppendLine($"{ AutoPoints?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel1: ").AppendLine($"{ ContainerCountLevel1?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel2: ").AppendLine($"{ ContainerCountLevel2?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel3: ").AppendLine($"{ ContainerCountLevel3?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel4: ").AppendLine($"{ ContainerCountLevel4?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel5: ").AppendLine($"{ ContainerCountLevel5?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerCountLevel6: ").AppendLine($"{ ContainerCountLevel6?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerPoints: ").AppendLine($"{ ContainerPoints?.ToString() ?? "[null]" }");
                    sb.Append("  ContainerSet: ").AppendLine($"{ ContainerSet?.ToString() ?? "[null]" }");
                    sb.Append("  FoulCount: ").AppendLine($"{ FoulCount?.ToString() ?? "[null]" }");
                    sb.Append("  FoulPoints: ").AppendLine($"{ FoulPoints?.ToString() ?? "[null]" }");
                    sb.Append("  LitterCountContainer: ").AppendLine($"{ LitterCountContainer?.ToString() ?? "[null]" }");
                    sb.Append("  LitterCountLandfill: ").AppendLine($"{ LitterCountLandfill?.ToString() ?? "[null]" }");
                    sb.Append("  LitterCountUnprocessed: ").AppendLine($"{ LitterCountUnprocessed?.ToString() ?? "[null]" }");
                    sb.Append("  LitterPoints: ").AppendLine($"{ LitterPoints?.ToString() ?? "[null]" }");
                    sb.Append("  RobotSet: ").AppendLine($"{ RobotSet?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopPoints: ").AppendLine($"{ TeleopPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TotalPoints: ").AppendLine($"{ TotalPoints?.ToString() ?? "[null]" }");
                    sb.Append("  ToteCountFar: ").AppendLine($"{ ToteCountFar?.ToString() ?? "[null]" }");
                    sb.Append("  ToteCountNear: ").AppendLine($"{ ToteCountNear?.ToString() ?? "[null]" }");
                    sb.Append("  TotePoints: ").AppendLine($"{ TotePoints?.ToString() ?? "[null]" }");
                    sb.Append("  ToteSet: ").AppendLine($"{ ToteSet?.ToString() ?? "[null]" }");
                    sb.Append("  ToteStack: ").AppendLine($"{ ToteStack?.ToString() ?? "[null]" }");
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
            
