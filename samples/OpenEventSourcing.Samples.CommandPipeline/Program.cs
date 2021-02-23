using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Samples.CommandPipeline
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.AddOpenEventSourcing()
                        .AddCommands(options =>
                        {
                            options.For<SampleCommand>()
                                .UseStopwatchProfiling()
                                .Use<SampleCommandHandler>()
                                .Use(async (context, next, command, cancellationToken) =>
                                {
                                    var logger = context.ApplicationServices.GetRequiredService<ILogger<SampleCommand>>();
                                    
                                    logger.LogInformation($"'{command.Subject}' received by middleware #3.");
                                    
                                    await Task.Delay(1000, cancellationToken);
                                    await next();
                                });
                        });
                        
                    services.AddHostedService<SampleCommandPublisher>();

                    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
                });
    }
}
