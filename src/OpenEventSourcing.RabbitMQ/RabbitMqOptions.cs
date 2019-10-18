using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqOptions
    {
        internal string ConnectionString { get; private set; }
        internal string Exchange { get; private set; }
        internal IList<RabbitMqSubscription> Subscriptions { get; }

        public RabbitMqOptions()
        {
            Subscriptions = new List<RabbitMqSubscription>();
        }

        public RabbitMqOptions UseExchange(string exchange)
        {
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentException($"'{nameof(exchange)}' cannot be null or empty.", nameof(exchange));

            Exchange = exchange;

            return this;
        }
        public RabbitMqOptions UseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.", nameof(connectionString));

            ConnectionString = connectionString;

            return this;
        }

        public RabbitMqOptions AddSubscription(Action<RabbitMqSubscription> optionsAction)
        {
            var options = new RabbitMqSubscription();

            optionsAction?.Invoke(options);

            Subscriptions.Add(options);

            return this;
        }
    }
}
