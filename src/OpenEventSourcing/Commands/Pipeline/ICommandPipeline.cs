using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands.Pipeline
{
    public interface ICommandPipeline<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(CommandContext context, TCommand command, CancellationToken cancellationToken);
    }
}