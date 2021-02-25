using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands.Pipeline
{
    public class CommandPipeline<TCommand> where TCommand : ICommand
    {
        private readonly CommandHandlerDelegate _pipe;

        public CommandPipeline(CommandHandlerDelegate pipe)
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            _pipe = pipe;
        }

        public Task ExecuteAsync(CommandContext context, TCommand command, CancellationToken cancellationToken)
            => _pipe(context, command, cancellationToken);
    }
}
