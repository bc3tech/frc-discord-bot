﻿namespace Common.JsonConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class UnixEpochDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).DateTime;

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteNumberValue(new DateTimeOffset(value).ToUnixTimeMilliseconds());
}
