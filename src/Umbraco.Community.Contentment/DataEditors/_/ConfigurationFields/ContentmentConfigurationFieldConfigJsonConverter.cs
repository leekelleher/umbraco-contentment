// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Core.PropertyEditors;

// Works around https://github.com/leekelleher/umbraco-contentment/issues/574 - avoids Umbraco's
// `IDictionary<string, object>` interface polymorphism by writing entries directly.
internal sealed class ContentmentConfigurationFieldConfigJsonConverter : JsonConverter<IDictionary<string, object>>
{
    public override IDictionary<string, object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);

    public override void Write(Utf8JsonWriter writer, IDictionary<string, object> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var item in value)
        {
            writer.WritePropertyName(item.Key);
            JsonSerializer.Serialize(writer, item.Value, options);
        }

        writer.WriteEndObject();
    }
}
