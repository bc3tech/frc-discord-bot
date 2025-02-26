namespace TheBlueAlliance.Model;
using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

internal sealed class JsonStringEnumConverterWithEnumMemberSupport<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonStringVal = reader.GetString();
        if (string.IsNullOrWhiteSpace(jsonStringVal))
        {
            return default;
        }

        if (!Enum.TryParse<TEnum>(jsonStringVal, ignoreCase: true, out var result))
        {
            // Find enum values with `EnumMemberAttribute` and compare with the string value
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute is not null && attribute.Value?.Equals(jsonStringVal, StringComparison.OrdinalIgnoreCase) is true)
                {
                    result = (TEnum)field.GetValue(null)!;
                    break;
                }
            }
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        // Write the enum value as a string using the value of EnumMemberAttribute, if present, otherwise the enum value
        var field = typeof(TEnum).GetField(value.ToString()!);
        var attribute = Attribute.GetCustomAttribute(field!, typeof(EnumMemberAttribute)) as EnumMemberAttribute;
        if (attribute is not null && !string.IsNullOrWhiteSpace(attribute.Value))
        {
            writer.WriteStringValue(attribute.Value);
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
