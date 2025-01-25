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
/// EventDistrictPoints
/// </summary>
internal partial class EventDistrictPoints
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="EventDistrictPoints" /> class.
            /// </summary>
            [JsonConstructor]
            protected EventDistrictPoints() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="EventDistrictPoints" /> class.
      /// </summary>
          /// <param name="points">Points gained for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the points as its value. (required).</param>
          /// <param name="tiebreakers">Tiebreaker values for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the tiebreaker elements as its value.</param>
      public EventDistrictPoints(Dictionary<string, EventDistrictPointsPointsValue> points, Dictionary<string, EventDistrictPointsTiebreakersValue>? tiebreakers = default)
      {
                    // to ensure "points" is required (not null)
                    ArgumentNullException.ThrowIfNull(points);
                    this.Points = points;
                      
                    this.Tiebreakers = tiebreakers;
      }
      
            /// <summary>
            /// Points gained for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the points as its value.
            /// </summary>
            /// <value>Points gained for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the points as its value.</value>
              [JsonRequired]
                [JsonPropertyName("points")]
                public Dictionary<string, EventDistrictPointsPointsValue> Points { get; set; }
                
            /// <summary>
            /// Tiebreaker values for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the tiebreaker elements as its value.
            /// </summary>
            /// <value>Tiebreaker values for each team at the event. Stored as a key-value pair with the team key as the key, and an object describing the tiebreaker elements as its value.</value>
              
                [JsonPropertyName("tiebreakers")]
                public Dictionary<string, EventDistrictPointsTiebreakersValue>? Tiebreakers { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class EventDistrictPoints {");
                  sb.Append("  Points: ").AppendLine($"{ Points }");
                  sb.Append("  Tiebreakers: ").AppendLine($"{ Tiebreakers?.ToString() ?? "[null]" }");
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
          
