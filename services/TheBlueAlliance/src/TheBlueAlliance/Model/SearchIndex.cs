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
/// SearchIndex
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record SearchIndex
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndex" /> class.
    /// </summary>
    [JsonConstructor]
    protected SearchIndex()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndex" /> class.
    /// </summary>
    /// <param name="events">events (required).</param>
    /// <param name="teams">teams (required).</param>
    public SearchIndex(Collection<SearchIndexEventsInner> events, Collection<SearchIndexTeamsInner> teams)
    {
        // to ensure "events" is required (not null)
        ArgumentNullException.ThrowIfNull(events);
        this.Events = events;

        // to ensure "teams" is required (not null)
        ArgumentNullException.ThrowIfNull(teams);
        this.Teams = teams;
    }

    /// <summary>
    /// Gets or Sets Events
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("events")]
    public Collection<SearchIndexEventsInner> Events { get; set; }

    /// <summary>
    /// Gets or Sets Teams
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("teams")]
    public Collection<SearchIndexTeamsInner> Teams { get; set; }

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
