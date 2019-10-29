using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;


namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Client
{
    public class PublishTests
    {
        public IServiceProvider ServiceProvider { get; }

        public PublishTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                         });
                    })
                    .AddJsonSerializers();

            ServiceProvider = services.BuildServiceProvider();
        }

        [IntegrationTest]
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
        [IntegrationTest]
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
