using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Projections
{
    public interface IProjector<TProjection>
        where TProjection : IProjection
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
