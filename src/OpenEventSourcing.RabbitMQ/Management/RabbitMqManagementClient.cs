using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.RabbitMQ.Connections;

namespace OpenEventSourcing.RabbitMQ.Management
{
    public class RabbitMqManagementClient : IRabbitMqManagementClient
    {
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly IOptions<RabbitMqOptions> _options;

        public RabbitMqManagementClient(IRabbitMqConnectionFactory connectionFactory,
                                        IOptions<RabbitMqOptions> options)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _connectionFactory = connectionFactory;
            _options = options;
        }

        public async Task CreateExchangeAsync(string name, string exchangeType, bool durable = true, bool autoDelete = false)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.CreateExchangeAsync(name, exchangeType, durable, autoDelete);
            }
        }
        public async Task CreateQueueAsync(string name, bool durable = true, bool autoDelete = false)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.CreateQueueAsync(name, durable, autoDelete);
            }
        }
        public async Task CreateSubscriptionAsync(string routingKey, string queue, string exchange)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.CreateSubscriptionAsync(routingKey, queue, exchange);
            }
        }
        public async Task<bool> ExchangeExistsAsync(string name)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                return await connection.ExchangeExistsAsync(name);
            }
        }
        public async Task<bool> QueueExistsAsync(string name)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                return await connection.QueueExistsAsync(name);
            }
        }
        public async Task RemoveExchangeAsync(string name)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.RemoveExchangeAsync(name);
            }
        }
        public async Task RemoveQueueAsync(string name)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.RemoveQueueAsync(name);
            }
        }
        public async Task RemoveSubscriptionAsync(string routingKey, string queue, string exchange)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.RemoveSubscriptionAsync(routingKey, queue, exchange);
            }
        }
        public void Dispose()
        {
            
        }
    }
}
