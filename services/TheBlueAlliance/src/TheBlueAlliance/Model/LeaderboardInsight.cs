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
/// LeaderboardInsight
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record LeaderboardInsight
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardInsight" /> class.
    /// </summary>
    [JsonConstructor]
    protected LeaderboardInsight()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardInsight" /> class.
    /// </summary>
    /// <param name="data">data (required).</param>
    /// <param name="name">Name of the insight. (required).</param>
    /// <param name="year">Year the insight was measured in (year&#x3D;0 for overall insights). (required).</param>
    public LeaderboardInsight(LeaderboardInsightData data, string name, int year)
    {
        // to ensure "data" is required (not null)
        ArgumentNullException.ThrowIfNull(data);
        this.Data = data;

        // to ensure "name" is required (not null)
        ArgumentNullException.ThrowIfNull(name);
        this.Name = name;

        this.Year = year;
    }

    /// <summary>
    /// Gets or Sets Data
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("data")]
    public LeaderboardInsightData Data { get; set; }

    /// <summary>
    /// Name of the insight.
    /// </summary>
    /// <value>Name of the insight.</value>
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Year the insight was measured in (year&#x3D;0 for overall insights).
    /// </summary>
    /// <value>Year the insight was measured in (year&#x3D;0 for overall insights).</value>
    [JsonRequired]
    [JsonPropertyName("year")]
    public int Year { get; set; }

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
