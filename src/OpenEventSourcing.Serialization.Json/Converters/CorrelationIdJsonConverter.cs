using System;
using Newtonsoft.Json;

namespace OpenEventSourcing.Serialization.Json.Converters
{
    public sealed class CorrelationIdJsonConverter : JsonConverter<CorrelationId>
    {
        public override void WriteJson(JsonWriter writer, CorrelationId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override CorrelationId ReadJson(JsonReader reader, Type objectType, CorrelationId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return CorrelationId.From(value);
        }
    }
}
