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
/// TeamSimple
/// </summary>
public partial class TeamSimple
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="TeamSimple" /> class.
            /// </summary>
            [JsonConstructor]
            protected TeamSimple() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="TeamSimple" /> class.
      /// </summary>
          /// <param name="city">City of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="country">Country of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="key">TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number. (required).</param>
          /// <param name="name">Official long name registered with FIRST. (required).</param>
          /// <param name="nickname">Team nickname provided by FIRST. (required).</param>
          /// <param name="stateProv">State of team derived from parsing the address registered with FIRST. (required).</param>
          /// <param name="teamNumber">Official team number issued by FIRST. (required).</param>
      public TeamSimple(string city, string country, string key, string name, string nickname, string stateProv, int teamNumber)
      {
                    // to ensure "city" is required (not null)
                    ArgumentNullException.ThrowIfNull(city);
                    this.City = city;
                      
                    // to ensure "country" is required (not null)
                    ArgumentNullException.ThrowIfNull(country);
                    this.Country = country;
                      
                    // to ensure "key" is required (not null)
                    ArgumentNullException.ThrowIfNull(key);
                    this.Key = key;
                      
                    // to ensure "name" is required (not null)
                    ArgumentNullException.ThrowIfNull(name);
                    this.Name = name;
                      
                    // to ensure "nickname" is required (not null)
                    ArgumentNullException.ThrowIfNull(nickname);
                    this.Nickname = nickname;
                      
                    // to ensure "stateProv" is required (not null)
                    ArgumentNullException.ThrowIfNull(stateProv);
                    this.StateProv = stateProv;
                      
                    this.TeamNumber = teamNumber;
      }
      
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
            /// TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number.
            /// </summary>
            /// <value>TBA team key with the format &#x60;frcXXXX&#x60; with &#x60;XXXX&#x60; representing the team number.</value>
              [JsonRequired]
                [JsonPropertyName("key")]
                public string Key { get; set; }
                
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
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class TeamSimple {");
                  sb.Append("  City: ").AppendLine($"{ City }");
                  sb.Append("  Country: ").AppendLine($"{ Country }");
                  sb.Append("  Key: ").AppendLine($"{ Key }");
                  sb.Append("  Name: ").AppendLine($"{ Name }");
                  sb.Append("  Nickname: ").AppendLine($"{ Nickname }");
                  sb.Append("  StateProv: ").AppendLine($"{ StateProv }");
                  sb.Append("  TeamNumber: ").AppendLine($"{ TeamNumber }");
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
          
