/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Api.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// MatchSimple
/// </summary>
public partial class MatchSimple : IValidatableObject
{
    /// <summary>
    /// The competition level the match was played at.
    /// </summary>
    /// <value>The competition level the match was played at.</value>
    [JsonConverter(typeof(JsonStringEnumConverter<CompLevelEnum>)), Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionClassModifiers = "public static", ExtensionClassName = "SimpleCompLevelEnumExtensions")]
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
    /// The competition level the match was played at.
    /// </summary>
    /// <value>The competition level the match was played at.</value>
    [JsonPropertyName("comp_level")]
    public CompLevelEnum CompLevel { get; set; }
    /// <summary>
    /// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
    /// </summary>
    /// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
    [JsonConverter(typeof(JsonStringEnumConverter<WinningAllianceEnum>)), Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionClassName = "SimpleWinningAllianceEnumExtensions")]
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
    /// The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.
    /// </summary>
    /// <value>The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie.</value>
    [JsonPropertyName("winning_alliance")]
    public WinningAllianceEnum WinningAlliance { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchSimple" /> class.
    /// </summary>
    [JsonConstructor]
    protected MatchSimple() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchSimple" /> class.
    /// </summary>
    /// <param name="key">TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may append the competition level if more than one match in required per set. (required).</param>
    /// <param name="compLevel">The competition level the match was played at. (required).</param>
    /// <param name="setNumber">The set number in a series of matches where more than one match is required in the match series. (required).</param>
    /// <param name="matchNumber">The match number of the match in the competition level. (required).</param>
    /// <param name="alliances">alliances (required).</param>
    /// <param name="winningAlliance">The color (red/blue) of the winning alliance. Will contain an empty string in the event of no winner, or a tie. (required).</param>
    /// <param name="eventKey">Event key of the event the match was played at. (required).</param>
    /// <param name="time">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule. (required).</param>
    /// <param name="predictedTime">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time. (required).</param>
    /// <param name="actualTime">UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time. (required).</param>
    public MatchSimple(string? key = default, CompLevelEnum compLevel = default, int setNumber = default, int matchNumber = default, MatchSimpleAlliances? alliances = default, WinningAllianceEnum winningAlliance = default, string? eventKey = default, long? time = default, long? predictedTime = default, long? actualTime = default)
    {
        // to ensure "key" is required (not null)
        ArgumentNullException.ThrowIfNull(key);
        this.Key = key;
        this.CompLevel = compLevel;
        this.SetNumber = setNumber;
        this.MatchNumber = matchNumber;
        // to ensure "alliances" is required (not null)
        ArgumentNullException.ThrowIfNull(alliances);
        this.Alliances = alliances;
        this.WinningAlliance = winningAlliance;
        // to ensure "eventKey" is required (not null)
        ArgumentNullException.ThrowIfNull(eventKey);
        this.EventKey = eventKey;
        // to ensure "time" is required (not null)
        if (time is null)
        {
            throw new ArgumentNullException("time is a required property for MatchSimple and cannot be null");
        }

        this.Time = time;
        // to ensure "predictedTime" is required (not null)
        if (predictedTime is null)
        {
            throw new ArgumentNullException("predictedTime is a required property for MatchSimple and cannot be null");
        }

        this.PredictedTime = predictedTime;
        // to ensure "actualTime" is required (not null)
        if (actualTime is null)
        {
            throw new ArgumentNullException("actualTime is a required property for MatchSimple and cannot be null");
        }

        this.ActualTime = actualTime;
    }

    /// <summary>
    /// TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may append the competition level if more than one match in required per set.
    /// </summary>
    /// <value>TBA match key with the format &#x60;yyyy[EVENT_CODE]_[COMP_LEVEL]m[MATCH_NUMBER]&#x60;, where &#x60;yyyy&#x60; is the year, and &#x60;EVENT_CODE&#x60; is the event code of the event, &#x60;COMP_LEVEL&#x60; is (qm, ef, qf, sf, f), and &#x60;MATCH_NUMBER&#x60; is the match number in the competition level. A set number may append the competition level if more than one match in required per set.</value>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// The set number in a series of matches where more than one match is required in the match series.
    /// </summary>
    /// <value>The set number in a series of matches where more than one match is required in the match series.</value>
    [JsonPropertyName("set_number")]
    public int SetNumber { get; set; }

    /// <summary>
    /// The match number of the match in the competition level.
    /// </summary>
    /// <value>The match number of the match in the competition level.</value>
    [JsonPropertyName("match_number")]
    public int MatchNumber { get; set; }

    /// <summary>
    /// Gets or Sets Alliances
    /// </summary>
    [JsonPropertyName("alliances")]
    public MatchSimpleAlliances Alliances { get; set; }

    /// <summary>
    /// Event key of the event the match was played at.
    /// </summary>
    /// <value>Event key of the event the match was played at.</value>
    [JsonPropertyName("event_key")]
    public string EventKey { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the scheduled match time, as taken from the published schedule.</value>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of the TBA predicted match start time.</value>
    [JsonPropertyName("predicted_time")]
    public long? PredictedTime { get; set; }

    /// <summary>
    /// UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.
    /// </summary>
    /// <value>UNIX timestamp (seconds since 1-Jan-1970 00:00:00) of actual match start time.</value>
    [JsonPropertyName("actual_time")]
    public long? ActualTime { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("class MatchSimple {");
        sb.Append("  Key: ").AppendLine(Key.ToString());
        sb.Append("  CompLevel: ").AppendLine(CompLevel.ToString());
        sb.Append("  SetNumber: ").AppendLine(SetNumber.ToString());
        sb.Append("  MatchNumber: ").AppendLine(MatchNumber.ToString());
        sb.Append("  Alliances: ").AppendLine(Alliances.ToString());
        sb.Append("  WinningAlliance: ").AppendLine(WinningAlliance.ToString());
        sb.Append("  EventKey: ").AppendLine(EventKey.ToString());
        sb.Append("  Time: ").AppendLine(Time.ToString());
        sb.Append("  PredictedTime: ").AppendLine(PredictedTime.ToString());
        sb.Append("  ActualTime: ").AppendLine(ActualTime.ToString());
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public virtual string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// To validate all properties of the instance
    /// </summary>
    /// <param name="validationContext">Validation context</param>
    /// <returns>Validation Result</returns>
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        yield break;
    }
}
