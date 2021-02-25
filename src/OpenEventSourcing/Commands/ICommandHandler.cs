using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(Func<Task> next, TCommand command, CancellationToken cancellationToken = default);
    }
}
