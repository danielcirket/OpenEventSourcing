using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusSubscription
    {
        internal string Name { get; set; }
        internal IList<Type> Events { get; }

        public ServiceBusSubscription()
        {
            Events = new List<Type>();
        }

        public ServiceBusSubscription UseName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            Name = name;

            return this;
        }
        public ServiceBusSubscription ForEvent<TEvent>()
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (!Events.Contains(type))
                Events.Add(type);

            return this;
        }
    }
}
