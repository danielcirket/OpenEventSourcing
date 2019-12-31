using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusOptions : IPostConfigureOptions<ServiceBusOptions>
    {
        internal string ConnectionString { get; private set; }
        internal ServiceBusTopicOptions Topic { get; private set; }
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
        public ServiceBusOptions UseTopic(Action<ServiceBusTopicOptions> optionsAction)
        {
            var options = new ServiceBusTopicOptions();

            optionsAction.Invoke(options);

            Topic = options;

            return this;
        }
        public ServiceBusOptions AddSubscription(Action<ServiceBusSubscription> optionsAction)
        {
            var options = new ServiceBusSubscription();

            optionsAction.Invoke(options);

            Subscriptions.Add(options);

            return this;
        }

        public void PostConfigure(string name, ServiceBusOptions options)
        {
            if (string.IsNullOrEmpty(ConnectionString) && options.Topic == null)
                throw new InvalidOperationException($"Azure service bus connection string doesn't contain an entity path and 'UseTopic(...)' has not been called. Either include the entity path in the connection string or by calling 'UseTopic(...)' during startup when configuring Azure Service Bus.");

            var entityPath = string.Empty;

            var parts = ConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var keyAndValue = part.Split(new[] { '=' }, 2);
                var key = keyAndValue[0];

                if (keyAndValue.Length != 2)
                    throw new ArgumentException($"Value for the connection string parameter name '{key}' was not found.");

                if (key.Equals("EntityPath", StringComparison.OrdinalIgnoreCase))
                    entityPath = keyAndValue[1].Trim();
            }

            if (!string.IsNullOrEmpty(entityPath) && options.Topic == null)
                options.UseTopic(t => t.WithName(entityPath));
        }
    }
}
