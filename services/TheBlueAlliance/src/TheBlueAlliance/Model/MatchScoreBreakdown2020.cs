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
/// See the 2020 FMS API documentation for a description of each value. https://frcevents2.docs.apiary.io/#/reference/match-results/score-details
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record MatchScoreBreakdown2020
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2020" /> class.
    /// </summary>
    [JsonConstructor]
    protected MatchScoreBreakdown2020()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2020" /> class.
    /// </summary>
    /// <param name="blue">blue (required).</param>
    /// <param name="red">red (required).</param>
    public MatchScoreBreakdown2020(MatchScoreBreakdown2020Alliance blue, MatchScoreBreakdown2020Alliance red)
    {
        // to ensure "blue" is required (not null)
        ArgumentNullException.ThrowIfNull(blue);
        this.Blue = blue;

        // to ensure "red" is required (not null)
        ArgumentNullException.ThrowIfNull(red);
        this.Red = red;
    }

    /// <summary>
    /// Gets or Sets Blue
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("blue")]
    public MatchScoreBreakdown2020Alliance Blue { get; set; }

    /// <summary>
    /// Gets or Sets Red
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("red")]
    public MatchScoreBreakdown2020Alliance Red { get; set; }

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
