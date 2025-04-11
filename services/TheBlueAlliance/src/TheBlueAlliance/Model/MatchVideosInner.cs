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
/// MatchVideosInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record MatchVideosInner
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchVideosInner" /> class.
    /// </summary>
    [JsonConstructor]
    protected MatchVideosInner()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchVideosInner" /> class.
    /// </summary>
    /// <param name="key">Unique key representing this video (required).</param>
    /// <param name="type">Can be one of &#39;youtube&#39; or &#39;tba&#39; (required).</param>
    public MatchVideosInner(string key, string type)
    {
        // to ensure "key" is required (not null)
        ArgumentNullException.ThrowIfNull(key);
        this.Key = key;

        // to ensure "type" is required (not null)
        ArgumentNullException.ThrowIfNull(type);
        this.Type = type;
    }

    /// <summary>
    /// Unique key representing this video
    /// </summary>
    /// <value>Unique key representing this video</value>
    [JsonRequired]
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// Can be one of &#39;youtube&#39; or &#39;tba&#39;
    /// </summary>
    /// <value>Can be one of &#39;youtube&#39; or &#39;tba&#39;</value>
    [JsonRequired]
    [JsonPropertyName("type")]
    public string Type { get; set; }

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
