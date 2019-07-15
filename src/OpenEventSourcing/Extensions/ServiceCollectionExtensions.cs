using System;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Events;
using OpenEventSourcing.Domain;

namespace OpenEventSourcing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IOpenEventSourcingBuilder AddOpenEventSourcing(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAggregateFactory, AggregateFactory>();
            services.AddSingleton<IEventTypeCache, EventTypeCache>();            

            return new OpenEventSourcingBuilder(services);
        }
    }
}
