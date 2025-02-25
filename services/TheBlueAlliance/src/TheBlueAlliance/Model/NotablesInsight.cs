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
/// NotablesInsight
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class NotablesInsight
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="NotablesInsight" /> class.
              /// </summary>
              [JsonConstructor]
              protected NotablesInsight() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="NotablesInsight" /> class.
        /// </summary>
            /// <param name="data">data (required).</param>
            /// <param name="name">name (required).</param>
            /// <param name="year">year (required).</param>
        public NotablesInsight(NotablesInsightData data, string name, int year)
        {
                      // to ensure "data" is required (not null)
                      ArgumentNullException.ThrowIfNull(data);
                      this.Data = data;
                        
                      // to ensure "name" is required (not null)
                      ArgumentNullException.ThrowIfNull(name);
                      this.Name = name;
                        
                      this.Year = year;
        }
        
              /// <summary>
              /// Gets or Sets Data
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("data")]
                  public NotablesInsightData Data { get; set; }
                  
              /// <summary>
              /// Gets or Sets Name
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("name")]
                  public string Name { get; set; }
                  
              /// <summary>
              /// Gets or Sets Year
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("year")]
                  public int Year { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class NotablesInsight {");
                    sb.Append("  Data: ").AppendLine($"{ Data }");
                    sb.Append("  Name: ").AppendLine($"{ Name }");
                    sb.Append("  Year: ").AppendLine($"{ Year }");
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
