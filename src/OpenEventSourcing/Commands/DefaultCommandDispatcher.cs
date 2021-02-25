using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands.Pipeline;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    internal sealed class DefaultCommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger<DefaultCommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandContext _context;

        public DefaultCommandDispatcher(ILogger<DefaultCommandDispatcher> logger,
                                 IServiceProvider serviceProvider,
                                 CommandContext context)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation($"Dispatching command '{typeof(TCommand).FriendlyName()}'.");

            var pipeline = _serviceProvider.GetService<CommandPipeline<TCommand>>();

            if (pipeline == null)
                throw new InvalidOperationException($"No command pipeline for type '{typeof(TCommand).FriendlyName()}' has been registered.");

            await pipeline.ExecuteAsync(_context, command, cancellationToken);
        }
    }
}
