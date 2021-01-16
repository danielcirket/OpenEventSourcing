using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus
{
    internal sealed class AzureServiceBus : IEventBusPublisher, IEventBusConsumer
    {
        private readonly ILogger<AzureServiceBus> _logger;
        private readonly ITopicMessageSender _messageSender;
        private readonly ISubscriptionClientManager _subscriptionClientManager;
        private bool _running = false;
        private IReadOnlyList<ISubscriptionClient> _clients;

        public AzureServiceBus(ILogger<AzureServiceBus> logger,
                               ITopicMessageSender messageSender,
                               ISubscriptionClientManager subscriptionClientManager)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (messageSender == null)
                throw new ArgumentNullException(nameof(messageSender));
            if (subscriptionClientManager == null)
                throw new ArgumentNullException(nameof(subscriptionClientManager));

            _logger = logger;
            _messageSender = messageSender;
            _subscriptionClientManager = subscriptionClientManager;
        }

        public async Task PublishAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            await _messageSender.SendAsync(context, cancellationToken);
        }
        public async Task PublishAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            await _messageSender.SendAsync(contexts, cancellationToken);
        }
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_running)
                return;

            _logger.LogInformation($"Starting Azure Service bus");

            _clients = await _subscriptionClientManager.ConfigureAsync();

            _running = true;

            _logger.LogInformation($"Successfully started Azure Service bus");
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_running)
                return;

            _logger.LogInformation($"Stopping Azure Service bus");

            foreach (var client in _clients)
                await client.CloseAsync();

            _running = false;

            _logger.LogInformation($"Successfully stopped Azure Service bus");
        }
    }
}
