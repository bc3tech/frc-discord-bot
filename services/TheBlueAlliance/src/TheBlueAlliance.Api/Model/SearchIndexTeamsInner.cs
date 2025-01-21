/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Api.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// SearchIndexTeamsInner
/// </summary>
public partial class SearchIndexTeamsInner : IValidatableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndexTeamsInner" /> class.
    /// </summary>
    [JsonConstructor]
    protected SearchIndexTeamsInner() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchIndexTeamsInner" /> class.
    /// </summary>
    /// <param name="key">key (required).</param>
    /// <param name="nickname">nickname (required).</param>
    public SearchIndexTeamsInner(string? key = default, string? nickname = default)
    {
        // to ensure "key" is required (not null)
        ArgumentNullException.ThrowIfNull(key);
        this.Key = key;
        // to ensure "nickname" is required (not null)
        ArgumentNullException.ThrowIfNull(nickname);
        this.Nickname = nickname;
    }

    /// <summary>
    /// Gets or Sets Key
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// Gets or Sets Nickname
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("class SearchIndexTeamsInner {");
        sb.Append("  Key: ").AppendLine(Key.ToString());
        sb.Append("  Nickname: ").AppendLine(Nickname.ToString());
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public virtual string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// To validate all properties of the instance
    /// </summary>
    /// <param name="validationContext">Validation context</param>
    /// <returns>Validation Result</returns>
    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        yield break;
    }
}
