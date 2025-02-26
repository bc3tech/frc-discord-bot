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
/// Award
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record Award
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="Award" /> class.
              /// </summary>
              [JsonConstructor]
              protected Award() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Award" /> class.
        /// </summary>
            /// <param name="awardType">Type of award given. See https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/award_type.py#L6 (required).</param>
            /// <param name="eventKey">The event_key of the event the award was won at. (required).</param>
            /// <param name="name">The name of the award as provided by FIRST. May vary for the same award type. (required).</param>
            /// <param name="recipientList">A list of recipients of the award at the event. May have either a team_key or an awardee, both, or neither (in the case the award wasn&#39;t awarded at the event). (required).</param>
            /// <param name="year">The year this award was won. (required).</param>
        public Award(int awardType, string eventKey, string name, Collection<AwardRecipient> recipientList, int year)
        {
                      this.AwardType = awardType;
                      // to ensure "eventKey" is required (not null)
                      ArgumentNullException.ThrowIfNull(eventKey);
                      this.EventKey = eventKey;
                        
                      // to ensure "name" is required (not null)
                      ArgumentNullException.ThrowIfNull(name);
                      this.Name = name;
                        
                      // to ensure "recipientList" is required (not null)
                      ArgumentNullException.ThrowIfNull(recipientList);
                      this.RecipientList = recipientList;
                        
                      this.Year = year;
        }
        
              /// <summary>
              /// Type of award given. See https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/award_type.py#L6
              /// </summary>
              /// <value>Type of award given. See https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/award_type.py#L6</value>
                [JsonRequired]
                  [JsonPropertyName("award_type")]
                  public int AwardType { get; set; }
                  
              /// <summary>
              /// The event_key of the event the award was won at.
              /// </summary>
              /// <value>The event_key of the event the award was won at.</value>
                [JsonRequired]
                  [JsonPropertyName("event_key")]
                  public string EventKey { get; set; }
                  
              /// <summary>
              /// The name of the award as provided by FIRST. May vary for the same award type.
              /// </summary>
              /// <value>The name of the award as provided by FIRST. May vary for the same award type.</value>
                [JsonRequired]
                  [JsonPropertyName("name")]
                  public string Name { get; set; }
                  
              /// <summary>
              /// A list of recipients of the award at the event. May have either a team_key or an awardee, both, or neither (in the case the award wasn&#39;t awarded at the event).
              /// </summary>
              /// <value>A list of recipients of the award at the event. May have either a team_key or an awardee, both, or neither (in the case the award wasn&#39;t awarded at the event).</value>
                [JsonRequired]
                  [JsonPropertyName("recipient_list")]
                  public Collection<AwardRecipient> RecipientList { get; set; }
                  
              /// <summary>
              /// The year this award was won.
              /// </summary>
              /// <value>The year this award was won.</value>
                [JsonRequired]
                  [JsonPropertyName("year")]
                  public int Year { get; set; }
                  
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
