/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

    namespace TheBlueAlliance.Model;
    
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
  using System.Collections.ObjectModel;
    
    /// <summary>
/// Match
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public partial record Match
  {
            /// <summary>
  /// The competition level the match was played at.
  /// </summary>
    /// <value>The competition level the match was played at.</value>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<CompLevelEnum>))]
  public enum CompLevelEnum
  {
        /// <summary>
        /// Enum Qm for value: qm
        /// </summary>
        [EnumMember(Value = "qm")]
        Qm = 1,
          
        /// <summary>
        /// Enum Ef for value: ef
        /// </summary>
        [EnumMember(Value = "ef")]
        Ef = 2,
          
        /// <summary>
        /// Enum Qf for value: qf
        /// </summary>
        [EnumMember(Value = "qf")]
        Qf = 3,
          
        /// <summary>
        /// Enum Sf for value: sf
        /// </summary>
        [EnumMember(Value = "sf")]
        Sf = 4,
          
        /// <summary>
        /// Enum F for value: f
        /// </summary>
        [EnumMember(Value = "f")]
        F = 5
  }
    
    /// <summary>
    /// Returns a <see cref="CompLevelEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompLevelEnum? CompLevelEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "qm" => CompLevelEnum.Qm,
            "ef" => CompLevelEnum.Ef,
            "qf" => CompLevelEnum.Qf,
            "sf" => CompLevelEnum.Sf,
            "f" => CompLevelEnum.F,
        _ => null
      };
    }

        
        /// <summary>
        /// The competition level the match was played at.
        /// </summary>
          /// <value>The competition level the match was played at.</value>
          [JsonRequired]
            [JsonPropertyName("comp_level")]
            public CompLevelEnum CompLevel { get; set; }
            /// <summary>
  /// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
  /// </summary>
    /// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
  [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchExtensions", ExtensionClassModifiers ="public static")]
  [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<WinningAllianceEnum>))]
  public enum WinningAllianceEnum
  {
        /// <summary>
        /// Enum Red for value: red
        /// </summary>
        [EnumMember(Value = "red")]
        Red = 1,
          
        /// <summary>
        /// Enum Blue for value: blue
        /// </summary>
        [EnumMember(Value = "blue")]
        Blue = 2,
          
        /// <summary>
        /// Enum Empty for value: 
        /// </summary>
        [EnumMember(Value = "")]
        Empty = 3
  }
    
    /// <summary>
    /// Returns a <see cref="WinningAllianceEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static WinningAllianceEnum? WinningAllianceEnumFromStringOrDefault(string value)
    {
      return value switch
      {
            "red" => WinningAllianceEnum.Red,
            "blue" => WinningAllianceEnum.Blue,
            "" => WinningAllianceEnum.Empty,
        _ => null
      };
    }

        
        /// <summary>
        /// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
        /// </summary>
          /// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
          [JsonRequired]
            [JsonPropertyName("winning_alliance")]
            public WinningAllianceEnum WinningAlliance { get; set; }
              // yup
              /// <summary>
              /// Initializes a new instance of the <see cref="Match" /> class.
              /// </summary>
              [JsonConstructor]
              protected Match() { 
            }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Match" /> class.
        /// </summary>
            /// <param name="actualTime">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time. (required).</param>
            /// <param name="alliances">alliances (required).</param>
            /// <param name="compLevel">The competition level the match was played at. (required).</param>
            /// <param name="eventKey">Event key of the event the match was played at. (required).</param>
            /// <param name="key">TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set. (required).</param>
            /// <param name="matchNumber">The match number of the match in the competition level. (required).</param>
            /// <param name="postResultTime">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted. (required).</param>
            /// <param name="predictedTime">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time. (required).</param>
            /// <param name="scoreBreakdown">scoreBreakdown (required).</param>
            /// <param name="setNumber">The set number in a series of matches where more than one match is required in the match series. (required).</param>
            /// <param name="time">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule. (required).</param>
            /// <param name="videos">Array of video objects associated with this match. (required).</param>
            /// <param name="winningAlliance">The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie. (required).</param>
        public Match(long? actualTime, MatchSimpleAlliances alliances, CompLevelEnum compLevel, string eventKey, string key, int matchNumber, long? postResultTime, long? predictedTime, MatchScoreBreakdown scoreBreakdown, int setNumber, long? time, Collection<MatchVideosInner> videos, WinningAllianceEnum winningAlliance)
        {
                      // to ensure "actualTime" is required (not null)
                      ArgumentNullException.ThrowIfNull(actualTime);
                      this.ActualTime = actualTime;
                        
                      // to ensure "alliances" is required (not null)
                      ArgumentNullException.ThrowIfNull(alliances);
                      this.Alliances = alliances;
                        
                      this.CompLevel = compLevel;
                      // to ensure "eventKey" is required (not null)
                      ArgumentNullException.ThrowIfNull(eventKey);
                      this.EventKey = eventKey;
                        
                      // to ensure "key" is required (not null)
                      ArgumentNullException.ThrowIfNull(key);
                      this.Key = key;
                        
                      this.MatchNumber = matchNumber;
                      // to ensure "postResultTime" is required (not null)
                      ArgumentNullException.ThrowIfNull(postResultTime);
                      this.PostResultTime = postResultTime;
                        
                      // to ensure "predictedTime" is required (not null)
                      ArgumentNullException.ThrowIfNull(predictedTime);
                      this.PredictedTime = predictedTime;
                        
                      // to ensure "scoreBreakdown" is required (not null)
                      ArgumentNullException.ThrowIfNull(scoreBreakdown);
                      this.ScoreBreakdown = scoreBreakdown;
                        
                      this.SetNumber = setNumber;
                      // to ensure "time" is required (not null)
                      ArgumentNullException.ThrowIfNull(time);
                      this.Time = time;
                        
                      // to ensure "videos" is required (not null)
                      ArgumentNullException.ThrowIfNull(videos);
                      this.Videos = videos;
                        
                      this.WinningAlliance = winningAlliance;
        }
        
              /// <summary>
              /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.
              /// </summary>
              /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.</value>
                [JsonRequired]
                  [JsonPropertyName("actual_time")]
                  public long? ActualTime { get; set; }
                  
              /// <summary>
              /// Gets or Sets Alliances
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("alliances")]
                  public MatchSimpleAlliances Alliances { get; set; }
                  
              /// <summary>
              /// Event key of the event the match was played at.
              /// </summary>
              /// <value>Event key of the event the match was played at.</value>
                [JsonRequired]
                  [JsonPropertyName("event_key")]
                  public string EventKey { get; set; }
                  
              /// <summary>
              /// TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.
              /// </summary>
              /// <value>TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may be appended to the competition level if more than one match in required per set.</value>
                [JsonRequired]
                  [JsonPropertyName("key")]
                  public string Key { get; set; }
                  
              /// <summary>
              /// The match number of the match in the competition level.
              /// </summary>
              /// <value>The match number of the match in the competition level.</value>
                [JsonRequired]
                  [JsonPropertyName("match_number")]
                  public int MatchNumber { get; set; }
                  
              /// <summary>
              /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted.
              /// </summary>
              /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) when the match result was posted.</value>
                [JsonRequired]
                  [JsonPropertyName("post_result_time")]
                  public long? PostResultTime { get; set; }
                  
              /// <summary>
              /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.
              /// </summary>
              /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.</value>
                [JsonRequired]
                  [JsonPropertyName("predicted_time")]
                  public long? PredictedTime { get; set; }
                  
              /// <summary>
              /// Gets or Sets ScoreBreakdown
              /// </summary>
                [JsonRequired]
                  [JsonPropertyName("score_breakdown")]
                  public MatchScoreBreakdown ScoreBreakdown { get; set; }
                  
              /// <summary>
              /// The set number in a series of matches where more than one match is required in the match series.
              /// </summary>
              /// <value>The set number in a series of matches where more than one match is required in the match series.</value>
                [JsonRequired]
                  [JsonPropertyName("set_number")]
                  public int SetNumber { get; set; }
                  
              /// <summary>
              /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.
              /// </summary>
              /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.</value>
                [JsonRequired]
                  [JsonPropertyName("time")]
                  public long? Time { get; set; }
                  
              /// <summary>
              /// Array of video objects associated with this match.
              /// </summary>
              /// <value>Array of video objects associated with this match.</value>
                [JsonRequired]
                  [JsonPropertyName("videos")]
                  public Collection<MatchVideosInner> Videos { get; set; }
                  
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
