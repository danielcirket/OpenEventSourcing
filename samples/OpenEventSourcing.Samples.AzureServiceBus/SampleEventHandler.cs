using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.AzureServiceBus
{
    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        public Task HandleAsync(IEventContext<SampleEvent> context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
