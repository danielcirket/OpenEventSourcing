using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.AzureServiceBus
{
    internal class SampleEventPublisher : BackgroundService
    {
        private readonly IEventBusPublisher _eventBusPublisher;

        public SampleEventPublisher(IEventBusPublisher eventBusPublisher)
        {
            if (eventBusPublisher == null)
                throw new ArgumentNullException(nameof(eventBusPublisher));

            _eventBusPublisher = eventBusPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var version = 1;

            while (!stoppingToken.IsCancellationRequested && version <= 10)
            {
                var @event = new SampleEvent(aggregateId: Guid.NewGuid(), version: version++);
                var context = new EventContext<SampleEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);

                await _eventBusPublisher.PublishAsync(context);

                await Task.Delay(1000);
            }
        }
    }
}
