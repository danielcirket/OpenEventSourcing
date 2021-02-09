using System;
using Newtonsoft.Json;

namespace OpenEventSourcing.Serialization.Json.Converters
{
    public sealed class NullableActorJsonConverter : JsonConverter<Actor?>
    {
        public override void WriteJson(JsonWriter writer, Actor? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToString());
        }

        public override Actor? ReadJson(JsonReader reader, Type objectType, Actor? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            
            if (value == null)
                return null;
            
            return Actor.From(value);
        }
    }
}
