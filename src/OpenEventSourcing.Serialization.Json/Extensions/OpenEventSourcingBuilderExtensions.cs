using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.Serialization.Json.Extensions
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddJsonSerializers(this IOpenEventSourcingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ICommandDeserializer, JsonCommandDeserializer>();
            builder.Services.AddSingleton<ICommandSerializer, JsonCommandSerializer>();
            builder.Services.AddSingleton<IEventDeserializer, JsonEventDeserializer>();
            builder.Services.AddSingleton<IEventSerializer, JsonEventSerializer>();
            builder.Services.AddSingleton<IQueryDeserializer, JsonQueryDeserializer>();
            builder.Services.AddSingleton<IQuerySerializer, JsonQuerySerializer>();

            return builder;
        }
    }
}
