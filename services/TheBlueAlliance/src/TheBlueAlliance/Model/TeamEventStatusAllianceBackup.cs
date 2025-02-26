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
/// Backup status, may be null.
/// </summary>

  public partial record TeamEventStatusAllianceBackup
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamEventStatusAllianceBackup" /> class.
        /// </summary>
            /// <param name="varIn">TBA key for the backup team called in.</param>
            /// <param name="varOut">TBA key for the team replaced by the backup.</param>
        public TeamEventStatusAllianceBackup(string? varIn = default, string? varOut = default)
        {
                      this.In = varIn;
                      this.Out = varOut;
        }
        
              /// <summary>
              /// TBA key for the backup team called in.
              /// </summary>
              /// <value>TBA key for the backup team called in.</value>
                
                  [JsonPropertyName("in")]
                  public string? In { get; set; }
                  
              /// <summary>
              /// TBA key for the team replaced by the backup.
              /// </summary>
              /// <value>TBA key for the team replaced by the backup.</value>
                
                  [JsonPropertyName("out")]
                  public string? Out { get; set; }
                  
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            
