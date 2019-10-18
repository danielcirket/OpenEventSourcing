
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                //.Where(library => !library.Name.StartsWith("Microsoft.Test"))
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToArray();

            var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                                  .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                                  .Where(typeInfo => eventType.IsAssignableFrom(typeInfo))
                                  .Select(typeInfo => typeInfo.AsType());

            _lookup = new ConcurrentDictionary<string, Type>(types.ToDictionary(type => type.FullName));

            // TODO(Dan): Should we eagerly check for type.Name duplicates?
        }

        public bool TryGet(string name, out Type type)
        {
            if (_lookup.TryGetValue(name, out type))
                return true;

            var potentialMatches = new List<Type>();

            foreach (var key in _lookup.Keys)
            {
                var part = key.Split('.').Last();
                
                part = key.Split('+').Last();

                if (part.Equals(name, StringComparison.OrdinalIgnoreCase))
                    potentialMatches.Add(_lookup[key]);
            }

            if (potentialMatches.Count < 1)
                return false;

            if (potentialMatches.Count > 1)
            {
                var typeNames = string.Join(", ", potentialMatches.Select(t => $"'{t.FullName}'"));
                throw new AmbiguousMatchException($"Multiple types are registered with the same name, but different namespaces. The types are: {typeNames}");
            }

            type = potentialMatches[0];

            return true;
        }
    }
}
