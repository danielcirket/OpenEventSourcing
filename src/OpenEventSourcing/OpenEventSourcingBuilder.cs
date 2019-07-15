using System;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing
{
    internal class OpenEventSourcingBuilder : IOpenEventSourcingBuilder
    {
        public IServiceCollection Services { get; }

        public OpenEventSourcingBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
