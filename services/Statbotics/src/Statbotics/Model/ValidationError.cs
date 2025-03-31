/*
 * Statbotics REST API
 *
 * The REST API for Statbotics. Please be nice to our servers! If you are looking to do large-scale data science projects, use the CSV exports on the GitHub repo.
 *
 * The version of the OpenAPI document: 3.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace Statbotics.Model;

using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// ValidationError
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial class ValidationError
{
    // yup
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError" /> class.
    /// </summary>
    [JsonConstructor]
    protected ValidationError()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError" /> class.
    /// </summary>
    /// <param name="loc">loc (required).</param>
    /// <param name="msg">msg (required).</param>
    /// <param name="type">type (required).</param>
    public ValidationError(Collection<ValidationErrorLocInner> loc, string msg, string type)
    {
        // to ensure "loc" is required (not null)
        ArgumentNullException.ThrowIfNull(loc);
        this.Loc = loc;

        // to ensure "msg" is required (not null)
        ArgumentNullException.ThrowIfNull(msg);
        this.Msg = msg;

        // to ensure "type" is required (not null)
        ArgumentNullException.ThrowIfNull(type);
        this.Type = type;
    }

    /// <summary>
    /// Gets or Sets Loc
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("loc")]
    public Collection<ValidationErrorLocInner> Loc { get; set; }

    /// <summary>
    /// Gets or Sets Msg
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    /// <summary>
    /// Gets or Sets Type
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>string presentation of the object</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine("class ValidationError {");
        sb.Append("  Loc: ").AppendLine($"{(Loc is null ? "[null]" : string.Join(", ", Loc))}");
        sb.Append("  Msg: ").AppendLine($"{Msg}");
        sb.Append("  Type: ").AppendLine($"{Type}");
        sb.AppendLine("}");
        return sb.ToString();
    }

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
