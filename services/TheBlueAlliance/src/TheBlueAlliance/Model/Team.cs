/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// Team
/// </summary>
internal partial class Team
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="Team" /> class.
            /// </summary>
            [JsonConstructor]
            protected Team() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="Team" /> class.
      /// </summary>
          /// <param name="address">Will be NULL, for future development. (required).</param>
          /// <param name="city">City of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="country">Country of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="gmapsPlaceId">Will be NULL, for future development. (required).</param>
          /// <param name="gmapsUrl">Will be NULL, for future development. (required).</param>
          /// <param name="key">TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number. (required).</param>
          /// <param name="lat">Will be NULL, for future development. (required).</param>
          /// <param name="lng">Will be NULL, for future development. (required).</param>
          /// <param name="locationName">Will be NULL, for future development. (required).</param>
          /// <param name="name">Official long name registered with FIRST. (required).</param>
          /// <param name="nickname">Team nickname provided by FIRST. (required).</param>
          /// <param name="postalCode">Postal code from the team address. (required).</param>
          /// <param name="rookieYear">First year the team officially competed. (required).</param>
          /// <param name="schoolName">Name of team school or affilited group registered with FIRST. (required).</param>
          /// <param name="stateProv">State of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="teamNumber">Official team number issued by FIRST. (required).</param>
          /// <param name="website">Official website associated with the team.</param>
      public Team(string address, string city, string country, string gmapsPlaceId, string gmapsUrl, string key, double? lat, double? lng, string locationName, string name, string nickname, string postalCode, int? rookieYear, string schoolName, string stateProv, int teamNumber, string? website = default)
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
                      
                    // to ensure "nickname" is required (not null)
                    ArgumentNullException.ThrowIfNull(nickname);
                    this.Nickname = nickname;
                      
                    // to ensure "postalCode" is required (not null)
                    ArgumentNullException.ThrowIfNull(postalCode);
                    this.PostalCode = postalCode;
                      
                    // to ensure "rookieYear" is required (not null)
                    ArgumentNullException.ThrowIfNull(rookieYear);
                    this.RookieYear = rookieYear;
                      
                    // to ensure "schoolName" is required (not null)
                    ArgumentNullException.ThrowIfNull(schoolName);
                    this.SchoolName = schoolName;
                      
                    // to ensure "stateProv" is required (not null)
                    ArgumentNullException.ThrowIfNull(stateProv);
                    this.StateProv = stateProv;
                      
                    this.TeamNumber = teamNumber;
                    this.Website = website;
      }
      
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("address")]
                public string Address { get; set; }
                
            /// <summary>
            /// City of team derived from parsing the address registered with FIRST.
            /// </summary>
            /// <value>City of team derived from parsing the address registered with FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("city")]
                public string City { get; set; }
                
            /// <summary>
            /// Country of team derived from parsing the address registered with FIRST.
            /// </summary>
            /// <value>Country of team derived from parsing the address registered with FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("country")]
                public string Country { get; set; }
                
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("gmaps_place_id")]
                public string GmapsPlaceId { get; set; }
                
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("gmaps_url")]
                public string GmapsUrl { get; set; }
                
            /// <summary>
            /// TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number.
            /// </summary>
            /// <value>TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number.</value>
              [JsonRequired]
                [JsonPropertyName("key")]
                public string Key { get; set; }
                
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("lat")]
                public double? Lat { get; set; }
                
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("lng")]
                public double? Lng { get; set; }
                
            /// <summary>
            /// Will be NULL, for future development.
            /// </summary>
            /// <value>Will be NULL, for future development.</value>
              [JsonRequired]
                [JsonPropertyName("location_name")]
                public string LocationName { get; set; }
                
            /// <summary>
            /// Official long name registered with FIRST.
            /// </summary>
            /// <value>Official long name registered with FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("name")]
                public string Name { get; set; }
                
            /// <summary>
            /// Team nickname provided by FIRST.
            /// </summary>
            /// <value>Team nickname provided by FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("nickname")]
                public string Nickname { get; set; }
                
            /// <summary>
            /// Postal code from the team address.
            /// </summary>
            /// <value>Postal code from the team address.</value>
              [JsonRequired]
                [JsonPropertyName("postal_code")]
                public string PostalCode { get; set; }
                
            /// <summary>
            /// First year the team officially competed.
            /// </summary>
            /// <value>First year the team officially competed.</value>
              [JsonRequired]
                [JsonPropertyName("rookie_year")]
                public int? RookieYear { get; set; }
                
            /// <summary>
            /// Name of team school or affilited group registered with FIRST.
            /// </summary>
            /// <value>Name of team school or affilited group registered with FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("school_name")]
                public string SchoolName { get; set; }
                
            /// <summary>
            /// State of team derived from parsing the address registered with FIRST.
            /// </summary>
            /// <value>State of team derived from parsing the address registered with FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("state_prov")]
                public string StateProv { get; set; }
                
            /// <summary>
            /// Official team number issued by FIRST.
            /// </summary>
            /// <value>Official team number issued by FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("team_number")]
                public int TeamNumber { get; set; }
                
            /// <summary>
            /// Official website associated with the team.
            /// </summary>
            /// <value>Official website associated with the team.</value>
              
                [JsonPropertyName("website")]
                public string? Website { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class Team {");
                  sb.Append("  Address: ").AppendLine($"{ Address }");
                  sb.Append("  City: ").AppendLine($"{ City }");
                  sb.Append("  Country: ").AppendLine($"{ Country }");
                  sb.Append("  GmapsPlaceId: ").AppendLine($"{ GmapsPlaceId }");
                  sb.Append("  GmapsUrl: ").AppendLine($"{ GmapsUrl }");
                  sb.Append("  Key: ").AppendLine($"{ Key }");
                  sb.Append("  Lat: ").AppendLine($"{ Lat }");
                  sb.Append("  Lng: ").AppendLine($"{ Lng }");
                  sb.Append("  LocationName: ").AppendLine($"{ LocationName }");
                  sb.Append("  Name: ").AppendLine($"{ Name }");
                  sb.Append("  Nickname: ").AppendLine($"{ Nickname }");
                  sb.Append("  PostalCode: ").AppendLine($"{ PostalCode }");
                  sb.Append("  RookieYear: ").AppendLine($"{ RookieYear }");
                  sb.Append("  SchoolName: ").AppendLine($"{ SchoolName }");
                  sb.Append("  StateProv: ").AppendLine($"{ StateProv }");
                  sb.Append("  TeamNumber: ").AppendLine($"{ TeamNumber }");
                  sb.Append("  Website: ").AppendLine($"{ Website?.ToString() ?? "[null]" }");
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
          
