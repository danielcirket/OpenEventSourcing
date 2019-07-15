
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace OpenEventSourcing.Events
{
    internal sealed class EventTypeCache : IEventTypeCache
    {
        private readonly ConcurrentDictionary<string, Type> _lookup;

        public EventTypeCache()
        {
            var eventType = typeof(IEvent);

            var assemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToArray();

            var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                                  .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                                  .Where(typeInfo => eventType.IsAssignableFrom(typeInfo))
                                  .Select(typeInfo => typeInfo.AsType());

            _lookup = new ConcurrentDictionary<string, Type>(types.ToDictionary(type => type.FullName));
        }

        public bool TryGet(string fullName, out Type type)
            => _lookup.TryGetValue(fullName, out type);
    }
}
