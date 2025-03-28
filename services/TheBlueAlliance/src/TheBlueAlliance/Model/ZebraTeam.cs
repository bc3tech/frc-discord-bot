/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// ZebraTeam
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record ZebraTeam
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="ZebraTeam" /> class.
    /// </summary>
    [JsonConstructor]
    protected ZebraTeam()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZebraTeam" /> class.
    /// </summary>
    /// <param name="teamKey">The TBA team key for the Zebra MotionWorks data. (required).</param>
    /// <param name="xs">A list containing doubles and nulls representing a teams X position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp. (required).</param>
    /// <param name="ys">A list containing doubles and nulls representing a teams Y position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp. (required).</param>
    public ZebraTeam(string teamKey, Collection<double> xs, Collection<double> ys)
    {
        // to ensure "teamKey" is required (not null)
        ArgumentNullException.ThrowIfNull(teamKey);
        this.TeamKey = teamKey;

        // to ensure "xs" is required (not null)
        ArgumentNullException.ThrowIfNull(xs);
        this.Xs = xs;

        // to ensure "ys" is required (not null)
        ArgumentNullException.ThrowIfNull(ys);
        this.Ys = ys;
    }

    /// <summary>
    /// The TBA team key for the Zebra MotionWorks data.
    /// </summary>
    /// <value>The TBA team key for the Zebra MotionWorks data.</value>
    /// <example>frc7332</example>
    [JsonRequired]
    [JsonPropertyName("team_key")]
    public string TeamKey { get; set; }

    /// <summary>
    /// A list containing doubles and nulls representing a teams X position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.
    /// </summary>
    /// <value>A list containing doubles and nulls representing a teams X position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.</value>
    [JsonRequired]
    [JsonPropertyName("xs")]
    public Collection<double> Xs { get; set; }

    /// <summary>
    /// A list containing doubles and nulls representing a teams Y position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.
    /// </summary>
    /// <value>A list containing doubles and nulls representing a teams Y position in feet at the corresponding timestamp. A null value represents no tracking data for a given timestamp.</value>
    [JsonRequired]
    [JsonPropertyName("ys")]
    public Collection<double> Ys { get; set; }

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
