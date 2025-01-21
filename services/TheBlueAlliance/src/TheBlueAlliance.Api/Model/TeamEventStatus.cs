/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Api.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// TeamEventStatus
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TeamEventStatus" /> class.
/// </remarks>
/// <param name="qual">qual.</param>
/// <param name="alliance">alliance.</param>
/// <param name="playoff">playoff.</param>
/// <param name="allianceStatusStr">An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status..</param>
/// <param name="playoffStatusStr">An HTML formatter string suitable for display to the user containing the team&#39;s playoff status..</param>
/// <param name="overallStatusStr">An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event..</param>
/// <param name="nextMatchKey">TBA match key for the next match the team is scheduled to play in at this event, or null..</param>
/// <param name="lastMatchKey">TBA match key for the last match the team played in at this event, or null..</param>
public partial class TeamEventStatus(TeamEventStatusRank? qual = default, TeamEventStatusAlliance? alliance = default, TeamEventStatusPlayoff? playoff = default, string? allianceStatusStr = default, string? playoffStatusStr = default, string? overallStatusStr = default, string? nextMatchKey = default, string? lastMatchKey = default) : IValidatableObject
{

    /// <summary>
    /// Gets or Sets Qual
    /// </summary>

    [JsonPropertyName("qual")]
    public TeamEventStatusRank Qual { get; set; } = qual;

    /// <summary>
    /// Gets or Sets Alliance
    /// </summary>

    [JsonPropertyName("alliance")]
    public TeamEventStatusAlliance Alliance { get; set; } = alliance;

    /// <summary>
    /// Gets or Sets Playoff
    /// </summary>

    [JsonPropertyName("playoff")]
    public TeamEventStatusPlayoff Playoff { get; set; } = playoff;

    /// <summary>
    /// An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status.
    /// </summary>
    /// <value>An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status.</value>

    [JsonPropertyName("alliance_status_str")]
    public string AllianceStatusStr { get; set; } = allianceStatusStr;

    /// <summary>
    /// An HTML formatter string suitable for display to the user containing the team&#39;s playoff status.
    /// </summary>
    /// <value>An HTML formatter string suitable for display to the user containing the team&#39;s playoff status.</value>

    [JsonPropertyName("playoff_status_str")]
    public string PlayoffStatusStr { get; set; } = playoffStatusStr;

    /// <summary>
    /// An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event.
    /// </summary>
    /// <value>An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event.</value>

    [JsonPropertyName("overall_status_str")]
    public string OverallStatusStr { get; set; } = overallStatusStr;

    /// <summary>
    /// TBA match key for the next match the team is scheduled to play in at this event, or null.
    /// </summary>
    /// <value>TBA match key for the next match the team is scheduled to play in at this event, or null.</value>

    [JsonPropertyName("next_match_key")]
    public string NextMatchKey { get; set; } = nextMatchKey;

    /// <summary>
    /// TBA match key for the last match the team played in at this event, or null.
    /// </summary>
    /// <value>TBA match key for the last match the team played in at this event, or null.</value>

    [JsonPropertyName("last_match_key")]
    public string LastMatchKey { get; set; } = lastMatchKey;

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("class TeamEventStatus {");
        sb.Append("  Qual: ").AppendLine(Qual.ToString());
        sb.Append("  Alliance: ").AppendLine(Alliance.ToString());
        sb.Append("  Playoff: ").AppendLine(Playoff.ToString());
        sb.Append("  AllianceStatusStr: ").AppendLine(AllianceStatusStr.ToString());
        sb.Append("  PlayoffStatusStr: ").AppendLine(PlayoffStatusStr.ToString());
        sb.Append("  OverallStatusStr: ").AppendLine(OverallStatusStr.ToString());
        sb.Append("  NextMatchKey: ").AppendLine(NextMatchKey.ToString());
        sb.Append("  LastMatchKey: ").AppendLine(LastMatchKey.ToString());
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
