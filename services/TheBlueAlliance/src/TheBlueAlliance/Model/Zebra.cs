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
/// Zebra
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record Zebra
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="Zebra" /> class.
              /// </summary>
              [JsonConstructor]
              protected Zebra() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Zebra" /> class.
        /// </summary>
            /// <param name="alliances">alliances (required).</param>
            /// <param name="key">TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set. (required).</param>
            /// <param name="times">A list of relative timestamps for each data point. Each timestamp will correspond to the X and Y value at the same index in a team xs and ys arrays. &#x60;times&#x60;, all teams &#x60;xs&#x60; and all teams &#x60;ys&#x60; are guarenteed to be the same length. (required).</param>
        public Zebra(ZebraAlliances alliances, string key, Collection<double> times)
        {
                      // to ensure "alliances" is required (not null)
                      ArgumentNullException.ThrowIfNull(alliances);
                      this.Alliances = alliances;
                        
                      // to ensure "key" is required (not null)
                      ArgumentNullException.ThrowIfNull(key);
                      this.Key = key;
                        
                      // to ensure "times" is required (not null)
                      ArgumentNullException.ThrowIfNull(times);
                      this.Times = times;
        }
        
              /// <summary>
              /// Gets or Sets Alliances
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("alliances")]
                  public ZebraAlliances Alliances { get; set; }
                  
              /// <summary>
              /// TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.
              /// </summary>
              /// <value>TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.</value>
                [JsonRequired]
                  [JsonPropertyName("key")]
                  public string Key { get; set; }
                  
              /// <summary>
              /// A list of relative timestamps for each data point. Each timestamp will correspond to the X and Y value at the same index in a team xs and ys arrays. &#x60;times&#x60;, all teams &#x60;xs&#x60; and all teams &#x60;ys&#x60; are guarenteed to be the same length.
              /// </summary>
              /// <value>A list of relative timestamps for each data point. Each timestamp will correspond to the X and Y value at the same index in a team xs and ys arrays. &#x60;times&#x60;, all teams &#x60;xs&#x60; and all teams &#x60;ys&#x60; are guarenteed to be the same length.</value>
                [JsonRequired]
                  [JsonPropertyName("times")]
                  public Collection<double> Times { get; set; }
                  
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
