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
/// See the 2015 FMS API documentation for a description of each value
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class MatchScoreBreakdown2015
  {
            /// <summary>
  /// Defines Coopertition
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2015Extensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum CoopertitionEnum
  {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        None = 1,
          
        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        Unknown = 2,
          
        /// <summary>
        /// Enum Stack for value: Stack
        /// </summary>
        Stack = 3
  }
    
    /// <summary>
    /// Returns a <see cref="CoopertitionEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static CoopertitionEnum CoopertitionEnumFromString(string value)
    {
      return value switch
      {
            "None" => CoopertitionEnum.None,
            "Unknown" => CoopertitionEnum.Unknown,
            "Stack" => CoopertitionEnum.Stack,
        _ => throw new NotImplementedException($"Could not convert value to type CoopertitionEnum: '{value}'")
      };
    }
    
    /// <summary>
    /// Returns a <see cref="CoopertitionEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CoopertitionEnum? CoopertitionEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "None" => CoopertitionEnum.None,
            "Unknown" => CoopertitionEnum.Unknown,
            "Stack" => CoopertitionEnum.Stack,
        _ => null
      };
    }
    
    /// <summary>
    /// Converts the <see cref="CoopertitionEnum"/> to the json value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
    public static string CoopertitionEnumToJsonValue(CoopertitionEnum value)
    {
        return value switch
        {
              CoopertitionEnum.None => "None",
              CoopertitionEnum.Unknown => "Unknown",
              CoopertitionEnum.Stack => "Stack",
          _ => throw new NotImplementedException($"Value could not be handled: '{value}'")
        };
    }

        
        /// <summary>
        /// Gets or Sets Coopertition
        /// </summary>
          [JsonRequired]
            [JsonPropertyName("coopertition")]
            public CoopertitionEnum Coopertition { get; set; }
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="MatchScoreBreakdown2015" /> class.
              /// </summary>
              [JsonConstructor]
              protected MatchScoreBreakdown2015() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2015" /> class.
        /// </summary>
            /// <param name="blue">blue (required).</param>
            /// <param name="coopertition">coopertition (required).</param>
            /// <param name="coopertitionPoints">coopertitionPoints (required).</param>
            /// <param name="red">red (required).</param>
        public MatchScoreBreakdown2015(MatchScoreBreakdown2015Alliance blue, CoopertitionEnum coopertition, int coopertitionPoints, MatchScoreBreakdown2015Alliance red)
        {
                      // to ensure "blue" is required (not null)
                      ArgumentNullException.ThrowIfNull(blue);
                      this.Blue = blue;
                        
                      this.Coopertition = coopertition;
                      this.CoopertitionPoints = coopertitionPoints;
                      // to ensure "red" is required (not null)
                      ArgumentNullException.ThrowIfNull(red);
                      this.Red = red;
        }
        
              /// <summary>
              /// Gets or Sets Blue
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("blue")]
                  public MatchScoreBreakdown2015Alliance Blue { get; set; }
                  
              /// <summary>
              /// Gets or Sets CoopertitionPoints
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("coopertition_points")]
                  public int CoopertitionPoints { get; set; }
                  
              /// <summary>
              /// Gets or Sets Red
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("red")]
                  public MatchScoreBreakdown2015Alliance Red { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class MatchScoreBreakdown2015 {");
                    sb.Append("  Blue: ").AppendLine($"{ Blue }");
                    sb.Append("  Coopertition: ").AppendLine($"{ Coopertition }");
                    sb.Append("  CoopertitionPoints: ").AppendLine($"{ CoopertitionPoints }");
                    sb.Append("  Red: ").AppendLine($"{ Red }");
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
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
