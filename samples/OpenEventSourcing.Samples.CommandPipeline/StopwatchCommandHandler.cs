
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    public class StopwatchCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ILogger<SampleCommandHandler> _logger;

        public StopwatchCommandHandler(ILogger<SampleCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(Func<Task> next, TCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"'{typeof(TCommand).Name}' received by handler '{typeof(StopwatchCommandHandler<TCommand>).Name}'.");

            return next();
        }
    }
}
