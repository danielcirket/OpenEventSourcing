using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.RabbitMQ.Messages;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Connections
{
    public interface IRabbitMqConnection : IDisposable
    {
        string ConnectionId { get; }
        bool IsOpen { get; }
        IConnection UnderlyingConnection { get; }

        Task PublishAsync(Message message);
        Task PublishAsync(IEnumerable<Message> messages);

        Task CreateExchangeAsync(string name, string exchangeType, bool durable = true, bool autoDelete = false);
        Task CreateQueueAsync(string name, bool durable = true, bool autoDelete = false);
        Task CreateSubscriptionAsync(string routingKey, string queue, string exchange);

        Task<bool> ExchangeExistsAsync(string name);
        Task<bool> QueueExistsAsync(string name);

        Task RemoveExchangeAsync(string name);
        Task RemoveQueueAsync(string name);
        Task RemoveSubscriptionAsync(string routingKey, string queue, string exchange);
    }
}
