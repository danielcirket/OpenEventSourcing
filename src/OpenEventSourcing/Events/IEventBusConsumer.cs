using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventBusConsumer
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
