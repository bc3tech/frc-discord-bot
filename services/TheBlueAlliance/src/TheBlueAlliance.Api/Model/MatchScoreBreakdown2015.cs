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
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
/// See the 2015 FMS API documentation for a description of each value
/// </summary>
public partial class MatchScoreBreakdown2015 : IValidatableObject
{
    /// <summary>
    /// Defines Coopertition
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<CoopertitionEnum>)), Microsoft.Extensions.EnumStrings.EnumStrings]
    public enum CoopertitionEnum
    {
        /// <summary>
        /// Enum None for value: None
        /// </summary>
        [EnumMember(Value = "None")]
        None = 1,

        /// <summary>
        /// Enum Unknown for value: Unknown
        /// </summary>
        [EnumMember(Value = "Unknown")]
        Unknown = 2,

        /// <summary>
        /// Enum Stack for value: Stack
        /// </summary>
        [EnumMember(Value = "Stack")]
        Stack = 3
    }

    /// <summary>
    /// Gets or Sets Coopertition
    /// </summary>
    [JsonPropertyName("coopertition")]
    public CoopertitionEnum Coopertition { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2015" /> class.
    /// </summary>
    [JsonConstructor]
    protected MatchScoreBreakdown2015() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchScoreBreakdown2015" /> class.
    /// </summary>
    /// <param name="blue">blue (required).</param>
    /// <param name="red">red (required).</param>
    /// <param name="coopertition">coopertition (required).</param>
    /// <param name="coopertitionPoints">coopertitionPoints (required).</param>
    public MatchScoreBreakdown2015(MatchScoreBreakdown2015Alliance? blue = default, MatchScoreBreakdown2015Alliance? red = default, CoopertitionEnum coopertition = default, int coopertitionPoints = default)
    {
        // to ensure "blue" is required (not null)
        ArgumentNullException.ThrowIfNull(blue);
        this.Blue = blue;
        // to ensure "red" is required (not null)
        ArgumentNullException.ThrowIfNull(red);
        this.Red = red;
        this.Coopertition = coopertition;
        this.CoopertitionPoints = coopertitionPoints;
    }

    /// <summary>
    /// Gets or Sets Blue
    /// </summary>
    [JsonPropertyName("blue")]
    public MatchScoreBreakdown2015Alliance Blue { get; set; }

    /// <summary>
    /// Gets or Sets Red
    /// </summary>
    [JsonPropertyName("red")]
    public MatchScoreBreakdown2015Alliance Red { get; set; }

    /// <summary>
    /// Gets or Sets CoopertitionPoints
    /// </summary>
    [JsonPropertyName("coopertition_points")]
    public int CoopertitionPoints { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("class MatchScoreBreakdown2015 {");
        sb.Append("  Blue: ").AppendLine(Blue.ToString());
        sb.Append("  Red: ").AppendLine(Red.ToString());
        sb.Append("  Coopertition: ").AppendLine(Coopertition.ToString());
        sb.Append("  CoopertitionPoints: ").AppendLine(CoopertitionPoints.ToString());
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
