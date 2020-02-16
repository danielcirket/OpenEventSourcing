using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqSubscription
    {
        public string Name { get; internal set; }
        public IList<Type> Events { get; internal set; }
        public bool ShouldAutoDelete { get; internal set; }
        public bool Durable { get; internal set; }

        public RabbitMqSubscription()
        {
            Events = new List<Type>();
            ShouldAutoDelete = false;
            Durable = true;
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
        /// <summary>
        /// Sets the subscription to auto delete once all consumers disconnect. Defaults to false.
        /// </summary>
        /// <returns></returns>
        public RabbitMqSubscription AutoDelete()
        {
            ShouldAutoDelete = true;

            return this;
        }
        /// <summary>
        /// Sets the subscription to be durable. Durable queues remain active when a server restarts. Non-durable queues (transient queues) are purged if/when a server restarts. Defaults to true.
        /// </summary>
        /// <param name="durable"></param>
        /// <returns></returns>
        public RabbitMqSubscription WithDurable(bool durable)
        {
            Durable = durable;

            return this;
        }
    }
}
