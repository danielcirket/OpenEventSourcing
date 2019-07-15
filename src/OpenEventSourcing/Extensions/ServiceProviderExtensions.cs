using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OpenEventSourcing.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static IEnumerable<T> GetRequiredServices<T>(this IServiceProvider source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.GetRequiredService<IEnumerable<T>>();
        }
    }
}
