using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.RabbitMq
{
    internal class SampleEventConsumer : IHostedService
    {
        private readonly IEventBusConsumer _eventBusConsumer;

        public SampleEventConsumer(IEventBusConsumer eventBusConsumer)
        {
            if (eventBusConsumer == null)

                throw new ArgumentNullException(nameof(eventBusConsumer));

            _eventBusConsumer = eventBusConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _eventBusConsumer.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _eventBusConsumer.StopAsync(cancellationToken);
        }
    }
}
