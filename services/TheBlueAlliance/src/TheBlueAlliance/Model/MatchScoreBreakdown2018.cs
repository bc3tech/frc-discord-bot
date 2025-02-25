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
/// See the 2018 FMS API documentation for a description of each value. https://frcevents2.docs.apiary.io/#/reference/match-results/score-details
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class MatchScoreBreakdown2018
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="MatchScoreBreakdown2018" /> class.
              /// </summary>
              [JsonConstructor]
              protected MatchScoreBreakdown2018() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchScoreBreakdown2018" /> class.
        /// </summary>
            /// <param name="blue">blue (required).</param>
            /// <param name="red">red (required).</param>
        public MatchScoreBreakdown2018(MatchScoreBreakdown2018Alliance blue, MatchScoreBreakdown2018Alliance red)
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
                  public MatchScoreBreakdown2018Alliance Blue { get; set; }
                  
              /// <summary>
              /// Gets or Sets Red
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("red")]
                  public MatchScoreBreakdown2018Alliance Red { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class MatchScoreBreakdown2018 {");
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
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
