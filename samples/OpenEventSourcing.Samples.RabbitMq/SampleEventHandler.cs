using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Samples.RabbitMq
{
    public class SampleEventHandler : IEventHandler<SampleEvent>
    {
        public Task HandleAsync(SampleEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
