using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    internal sealed class DefaultCommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger<DefaultCommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandStore _commandStore;

        public DefaultCommandDispatcher(ILogger<DefaultCommandDispatcher> logger,
                                 IServiceProvider serviceProvider,
                                 ICommandStore commandStore)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (commandStore == null)
                throw new ArgumentNullException(nameof(commandStore));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _commandStore = commandStore;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation($"Dispatching command '{typeof(TCommand).FriendlyName()}'.");

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
                throw new InvalidOperationException($"No command handler for type '{typeof(TCommand).FriendlyName()}' has been registered.");

            await handler.ExecuteAsync(command, cancellationToken);
            await _commandStore.SaveAsync(command, cancellationToken);
        }
    }
}
