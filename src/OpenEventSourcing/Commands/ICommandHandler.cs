using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
