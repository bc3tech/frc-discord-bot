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
/// EventSimple
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record EventSimple
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="EventSimple" /> class.
              /// </summary>
              [JsonConstructor]
              protected EventSimple() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSimple" /> class.
        /// </summary>
            /// <param name="city">City, town, village, etc. the event is located in. (required).</param>
            /// <param name="country">Country the event is located in. (required).</param>
            /// <param name="district">district (required).</param>
            /// <param name="endDate">Event end date in &#x60;yyyy-mm-dd&#x60; format. (required).</param>
            /// <param name="eventCode">Event short code, as provided by FIRST. (required).</param>
            /// <param name="eventType">Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2 (required).</param>
            /// <param name="key">TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. (required).</param>
            /// <param name="name">Official name of event on record either provided by FIRST or organizers of offseason event. (required).</param>
            /// <param name="startDate">Event start date in &#x60;yyyy-mm-dd&#x60; format. (required).</param>
            /// <param name="stateProv">State or Province the event is located in. (required).</param>
            /// <param name="year">Year the event data is for. (required).</param>
        public EventSimple(string city, string country, DistrictList district, DateOnly endDate, string eventCode, int eventType, string key, string name, DateOnly startDate, string stateProv, int year)
        {
                      // to ensure "city" is required (not null)
                      ArgumentNullException.ThrowIfNull(city);
                      this.City = city;
                        
                      // to ensure "country" is required (not null)
                      ArgumentNullException.ThrowIfNull(country);
                      this.Country = country;
                        
                      // to ensure "district" is required (not null)
                      ArgumentNullException.ThrowIfNull(district);
                      this.District = district;
                        
                      this.EndDate = endDate;
                      // to ensure "eventCode" is required (not null)
                      ArgumentNullException.ThrowIfNull(eventCode);
                      this.EventCode = eventCode;
                        
                      this.EventType = eventType;
                      // to ensure "key" is required (not null)
                      ArgumentNullException.ThrowIfNull(key);
                      this.Key = key;
                        
                      // to ensure "name" is required (not null)
                      ArgumentNullException.ThrowIfNull(name);
                      this.Name = name;
                        
                      this.StartDate = startDate;
                      // to ensure "stateProv" is required (not null)
                      ArgumentNullException.ThrowIfNull(stateProv);
                      this.StateProv = stateProv;
                        
                      this.Year = year;
        }
        
              /// <summary>
              /// City, town, village, etc. the event is located in.
              /// </summary>
              /// <value>City, town, village, etc. the event is located in.</value>
                [JsonRequired]
                  [JsonPropertyName("city")]
                  public string City { get; set; }
                  
              /// <summary>
              /// Country the event is located in.
              /// </summary>
              /// <value>Country the event is located in.</value>
                [JsonRequired]
                  [JsonPropertyName("country")]
                  public string Country { get; set; }
                  
              /// <summary>
              /// Gets or Sets District
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("district")]
                  public DistrictList District { get; set; }
                  
              /// <summary>
              /// Event end date in &#x60;yyyy-mm-dd&#x60; format.
              /// </summary>
              /// <value>Event end date in &#x60;yyyy-mm-dd&#x60; format.</value>
                [JsonRequired]
                  [JsonPropertyName("end_date")]
                  public DateOnly EndDate { get; set; }
                  
              /// <summary>
              /// Event short code, as provided by FIRST.
              /// </summary>
              /// <value>Event short code, as provided by FIRST.</value>
                [JsonRequired]
                  [JsonPropertyName("event_code")]
                  public string EventCode { get; set; }
                  
              /// <summary>
              /// Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2
              /// </summary>
              /// <value>Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2</value>
                [JsonRequired]
                  [JsonPropertyName("event_type")]
                  public int EventType { get; set; }
                  
              /// <summary>
              /// TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.
              /// </summary>
              /// <value>TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.</value>
                [JsonRequired]
                  [JsonPropertyName("key")]
                  public string Key { get; set; }
                  
              /// <summary>
              /// Official name of event on record either provided by FIRST or organizers of offseason event.
              /// </summary>
              /// <value>Official name of event on record either provided by FIRST or organizers of offseason event.</value>
                [JsonRequired]
                  [JsonPropertyName("name")]
                  public string Name { get; set; }
                  
              /// <summary>
              /// Event start date in &#x60;yyyy-mm-dd&#x60; format.
              /// </summary>
              /// <value>Event start date in &#x60;yyyy-mm-dd&#x60; format.</value>
                [JsonRequired]
                  [JsonPropertyName("start_date")]
                  public DateOnly StartDate { get; set; }
                  
              /// <summary>
              /// State or Province the event is located in.
              /// </summary>
              /// <value>State or Province the event is located in.</value>
                [JsonRequired]
                  [JsonPropertyName("state_prov")]
                  public string StateProv { get; set; }
                  
              /// <summary>
              /// Year the event data is for.
              /// </summary>
              /// <value>Year the event data is for.</value>
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
