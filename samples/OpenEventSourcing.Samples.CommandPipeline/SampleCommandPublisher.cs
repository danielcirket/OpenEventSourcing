using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    public class SampleCommandPublisher : BackgroundService
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public SampleCommandPublisher(ICommandDispatcher commandDispatcher)
        {
            if (commandDispatcher == null)
                throw new ArgumentNullException(nameof(commandDispatcher));

            _commandDispatcher = commandDispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var version = 1;

            while (!stoppingToken.IsCancellationRequested && version <= 100)
            {
                var command = new SampleCommand($"{nameof(SampleCommand)}-{version.ToString()}");

                await _commandDispatcher.DispatchAsync(command, stoppingToken);

                await Task.Delay(1000, stoppingToken);

                version++;
            }
        }
    }
}
