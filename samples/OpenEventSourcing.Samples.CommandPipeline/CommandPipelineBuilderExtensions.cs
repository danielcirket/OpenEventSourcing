using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Commands.Pipeline;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    internal static class CommandPipelineBuilderExtensions
    {
        public static CommandPipelineBuilder<TCommand> UseStopwatchProfiling<TCommand>(this CommandPipelineBuilder<TCommand> builder)
            where TCommand : ICommand
        {
            return builder.Use(async (context, next, command, cancellationToken) =>
            {
                var logger = context.ApplicationServices.GetRequiredService<ILogger<SampleCommand>>();
                                    
                logger.LogInformation($"'{command.Subject}' received by '{nameof(UseStopwatchProfiling)}'.");
                                    
                var sw = Stopwatch.StartNew();
                                    
                await next();

                var elapsed = sw.ElapsedMilliseconds;

                sw.Stop();
                                    
                logger.LogInformation($"'{command.Subject}' after middleware pipeline executed from '{nameof(UseStopwatchProfiling)}'. {elapsed}ms.");                
            });
        }
    }
}
