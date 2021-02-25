using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands.Pipeline
{
    public delegate Task CommandHandlerDelegate(CommandContext context, ICommand command, CancellationToken cancellationToken);
}
