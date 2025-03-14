/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// TeamEventStatus
/// </summary>

public partial record TeamEventStatus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamEventStatus" /> class.
    /// </summary>
    /// <param name="alliance">alliance.</param>
    /// <param name="allianceStatusStr">An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status.</param>
    /// <param name="lastMatchKey">TBA match key for the last match the team played in at this event, or null.</param>
    /// <param name="nextMatchKey">TBA match key for the next match the team is scheduled to play in at this event, or null.</param>
    /// <param name="overallStatusStr">An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event.</param>
    /// <param name="playoff">playoff.</param>
    /// <param name="playoffStatusStr">An HTML formatter string suitable for display to the user containing the team&#39;s playoff status.</param>
    /// <param name="qual">qual.</param>
    public TeamEventStatus(TeamEventStatusAlliance? alliance = default, string? allianceStatusStr = default, string? lastMatchKey = default, string? nextMatchKey = default, string? overallStatusStr = default, TeamEventStatusPlayoff? playoff = default, string? playoffStatusStr = default, TeamEventStatusRank? qual = default)
    {
        this.Alliance = alliance;
        this.AllianceStatusStr = allianceStatusStr;
        this.LastMatchKey = lastMatchKey;
        this.NextMatchKey = nextMatchKey;
        this.OverallStatusStr = overallStatusStr;
        this.Playoff = playoff;
        this.PlayoffStatusStr = playoffStatusStr;
        this.Qual = qual;
    }

    /// <summary>
    /// Gets or Sets Alliance
    /// </summary>

    [JsonPropertyName("alliance")]
    public TeamEventStatusAlliance? Alliance { get; set; }

    /// <summary>
    /// An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status.
    /// </summary>
    /// <value>An HTML formatted string suitable for display to the user containing the team&#39;s alliance pick status.</value>

    [JsonPropertyName("alliance_status_str")]
    public string? AllianceStatusStr { get; set; }

    /// <summary>
    /// TBA match key for the last match the team played in at this event, or null.
    /// </summary>
    /// <value>TBA match key for the last match the team played in at this event, or null.</value>

    [JsonPropertyName("last_match_key")]
    public string? LastMatchKey { get; set; }

    /// <summary>
    /// TBA match key for the next match the team is scheduled to play in at this event, or null.
    /// </summary>
    /// <value>TBA match key for the next match the team is scheduled to play in at this event, or null.</value>

    [JsonPropertyName("next_match_key")]
    public string? NextMatchKey { get; set; }

    /// <summary>
    /// An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event.
    /// </summary>
    /// <value>An HTML formatted string suitable for display to the user containing the team&#39;s overall status summary of the event.</value>

    [JsonPropertyName("overall_status_str")]
    public string? OverallStatusStr { get; set; }

    /// <summary>
    /// Gets or Sets Playoff
    /// </summary>

    [JsonPropertyName("playoff")]
    public TeamEventStatusPlayoff? Playoff { get; set; }

    /// <summary>
    /// An HTML formatter string suitable for display to the user containing the team&#39;s playoff status.
    /// </summary>
    /// <value>An HTML formatter string suitable for display to the user containing the team&#39;s playoff status.</value>

    [JsonPropertyName("playoff_status_str")]
    public string? PlayoffStatusStr { get; set; }

    /// <summary>
    /// Gets or Sets Qual
    /// </summary>

    [JsonPropertyName("qual")]
    public TeamEventStatusRank? Qual { get; set; }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}

