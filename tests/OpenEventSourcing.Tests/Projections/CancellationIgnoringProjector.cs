using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.Tests.Projections
{
    internal class CancellationIgnoringProjector : BackgroundProjector<NoOpProjection>
    {
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return new TaskCompletionSource<object>().Task;
        }
    }
}
