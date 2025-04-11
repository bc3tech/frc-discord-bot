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
/// GetTeamEventsStatusesByYear200ResponseValue
/// </summary>

[JsonConverter(typeof(AbstractOpenAPISchemaJsonConverter<GetTeamEventsStatusesByYear200ResponseValue>))]
public sealed partial record GetTeamEventsStatusesByYear200ResponseValue : AbstractOpenAPISchema
{
    public override string SchemaType { get; } = AnyOf;
    public override bool IsNullable { get; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamEventsStatusesByYear200ResponseValue" /> class.
    /// </summary>
    public GetTeamEventsStatusesByYear200ResponseValue() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTeamEventsStatusesByYear200ResponseValue" /> class
    /// with the <see cref="TeamEventStatus" /> class
    /// </summary>
    /// <param name="actualInstance">An instance of TeamEventStatus.</param>
    public GetTeamEventsStatusesByYear200ResponseValue(TeamEventStatus actualInstance) => _actualInstance = actualInstance;

    private object? _actualInstance;

    /// <summary>
    /// Gets or Sets ActualInstance
    /// </summary>
    public override object? ActualInstance
    {
        get => _actualInstance;
        set
        {
            _actualInstance = value switch
            {
                null => null,
                TeamEventStatus _ => value,
                _ => throw new ArgumentException("Invalid instance found. Must be the following types: [TeamEventStatus]")
            };
        }
    }

    /// <summary>
    /// Get the actual instance of `TeamEventStatus`. If the actual instance is not `TeamEventStatus`,
    /// the InvalidClassException will be thrown
    /// </summary>
    /// <returns>An instance of TeamEventStatus</returns>
    public TeamEventStatus? GetTeamEventStatus() => (TeamEventStatus?)this.ActualInstance;

    /// <summary>
    /// Returns the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public override string ToJson() => JsonSerializer.Serialize(this.ActualInstance, SerializerSettings);

    /// <summary>
    /// Converts the JSON string into an instance of GetTeamEventsStatusesByYear200ResponseValue
    /// </summary>
    /// <param name="jsonString">JSON string</param>
    /// <returns>An instance of GetTeamEventsStatusesByYear200ResponseValue</returns>
    public static GetTeamEventsStatusesByYear200ResponseValue? FromJson(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return default;
        }

        try
        {
            return new GetTeamEventsStatusesByYear200ResponseValue(JsonSerializer.Deserialize<TeamEventStatus>(jsonString, SerializerSettings)!); // We expect the force-dereference to cause a nullref here
        }
        catch (Exception exception)
        {
            // deserialization failed, try the next one
            System.Diagnostics.Debug.WriteLine(string.Format("Failed to deserialize `{0}` into TeamEventStatus: {1}", jsonString, exception.ToString()));
        }

        // no match found, throw an exception
        throw new InvalidDataException("The JSON string `" + jsonString + "` cannot be deserialized into any schema defined.");
    }
}

