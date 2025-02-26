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
/// Backup team called in, may be null.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EliminationAllianceBackup
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EliminationAllianceBackup" /> class.
              /// </summary>
              [JsonConstructor]
              protected EliminationAllianceBackup() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EliminationAllianceBackup" /> class.
        /// </summary>
            /// <param name="varIn">Team key that was called in as the backup. (required).</param>
            /// <param name="varOut">Team key that was replaced by the backup team. (required).</param>
        public EliminationAllianceBackup(string varIn, string varOut)
        {
                      // to ensure "varIn" is required (not null)
                      ArgumentNullException.ThrowIfNull(varIn);
                      this.In = varIn;
                        
                      // to ensure "varOut" is required (not null)
                      ArgumentNullException.ThrowIfNull(varOut);
                      this.Out = varOut;
        }
        
              /// <summary>
              /// Team key that was called in as the backup.
              /// </summary>
              /// <value>Team key that was called in as the backup.</value>
                [JsonRequired]
                  [JsonPropertyName("in")]
                  public string In { get; set; }
                  
              /// <summary>
              /// Team key that was replaced by the backup team.
              /// </summary>
              /// <value>Team key that was replaced by the backup team.</value>
                [JsonRequired]
                  [JsonPropertyName("out")]
                  public string Out { get; set; }
                  
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
