using System;
using Newtonsoft.Json;

namespace OpenEventSourcing.Serialization.Json.Converters
{
    public sealed class NullableCorrelationIdJsonConverter : JsonConverter<CorrelationId?>
    {
        public override void WriteJson(JsonWriter writer, CorrelationId? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override CorrelationId? ReadJson(JsonReader reader, Type objectType, CorrelationId? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);

            if (value == null)
                return null;

            return CorrelationId.From(value);
        }
    }
}
