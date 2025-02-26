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
/// LeaderboardInsightDataRankingsInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record LeaderboardInsightDataRankingsInner
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="LeaderboardInsightDataRankingsInner" /> class.
              /// </summary>
              [JsonConstructor]
              protected LeaderboardInsightDataRankingsInner() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardInsightDataRankingsInner" /> class.
        /// </summary>
            /// <param name="keys">Team/Event/Match keys that have the corresponding value. (required).</param>
            /// <param name="value">Value of the insight that the corresponding team/event/matches have, e.g. number of blue banners, or number of matches played. (required).</param>
        public LeaderboardInsightDataRankingsInner(Collection<string> keys, decimal value)
        {
                      // to ensure "keys" is required (not null)
                      ArgumentNullException.ThrowIfNull(keys);
                      this.Keys = keys;
                        
                      this.Value = value;
        }
        
              /// <summary>
              /// Team/Event/Match keys that have the corresponding value.
              /// </summary>
              /// <value>Team/Event/Match keys that have the corresponding value.</value>
                [JsonRequired]
                  [JsonPropertyName("keys")]
                  public Collection<string> Keys { get; set; }
                  
              /// <summary>
              /// Value of the insight that the corresponding team/event/matches have, e.g. number of blue banners, or number of matches played.
              /// </summary>
              /// <value>Value of the insight that the corresponding team/event/matches have, e.g. number of blue banners, or number of matches played.</value>
                [JsonRequired]
                  [JsonPropertyName("value")]
                  public decimal Value { get; set; }
                  
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
