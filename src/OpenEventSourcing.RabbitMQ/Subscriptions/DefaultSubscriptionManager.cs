using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Management;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Subscriptions
{
    internal sealed class DefaultSubscriptionManager : ISubscriptionManager
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly IRabbitMqConnectionFactory _connectionFactory;

        public DefaultSubscriptionManager(IOptions<RabbitMqOptions> options,
                                          IRabbitMqConnectionFactory connectionFactory)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _options = options;
            _connectionFactory = connectionFactory;
        }

        public async Task ConfigureAsync()
        {
            using (var client = new RabbitMqManagementClient(_connectionFactory, _options))
            {
                if (!await client.ExchangeExistsAsync(name: _options.Value.Exchange))
                    await client.CreateExchangeAsync(name: _options.Value.Exchange, exchangeType: ExchangeType.Topic);

                foreach (var subscription in _options.Value.Subscriptions)
                {
                    if (!await client.QueueExistsAsync(name: subscription.Name))
                        await client.CreateQueueAsync(name: subscription.Name);

                    foreach (var @event in subscription.Events)
                        await client.CreateSubscriptionAsync(routingKey: @event.Name, queue: subscription.Name, exchange: _options.Value.Exchange);
                }
            }
        }
    }
}
