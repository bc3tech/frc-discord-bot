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
/// EventRankingExtraStatsInfoInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EventRankingExtraStatsInfoInner
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EventRankingExtraStatsInfoInner" /> class.
              /// </summary>
              [JsonConstructor]
              protected EventRankingExtraStatsInfoInner() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EventRankingExtraStatsInfoInner" /> class.
        /// </summary>
            /// <param name="name">Name of the field used in the &#x60;extra_stats&#x60; array. (required).</param>
            /// <param name="precision">Integer expressing the number of digits of precision in the number provided in &#x60;sort_orders&#x60;. (required).</param>
        public EventRankingExtraStatsInfoInner(string name, decimal precision)
        {
                      // to ensure "name" is required (not null)
                      ArgumentNullException.ThrowIfNull(name);
                      this.Name = name;
                        
                      this.Precision = precision;
        }
        
              /// <summary>
              /// Name of the field used in the &#x60;extra_stats&#x60; array.
              /// </summary>
              /// <value>Name of the field used in the &#x60;extra_stats&#x60; array.</value>
                [JsonRequired]
                  [JsonPropertyName("name")]
                  public string Name { get; set; }
                  
              /// <summary>
              /// Integer expressing the number of digits of precision in the number provided in &#x60;sort_orders&#x60;.
              /// </summary>
              /// <value>Integer expressing the number of digits of precision in the number provided in &#x60;sort_orders&#x60;.</value>
                [JsonRequired]
                  [JsonPropertyName("precision")]
                  public decimal Precision { get; set; }
                  
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
