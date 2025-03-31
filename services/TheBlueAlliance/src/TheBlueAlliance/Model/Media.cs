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
/// The &#x60;Media&#x60; object contains a reference for most any media associated with a team or event on TBA.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial record Media
{
    /// <summary>
    /// String type of the media element.
    /// </summary>
    /// <value>String type of the media element.</value>
    [Microsoft.Extensions.EnumStrings.EnumStrings(ExtensionNamespace = "TheBlueAlliance.Model.MediaExtensions", ExtensionClassModifiers = "public static")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberSupport<TypeEnum>))]
    public enum TypeEnum
    {
        /// <summary>
        /// Enum Youtube for value: youtube
        /// </summary>
        [EnumMember(Value = "youtube")]
        Youtube = 1,

        /// <summary>
        /// Enum Cdphotothread for value: cdphotothread
        /// </summary>
        [EnumMember(Value = "cdphotothread")]
        Cdphotothread = 2,

        /// <summary>
        /// Enum Imgur for value: imgur
        /// </summary>
        [EnumMember(Value = "imgur")]
        Imgur = 3,

        /// <summary>
        /// Enum FacebookProfile for value: facebook-profile
        /// </summary>
        [EnumMember(Value = "facebook-profile")]
        FacebookProfile = 4,

        /// <summary>
        /// Enum YoutubeChannel for value: youtube-channel
        /// </summary>
        [EnumMember(Value = "youtube-channel")]
        YoutubeChannel = 5,

        /// <summary>
        /// Enum TwitterProfile for value: twitter-profile
        /// </summary>
        [EnumMember(Value = "twitter-profile")]
        TwitterProfile = 6,

        /// <summary>
        /// Enum GithubProfile for value: github-profile
        /// </summary>
        [EnumMember(Value = "github-profile")]
        GithubProfile = 7,

        /// <summary>
        /// Enum InstagramProfile for value: instagram-profile
        /// </summary>
        [EnumMember(Value = "instagram-profile")]
        InstagramProfile = 8,

        /// <summary>
        /// Enum PeriscopeProfile for value: periscope-profile
        /// </summary>
        [EnumMember(Value = "periscope-profile")]
        PeriscopeProfile = 9,

        /// <summary>
        /// Enum GitlabProfile for value: gitlab-profile
        /// </summary>
        [EnumMember(Value = "gitlab-profile")]
        GitlabProfile = 10,

        /// <summary>
        /// Enum Grabcad for value: grabcad
        /// </summary>
        [EnumMember(Value = "grabcad")]
        Grabcad = 11,

        /// <summary>
        /// Enum InstagramImage for value: instagram-image
        /// </summary>
        [EnumMember(Value = "instagram-image")]
        InstagramImage = 12,

        /// <summary>
        /// Enum ExternalLink for value: external-link
        /// </summary>
        [EnumMember(Value = "external-link")]
        ExternalLink = 13,

        /// <summary>
        /// Enum Avatar for value: avatar
        /// </summary>
        [EnumMember(Value = "avatar")]
        Avatar = 14
    }

    /// <summary>
    /// Returns a <see cref="TypeEnum"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TypeEnum? TypeEnumFromStringOrDefault(string value)
    {
        return value switch
        {
            "youtube" => TypeEnum.Youtube,
            "cdphotothread" => TypeEnum.Cdphotothread,
            "imgur" => TypeEnum.Imgur,
            "facebook-profile" => TypeEnum.FacebookProfile,
            "youtube-channel" => TypeEnum.YoutubeChannel,
            "twitter-profile" => TypeEnum.TwitterProfile,
            "github-profile" => TypeEnum.GithubProfile,
            "instagram-profile" => TypeEnum.InstagramProfile,
            "periscope-profile" => TypeEnum.PeriscopeProfile,
            "gitlab-profile" => TypeEnum.GitlabProfile,
            "grabcad" => TypeEnum.Grabcad,
            "instagram-image" => TypeEnum.InstagramImage,
            "external-link" => TypeEnum.ExternalLink,
            "avatar" => TypeEnum.Avatar,
            _ => null
        };
    }

    /// <summary>
    /// String type of the media element.
    /// </summary>
    /// <value>String type of the media element.</value>
    [JsonRequired]
    [JsonPropertyName("type")]
    public TypeEnum Type { get; set; }
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="Media" /> class.
    /// </summary>
    [JsonConstructor]
    protected Media()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Media" /> class.
    /// </summary>
    /// <param name="details">If required, a JSON dict of additional media information.</param>
    /// <param name="directUrl">Direct URL to the media.</param>
    /// <param name="foreignKey">The key used to identify this media on the media site. (required).</param>
    /// <param name="preferred">True if the media is of high quality.</param>
    /// <param name="teamKeys">List of teams that this media belongs to. Most likely length 1. (required).</param>
    /// <param name="type">String type of the media element. (required).</param>
    /// <param name="viewUrl">The URL that leads to the full web page for the media, if one exists.</param>
    public Media(string foreignKey, Collection<string> teamKeys, TypeEnum type, Dictionary<string, Object>? details = default, string? directUrl = default, bool? preferred = default, string? viewUrl = default)
    {
        this.Details = details;
        this.DirectUrl = directUrl;
        // to ensure "foreignKey" is required (not null)
        ArgumentNullException.ThrowIfNull(foreignKey);
        this.ForeignKey = foreignKey;

        this.Preferred = preferred;
        // to ensure "teamKeys" is required (not null)
        ArgumentNullException.ThrowIfNull(teamKeys);
        this.TeamKeys = teamKeys;

        this.Type = type;
        this.ViewUrl = viewUrl;
    }

    /// <summary>
    /// If required, a JSON dict of additional media information.
    /// </summary>
    /// <value>If required, a JSON dict of additional media information.</value>

    [JsonPropertyName("details")]
    public Dictionary<string, Object>? Details { get; set; }

    /// <summary>
    /// Direct URL to the media.
    /// </summary>
    /// <value>Direct URL to the media.</value>

    [JsonPropertyName("direct_url")]
    public string? DirectUrl { get; set; }

    /// <summary>
    /// The key used to identify this media on the media site.
    /// </summary>
    /// <value>The key used to identify this media on the media site.</value>
    [JsonRequired]
    [JsonPropertyName("foreign_key")]
    public string ForeignKey { get; set; }

    /// <summary>
    /// True if the media is of high quality.
    /// </summary>
    /// <value>True if the media is of high quality.</value>

    [JsonPropertyName("preferred")]
    public bool? Preferred { get; set; }

    /// <summary>
    /// List of teams that this media belongs to. Most likely length 1.
    /// </summary>
    /// <value>List of teams that this media belongs to. Most likely length 1.</value>
    [JsonRequired]
    [JsonPropertyName("team_keys")]
    public Collection<string> TeamKeys { get; set; }

    /// <summary>
    /// The URL that leads to the full web page for the media, if one exists.
    /// </summary>
    /// <value>The URL that leads to the full web page for the media, if one exists.</value>

    [JsonPropertyName("view_url")]
    public string? ViewUrl { get; set; }

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
