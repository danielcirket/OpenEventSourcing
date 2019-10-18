﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;

namespace OpenEventSourcing.Samples.RabbitMq
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
                            .AddRabbitMq(options =>
                            {
                                options.UseConnection("amqp://guest:guest@localhost:5672/")
                                       .UseExchange($"sample-exchange-{Guid.NewGuid()}")
                                       .AddSubscription(s =>
                                       {
                                           s.UseName(queueName: "rabbit-mq-sample-queue");

                                           s.ForEvent<SampleEvent>();
                                       });
                            })
                            .AddEvents()
                            .AddJsonSerializers();

                    services.AddHostedService<SampleEventPublisher>();
                    services.AddHostedService<SampleEventConsumer>();

                    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
                });
    }
}
