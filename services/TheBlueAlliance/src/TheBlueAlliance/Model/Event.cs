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
/// Event
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class Event
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="Event" /> class.
              /// </summary>
              [JsonConstructor]
              protected Event() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Event" /> class.
        /// </summary>
            /// <param name="address">Address of the event&#39;s venue, if available. (required).</param>
            /// <param name="city">City, town, village, etc. the event is located in. (required).</param>
            /// <param name="country">Country the event is located in. (required).</param>
            /// <param name="district">district (required).</param>
            /// <param name="divisionKeys">An array of event keys for the divisions at this event. (required).</param>
            /// <param name="endDate">Event end date in &#x60;yyyy-mm-dd&#x60; format. (required).</param>
            /// <param name="eventCode">Event short code, as provided by FIRST. (required).</param>
            /// <param name="eventType">Event Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/event_type.py#L2 (required).</param>
            /// <param name="eventTypeString">Event Type, eg Regional, District, or Offseason. (required).</param>
            /// <param name="firstEventCode">Public facing event code used by FIRST (on frc-events.firstinspires.org, for example) (required).</param>
            /// <param name="firstEventId">The FIRST internal Event ID, used to link to the event on the FRC webpage. (required).</param>
            /// <param name="gmapsPlaceId">Google Maps Place ID for the event address. (required).</param>
            /// <param name="gmapsUrl">Link to address location on Google Maps. (required).</param>
            /// <param name="key">TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event. (required).</param>
            /// <param name="lat">Latitude for the event address. (required).</param>
            /// <param name="lng">Longitude for the event address. (required).</param>
            /// <param name="locationName">Name of the location at the address for the event, eg. Blue Alliance High School. (required).</param>
            /// <param name="name">Official name of event on record either provided by FIRST or organizers of offseason event. (required).</param>
            /// <param name="parentEventKey">The TBA Event key that represents the event&#39;s parent. Used to link back to the event from a division event. It is also the inverse relation of &#x60;divison_keys&#x60;. (required).</param>
            /// <param name="playoffType">Playoff Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/playoff_type.py#L4, or null. (required).</param>
            /// <param name="playoffTypeString">String representation of the &#x60;playoff_type&#x60;, or null. (required).</param>
            /// <param name="postalCode">Postal code from the event address. (required).</param>
            /// <param name="shortName">Same as &#x60;name&#x60; but doesn&#39;t include event specifiers, such as &#39;Regional&#39; or &#39;District&#39;. May be null. (required).</param>
            /// <param name="startDate">Event start date in &#x60;yyyy-mm-dd&#x60; format. (required).</param>
            /// <param name="stateProv">State or Province the event is located in. (required).</param>
            /// <param name="timezone">Timezone name. (required).</param>
            /// <param name="webcasts">webcasts (required).</param>
            /// <param name="website">The event&#39;s website, if any. (required).</param>
            /// <param name="week">Week of the event relative to the first official season event, zero-indexed. Only valid for Regionals, Districts, and District Championships. Null otherwise. (Eg. A season with a week 0 &#39;preseason&#39; event does not count, and week 1 events will show 0 here. Seasons with a week 0.5 regional event will show week 0 for those event(s) and week 1 for week 1 events and so on.) (required).</param>
            /// <param name="year">Year the event data is for. (required).</param>
        public Event(string address, string city, string country, DistrictList district, Collection<string> divisionKeys, DateOnly endDate, string eventCode, int eventType, string eventTypeString, string firstEventCode, string firstEventId, string gmapsPlaceId, string gmapsUrl, string key, double? lat, double? lng, string locationName, string name, string parentEventKey, int? playoffType, string playoffTypeString, string postalCode, string shortName, DateOnly startDate, string stateProv, string timezone, Collection<Webcast> webcasts, string website, int? week, int year)
        {
                      // to ensure "address" is required (not null)
                      ArgumentNullException.ThrowIfNull(address);
                      this.Address = address;
                        
                      // to ensure "city" is required (not null)
                      ArgumentNullException.ThrowIfNull(city);
                      this.City = city;
                        
                      // to ensure "country" is required (not null)
                      ArgumentNullException.ThrowIfNull(country);
                      this.Country = country;
                        
                      // to ensure "district" is required (not null)
                      ArgumentNullException.ThrowIfNull(district);
                      this.District = district;
                        
                      // to ensure "divisionKeys" is required (not null)
                      ArgumentNullException.ThrowIfNull(divisionKeys);
                      this.DivisionKeys = divisionKeys;
                        
                      this.EndDate = endDate;
                      // to ensure "eventCode" is required (not null)
                      ArgumentNullException.ThrowIfNull(eventCode);
                      this.EventCode = eventCode;
                        
                      this.EventType = eventType;
                      // to ensure "eventTypeString" is required (not null)
                      ArgumentNullException.ThrowIfNull(eventTypeString);
                      this.EventTypeString = eventTypeString;
                        
                      // to ensure "firstEventCode" is required (not null)
                      ArgumentNullException.ThrowIfNull(firstEventCode);
                      this.FirstEventCode = firstEventCode;
                        
                      // to ensure "firstEventId" is required (not null)
                      ArgumentNullException.ThrowIfNull(firstEventId);
                      this.FirstEventId = firstEventId;
                        
                      // to ensure "gmapsPlaceId" is required (not null)
                      ArgumentNullException.ThrowIfNull(gmapsPlaceId);
                      this.GmapsPlaceId = gmapsPlaceId;
                        
                      // to ensure "gmapsUrl" is required (not null)
                      ArgumentNullException.ThrowIfNull(gmapsUrl);
                      this.GmapsUrl = gmapsUrl;
                        
                      // to ensure "key" is required (not null)
                      ArgumentNullException.ThrowIfNull(key);
                      this.Key = key;
                        
                      // to ensure "lat" is required (not null)
                      ArgumentNullException.ThrowIfNull(lat);
                      this.Lat = lat;
                        
                      // to ensure "lng" is required (not null)
                      ArgumentNullException.ThrowIfNull(lng);
                      this.Lng = lng;
                        
                      // to ensure "locationName" is required (not null)
                      ArgumentNullException.ThrowIfNull(locationName);
                      this.LocationName = locationName;
                        
                      // to ensure "name" is required (not null)
                      ArgumentNullException.ThrowIfNull(name);
                      this.Name = name;
                        
                      // to ensure "parentEventKey" is required (not null)
                      ArgumentNullException.ThrowIfNull(parentEventKey);
                      this.ParentEventKey = parentEventKey;
                        
                      // to ensure "playoffType" is required (not null)
                      ArgumentNullException.ThrowIfNull(playoffType);
                      this.PlayoffType = playoffType;
                        
                      // to ensure "playoffTypeString" is required (not null)
                      ArgumentNullException.ThrowIfNull(playoffTypeString);
                      this.PlayoffTypeString = playoffTypeString;
                        
                      // to ensure "postalCode" is required (not null)
                      ArgumentNullException.ThrowIfNull(postalCode);
                      this.PostalCode = postalCode;
                        
                      // to ensure "shortName" is required (not null)
                      ArgumentNullException.ThrowIfNull(shortName);
                      this.ShortName = shortName;
                        
                      this.StartDate = startDate;
                      // to ensure "stateProv" is required (not null)
                      ArgumentNullException.ThrowIfNull(stateProv);
                      this.StateProv = stateProv;
                        
                      // to ensure "timezone" is required (not null)
                      ArgumentNullException.ThrowIfNull(timezone);
                      this.Timezone = timezone;
                        
                      // to ensure "webcasts" is required (not null)
                      ArgumentNullException.ThrowIfNull(webcasts);
                      this.Webcasts = webcasts;
                        
                      // to ensure "website" is required (not null)
                      ArgumentNullException.ThrowIfNull(website);
                      this.Website = website;
                        
                      // to ensure "week" is required (not null)
                      ArgumentNullException.ThrowIfNull(week);
                      this.Week = week;
                        
                      this.Year = year;
        }
        
              /// <summary>
              /// Address of the event&#39;s venue, if available.
              /// </summary>
              /// <value>Address of the event&#39;s venue, if available.</value>
                [JsonRequired]
                  [JsonPropertyName("address")]
                  public string Address { get; set; }
                  
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
              /// An array of event keys for the divisions at this event.
              /// </summary>
              /// <value>An array of event keys for the divisions at this event.</value>
                [JsonRequired]
                  [JsonPropertyName("division_keys")]
                  public Collection<string> DivisionKeys { get; set; }
                  
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
              /// Event Type, eg Regional, District, or Offseason.
              /// </summary>
              /// <value>Event Type, eg Regional, District, or Offseason.</value>
                [JsonRequired]
                  [JsonPropertyName("event_type_string")]
                  public string EventTypeString { get; set; }
                  
              /// <summary>
              /// Public facing event code used by FIRST (on frc-events.firstinspires.org, for example)
              /// </summary>
              /// <value>Public facing event code used by FIRST (on frc-events.firstinspires.org, for example)</value>
                [JsonRequired]
                  [JsonPropertyName("first_event_code")]
                  public string FirstEventCode { get; set; }
                  
              /// <summary>
              /// The FIRST internal Event ID, used to link to the event on the FRC webpage.
              /// </summary>
              /// <value>The FIRST internal Event ID, used to link to the event on the FRC webpage.</value>
                [JsonRequired]
                  [JsonPropertyName("first_event_id")]
                  public string FirstEventId { get; set; }
                  
              /// <summary>
              /// Google Maps Place ID for the event address.
              /// </summary>
              /// <value>Google Maps Place ID for the event address.</value>
                [JsonRequired]
                  [JsonPropertyName("gmaps_place_id")]
                  public string GmapsPlaceId { get; set; }
                  
              /// <summary>
              /// Link to address location on Google Maps.
              /// </summary>
              /// <value>Link to address location on Google Maps.</value>
                [JsonRequired]
                  [JsonPropertyName("gmaps_url")]
                  public string GmapsUrl { get; set; }
                  
              /// <summary>
              /// TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.
              /// </summary>
              /// <value>TBA event key with the format yyyy[EVENT_CODE], where yyyy is the year, and EVENT_CODE is the event code of the event.</value>
                [JsonRequired]
                  [JsonPropertyName("key")]
                  public string Key { get; set; }
                  
              /// <summary>
              /// Latitude for the event address.
              /// </summary>
              /// <value>Latitude for the event address.</value>
                [JsonRequired]
                  [JsonPropertyName("lat")]
                  public double? Lat { get; set; }
                  
              /// <summary>
              /// Longitude for the event address.
              /// </summary>
              /// <value>Longitude for the event address.</value>
                [JsonRequired]
                  [JsonPropertyName("lng")]
                  public double? Lng { get; set; }
                  
              /// <summary>
              /// Name of the location at the address for the event, eg. Blue Alliance High School.
              /// </summary>
              /// <value>Name of the location at the address for the event, eg. Blue Alliance High School.</value>
                [JsonRequired]
                  [JsonPropertyName("location_name")]
                  public string LocationName { get; set; }
                  
              /// <summary>
              /// Official name of event on record either provided by FIRST or organizers of offseason event.
              /// </summary>
              /// <value>Official name of event on record either provided by FIRST or organizers of offseason event.</value>
                [JsonRequired]
                  [JsonPropertyName("name")]
                  public string Name { get; set; }
                  
              /// <summary>
              /// The TBA Event key that represents the event&#39;s parent. Used to link back to the event from a division event. It is also the inverse relation of &#x60;divison_keys&#x60;.
              /// </summary>
              /// <value>The TBA Event key that represents the event&#39;s parent. Used to link back to the event from a division event. It is also the inverse relation of &#x60;divison_keys&#x60;.</value>
                [JsonRequired]
                  [JsonPropertyName("parent_event_key")]
                  public string ParentEventKey { get; set; }
                  
              /// <summary>
              /// Playoff Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/playoff_type.py#L4, or null.
              /// </summary>
              /// <value>Playoff Type, as defined here: https://github.com/the-blue-alliance/the-blue-alliance/blob/master/consts/playoff_type.py#L4, or null.</value>
                [JsonRequired]
                  [JsonPropertyName("playoff_type")]
                  public int? PlayoffType { get; set; }
                  
              /// <summary>
              /// String representation of the &#x60;playoff_type&#x60;, or null.
              /// </summary>
              /// <value>String representation of the &#x60;playoff_type&#x60;, or null.</value>
                [JsonRequired]
                  [JsonPropertyName("playoff_type_string")]
                  public string PlayoffTypeString { get; set; }
                  
              /// <summary>
              /// Postal code from the event address.
              /// </summary>
              /// <value>Postal code from the event address.</value>
                [JsonRequired]
                  [JsonPropertyName("postal_code")]
                  public string PostalCode { get; set; }
                  
              /// <summary>
              /// Same as &#x60;name&#x60; but doesn&#39;t include event specifiers, such as &#39;Regional&#39; or &#39;District&#39;. May be null.
              /// </summary>
              /// <value>Same as &#x60;name&#x60; but doesn&#39;t include event specifiers, such as &#39;Regional&#39; or &#39;District&#39;. May be null.</value>
                [JsonRequired]
                  [JsonPropertyName("short_name")]
                  public string ShortName { get; set; }
                  
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
              /// Timezone name.
              /// </summary>
              /// <value>Timezone name.</value>
                [JsonRequired]
                  [JsonPropertyName("timezone")]
                  public string Timezone { get; set; }
                  
              /// <summary>
              /// Gets or Sets Webcasts
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("webcasts")]
                  public Collection<Webcast> Webcasts { get; set; }
                  
              /// <summary>
              /// The event&#39;s website, if any.
              /// </summary>
              /// <value>The event&#39;s website, if any.</value>
                [JsonRequired]
                  [JsonPropertyName("website")]
                  public string Website { get; set; }
                  
              /// <summary>
              /// Week of the event relative to the first official season event, zero-indexed. Only valid for Regionals, Districts, and District Championships. Null otherwise. (Eg. A season with a week 0 &#39;preseason&#39; event does not count, and week 1 events will show 0 here. Seasons with a week 0.5 regional event will show week 0 for those event(s) and week 1 for week 1 events and so on.)
              /// </summary>
              /// <value>Week of the event relative to the first official season event, zero-indexed. Only valid for Regionals, Districts, and District Championships. Null otherwise. (Eg. A season with a week 0 &#39;preseason&#39; event does not count, and week 1 events will show 0 here. Seasons with a week 0.5 regional event will show week 0 for those event(s) and week 1 for week 1 events and so on.)</value>
                [JsonRequired]
                  [JsonPropertyName("week")]
                  public int? Week { get; set; }
                  
              /// <summary>
              /// Year the event data is for.
              /// </summary>
              /// <value>Year the event data is for.</value>
                [JsonRequired]
                  [JsonPropertyName("year")]
                  public int Year { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class Event {");
                    sb.Append("  Address: ").AppendLine($"{ Address }");
                    sb.Append("  City: ").AppendLine($"{ City }");
                    sb.Append("  Country: ").AppendLine($"{ Country }");
                    sb.Append("  District: ").AppendLine($"{ District }");
                    sb.Append("  DivisionKeys: ").AppendLine($"{(DivisionKeys is null ? "[null]" : string.Join(", ", DivisionKeys))}");
                    sb.Append("  EndDate: ").AppendLine($"{ EndDate }");
                    sb.Append("  EventCode: ").AppendLine($"{ EventCode }");
                    sb.Append("  EventType: ").AppendLine($"{ EventType }");
                    sb.Append("  EventTypeString: ").AppendLine($"{ EventTypeString }");
                    sb.Append("  FirstEventCode: ").AppendLine($"{ FirstEventCode }");
                    sb.Append("  FirstEventId: ").AppendLine($"{ FirstEventId }");
                    sb.Append("  GmapsPlaceId: ").AppendLine($"{ GmapsPlaceId }");
                    sb.Append("  GmapsUrl: ").AppendLine($"{ GmapsUrl }");
                    sb.Append("  Key: ").AppendLine($"{ Key }");
                    sb.Append("  Lat: ").AppendLine($"{ Lat }");
                    sb.Append("  Lng: ").AppendLine($"{ Lng }");
                    sb.Append("  LocationName: ").AppendLine($"{ LocationName }");
                    sb.Append("  Name: ").AppendLine($"{ Name }");
                    sb.Append("  ParentEventKey: ").AppendLine($"{ ParentEventKey }");
                    sb.Append("  PlayoffType: ").AppendLine($"{ PlayoffType }");
                    sb.Append("  PlayoffTypeString: ").AppendLine($"{ PlayoffTypeString }");
                    sb.Append("  PostalCode: ").AppendLine($"{ PostalCode }");
                    sb.Append("  ShortName: ").AppendLine($"{ ShortName }");
                    sb.Append("  StartDate: ").AppendLine($"{ StartDate }");
                    sb.Append("  StateProv: ").AppendLine($"{ StateProv }");
                    sb.Append("  Timezone: ").AppendLine($"{ Timezone }");
                    sb.Append("  Webcasts: ").AppendLine($"{(Webcasts is null ? "[null]" : string.Join(", ", Webcasts))}");
                    sb.Append("  Website: ").AppendLine($"{ Website }");
                    sb.Append("  Week: ").AppendLine($"{ Week }");
                    sb.Append("  Year: ").AppendLine($"{ Year }");
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
