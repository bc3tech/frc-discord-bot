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
using System.IO;
using System.Text;
using System.Text.Json;

/// <summary>
/// GetTeamEventsStatusesByYear200ResponseValue
/// </summary>
public partial class GetTeamEventsStatusesByYear200ResponseValue : AbstractOpenAPISchema, IValidatableObject
{
    public override string SchemaType { get; } = AnyOf;
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamEventsStatusesByYear200ResponseValue" /> class.
    /// </summary>
    public GetTeamEventsStatusesByYear200ResponseValue()
    {
        this.IsNullable = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamEventsStatusesByYear200ResponseValue" /> class
    /// with the <see cref="TeamEventStatus" /> class
    /// </summary>
    /// <param name="actualInstance">An instance of TeamEventStatus.</param>
    public GetTeamEventsStatusesByYear200ResponseValue(TeamEventStatus actualInstance)
    {
        this.IsNullable = true;
        this.ActualInstance = actualInstance;
    }

    private object _actualInstance;

    /// <summary>
    /// Gets or Sets ActualInstance
    /// </summary>
    public override object ActualInstance
    {
        get
        {
            return _actualInstance;
        }
        set
        {
            if (value.GetType() == typeof(TeamEventStatus))
            {
                this._actualInstance = value;
            }
            else
            {
                throw new ArgumentException("Invalid instance found. Must be the following types: TeamEventStatus");
            }
        }
    }

    /// <summary>
    /// Get the actual instance of `TeamEventStatus`. If the actual instance is not `TeamEventStatus`,
    /// the InvalidClassException will be thrown
    /// </summary>
    /// <returns>An instance of TeamEventStatus</returns>
    public TeamEventStatus GetTeamEventStatus()
    {
        return (TeamEventStatus)this.ActualInstance;
    }

    /// <summary>
    /// Returns the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("class GetTeamEventsStatusesByYear200ResponseValue {");
        sb.Append("  ActualInstance: ").Append(this.ActualInstance).Append('\n');
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public override string ToJson()
    {
        return JsonSerializer.Serialize(this.ActualInstance, SerializerSettings);
    }

    /// <summary>
    /// Converts the JSON string into an instance of GetTeamEventsStatusesByYear200ResponseValue
    /// </summary>
    /// <param name="jsonString">JSON string</param>
    /// <returns>An instance of GetTeamEventsStatusesByYear200ResponseValue</returns>
    public static GetTeamEventsStatusesByYear200ResponseValue FromJson(string jsonString)
    {
        GetTeamEventsStatusesByYear200ResponseValue? newGetTeamEventsStatusesByYear200ResponseValue = null;

        if (string.IsNullOrEmpty(jsonString))
        {
            return newGetTeamEventsStatusesByYear200ResponseValue;
        }

        try
        {
            newGetTeamEventsStatusesByYear200ResponseValue = new GetTeamEventsStatusesByYear200ResponseValue(JsonSerializer.Deserialize<TeamEventStatus>(jsonString, SerializerSettings));
            // deserialization is considered successful at this point if no exception has been thrown.
            return newGetTeamEventsStatusesByYear200ResponseValue;
        }
        catch (Exception exception)
        {
            // deserialization failed, try the next one
            System.Diagnostics.Debug.WriteLine(string.Format("Failed to deserialize `{0}` into TeamEventStatus: {1}", jsonString, exception.ToString()));
        }

        // no match found, throw an exception
        throw new InvalidDataException("The JSON string `" + jsonString + "` cannot be deserialized into any schema defined.");
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
