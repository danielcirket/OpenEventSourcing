using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.Tests.Projections
{
    internal class WaitOnCancellationProjector : BackgroundProjector<NoOpProjection>
    {
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}
