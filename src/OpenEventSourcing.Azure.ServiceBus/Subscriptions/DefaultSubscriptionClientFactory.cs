using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace OpenEventSourcing.Azure.ServiceBus.Subscriptions
{
    internal sealed class DefaultSubscriptionClientFactory : ISubscriptionClientFactory
    {
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly ServiceBusConnectionStringBuilder _connectionStringBuilder;

        public DefaultSubscriptionClientFactory(IOptions<ServiceBusOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _connectionStringBuilder = new ServiceBusConnectionStringBuilder(_options.Value.ConnectionString);
        }

        public ISubscriptionClient Create(ServiceBusSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var client = new SubscriptionClient(
                connectionStringBuilder: _connectionStringBuilder,
                subscriptionName: subscription.Name);

            return client;
        }
    }
}
