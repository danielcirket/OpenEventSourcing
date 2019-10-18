using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqSubscription
    {
        internal string Name { get; set; }
        internal IList<Type> Events { get; }

        public RabbitMqSubscription()
        {
            Events = new List<Type>();
        }

        public RabbitMqSubscription UseName(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentException($"'{nameof(queueName)}' cannot be null or empty.", nameof(queueName));

            Name = queueName;

            return this;
        }
        public RabbitMqSubscription ForEvent<TEvent>()
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (!Events.Contains(type))
                Events.Add(type);

            return this;
        }
    }
}
