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
/// See the 2022 FMS API documentation for a description of each value. https://frc-api-docs.firstinspires.org
/// </summary>
public partial class MatchScoreBreakdown2022
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="MatchScoreBreakdown2022" /> class.
            /// </summary>
            [JsonConstructor]
            protected MatchScoreBreakdown2022() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="MatchScoreBreakdown2022" /> class.
      /// </summary>
          /// <param name="blue">blue (required).</param>
          /// <param name="red">red (required).</param>
      public MatchScoreBreakdown2022(MatchScoreBreakdown2022Alliance blue, MatchScoreBreakdown2022Alliance red)
      {
                    // to ensure "blue" is required (not null)
                    ArgumentNullException.ThrowIfNull(blue);
                    this.Blue = blue;
                      
                    // to ensure "red" is required (not null)
                    ArgumentNullException.ThrowIfNull(red);
                    this.Red = red;
      }
      
            /// <summary>
            /// Gets or Sets Blue
            /// </summary>
              [JsonRequired]
                [JsonPropertyName("blue")]
                public MatchScoreBreakdown2022Alliance Blue { get; set; }
                
            /// <summary>
            /// Gets or Sets Red
            /// </summary>
              [JsonRequired]
                [JsonPropertyName("red")]
                public MatchScoreBreakdown2022Alliance Red { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class MatchScoreBreakdown2022 {");
                  sb.Append("  Blue: ").AppendLine($"{ Blue }");
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
          
