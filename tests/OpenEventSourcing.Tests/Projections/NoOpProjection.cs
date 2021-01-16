using System.Threading.Tasks;
using OpenEventSourcing.Events;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.Tests.Projections
{
    internal class NoOpProjection : IProjection
    {
        public Task HandleAsync(IEventContext<IEvent> @event)
        {
            return Task.CompletedTask;
        }
    }
}
