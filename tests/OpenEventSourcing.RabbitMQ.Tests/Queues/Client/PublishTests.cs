using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Client
{
    public class PublishTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public PublishTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
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

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [RabbitMqTest]
        public void WhenPublishAsyncCalledWithNullMessageThenShouldThrowArgumentNullException()
        {
            var connectionFactory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var options = ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();

            Func<Task> act = async () =>
            {
                var connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
                await connection.PublishAsync((Message)null);
            };

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("message");
        }
        [RabbitMqTest]
        public void WhenPublishAsyncCalledWithNullMessagesThenShouldThrowArgumentNullException()
        {
            var connectionFactory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var options = ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();

            Func<Task> act = async () =>
            {
                var connection = await connectionFactory.CreateConnectionAsync(CancellationToken.None);
                await connection.PublishAsync((IEnumerable<Message>)null);
            };

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("messages");
        }
    }
}
