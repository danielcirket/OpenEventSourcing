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

        public async Task CancelScheduledMessageAsync(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException($"'{nameof(identifier)}' cannot be null or empty.", nameof(identifier));

            await _messageSender.CancelScheduledMessageAsync(identifier);
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
        public async Task<string> PublishAsync<TEvent>(TEvent @event, DateTimeOffset publishOnUtc) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            return await _messageSender.SendAsync(@event, publishOnUtc);
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
