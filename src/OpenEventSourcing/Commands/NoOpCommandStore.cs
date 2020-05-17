using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Commands
{
    internal sealed class NoOpCommandStore : ICommandStore
    {
        public Task SaveAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return Task.CompletedTask;
        }
    }
}
