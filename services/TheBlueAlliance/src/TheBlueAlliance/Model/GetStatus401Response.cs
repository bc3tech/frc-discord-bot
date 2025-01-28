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
/// GetStatus401Response
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class GetStatus401Response
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="GetStatus401Response" /> class.
              /// </summary>
              [JsonConstructor]
              protected GetStatus401Response() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="GetStatus401Response" /> class.
        /// </summary>
            /// <param name="error">Authorization error description. (required).</param>
        public GetStatus401Response(string error)
        {
                      // to ensure "error" is required (not null)
                      ArgumentNullException.ThrowIfNull(error);
                      this.Error = error;
        }
        
              /// <summary>
              /// Authorization error description.
              /// </summary>
              /// <value>Authorization error description.</value>
                [JsonRequired]
                  [JsonPropertyName("Error")]
                  public string Error { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class GetStatus401Response {");
                    sb.Append("  Error: ").AppendLine($"{ Error }");
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
