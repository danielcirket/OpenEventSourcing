using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Tests.Events
{
    internal class FakeEventHandler : IEventHandler<FakeEvent>
    {
        private int _calls = 0;

        public int Calls => _calls;

        public Task HandleAsync(FakeEvent @event)
        {
            Interlocked.Increment(ref _calls);

            return Task.CompletedTask;
        }
    }
}
