using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Management;

namespace OpenEventSourcing.RabbitMQ.Subscriptions
{
    internal sealed class DefaultSubscriptionManager : ISubscriptionManager
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly IRabbitMqManagementClient _client;
        private readonly IRabbitMqConnectionFactory _connectionFactory;

        public DefaultSubscriptionManager(IOptions<RabbitMqOptions> options,
                                          IRabbitMqManagementClient client,
                                          IRabbitMqConnectionFactory connectionFactory)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _options = options;
            _client = client;
            _connectionFactory = connectionFactory;
        }

        public async Task ConfigureAsync()
        {
            if (!await _client.ExchangeExistsAsync(name: _options.Value.Exchange.Name))
                await _client.CreateExchangeAsync(name: _options.Value.Exchange.Name, exchangeType: _options.Value.Exchange.Type);

            var managementApiEnabled = _options.Value.ManagementApi != null;

            foreach (var subscription in _options.Value.Subscriptions)
            {
                if (!await _client.QueueExistsAsync(name: subscription.Name))
                    await _client.CreateQueueAsync(name: subscription.Name);

                if (managementApiEnabled)
                {
                    var currentSubscriptions = await _client.RetrieveSubscriptionsAsync(subscription.Name);
                    var subscriptionsToCreate = subscription.Events.Where(e => !currentSubscriptions.Any(s => s.Queue == subscription.Name && s.RoutingKey == e.Name));
                    var subscriptionsToRemove = currentSubscriptions.Where(s => !subscription.Events.Any(e => e.Name == s.RoutingKey) && s.Queue == subscription.Name);

                    foreach (var sub in subscriptionsToRemove)
                        await _client.RemoveSubscriptionAsync(sub.RoutingKey, subscription.Name, _options.Value.Exchange.Name);

                    foreach (var sub in subscriptionsToCreate)
                        await _client.CreateSubscriptionAsync(sub.Name, subscription.Name, _options.Value.Exchange.Name);
                }
                else
                {
                    foreach (var @event in subscription.Events)
                    {
                        await _client.CreateSubscriptionAsync(routingKey: @event.Name, queue: subscription.Name, exchange: _options.Value.Exchange.Name);
                    }
                }
            }
        }
    }
}
