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
/// DistrictRankingEventPointsInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record DistrictRankingEventPointsInner
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="DistrictRankingEventPointsInner" /> class.
    /// </summary>
    [JsonConstructor]
    protected DistrictRankingEventPointsInner()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DistrictRankingEventPointsInner" /> class.
    /// </summary>
    /// <param name="alliancePoints">Points awarded for alliance selection. (required).</param>
    /// <param name="awardPoints">Points awarded for event awards. (required).</param>
    /// <param name="districtCmp">&#x60;true&#x60; if this event is a District Championship event. (required).</param>
    /// <param name="elimPoints">Points awarded for elimination match performance. (required).</param>
    /// <param name="eventKey">TBA Event key for this event. (required).</param>
    /// <param name="qualPoints">Points awarded for qualification match performance. (required).</param>
    /// <param name="total">Total points awarded at this event. (required).</param>
    public DistrictRankingEventPointsInner(int alliancePoints, int awardPoints, bool districtCmp, int elimPoints, string eventKey, int qualPoints, int total)
    {
        this.AlliancePoints = alliancePoints;
        this.AwardPoints = awardPoints;
        this.DistrictCmp = districtCmp;
        this.ElimPoints = elimPoints;
        // to ensure "eventKey" is required (not null)
        ArgumentNullException.ThrowIfNull(eventKey);
        this.EventKey = eventKey;

        this.QualPoints = qualPoints;
        this.Total = total;
    }

    /// <summary>
    /// Points awarded for alliance selection.
    /// </summary>
    /// <value>Points awarded for alliance selection.</value>
    [JsonRequired]
    [JsonPropertyName("alliance_points")]
    public int AlliancePoints { get; set; }

    /// <summary>
    /// Points awarded for event awards.
    /// </summary>
    /// <value>Points awarded for event awards.</value>
    [JsonRequired]
    [JsonPropertyName("award_points")]
    public int AwardPoints { get; set; }

    /// <summary>
    /// &#x60;true&#x60; if this event is a District Championship event.
    /// </summary>
    /// <value>&#x60;true&#x60; if this event is a District Championship event.</value>
    [JsonRequired]
    [JsonPropertyName("district_cmp")]
    public bool DistrictCmp { get; set; }

    /// <summary>
    /// Points awarded for elimination match performance.
    /// </summary>
    /// <value>Points awarded for elimination match performance.</value>
    [JsonRequired]
    [JsonPropertyName("elim_points")]
    public int ElimPoints { get; set; }

    /// <summary>
    /// TBA Event key for this event.
    /// </summary>
    /// <value>TBA Event key for this event.</value>
    [JsonRequired]
    [JsonPropertyName("event_key")]
    public string EventKey { get; set; }

    /// <summary>
    /// Points awarded for qualification match performance.
    /// </summary>
    /// <value>Points awarded for qualification match performance.</value>
    [JsonRequired]
    [JsonPropertyName("qual_points")]
    public int QualPoints { get; set; }

    /// <summary>
    /// Total points awarded at this event.
    /// </summary>
    /// <value>Total points awarded at this event.</value>
    [JsonRequired]
    [JsonPropertyName("total")]
    public int Total { get; set; }

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
