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
/// TeamEventStatusAlliance
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial class TeamEventStatusAlliance
  {
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="TeamEventStatusAlliance" /> class.
              /// </summary>
              [JsonConstructor]
              protected TeamEventStatusAlliance() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamEventStatusAlliance" /> class.
        /// </summary>
            /// <param name="backup">backup.</param>
            /// <param name="name">Alliance name, may be null.</param>
            /// <param name="number">Alliance number. (required).</param>
            /// <param name="pick">Order the team was picked in the alliance from 0-2, with 0 being alliance captain. (required).</param>
        public TeamEventStatusAlliance(int number, int pick, TeamEventStatusAllianceBackup? backup = default, string? name = default)
        {
                      this.Backup = backup;
                      this.Name = name;
                      this.Number = number;
                      this.Pick = pick;
        }
        
              /// <summary>
              /// Gets or Sets Backup
              /// </summary>
                
                  [JsonPropertyName("backup")]
                  public TeamEventStatusAllianceBackup? Backup { get; set; }
                  
              /// <summary>
              /// Alliance name, may be null.
              /// </summary>
              /// <value>Alliance name, may be null.</value>
                
                  [JsonPropertyName("name")]
                  public string? Name { get; set; }
                  
              /// <summary>
              /// Alliance number.
              /// </summary>
              /// <value>Alliance number.</value>
                [JsonRequired]
                  [JsonPropertyName("number")]
                  public int Number { get; set; }
                  
              /// <summary>
              /// Order the team was picked in the alliance from 0-2, with 0 being alliance captain.
              /// </summary>
              /// <value>Order the team was picked in the alliance from 0-2, with 0 being alliance captain.</value>
                [JsonRequired]
                  [JsonPropertyName("pick")]
                  public int Pick { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class TeamEventStatusAlliance {");
                    sb.Append("  Backup: ").AppendLine($"{ Backup?.ToString() ?? "[null]" }");
                    sb.Append("  Name: ").AppendLine($"{ Name?.ToString() ?? "[null]" }");
                    sb.Append("  Number: ").AppendLine($"{ Number }");
                    sb.Append("  Pick: ").AppendLine($"{ Pick }");
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
