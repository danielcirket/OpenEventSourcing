using System;
using System.Collections.Generic;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqOptions
    {
        internal string ConnectionString { get; private set; }
        internal RabbitMqExchangeOptions Exchange { get; private set; }
        internal RabbitMqManagementApiOptions ManagementApi { get; private set; }
        internal IList<RabbitMqSubscription> Subscriptions { get; }

        public RabbitMqOptions()
        {
            Subscriptions = new List<RabbitMqSubscription>();
        }

        public RabbitMqOptions UseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.", nameof(connectionString));

            ConnectionString = connectionString;

            return this;
        }
        public RabbitMqOptions UseExchange(Action<RabbitMqExchangeOptions> optionsAction)
        {
            var options = new RabbitMqExchangeOptions();

            optionsAction?.Invoke(options);

            if (string.IsNullOrEmpty(options.Name))
                throw new ArgumentException($"Exchange name cannot be null or empty.");
            if (string.IsNullOrEmpty(options.Type))
                throw new ArgumentException($"Exchange type cannot be null or empty.");

            Exchange = options;

            return this;
        }
        public RabbitMqOptions UseManagementApi(Action<RabbitMqManagementApiOptions> optionsAction)
        {
            var options = new RabbitMqManagementApiOptions();

            optionsAction?.Invoke(options);

            ManagementApi = options;

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
