/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// MatchScoreBreakdown2017Alliance
/// </summary>

  public partial class MatchScoreBreakdown2017Alliance
  {
            /// <summary>
  /// Defines Robot1Auto
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2017AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum Robot1AutoEnum
  {
        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        Unknown,
          
        /// <summary>
        /// Enum Mobility for value: Mobility
        /// </summary>
        Mobility,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        None
  }
    
    /// <summary>
    /// Returns a <see cref="Robot1AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Robot1AutoEnum Robot1AutoEnumFromString(string value)
    {
      return value switch
      {
            "Unknown" => Robot1AutoEnum.Unknown,
            "Mobility" => Robot1AutoEnum.Mobility,
            "None" => Robot1AutoEnum.None,
        _ => throw new NotImplementedException($"Could not convert value to type Robot1AutoEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="Robot1AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Robot1AutoEnum? Robot1AutoEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "Unknown" => Robot1AutoEnum.Unknown,
            "Mobility" => Robot1AutoEnum.Mobility,
            "None" => Robot1AutoEnum.None,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="Robot1AutoEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string Robot1AutoEnumToJsonValue(Robot1AutoEnum value)
    {
        return value switch
        {
              Robot1AutoEnum.Unknown => "Unknown",
              Robot1AutoEnum.Mobility => "Mobility",
              Robot1AutoEnum.None => "None",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// Gets or Sets Robot1Auto
        /// </summary>
          
            [JsonPropertyName("robot1Auto")]
            public Robot1AutoEnum? Robot1Auto { get; set; }
            /// <summary>
  /// Defines Robot2Auto
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2017AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum Robot2AutoEnum
  {
        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        Unknown,
          
        /// <summary>
        /// Enum Mobility for value: Mobility
        /// </summary>
        Mobility,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        None
  }
    
    /// <summary>
    /// Returns a <see cref="Robot2AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Robot2AutoEnum Robot2AutoEnumFromString(string value)
    {
      return value switch
      {
            "Unknown" => Robot2AutoEnum.Unknown,
            "Mobility" => Robot2AutoEnum.Mobility,
            "None" => Robot2AutoEnum.None,
        _ => throw new NotImplementedException($"Could not convert value to type Robot2AutoEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="Robot2AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Robot2AutoEnum? Robot2AutoEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "Unknown" => Robot2AutoEnum.Unknown,
            "Mobility" => Robot2AutoEnum.Mobility,
            "None" => Robot2AutoEnum.None,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="Robot2AutoEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string Robot2AutoEnumToJsonValue(Robot2AutoEnum value)
    {
        return value switch
        {
              Robot2AutoEnum.Unknown => "Unknown",
              Robot2AutoEnum.Mobility => "Mobility",
              Robot2AutoEnum.None => "None",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// Gets or Sets Robot2Auto
        /// </summary>
          
            [JsonPropertyName("robot2Auto")]
            public Robot2AutoEnum? Robot2Auto { get; set; }
            /// <summary>
  /// Defines Robot3Auto
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2017AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum Robot3AutoEnum
  {
        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        Unknown,
          
        /// <summary>
        /// Enum Mobility for value: Mobility
        /// </summary>
        Mobility,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        None
  }
    
    /// <summary>
    /// Returns a <see cref="Robot3AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Robot3AutoEnum Robot3AutoEnumFromString(string value)
    {
      return value switch
      {
            "Unknown" => Robot3AutoEnum.Unknown,
            "Mobility" => Robot3AutoEnum.Mobility,
            "None" => Robot3AutoEnum.None,
        _ => throw new NotImplementedException($"Could not convert value to type Robot3AutoEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="Robot3AutoEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Robot3AutoEnum? Robot3AutoEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "Unknown" => Robot3AutoEnum.Unknown,
            "Mobility" => Robot3AutoEnum.Mobility,
            "None" => Robot3AutoEnum.None,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="Robot3AutoEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string Robot3AutoEnumToJsonValue(Robot3AutoEnum value)
    {
        return value switch
        {
              Robot3AutoEnum.Unknown => "Unknown",
              Robot3AutoEnum.Mobility => "Mobility",
              Robot3AutoEnum.None => "None",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// Gets or Sets Robot3Auto
        /// </summary>
          
            [JsonPropertyName("robot3Auto")]
            public Robot3AutoEnum? Robot3Auto { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2017Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoFuelHigh">autoFuelHigh.</param>
            /// <param name="autoFuelLow">autoFuelLow.</param>
            /// <param name="autoFuelPoints">autoFuelPoints.</param>
            /// <param name="autoMobilityPoints">autoMobilityPoints.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="autoRotorPoints">autoRotorPoints.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="kPaBonusPoints">kPaBonusPoints.</param>
            /// <param name="kPaRankingPointAchieved">kPaRankingPointAchieved.</param>
            /// <param name="robot1Auto">robot1Auto.</param>
            /// <param name="robot2Auto">robot2Auto.</param>
            /// <param name="robot3Auto">robot3Auto.</param>
            /// <param name="rotor1Auto">rotor1Auto.</param>
            /// <param name="rotor1Engaged">rotor1Engaged.</param>
            /// <param name="rotor2Auto">rotor2Auto.</param>
            /// <param name="rotor2Engaged">rotor2Engaged.</param>
            /// <param name="rotor3Engaged">rotor3Engaged.</param>
            /// <param name="rotor4Engaged">rotor4Engaged.</param>
            /// <param name="rotorBonusPoints">rotorBonusPoints.</param>
            /// <param name="rotorRankingPointAchieved">rotorRankingPointAchieved.</param>
            /// <param name="techFoulCount">techFoulCount.</param>
            /// <param name="teleopFuelHigh">teleopFuelHigh.</param>
            /// <param name="teleopFuelLow">teleopFuelLow.</param>
            /// <param name="teleopFuelPoints">teleopFuelPoints.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="teleopRotorPoints">teleopRotorPoints.</param>
            /// <param name="teleopTakeoffPoints">teleopTakeoffPoints.</param>
            /// <param name="totalPoints">totalPoints.</param>
            /// <param name="touchpadFar">touchpadFar.</param>
            /// <param name="touchpadMiddle">touchpadMiddle.</param>
            /// <param name="touchpadNear">touchpadNear.</param>
        public MatchScoreBreakdown2017Alliance(int? adjustPoints = default, int? autoFuelHigh = default, int? autoFuelLow = default, int? autoFuelPoints = default, int? autoMobilityPoints = default, int? autoPoints = default, int? autoRotorPoints = default, int? foulCount = default, int? foulPoints = default, int? kPaBonusPoints = default, bool? kPaRankingPointAchieved = default, Robot1AutoEnum? robot1Auto = default, Robot2AutoEnum? robot2Auto = default, Robot3AutoEnum? robot3Auto = default, bool? rotor1Auto = default, bool? rotor1Engaged = default, bool? rotor2Auto = default, bool? rotor2Engaged = default, bool? rotor3Engaged = default, bool? rotor4Engaged = default, int? rotorBonusPoints = default, bool? rotorRankingPointAchieved = default, int? techFoulCount = default, int? teleopFuelHigh = default, int? teleopFuelLow = default, int? teleopFuelPoints = default, int? teleopPoints = default, int? teleopRotorPoints = default, int? teleopTakeoffPoints = default, int? totalPoints = default, string? touchpadFar = default, string? touchpadMiddle = default, string? touchpadNear = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoFuelHigh = autoFuelHigh;
                      this.AutoFuelLow = autoFuelLow;
                      this.AutoFuelPoints = autoFuelPoints;
                      this.AutoMobilityPoints = autoMobilityPoints;
                      this.AutoPoints = autoPoints;
                      this.AutoRotorPoints = autoRotorPoints;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.KPaBonusPoints = kPaBonusPoints;
                      this.KPaRankingPointAchieved = kPaRankingPointAchieved;
                      this.Robot1Auto = robot1Auto;
                      this.Robot2Auto = robot2Auto;
                      this.Robot3Auto = robot3Auto;
                      this.Rotor1Auto = rotor1Auto;
                      this.Rotor1Engaged = rotor1Engaged;
                      this.Rotor2Auto = rotor2Auto;
                      this.Rotor2Engaged = rotor2Engaged;
                      this.Rotor3Engaged = rotor3Engaged;
                      this.Rotor4Engaged = rotor4Engaged;
                      this.RotorBonusPoints = rotorBonusPoints;
                      this.RotorRankingPointAchieved = rotorRankingPointAchieved;
                      this.TechFoulCount = techFoulCount;
                      this.TeleopFuelHigh = teleopFuelHigh;
                      this.TeleopFuelLow = teleopFuelLow;
                      this.TeleopFuelPoints = teleopFuelPoints;
                      this.TeleopPoints = teleopPoints;
                      this.TeleopRotorPoints = teleopRotorPoints;
                      this.TeleopTakeoffPoints = teleopTakeoffPoints;
                      this.TotalPoints = totalPoints;
                      this.TouchpadFar = touchpadFar;
                      this.TouchpadMiddle = touchpadMiddle;
                      this.TouchpadNear = touchpadNear;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjustPoints")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoFuelHigh
              /// </summary>
                
                  [JsonPropertyName("autoFuelHigh")]
                  public int? AutoFuelHigh { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoFuelLow
              /// </summary>
                
                  [JsonPropertyName("autoFuelLow")]
                  public int? AutoFuelLow { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoFuelPoints
              /// </summary>
                
                  [JsonPropertyName("autoFuelPoints")]
                  public int? AutoFuelPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoMobilityPoints
              /// </summary>
                
                  [JsonPropertyName("autoMobilityPoints")]
                  public int? AutoMobilityPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("autoPoints")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoRotorPoints
              /// </summary>
                
                  [JsonPropertyName("autoRotorPoints")]
                  public int? AutoRotorPoints { get; set; }
                  
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
              /// Gets or Sets KPaBonusPoints
              /// </summary>
                
                  [JsonPropertyName("kPaBonusPoints")]
                  public int? KPaBonusPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets KPaRankingPointAchieved
              /// </summary>
                
                  [JsonPropertyName("kPaRankingPointAchieved")]
                  public bool? KPaRankingPointAchieved { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor1Auto
              /// </summary>
                
                  [JsonPropertyName("rotor1Auto")]
                  public bool? Rotor1Auto { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor1Engaged
              /// </summary>
                
                  [JsonPropertyName("rotor1Engaged")]
                  public bool? Rotor1Engaged { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor2Auto
              /// </summary>
                
                  [JsonPropertyName("rotor2Auto")]
                  public bool? Rotor2Auto { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor2Engaged
              /// </summary>
                
                  [JsonPropertyName("rotor2Engaged")]
                  public bool? Rotor2Engaged { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor3Engaged
              /// </summary>
                
                  [JsonPropertyName("rotor3Engaged")]
                  public bool? Rotor3Engaged { get; set; }
                  
              /// <summary>
              /// Gets or Sets Rotor4Engaged
              /// </summary>
                
                  [JsonPropertyName("rotor4Engaged")]
                  public bool? Rotor4Engaged { get; set; }
                  
              /// <summary>
              /// Gets or Sets RotorBonusPoints
              /// </summary>
                
                  [JsonPropertyName("rotorBonusPoints")]
                  public int? RotorBonusPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets RotorRankingPointAchieved
              /// </summary>
                
                  [JsonPropertyName("rotorRankingPointAchieved")]
                  public bool? RotorRankingPointAchieved { get; set; }
                  
              /// <summary>
              /// Gets or Sets TechFoulCount
              /// </summary>
                
                  [JsonPropertyName("techFoulCount")]
                  public int? TechFoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopFuelHigh
              /// </summary>
                
                  [JsonPropertyName("teleopFuelHigh")]
                  public int? TeleopFuelHigh { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopFuelLow
              /// </summary>
                
                  [JsonPropertyName("teleopFuelLow")]
                  public int? TeleopFuelLow { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopFuelPoints
              /// </summary>
                
                  [JsonPropertyName("teleopFuelPoints")]
                  public int? TeleopFuelPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleopPoints")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopRotorPoints
              /// </summary>
                
                  [JsonPropertyName("teleopRotorPoints")]
                  public int? TeleopRotorPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopTakeoffPoints
              /// </summary>
                
                  [JsonPropertyName("teleopTakeoffPoints")]
                  public int? TeleopTakeoffPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotalPoints
              /// </summary>
                
                  [JsonPropertyName("totalPoints")]
                  public int? TotalPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TouchpadFar
              /// </summary>
                
                  [JsonPropertyName("touchpadFar")]
                  public string? TouchpadFar { get; set; }
                  
              /// <summary>
              /// Gets or Sets TouchpadMiddle
              /// </summary>
                
                  [JsonPropertyName("touchpadMiddle")]
                  public string? TouchpadMiddle { get; set; }
                  
              /// <summary>
              /// Gets or Sets TouchpadNear
              /// </summary>
                
                  [JsonPropertyName("touchpadNear")]
                  public string? TouchpadNear { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class MatchScoreBreakdown2017Alliance {");
                    sb.Append("  AdjustPoints: ").AppendLine($"{ AdjustPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoFuelHigh: ").AppendLine($"{ AutoFuelHigh?.ToString() ?? "[null]" }");
                    sb.Append("  AutoFuelLow: ").AppendLine($"{ AutoFuelLow?.ToString() ?? "[null]" }");
                    sb.Append("  AutoFuelPoints: ").AppendLine($"{ AutoFuelPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoMobilityPoints: ").AppendLine($"{ AutoMobilityPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoPoints: ").AppendLine($"{ AutoPoints?.ToString() ?? "[null]" }");
                    sb.Append("  AutoRotorPoints: ").AppendLine($"{ AutoRotorPoints?.ToString() ?? "[null]" }");
                    sb.Append("  FoulCount: ").AppendLine($"{ FoulCount?.ToString() ?? "[null]" }");
                    sb.Append("  FoulPoints: ").AppendLine($"{ FoulPoints?.ToString() ?? "[null]" }");
                    sb.Append("  KPaBonusPoints: ").AppendLine($"{ KPaBonusPoints?.ToString() ?? "[null]" }");
                    sb.Append("  KPaRankingPointAchieved: ").AppendLine($"{ KPaRankingPointAchieved?.ToString() ?? "[null]" }");
                    sb.Append("  Robot1Auto: ").AppendLine($"{ Robot1Auto?.ToString() ?? "[null]" }");
                    sb.Append("  Robot2Auto: ").AppendLine($"{ Robot2Auto?.ToString() ?? "[null]" }");
                    sb.Append("  Robot3Auto: ").AppendLine($"{ Robot3Auto?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor1Auto: ").AppendLine($"{ Rotor1Auto?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor1Engaged: ").AppendLine($"{ Rotor1Engaged?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor2Auto: ").AppendLine($"{ Rotor2Auto?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor2Engaged: ").AppendLine($"{ Rotor2Engaged?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor3Engaged: ").AppendLine($"{ Rotor3Engaged?.ToString() ?? "[null]" }");
                    sb.Append("  Rotor4Engaged: ").AppendLine($"{ Rotor4Engaged?.ToString() ?? "[null]" }");
                    sb.Append("  RotorBonusPoints: ").AppendLine($"{ RotorBonusPoints?.ToString() ?? "[null]" }");
                    sb.Append("  RotorRankingPointAchieved: ").AppendLine($"{ RotorRankingPointAchieved?.ToString() ?? "[null]" }");
                    sb.Append("  TechFoulCount: ").AppendLine($"{ TechFoulCount?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopFuelHigh: ").AppendLine($"{ TeleopFuelHigh?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopFuelLow: ").AppendLine($"{ TeleopFuelLow?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopFuelPoints: ").AppendLine($"{ TeleopFuelPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopPoints: ").AppendLine($"{ TeleopPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopRotorPoints: ").AppendLine($"{ TeleopRotorPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TeleopTakeoffPoints: ").AppendLine($"{ TeleopTakeoffPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TotalPoints: ").AppendLine($"{ TotalPoints?.ToString() ?? "[null]" }");
                    sb.Append("  TouchpadFar: ").AppendLine($"{ TouchpadFar?.ToString() ?? "[null]" }");
                    sb.Append("  TouchpadMiddle: ").AppendLine($"{ TouchpadMiddle?.ToString() ?? "[null]" }");
                    sb.Append("  TouchpadNear: ").AppendLine($"{ TouchpadNear?.ToString() ?? "[null]" }");
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
            
