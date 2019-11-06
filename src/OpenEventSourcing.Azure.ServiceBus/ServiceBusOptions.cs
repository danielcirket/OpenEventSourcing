using System;
using System.Collections.Generic;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusOptions
    {
        internal string ConnectionString { get; private set; }
        internal IList<ServiceBusSubscription> Subscriptions { get; }

        public ServiceBusOptions()
        {
            Subscriptions = new List<ServiceBusSubscription>();
        }

        public ServiceBusOptions UseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.", nameof(connectionString));

            ConnectionString = connectionString;

            return this;
        }

        public ServiceBusOptions AddSubscription(Action<ServiceBusSubscription> optionsAction)
        {
            var options = new ServiceBusSubscription();

            optionsAction.Invoke(options);

            Subscriptions.Add(options);

            return this;
        }
    }
}
