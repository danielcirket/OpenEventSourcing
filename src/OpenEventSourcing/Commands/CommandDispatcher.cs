using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    internal sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandStore _commandStore;

        public CommandDispatcher(ILogger<CommandDispatcher> logger,
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

        public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _logger.LogInformation($"Dispatching command '{typeof(TCommand).FriendlyName()}'.");

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
                throw new InvalidOperationException($"No command handler for type '{typeof(TCommand).FriendlyName()}' has been registered.");

            await handler.ExecuteAsync(command);
            await _commandStore.SaveAsync(command);
        }
    }
}
