using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        private readonly ILogger<SampleCommandHandler> _logger;

        public SampleCommandHandler(ILogger<SampleCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(Func<Task> next, SampleCommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"'{command.Subject}' received by handler '{nameof(SampleCommandHandler)}'.");

            return next();
        }
    }
}
