using System;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Tests.Fakes
{
    internal sealed class FakeDispatchableCommandHandler : ICommandHandler<FakeDispatchableCommand>
    {
        private int _handled = 0;
        public int Handled => _handled;

        public Task ExecuteAsync(Func<Task> next, FakeDispatchableCommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            Interlocked.Increment(ref _handled);

            return next();
        }
    }
}
