using System;
using Newtonsoft.Json;

namespace OpenEventSourcing.Serialization.Json.Converters
{
    public sealed class NullableCausationIdJsonConverter : JsonConverter<CausationId?>
    {
        public override void WriteJson(JsonWriter writer, CausationId? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToString());
        }

        public override CausationId? ReadJson(JsonReader reader, Type objectType, CausationId? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            
            if (value == null)
                return null;
            
            return CausationId.From(value);
        }
    }
}
