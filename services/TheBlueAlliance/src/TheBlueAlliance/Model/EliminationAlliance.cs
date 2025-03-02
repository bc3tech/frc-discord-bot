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
/// EliminationAlliance
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EliminationAlliance
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EliminationAlliance" /> class.
              /// </summary>
              [JsonConstructor]
              protected EliminationAlliance() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EliminationAlliance" /> class.
        /// </summary>
            /// <param name="backup">backup.</param>
            /// <param name="declines">List of teams that declined the alliance. (required).</param>
            /// <param name="name">Alliance name, may be null.</param>
            /// <param name="picks">List of team keys picked for the alliance. First pick is captain. (required).</param>
            /// <param name="status">status.</param>
        public EliminationAlliance(Collection<string> declines, Collection<string> picks, EliminationAllianceBackup? backup = default, string? name = default, EliminationAllianceStatus? status = default)
        {
                      this.Backup = backup;
                      // to ensure "declines" is required (not null)
                      ArgumentNullException.ThrowIfNull(declines);
                      this.Declines = declines;
                        
                      this.Name = name;
                      // to ensure "picks" is required (not null)
                      ArgumentNullException.ThrowIfNull(picks);
                      this.Picks = picks;
                        
                      this.Status = status;
        }
        
              /// <summary>
              /// Gets or Sets Backup
              /// </summary>
                
                  [JsonPropertyName("backup")]
                  public EliminationAllianceBackup? Backup { get; set; }
                  
              /// <summary>
              /// List of teams that declined the alliance.
              /// </summary>
              /// <value>List of teams that declined the alliance.</value>
                [JsonRequired]
                  [JsonPropertyName("declines")]
                  public Collection<string>? Declines { get; set; }
                  
              /// <summary>
              /// Alliance name, may be null.
              /// </summary>
              /// <value>Alliance name, may be null.</value>
                
                  [JsonPropertyName("name")]
                  public string? Name { get; set; }
                  
              /// <summary>
              /// List of team keys picked for the alliance. First pick is captain.
              /// </summary>
              /// <value>List of team keys picked for the alliance. First pick is captain.</value>
                [JsonRequired]
                  [JsonPropertyName("picks")]
                  public Collection<string> Picks { get; set; }
                  
              /// <summary>
              /// Gets or Sets Status
              /// </summary>
                
                  [JsonPropertyName("status")]
                  public EliminationAllianceStatus? Status { get; set; }
                  
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
