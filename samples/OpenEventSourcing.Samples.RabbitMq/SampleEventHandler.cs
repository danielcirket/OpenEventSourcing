using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.RabbitMq
{
    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        public Task HandleAsync(SampleEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
