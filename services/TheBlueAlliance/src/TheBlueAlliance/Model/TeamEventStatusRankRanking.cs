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
/// TeamEventStatusRankRanking
/// </summary>

  public partial class TeamEventStatusRankRanking
  {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamEventStatusRankRanking" /> class.
        /// </summary>
            /// <param name="dq">Number of matches the team was disqualified for.</param>
            /// <param name="matchesPlayed">Number of matches played.</param>
            /// <param name="qualAverage">For some years, average qualification score. Can be null.</param>
            /// <param name="rank">Relative rank of this team.</param>
            /// <param name="record">record.</param>
            /// <param name="sortOrders">Ordered list of values used to determine the rank. See the &#x60;sort_order_info&#x60; property for the name of each value.</param>
            /// <param name="teamKey">TBA team key for this rank.</param>
        public TeamEventStatusRankRanking(int? dq = default, int? matchesPlayed = default, double? qualAverage = default, int? rank = default, WLTRecord? record = default, Collection<decimal>? sortOrders = default, string? teamKey = default)
        {
                      this.Dq = dq;
                      this.MatchesPlayed = matchesPlayed;
                      this.QualAverage = qualAverage;
                      this.Rank = rank;
                      this.Record = record;
                      this.SortOrders = sortOrders;
                      this.TeamKey = teamKey;
        }
        
              /// <summary>
              /// Number of matches the team was disqualified for.
              /// </summary>
              /// <value>Number of matches the team was disqualified for.</value>
                
                  [JsonPropertyName("dq")]
                  public int? Dq { get; set; }
                  
              /// <summary>
              /// Number of matches played.
              /// </summary>
              /// <value>Number of matches played.</value>
                
                  [JsonPropertyName("matches_played")]
                  public int? MatchesPlayed { get; set; }
                  
              /// <summary>
              /// For some years, average qualification score. Can be null.
              /// </summary>
              /// <value>For some years, average qualification score. Can be null.</value>
                
                  [JsonPropertyName("qual_average")]
                  public double? QualAverage { get; set; }
                  
              /// <summary>
              /// Relative rank of this team.
              /// </summary>
              /// <value>Relative rank of this team.</value>
                
                  [JsonPropertyName("rank")]
                  public int? Rank { get; set; }
                  
              /// <summary>
              /// Gets or Sets Record
              /// </summary>
                
                  [JsonPropertyName("record")]
                  public WLTRecord? Record { get; set; }
                  
              /// <summary>
              /// Ordered list of values used to determine the rank. See the &#x60;sort_order_info&#x60; property for the name of each value.
              /// </summary>
              /// <value>Ordered list of values used to determine the rank. See the &#x60;sort_order_info&#x60; property for the name of each value.</value>
                
                  [JsonPropertyName("sort_orders")]
                  public Collection<decimal>? SortOrders { get; set; }
                  
              /// <summary>
              /// TBA team key for this rank.
              /// </summary>
              /// <value>TBA team key for this rank.</value>
                
                  [JsonPropertyName("team_key")]
                  public string? TeamKey { get; set; }
                  
              /// <summary>
              /// Returns the string presentation of the object
              /// </summary>
              /// <returns>string presentation of the object</returns>
              public override string ToString()
              {
                StringBuilder sb = new();
                sb.AppendLine("class TeamEventStatusRankRanking {");
                    sb.Append("  Dq: ").AppendLine($"{ Dq?.ToString() ?? "[null]" }");
                    sb.Append("  MatchesPlayed: ").AppendLine($"{ MatchesPlayed?.ToString() ?? "[null]" }");
                    sb.Append("  QualAverage: ").AppendLine($"{ QualAverage?.ToString() ?? "[null]" }");
                    sb.Append("  Rank: ").AppendLine($"{ Rank?.ToString() ?? "[null]" }");
                    sb.Append("  Record: ").AppendLine($"{ Record?.ToString() ?? "[null]" }");
                    sb.Append("  SortOrders: ").AppendLine($"{(SortOrders is null ? "[null]" : string.Join(", ", SortOrders))}");
                    sb.Append("  TeamKey: ").AppendLine($"{ TeamKey?.ToString() ?? "[null]" }");
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
            
