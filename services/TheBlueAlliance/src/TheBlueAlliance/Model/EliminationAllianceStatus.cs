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
/// EliminationAllianceStatus
/// </summary>

  public partial record EliminationAllianceStatus
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="EliminationAllianceStatus" /> class.
        /// </summary>
            /// <param name="currentLevelRecord">currentLevelRecord.</param>
            /// <param name="level">level.</param>
            /// <param name="playoffAverage">playoffAverage.</param>
            /// <param name="record">record.</param>
            /// <param name="status">status.</param>
        public EliminationAllianceStatus(WLTRecord? currentLevelRecord = default, string? level = default, double? playoffAverage = default, WLTRecord? record = default, string? status = default)
        {
                      this.CurrentLevelRecord = currentLevelRecord;
                      this.Level = level;
                      this.PlayoffAverage = playoffAverage;
                      this.Record = record;
                      this.Status = status;
        }
        
              /// <summary>
              /// Gets or Sets CurrentLevelRecord
              /// </summary>
                
                  [JsonPropertyName("current_level_record")]
                  public WLTRecord? CurrentLevelRecord { get; set; }
                  
              /// <summary>
              /// Gets or Sets Level
              /// </summary>
                
                  [JsonPropertyName("level")]
                  public string? Level { get; set; }
                  
              /// <summary>
              /// Gets or Sets PlayoffAverage
              /// </summary>
                
                  [JsonPropertyName("playoff_average")]
                  public double? PlayoffAverage { get; set; }
                  
              /// <summary>
              /// Gets or Sets Record
              /// </summary>
                
                  [JsonPropertyName("record")]
                  public WLTRecord? Record { get; set; }
                  
              /// <summary>
              /// Gets or Sets Status
              /// </summary>
                
                  [JsonPropertyName("status")]
                  public string? Status { get; set; }
                  
              /// <summary>
              /// Returns the JSON string presentation of the object
              /// </summary>
              /// <returns>JSON string presentation of the object</returns>
              public string ToJson()
              {
                return JsonSerializer.Serialize(this);
              }
            }
            
