using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.Tests.Projections
{
    internal class TestProjector : BackgroundProjector<NoOpProjection>
    {
        private readonly Task _task;

        public Task ExecutingTask { get; set; }

        public TestProjector(Task task)
        {
            _task = task;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            ExecutingTask = ExecuteInternalAsync(cancellationToken);
            await ExecutingTask;
        }
        private async Task ExecuteInternalAsync(CancellationToken cancellationToken)
        {
            var task = await Task.WhenAny(_task, Task.Delay(Timeout.Infinite, cancellationToken));
            await task;
        }
    }
}
