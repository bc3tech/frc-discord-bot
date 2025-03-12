/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Model;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
///  Abstract base class for oneOf, anyOf schemas in the OpenAPI specification
/// </summary>
public abstract partial record AbstractOpenAPISchema
{
    /// <summary>
    ///  Custom JSON serializer
    /// </summary>
    static public readonly JsonSerializerOptions SerializerSettings = new()
    {
        // OpenAPI generated types generally hide default constructors.
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    /// <summary>
    ///  Custom JSON serializer for objects with additional properties
    /// </summary>
    static public readonly JsonSerializerOptions AdditionalPropertiesSerializerSettings = new()
    {
        // OpenAPI generated types generally hide default constructors.
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Gets or Sets the actual instance
    /// </summary>
    public abstract object? ActualInstance { get; set; }

    /// <summary>
    /// Gets or Sets IsNullable to indicate whether the instance is nullable
    /// </summary>
    public virtual bool IsNullable { get; }

    protected const string OneOf = "oneOf";
    protected const string AnyOf = "anyOf";

    /// <summary>
    /// Gets or Sets the schema type, which can be either `oneOf` or `anyOf`
    /// </summary>
    public abstract string SchemaType { get; }

    /// <summary>
    /// Converts the instance into JSON string.
    /// </summary>
    public abstract string ToJson();
}

public sealed class AbstractOpenAPISchemaJsonConverter<T> : JsonConverter<T> where T : AbstractOpenAPISchema
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the entire JSON element as a string
        using (JsonDocument document = JsonDocument.ParseValue(ref reader))
        {
            string jsonString = document.RootElement.GetRawText();
            // Use the FromJson method to deserialize
            return (T?)typeof(T).GetMethod("FromJson")?.Invoke(null, [jsonString]);
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToJson());
    }
}