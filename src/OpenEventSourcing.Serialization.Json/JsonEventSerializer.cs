using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenEventSourcing.Serialization.Json
{
    internal class JsonEventSerializer : IEventSerializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonEventSerializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
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
