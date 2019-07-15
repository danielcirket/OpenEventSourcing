using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Projections
{
    public interface IProjection
    {
        Task HandleAsync(IEvent @event);
    }
}
