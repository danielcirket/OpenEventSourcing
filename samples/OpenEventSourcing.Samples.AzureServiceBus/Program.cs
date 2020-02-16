using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;

namespace OpenEventSourcing.Samples.AzureServiceBus
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
                            .AddAzureServiceBus(options =>
                            {
                                options.UseConnection("A connection string with or without the entity path.")
                                       .UseTopic(topic =>
                                       {
                                           // If you set this it will take precedence over the entity path in the connection string...
                                           topic.WithName("sample-topic")
                                                .UseTimeToLive(TimeSpan.MaxValue)
                                                .UseLockDuration(TimeSpan.FromMinutes(5))
                                                .AutoDeleteOnIdleAfter(TimeSpan.MaxValue);
                                       })
                                       .AddSubscription(subscription =>
                                       {
                                           subscription.UseName(name: "sample-topic-subscription")
                                                       .UseTimeToLive(TimeSpan.MaxValue)
                                                       .UseLockDuration(TimeSpan.FromMinutes(1))
                                                       .WithMaxDeliveryCount(25)
                                                       .UseDeadLetterOnMessageExpiration(false)
                                                       .AutoDeleteOnIdleAfter(TimeSpan.MaxValue)
                                                       .ForEvent<SampleEvent>();
                                       });
                            })
                            .AddEvents()
                            .AddJsonSerializers();

                    services.AddHostedService<SampleEventConsumer>();
                    services.AddHostedService<SampleEventPublisher>();

                    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
                });
    }
}
