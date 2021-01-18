﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenEventSourcing.Serialization.Json.Converters;
using OpenEventSourcing.Serialization.Json.Resolvers;

namespace OpenEventSourcing.Serialization.Json
{
    internal class JsonEventDeserializer : IEventDeserializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public JsonEventDeserializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new ImmutablePropertyCamelCasePropertyNamesContactResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
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

        public object Deserialize(string data, Type type)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.DeserializeObject(data, type, _serializerSettings);
        }
        public T Deserialize<T>(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.DeserializeObject<T>(data, _serializerSettings);
        }
    }
}
