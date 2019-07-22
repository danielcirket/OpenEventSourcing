using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static IEnumerable<T> GetRequiredServices<T>(this IServiceProvider source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var services = source.GetRequiredService<IEnumerable<T>>();

            if (!services.Any())
                throw new InvalidOperationException($"No services could be found for '{typeof(T).FriendlyName()}'");

            return services;
        }
    }
}
