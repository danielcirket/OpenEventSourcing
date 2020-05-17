using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    }
}
