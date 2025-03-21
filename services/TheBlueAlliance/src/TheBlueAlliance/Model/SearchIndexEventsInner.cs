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
/// SearchIndexEventsInner
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record SearchIndexEventsInner
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndexEventsInner" /> class.
    /// </summary>
    [JsonConstructor]
    protected SearchIndexEventsInner()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndexEventsInner" /> class.
    /// </summary>
    /// <param name="key">key (required).</param>
    /// <param name="name">name (required).</param>
    public SearchIndexEventsInner(string key, string name)
    {
        // to ensure "key" is required (not null)
        ArgumentNullException.ThrowIfNull(key);
        this.Key = key;

        // to ensure "name" is required (not null)
        ArgumentNullException.ThrowIfNull(name);
        this.Name = name;
    }

    /// <summary>
    /// Gets or Sets Key
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// Gets or Sets Name
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("name")]
    public string Name { get; set; }

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
