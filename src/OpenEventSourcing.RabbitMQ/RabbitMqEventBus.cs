using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.RabbitMQ.Queues;

namespace OpenEventSourcing.RabbitMQ
{
    internal sealed class RabbitMqEventBus : IEventBusPublisher, IEventBusConsumer
    {
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IQueueMessageReceiver _queueMessageReceiver;

        public RabbitMqEventBus(ILogger<RabbitMqEventBus> logger,
                                IServiceScopeFactory serviceScopeFactory,
                                IQueueMessageReceiver queueMessageReceiver)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (queueMessageReceiver == null)
                throw new ArgumentNullException(nameof(queueMessageReceiver));

            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _queueMessageReceiver = queueMessageReceiver;
        }

        public async Task PublishAsync<TEvent>(IEventContext<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                await sender.SendAsync(context, cancellationToken);
            }
        }

        public async Task PublishAsync(IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                await sender.SendAsync(events, cancellationToken);
            }
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
