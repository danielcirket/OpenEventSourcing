﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using RabbitMQ.Client.Exceptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections
{
    public class ConnectionTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public ConnectionTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();
            
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        [RabbitMqTest]
        public async Task WhenCreateConnectionAsyncCalledThenShouldReturnConnection()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();

            var connection = await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

            connection.Should().NotBeNull();

            connection.Dispose();
        }
        [RabbitMqTest]
        public void WhenRabbitMqNotReachableCreateConnectionAsyncShouldThrowBrokerUnreachableException()
        {
            var services = new ServiceCollection();
            
            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:1324/")
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();
            
            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

            var factory = sp.GetRequiredService<IRabbitMqConnectionFactory>();

            Func<Task<IRabbitMqConnection>> act = async () => await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

            act.Should().Throw<BrokerUnreachableException>();
        }
    }
}
