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
/// EventRankingRankingsInner
/// </summary>
internal partial class EventRankingRankingsInner
{
            // yup
            /// <summary>
            /// Initializes a new instance of the <see cref="EventRankingRankingsInner" /> class.
            /// </summary>
            [JsonConstructor]
            protected EventRankingRankingsInner() { 
          }
          
      /// <summary>
      /// Initializes a new instance of the <see cref="EventRankingRankingsInner" /> class.
      /// </summary>
          /// <param name="dq">Number of times disqualified. (required).</param>
          /// <param name="extraStats">Additional special data on the team&#39;s performance calculated by TBA. (required).</param>
          /// <param name="matchesPlayed">Number of matches played by this team. (required).</param>
          /// <param name="qualAverage">The average match score during qualifications. Year specific. May be null if not relevant for a given year. (required).</param>
          /// <param name="rank">The team&#39;s rank at the event as provided by FIRST. (required).</param>
          /// <param name="record">record (required).</param>
          /// <param name="sortOrders">Additional year-specific information, may be null. See parent &#x60;sort_order_info&#x60; for details. (required).</param>
          /// <param name="teamKey">The team with this rank. (required).</param>
      public EventRankingRankingsInner(int dq, Collection<decimal> extraStats, int matchesPlayed, int? qualAverage, int rank, WLTRecord record, Collection<decimal> sortOrders, string teamKey)
      {
                    this.Dq = dq;
                    // to ensure "extraStats" is required (not null)
                    ArgumentNullException.ThrowIfNull(extraStats);
                    this.ExtraStats = extraStats;
                      
                    this.MatchesPlayed = matchesPlayed;
                    // to ensure "qualAverage" is required (not null)
                    ArgumentNullException.ThrowIfNull(qualAverage);
                    this.QualAverage = qualAverage;
                      
                    this.Rank = rank;
                    // to ensure "record" is required (not null)
                    ArgumentNullException.ThrowIfNull(record);
                    this.Record = record;
                      
                    // to ensure "sortOrders" is required (not null)
                    ArgumentNullException.ThrowIfNull(sortOrders);
                    this.SortOrders = sortOrders;
                      
                    // to ensure "teamKey" is required (not null)
                    ArgumentNullException.ThrowIfNull(teamKey);
                    this.TeamKey = teamKey;
      }
      
            /// <summary>
            /// Number of times disqualified.
            /// </summary>
            /// <value>Number of times disqualified.</value>
              [JsonRequired]
                [JsonPropertyName("dq")]
                public int Dq { get; set; }
                
            /// <summary>
            /// Additional special data on the team&#39;s performance calculated by TBA.
            /// </summary>
            /// <value>Additional special data on the team&#39;s performance calculated by TBA.</value>
              [JsonRequired]
                [JsonPropertyName("extra_stats")]
                public Collection<decimal> ExtraStats { get; set; }
                
            /// <summary>
            /// Number of matches played by this team.
            /// </summary>
            /// <value>Number of matches played by this team.</value>
              [JsonRequired]
                [JsonPropertyName("matches_played")]
                public int MatchesPlayed { get; set; }
                
            /// <summary>
            /// The average match score during qualifications. Year specific. May be null if not relevant for a given year.
            /// </summary>
            /// <value>The average match score during qualifications. Year specific. May be null if not relevant for a given year.</value>
              [JsonRequired]
                [JsonPropertyName("qual_average")]
                public int? QualAverage { get; set; }
                
            /// <summary>
            /// The team&#39;s rank at the event as provided by FIRST.
            /// </summary>
            /// <value>The team&#39;s rank at the event as provided by FIRST.</value>
              [JsonRequired]
                [JsonPropertyName("rank")]
                public int Rank { get; set; }
                
            /// <summary>
            /// Gets or Sets Record
            /// </summary>
              [JsonRequired]
                [JsonPropertyName("record")]
                public WLTRecord Record { get; set; }
                
            /// <summary>
            /// Additional year-specific information, may be null. See parent &#x60;sort_order_info&#x60; for details.
            /// </summary>
            /// <value>Additional year-specific information, may be null. See parent &#x60;sort_order_info&#x60; for details.</value>
              [JsonRequired]
                [JsonPropertyName("sort_orders")]
                public Collection<decimal> SortOrders { get; set; }
                
            /// <summary>
            /// The team with this rank.
            /// </summary>
            /// <value>The team with this rank.</value>
              [JsonRequired]
                [JsonPropertyName("team_key")]
                public string TeamKey { get; set; }
                
            /// <summary>
            /// Returns the string presentation of the object
            /// </summary>
            /// <returns>string presentation of the object</returns>
            public override string ToString()
            {
              StringBuilder sb = new();
              sb.AppendLine("class EventRankingRankingsInner {");
                  sb.Append("  Dq: ").AppendLine($"{ Dq }");
                  sb.Append("  ExtraStats: ").AppendLine($"{(ExtraStats is null ? "[null]" : string.Join(", ", ExtraStats))}");
                  sb.Append("  MatchesPlayed: ").AppendLine($"{ MatchesPlayed }");
                  sb.Append("  QualAverage: ").AppendLine($"{ QualAverage }");
                  sb.Append("  Rank: ").AppendLine($"{ Rank }");
                  sb.Append("  Record: ").AppendLine($"{ Record }");
                  sb.Append("  SortOrders: ").AppendLine($"{(SortOrders is null ? "[null]" : string.Join(", ", SortOrders))}");
                  sb.Append("  TeamKey: ").AppendLine($"{ TeamKey }");
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
          
