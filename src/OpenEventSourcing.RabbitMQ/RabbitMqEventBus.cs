using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.RabbitMQ.Subscriptions;

namespace OpenEventSourcing.RabbitMQ
{
    internal sealed class RabbitMqEventBus : IEventBusPublisher, IEventBusConsumer
    {
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly IQueueMessageSender _messageSender;
        private readonly IQueueMessageReceiver _queueMessageReceiver;

        public RabbitMqEventBus(ILogger<RabbitMqEventBus> logger,
                                IQueueMessageSender messageSender,
                                IQueueMessageReceiver queueMessageReceiver,
                                ISubscriptionManager subscriptionManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (messageSender == null)
                throw new ArgumentNullException(nameof(messageSender));
            if (subscriptionManager == null)
                throw new ArgumentNullException(nameof(subscriptionManager));

            _logger = logger;
            _messageSender = messageSender;
            _queueMessageReceiver = queueMessageReceiver;
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            await _messageSender.SendAsync(@event);
        }

        public async Task PublishAsync(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            await _messageSender.SendAsync(events);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            //if (_running)
            //    return;

            _logger.LogInformation($"Starting RabbitMQ bus");


            await _queueMessageReceiver.StartAsync(cancellationToken);


            //_running = true;

            _logger.LogInformation($"Successfully started RabbitMQ bus");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            //if (!_running)
            //    return;

            _logger.LogInformation($"Stopping RabbitMQ bus");

            await _queueMessageReceiver.StopAsync(cancellationToken);

            //_running = false;

            _logger.LogInformation($"Successfully stopped RabbitMQ bus");
        }
    }
}
