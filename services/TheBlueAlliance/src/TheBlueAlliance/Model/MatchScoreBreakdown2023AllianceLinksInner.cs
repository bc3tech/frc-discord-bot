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
/// MatchScoreBreakdown2023AllianceLinksInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record MatchScoreBreakdown2023AllianceLinksInner
  {
              /// <summary>
  /// Defines Nodes
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2023AllianceLinksInnerExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<NodesEnum>))]
  public enum NodesEnum
  {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 1,
          
        /// <summary>
        /// Enum Cone for value: Cone
        /// </summary>
        [EnumMember(Value = "Cone")]
        Cone = 2,
          
        /// <summary>
        /// Enum Cube for value: Cube
        /// </summary>
        [EnumMember(Value = "Cube")]
        Cube = 3
  }
    
    /// <summary>
    /// Returns a <see cref="NodesEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static NodesEnum? NodesEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "None" => NodesEnum.None,
            "Cone" => NodesEnum.Cone,
            "Cube" => NodesEnum.Cube,
        _ => null
      };
    }

            /// <summary>
  /// Defines Row
  /// </summary>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2023AllianceLinksInnerExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<RowEnum>))]
  public enum RowEnum
  {
        /// <summary>
        /// Enum Bottom for value: Bottom
        /// </summary>
        [EnumMember(Value = "Bottom")]
        Bottom = 1,
          
        /// <summary>
        /// Enum Mid for value: Mid
        /// </summary>
        [EnumMember(Value = "Mid")]
        Mid = 2,
          
        /// <summary>
        /// Enum Top for value: Top
        /// </summary>
        [EnumMember(Value = "Top")]
        Top = 3
  }
    
    /// <summary>
    /// Returns a <see cref="RowEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static RowEnum? RowEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "Bottom" => RowEnum.Bottom,
            "Mid" => RowEnum.Mid,
            "Top" => RowEnum.Top,
        _ => null
      };
    }

        
        /// <summary>
        /// Gets or Sets Row
        /// </summary>
          [JsonRequired]
            [JsonPropertyName("row")]
            public RowEnum Row { get; set; }
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="MatchScoreBreakdown2023AllianceLinksInner" /> class.
              /// </summary>
              [JsonConstructor]
              protected MatchScoreBreakdown2023AllianceLinksInner() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2023AllianceLinksInner" /> class.
        /// </summary>
            /// <param name="nodes">nodes (required).</param>
            /// <param name="row">row (required).</param>
        public MatchScoreBreakdown2023AllianceLinksInner(Collection<NodesEnum> nodes, RowEnum row)
        {
                      // to ensure "nodes" is required (not null)
                      ArgumentNullException.ThrowIfNull(nodes);
                      this.Nodes = nodes;
                        
                      this.Row = row;
        }
        
              /// <summary>
              /// Gets or Sets Nodes
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("nodes")]
                  public Collection<MatchScoreBreakdown2023AllianceLinksInner.NodesEnum> Nodes { get; set; }
                  
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
