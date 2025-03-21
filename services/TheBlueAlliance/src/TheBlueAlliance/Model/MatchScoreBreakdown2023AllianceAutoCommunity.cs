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
/// MatchScoreBreakdown2023AllianceAutoCommunity
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record MatchScoreBreakdown2023AllianceAutoCommunity
{
    /// <summary>
    /// Defines B
    /// </summary>
    [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2023AllianceAutoCommunityExtensions", ExtensionClassModifiers = "public static")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<BEnum>))]
    public enum BEnum
    {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 1,

        /// <summary>
        /// Enum Cone for value: Cone
        /// </summary>
        [EnumMember(Value = "Cone")]
        Cone = 2,

        /// <summary>
        /// Enum Cube for value: Cube
        /// </summary>
        [EnumMember(Value = "Cube")]
        Cube = 3
    }

    /// <summary>
    /// Returns a <see cref="BEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static BEnum? BEnumFromStringOrDefault(string value)
    {
        return value switch
        {
            "None" => BEnum.None,
            "Cone" => BEnum.Cone,
            "Cube" => BEnum.Cube,
            _ => null
        };
    }

    /// <summary>
    /// Defines M
    /// </summary>
    [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2023AllianceAutoCommunityExtensions", ExtensionClassModifiers = "public static")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<MEnum>))]
    public enum MEnum
    {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 1,

        /// <summary>
        /// Enum Cone for value: Cone
        /// </summary>
        [EnumMember(Value = "Cone")]
        Cone = 2,

        /// <summary>
        /// Enum Cube for value: Cube
        /// </summary>
        [EnumMember(Value = "Cube")]
        Cube = 3
    }

    /// <summary>
    /// Returns a <see cref="MEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static MEnum? MEnumFromStringOrDefault(string value)
    {
        return value switch
        {
            "None" => MEnum.None,
            "Cone" => MEnum.Cone,
            "Cube" => MEnum.Cube,
            _ => null
        };
    }

    /// <summary>
    /// Defines T
    /// </summary>
    [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MatchScoreBreakdown2023AllianceAutoCommunityExtensions", ExtensionClassModifiers = "public static")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<TEnum>))]
    public enum TEnum
    {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 1,

        /// <summary>
        /// Enum Cone for value: Cone
        /// </summary>
        [EnumMember(Value = "Cone")]
        Cone = 2,

        /// <summary>
        /// Enum Cube for value: Cube
        /// </summary>
        [EnumMember(Value = "Cube")]
        Cube = 3
    }

    /// <summary>
    /// Returns a <see cref="TEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TEnum? TEnumFromStringOrDefault(string value)
    {
        return value switch
        {
            "None" => TEnum.None,
            "Cone" => TEnum.Cone,
            "Cube" => TEnum.Cube,
            _ => null
        };
    }

    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2023AllianceAutoCommunity" /> class.
    /// </summary>
    [JsonConstructor]
    protected MatchScoreBreakdown2023AllianceAutoCommunity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2023AllianceAutoCommunity" /> class.
    /// </summary>
    /// <param name="b">b (required).</param>
    /// <param name="m">m (required).</param>
    /// <param name="t">t (required).</param>
    public MatchScoreBreakdown2023AllianceAutoCommunity(Collection<BEnum> b, Collection<MEnum> m, Collection<TEnum> t)
    {
        // to ensure "b" is required (not null)
        ArgumentNullException.ThrowIfNull(b);
        this.B = b;

        // to ensure "m" is required (not null)
        ArgumentNullException.ThrowIfNull(m);
        this.M = m;

        // to ensure "t" is required (not null)
        ArgumentNullException.ThrowIfNull(t);
        this.T = t;
    }

    /// <summary>
    /// Gets or Sets B
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("B")]
    public Collection<BEnum> B { get; set; }

    /// <summary>
    /// Gets or Sets M
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("M")]
    public Collection<MEnum> M { get; set; }

    /// <summary>
    /// Gets or Sets T
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("T")]
    public Collection<TEnum> T { get; set; }

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
