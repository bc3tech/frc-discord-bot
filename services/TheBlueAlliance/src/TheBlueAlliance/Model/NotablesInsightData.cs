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
/// NotablesInsightData
/// </summary>
public partial class NotablesInsightData
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="NotablesInsightData" /> class.
            /// </summary>
            [JsonConstructor]
            protected NotablesInsightData() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="NotablesInsightData" /> class.
      /// </summary>
          /// <param name="entries">entries (required).</param>
      public NotablesInsightData(Collection<NotablesInsightDataEntriesInner> entries)
      {
                    // to ensure "entries" is required (not null)
                    ArgumentNullException.ThrowIfNull(entries);
                    this.Entries = entries;
      }
      
            /// <summary>
            /// Gets or Sets Entries
            /// </summary>
              [JsonRequired]
                [JsonPropertyName("entries")]
                public Collection<NotablesInsightDataEntriesInner> Entries { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class NotablesInsightData {");
                  sb.Append("  Entries: ").AppendLine($"{(Entries is null ? "[null]" : string.Join(", ", Entries))}");
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
          
