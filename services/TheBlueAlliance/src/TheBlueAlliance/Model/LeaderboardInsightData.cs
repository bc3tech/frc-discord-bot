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
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// LeaderboardInsightData
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record LeaderboardInsightData
{
    /// <summary>
    /// What type of key is used in the rankings; either &#39;team&#39;, &#39;event&#39;, or &#39;match&#39;.
    /// </summary>
    /// <value>What type of key is used in the rankings; either &#39;team&#39;, &#39;event&#39;, or &#39;match&#39;.</value>
    [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.LeaderboardInsightDataExtensions", ExtensionClassModifiers = "public static")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<KeyTypeEnum>))]
    public enum KeyTypeEnum
    {
        /// <summary>
        /// Enum Team for value: team
        /// </summary>
        [EnumMember(Value = "team")]
        Team = 1,

        /// <summary>
        /// Enum Event for value: event
        /// </summary>
        [EnumMember(Value = "event")]
        Event = 2,

        /// <summary>
        /// Enum Match for value: match
        /// </summary>
        [EnumMember(Value = "match")]
        Match = 3
    }

    /// <summary>
    /// Returns a <see cref="KeyTypeEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static KeyTypeEnum? KeyTypeEnumFromStringOrDefault(string value)
    {
        return value switch
        {
            "team" => KeyTypeEnum.Team,
            "event" => KeyTypeEnum.Event,
            "match" => KeyTypeEnum.Match,
            _ => null
        };
    }

    /// <summary>
    /// What type of key is used in the rankings; either &#39;team&#39;, &#39;event&#39;, or &#39;match&#39;.
    /// </summary>
    /// <value>What type of key is used in the rankings; either &#39;team&#39;, &#39;event&#39;, or &#39;match&#39;.</value>
    [JsonRequired]
    [JsonPropertyName("key_type")]
    public KeyTypeEnum KeyType { get; set; }
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardInsightData" /> class.
    /// </summary>
    [JsonConstructor]
    protected LeaderboardInsightData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardInsightData" /> class.
    /// </summary>
    /// <param name="keyType">What type of key is used in the rankings; either &#39;team&#39;, &#39;event&#39;, or &#39;match&#39;. (required).</param>
    /// <param name="rankings">rankings (required).</param>
    public LeaderboardInsightData(KeyTypeEnum keyType, Collection<LeaderboardInsightDataRankingsInner> rankings)
    {
        this.KeyType = keyType;
        // to ensure "rankings" is required (not null)
        ArgumentNullException.ThrowIfNull(rankings);
        this.Rankings = rankings;
    }

    /// <summary>
    /// Gets or Sets Rankings
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("rankings")]
    public Collection<LeaderboardInsightDataRankingsInner> Rankings { get; set; }

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
