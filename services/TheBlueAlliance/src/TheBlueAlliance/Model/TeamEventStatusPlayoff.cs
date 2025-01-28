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
/// Playoff status for this team, may be null if the team did not make playoffs, or playoffs have not begun.
/// </summary>

  public partial class TeamEventStatusPlayoff
  {
            /// <summary>
  /// The highest playoff level the team reached.
  /// </summary>
    /// <value>The highest playoff level the team reached.</value>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.TeamEventStatusPlayoffExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum LevelEnum
  {
        /// <summary>
        /// Enum Qm for value: qm
        /// </summary>
        Qm = 1,
          
        /// <summary>
        /// Enum Ef for value: ef
        /// </summary>
        Ef = 2,
          
        /// <summary>
        /// Enum Qf for value: qf
        /// </summary>
        Qf = 3,
          
        /// <summary>
        /// Enum Sf for value: sf
        /// </summary>
        Sf = 4,
          
        /// <summary>
        /// Enum F for value: f
        /// </summary>
        F = 5
  }
    
    /// <summary>
    /// Returns a <see cref="LevelEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static LevelEnum LevelEnumFromString(string value)
    {
      return value switch
      {
            "qm" => LevelEnum.Qm,
            "ef" => LevelEnum.Ef,
            "qf" => LevelEnum.Qf,
            "sf" => LevelEnum.Sf,
            "f" => LevelEnum.F,
        _ => throw new NotImplementedException($"Could not convert value to type LevelEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="LevelEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static LevelEnum? LevelEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "qm" => LevelEnum.Qm,
            "ef" => LevelEnum.Ef,
            "qf" => LevelEnum.Qf,
            "sf" => LevelEnum.Sf,
            "f" => LevelEnum.F,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="LevelEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string LevelEnumToJsonValue(LevelEnum value)
    {
        return value switch
        {
              LevelEnum.Qm => "qm",
              LevelEnum.Ef => "ef",
              LevelEnum.Qf => "qf",
              LevelEnum.Sf => "sf",
              LevelEnum.F => "f",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// The highest playoff level the team reached.
        /// </summary>
          /// <value>The highest playoff level the team reached.</value>
          
            [JsonPropertyName("level")]
            public LevelEnum? Level { get; set; }
            /// <summary>
  /// Current competition status for the playoffs.
  /// </summary>
    /// <value>Current competition status for the playoffs.</value>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.TeamEventStatusPlayoffExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum StatusEnum
  {
        /// <summary>
        /// Enum Won for value: won
        /// </summary>
        Won = 1,
          
        /// <summary>
        /// Enum Eliminated for value: eliminated
        /// </summary>
        Eliminated = 2,
          
        /// <summary>
        /// Enum Playing for value: playing
        /// </summary>
        Playing = 3
  }
    
    /// <summary>
    /// Returns a <see cref="StatusEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static StatusEnum StatusEnumFromString(string value)
    {
      return value switch
      {
            "won" => StatusEnum.Won,
            "eliminated" => StatusEnum.Eliminated,
            "playing" => StatusEnum.Playing,
        _ => throw new NotImplementedException($"Could not convert value to type StatusEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="StatusEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StatusEnum? StatusEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "won" => StatusEnum.Won,
            "eliminated" => StatusEnum.Eliminated,
            "playing" => StatusEnum.Playing,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="StatusEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string StatusEnumToJsonValue(StatusEnum value)
    {
        return value switch
        {
              StatusEnum.Won => "won",
              StatusEnum.Eliminated => "eliminated",
              StatusEnum.Playing => "playing",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// Current competition status for the playoffs.
        /// </summary>
          /// <value>Current competition status for the playoffs.</value>
          
            [JsonPropertyName("status")]
            public StatusEnum? Status { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamEventStatusPlayoff" /> class.
        /// </summary>
            /// <param name="currentLevelRecord">currentLevelRecord.</param>
            /// <param name="level">The highest playoff level the team reached.</param>
            /// <param name="playoffAverage">The average match score during playoffs. Year specific. May be null if not relevant for a given year.</param>
            /// <param name="record">record.</param>
            /// <param name="status">Current competition status for the playoffs.</param>
        public TeamEventStatusPlayoff(WLTRecord? currentLevelRecord = default, LevelEnum? level = default, int? playoffAverage = default, WLTRecord? record = default, StatusEnum? status = default)
        {
                      this.CurrentLevelRecord = currentLevelRecord;
                      this.Level = level;
                      this.PlayoffAverage = playoffAverage;
                      this.Record = record;
                      this.Status = status;
        }
        
              /// <summary>
              /// Gets or Sets CurrentLevelRecord
              /// </summary>
                
                  [JsonPropertyName("current_level_record")]
                  public WLTRecord? CurrentLevelRecord { get; set; }
                  
              /// <summary>
              /// The average match score during playoffs. Year specific. May be null if not relevant for a given year.
              /// </summary>
              /// <value>The average match score during playoffs. Year specific. May be null if not relevant for a given year.</value>
                
                  [JsonPropertyName("playoff_average")]
                  public int? PlayoffAverage { get; set; }
                  
              /// <summary>
              /// Gets or Sets Record
              /// </summary>
                
                  [JsonPropertyName("record")]
                  public WLTRecord? Record { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class TeamEventStatusPlayoff {");
                    sb.Append("  CurrentLevelRecord: ").AppendLine($"{ CurrentLevelRecord?.ToString() ?? "[null]" }");
                    sb.Append("  Level: ").AppendLine($"{ Level?.ToString() ?? "[null]" }");
                    sb.Append("  PlayoffAverage: ").AppendLine($"{ PlayoffAverage?.ToString() ?? "[null]" }");
                    sb.Append("  Record: ").AppendLine($"{ Record?.ToString() ?? "[null]" }");
                    sb.Append("  Status: ").AppendLine($"{ Status?.ToString() ?? "[null]" }");
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
            
