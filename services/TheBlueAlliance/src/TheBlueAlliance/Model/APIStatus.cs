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
/// APIStatus
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record APIStatus
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="APIStatus" /> class.
    /// </summary>
    [JsonConstructor]
    protected APIStatus()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="APIStatus" /> class.
    /// </summary>
    /// <param name="android">android (required).</param>
    /// <param name="currentSeason">Year of the current FRC season. (required).</param>
    /// <param name="downEvents">An array of strings containing event keys of any active events that are no longer updating. (required).</param>
    /// <param name="ios">ios (required).</param>
    /// <param name="isDatafeedDown">True if the entire FMS API provided by FIRST is down. (required).</param>
    /// <param name="maxSeason">Maximum FRC season year for valid queries. (required).</param>
    public APIStatus(APIStatusAppVersion android, int currentSeason, Collection<string> downEvents, APIStatusAppVersion ios, bool isDatafeedDown, int maxSeason)
    {
        // to ensure "android" is required (not null)
        ArgumentNullException.ThrowIfNull(android);
        this.Android = android;

        this.CurrentSeason = currentSeason;
        // to ensure "downEvents" is required (not null)
        ArgumentNullException.ThrowIfNull(downEvents);
        this.DownEvents = downEvents;

        // to ensure "ios" is required (not null)
        ArgumentNullException.ThrowIfNull(ios);
        this.Ios = ios;

        this.IsDatafeedDown = isDatafeedDown;
        this.MaxSeason = maxSeason;
    }

    /// <summary>
    /// Gets or Sets Android
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("android")]
    public APIStatusAppVersion Android { get; set; }

    /// <summary>
    /// Year of the current FRC season.
    /// </summary>
    /// <value>Year of the current FRC season.</value>
    [JsonRequired]
    [JsonPropertyName("current_season")]
    public int CurrentSeason { get; set; }

    /// <summary>
    /// An array of strings containing event keys of any active events that are no longer updating.
    /// </summary>
    /// <value>An array of strings containing event keys of any active events that are no longer updating.</value>
    [JsonRequired]
    [JsonPropertyName("down_events")]
    public Collection<string> DownEvents { get; set; }

    /// <summary>
    /// Gets or Sets Ios
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("ios")]
    public APIStatusAppVersion Ios { get; set; }

    /// <summary>
    /// True if the entire FMS API provided by FIRST is down.
    /// </summary>
    /// <value>True if the entire FMS API provided by FIRST is down.</value>
    [JsonRequired]
    [JsonPropertyName("is_datafeed_down")]
    public bool IsDatafeedDown { get; set; }

    /// <summary>
    /// Maximum FRC season year for valid queries.
    /// </summary>
    /// <value>Maximum FRC season year for valid queries.</value>
    [JsonRequired]
    [JsonPropertyName("max_season")]
    public int MaxSeason { get; set; }

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
