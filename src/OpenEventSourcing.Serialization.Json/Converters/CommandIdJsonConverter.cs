using System;
using Newtonsoft.Json;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Serialization.Json.Converters
{
    public sealed class CommandIdJsonConverter : JsonConverter<CommandId>
    {
        public override void WriteJson(JsonWriter writer, CommandId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override CommandId ReadJson(JsonReader reader, Type objectType, CommandId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return CommandId.From(value);
        }
    }
}
