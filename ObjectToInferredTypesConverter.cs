using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cachet_monitor
{
    public class ObjectToInferredTypesConverter
    : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                {
                    return l;
                }

                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    return datetime;
                }
                return reader.GetString();

            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                try
                {
                    var localoptions = new JsonSerializerOptions();
                    localoptions.Converters.Add(new ObjectToInferredTypesConverter());
                    return JsonSerializer.Deserialize<Dictionary<string, dynamic>>(JsonDocument.ParseValue(ref reader).RootElement.Clone().ToString(), options);
                }
                catch (Exception)
                {
                    using JsonDocument localdocument = JsonDocument.ParseValue(ref reader);
                    return localdocument.RootElement.Clone();
                }

            }
            // Use JsonElement as fallback.
            // Newtonsoft uses JArray or JObject.
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
                throw new InvalidOperationException("Should not get here.");
    }
}
