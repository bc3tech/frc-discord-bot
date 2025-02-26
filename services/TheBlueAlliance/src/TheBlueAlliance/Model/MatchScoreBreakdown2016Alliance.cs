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
/// MatchScoreBreakdown2016Alliance
/// </summary>

  public partial record MatchScoreBreakdown2016Alliance
  {
            /// <summary>
  /// Defines Robot1Auto
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2016AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<Robot1AutoEnum>))]
  public enum Robot1AutoEnum
  {
        /// <summary>
        /// Enum Crossed for value: Crossed
        /// </summary>
        [EnumMember(Value = "Crossed")]
        Crossed = 1,
          
        /// <summary>
        /// Enum Reached for value: Reached
        /// </summary>
        [EnumMember(Value = "Reached")]
        Reached = 2,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 3
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
            "Crossed" => Robot1AutoEnum.Crossed,
            "Reached" => Robot1AutoEnum.Reached,
            "None" => Robot1AutoEnum.None,
        _ => null
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
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2016AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<Robot2AutoEnum>))]
  public enum Robot2AutoEnum
  {
        /// <summary>
        /// Enum Crossed for value: Crossed
        /// </summary>
        [EnumMember(Value = "Crossed")]
        Crossed = 1,
          
        /// <summary>
        /// Enum Reached for value: Reached
        /// </summary>
        [EnumMember(Value = "Reached")]
        Reached = 2,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 3
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
            "Crossed" => Robot2AutoEnum.Crossed,
            "Reached" => Robot2AutoEnum.Reached,
            "None" => Robot2AutoEnum.None,
        _ => null
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
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2016AllianceExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<Robot3AutoEnum>))]
  public enum Robot3AutoEnum
  {
        /// <summary>
        /// Enum Crossed for value: Crossed
        /// </summary>
        [EnumMember(Value = "Crossed")]
        Crossed = 1,
          
        /// <summary>
        /// Enum Reached for value: Reached
        /// </summary>
        [EnumMember(Value = "Reached")]
        Reached = 2,
          
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 3
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
            "Crossed" => Robot3AutoEnum.Crossed,
            "Reached" => Robot3AutoEnum.Reached,
            "None" => Robot3AutoEnum.None,
        _ => null
      };
    }

        
        /// <summary>
        /// Gets or Sets Robot3Auto
        /// </summary>
          
            [JsonPropertyName("robot3Auto")]
            public Robot3AutoEnum? Robot3Auto { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2016Alliance" /> class.
        /// </summary>
            /// <param name="adjustPoints">adjustPoints.</param>
            /// <param name="autoBoulderPoints">autoBoulderPoints.</param>
            /// <param name="autoBouldersHigh">autoBouldersHigh.</param>
            /// <param name="autoBouldersLow">autoBouldersLow.</param>
            /// <param name="autoCrossingPoints">autoCrossingPoints.</param>
            /// <param name="autoPoints">autoPoints.</param>
            /// <param name="autoReachPoints">autoReachPoints.</param>
            /// <param name="breachPoints">breachPoints.</param>
            /// <param name="capturePoints">capturePoints.</param>
            /// <param name="foulCount">foulCount.</param>
            /// <param name="foulPoints">foulPoints.</param>
            /// <param name="position1crossings">position1crossings.</param>
            /// <param name="position2">position2.</param>
            /// <param name="position2crossings">position2crossings.</param>
            /// <param name="position3">position3.</param>
            /// <param name="position3crossings">position3crossings.</param>
            /// <param name="position4">position4.</param>
            /// <param name="position4crossings">position4crossings.</param>
            /// <param name="position5">position5.</param>
            /// <param name="position5crossings">position5crossings.</param>
            /// <param name="robot1Auto">robot1Auto.</param>
            /// <param name="robot2Auto">robot2Auto.</param>
            /// <param name="robot3Auto">robot3Auto.</param>
            /// <param name="techFoulCount">techFoulCount.</param>
            /// <param name="teleopBoulderPoints">teleopBoulderPoints.</param>
            /// <param name="teleopBouldersHigh">teleopBouldersHigh.</param>
            /// <param name="teleopBouldersLow">teleopBouldersLow.</param>
            /// <param name="teleopChallengePoints">teleopChallengePoints.</param>
            /// <param name="teleopCrossingPoints">teleopCrossingPoints.</param>
            /// <param name="teleopDefensesBreached">teleopDefensesBreached.</param>
            /// <param name="teleopPoints">teleopPoints.</param>
            /// <param name="teleopScalePoints">teleopScalePoints.</param>
            /// <param name="teleopTowerCaptured">teleopTowerCaptured.</param>
            /// <param name="totalPoints">totalPoints.</param>
            /// <param name="towerEndStrength">towerEndStrength.</param>
            /// <param name="towerFaceA">towerFaceA.</param>
            /// <param name="towerFaceB">towerFaceB.</param>
            /// <param name="towerFaceC">towerFaceC.</param>
        public MatchScoreBreakdown2016Alliance(int? adjustPoints = default, int? autoBoulderPoints = default, int? autoBouldersHigh = default, int? autoBouldersLow = default, int? autoCrossingPoints = default, int? autoPoints = default, int? autoReachPoints = default, int? breachPoints = default, int? capturePoints = default, int? foulCount = default, int? foulPoints = default, int? position1crossings = default, string? position2 = default, int? position2crossings = default, string? position3 = default, int? position3crossings = default, string? position4 = default, int? position4crossings = default, string? position5 = default, int? position5crossings = default, Robot1AutoEnum? robot1Auto = default, Robot2AutoEnum? robot2Auto = default, Robot3AutoEnum? robot3Auto = default, int? techFoulCount = default, int? teleopBoulderPoints = default, int? teleopBouldersHigh = default, int? teleopBouldersLow = default, int? teleopChallengePoints = default, int? teleopCrossingPoints = default, bool? teleopDefensesBreached = default, int? teleopPoints = default, int? teleopScalePoints = default, bool? teleopTowerCaptured = default, int? totalPoints = default, int? towerEndStrength = default, string? towerFaceA = default, string? towerFaceB = default, string? towerFaceC = default)
        {
                      this.AdjustPoints = adjustPoints;
                      this.AutoBoulderPoints = autoBoulderPoints;
                      this.AutoBouldersHigh = autoBouldersHigh;
                      this.AutoBouldersLow = autoBouldersLow;
                      this.AutoCrossingPoints = autoCrossingPoints;
                      this.AutoPoints = autoPoints;
                      this.AutoReachPoints = autoReachPoints;
                      this.BreachPoints = breachPoints;
                      this.CapturePoints = capturePoints;
                      this.FoulCount = foulCount;
                      this.FoulPoints = foulPoints;
                      this.Position1crossings = position1crossings;
                      this.Position2 = position2;
                      this.Position2crossings = position2crossings;
                      this.Position3 = position3;
                      this.Position3crossings = position3crossings;
                      this.Position4 = position4;
                      this.Position4crossings = position4crossings;
                      this.Position5 = position5;
                      this.Position5crossings = position5crossings;
                      this.Robot1Auto = robot1Auto;
                      this.Robot2Auto = robot2Auto;
                      this.Robot3Auto = robot3Auto;
                      this.TechFoulCount = techFoulCount;
                      this.TeleopBoulderPoints = teleopBoulderPoints;
                      this.TeleopBouldersHigh = teleopBouldersHigh;
                      this.TeleopBouldersLow = teleopBouldersLow;
                      this.TeleopChallengePoints = teleopChallengePoints;
                      this.TeleopCrossingPoints = teleopCrossingPoints;
                      this.TeleopDefensesBreached = teleopDefensesBreached;
                      this.TeleopPoints = teleopPoints;
                      this.TeleopScalePoints = teleopScalePoints;
                      this.TeleopTowerCaptured = teleopTowerCaptured;
                      this.TotalPoints = totalPoints;
                      this.TowerEndStrength = towerEndStrength;
                      this.TowerFaceA = towerFaceA;
                      this.TowerFaceB = towerFaceB;
                      this.TowerFaceC = towerFaceC;
        }
        
              /// <summary>
              /// Gets or Sets AdjustPoints
              /// </summary>
                
                  [JsonPropertyName("adjustPoints")]
                  public int? AdjustPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoBoulderPoints
              /// </summary>
                
                  [JsonPropertyName("autoBoulderPoints")]
                  public int? AutoBoulderPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoBouldersHigh
              /// </summary>
                
                  [JsonPropertyName("autoBouldersHigh")]
                  public int? AutoBouldersHigh { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoBouldersLow
              /// </summary>
                
                  [JsonPropertyName("autoBouldersLow")]
                  public int? AutoBouldersLow { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoCrossingPoints
              /// </summary>
                
                  [JsonPropertyName("autoCrossingPoints")]
                  public int? AutoCrossingPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoPoints
              /// </summary>
                
                  [JsonPropertyName("autoPoints")]
                  public int? AutoPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets AutoReachPoints
              /// </summary>
                
                  [JsonPropertyName("autoReachPoints")]
                  public int? AutoReachPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets BreachPoints
              /// </summary>
                
                  [JsonPropertyName("breachPoints")]
                  public int? BreachPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets CapturePoints
              /// </summary>
                
                  [JsonPropertyName("capturePoints")]
                  public int? CapturePoints { get; set; }
                  
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
              /// Gets or Sets Position1crossings
              /// </summary>
                
                  [JsonPropertyName("position1crossings")]
                  public int? Position1crossings { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position2
              /// </summary>
                
                  [JsonPropertyName("position2")]
                  public string? Position2 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position2crossings
              /// </summary>
                
                  [JsonPropertyName("position2crossings")]
                  public int? Position2crossings { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position3
              /// </summary>
                
                  [JsonPropertyName("position3")]
                  public string? Position3 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position3crossings
              /// </summary>
                
                  [JsonPropertyName("position3crossings")]
                  public int? Position3crossings { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position4
              /// </summary>
                
                  [JsonPropertyName("position4")]
                  public string? Position4 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position4crossings
              /// </summary>
                
                  [JsonPropertyName("position4crossings")]
                  public int? Position4crossings { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position5
              /// </summary>
                
                  [JsonPropertyName("position5")]
                  public string? Position5 { get; set; }
                  
              /// <summary>
              /// Gets or Sets Position5crossings
              /// </summary>
                
                  [JsonPropertyName("position5crossings")]
                  public int? Position5crossings { get; set; }
                  
              /// <summary>
              /// Gets or Sets TechFoulCount
              /// </summary>
                
                  [JsonPropertyName("techFoulCount")]
                  public int? TechFoulCount { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopBoulderPoints
              /// </summary>
                
                  [JsonPropertyName("teleopBoulderPoints")]
                  public int? TeleopBoulderPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopBouldersHigh
              /// </summary>
                
                  [JsonPropertyName("teleopBouldersHigh")]
                  public int? TeleopBouldersHigh { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopBouldersLow
              /// </summary>
                
                  [JsonPropertyName("teleopBouldersLow")]
                  public int? TeleopBouldersLow { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopChallengePoints
              /// </summary>
                
                  [JsonPropertyName("teleopChallengePoints")]
                  public int? TeleopChallengePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopCrossingPoints
              /// </summary>
                
                  [JsonPropertyName("teleopCrossingPoints")]
                  public int? TeleopCrossingPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopDefensesBreached
              /// </summary>
                
                  [JsonPropertyName("teleopDefensesBreached")]
                  public bool? TeleopDefensesBreached { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopPoints
              /// </summary>
                
                  [JsonPropertyName("teleopPoints")]
                  public int? TeleopPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopScalePoints
              /// </summary>
                
                  [JsonPropertyName("teleopScalePoints")]
                  public int? TeleopScalePoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TeleopTowerCaptured
              /// </summary>
                
                  [JsonPropertyName("teleopTowerCaptured")]
                  public bool? TeleopTowerCaptured { get; set; }
                  
              /// <summary>
              /// Gets or Sets TotalPoints
              /// </summary>
                
                  [JsonPropertyName("totalPoints")]
                  public int? TotalPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets TowerEndStrength
              /// </summary>
                
                  [JsonPropertyName("towerEndStrength")]
                  public int? TowerEndStrength { get; set; }
                  
              /// <summary>
              /// Gets or Sets TowerFaceA
              /// </summary>
                
                  [JsonPropertyName("towerFaceA")]
                  public string? TowerFaceA { get; set; }
                  
              /// <summary>
              /// Gets or Sets TowerFaceB
              /// </summary>
                
                  [JsonPropertyName("towerFaceB")]
                  public string? TowerFaceB { get; set; }
                  
              /// <summary>
              /// Gets or Sets TowerFaceC
              /// </summary>
                
                  [JsonPropertyName("towerFaceC")]
                  public string? TowerFaceC { get; set; }
                  
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            
