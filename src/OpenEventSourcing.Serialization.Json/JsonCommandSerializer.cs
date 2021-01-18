using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenEventSourcing.Serialization.Json.Converters;

namespace OpenEventSourcing.Serialization.Json
{
    internal class JsonCommandSerializer : ICommandSerializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonCommandSerializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                Converters = new List<JsonConverter>
                {
                    new CausationIdJsonConverter(),
                    new NullableCausationIdJsonConverter(),
                    new CorrelationIdJsonConverter(),
                    new NullableCorrelationIdJsonConverter(),
                    new QueryIdJsonConverter(),
                    new NullableQueryIdJsonConverter(),
                }
            };
        }

        public string Serialize<T>(T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.SerializeObject(data, _serializerSettings);
        }
    }
}
