using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Events;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Messages;

namespace OpenEventSourcing.RabbitMQ.Queues
{
    internal sealed class DefaultQueueMessageSender : IQueueMessageSender
    {
        private readonly ILogger<DefaultQueueMessageSender> _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly IMessageFactory _messageFactory;
        private readonly IRabbitMqConnectionFactory _connectionFactory;

        public DefaultQueueMessageSender(ILogger<DefaultQueueMessageSender> logger,
                                         IOptions<RabbitMqOptions> options,
                                         IMessageFactory messageFactory,
                                         IRabbitMqConnectionFactory connectionFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _logger = logger;
            _options = options;
            _messageFactory = messageFactory;
            _connectionFactory = connectionFactory;
        }

        public async Task SendAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var message = _messageFactory.CreateMessage(@event);

            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.PublishAsync(message);
            }
        }

        public async Task SendAsync(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var messages = events.Select(@event => _messageFactory.CreateMessage(@event));

            using (var connection = await _connectionFactory.CreateConnectionAsync(CancellationToken.None))
            {
                await connection.PublishAsync(messages);
            }
        }
    }
}
