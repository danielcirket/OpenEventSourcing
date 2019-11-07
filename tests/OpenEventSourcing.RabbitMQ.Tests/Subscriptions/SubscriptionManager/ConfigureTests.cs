using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Tests.Subscriptions.SubscriptionManager
{
    public class ConfigureTests
    {
        [IntegrationTest]
        public void WhenConfigureAsyncCalledThenShouldConfigureExchange()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-queue-{Guid.NewGuid()}");
                         });
                    })
                    .AddJsonSerializers();

            var sp = services.BuildServiceProvider();
            var manager = sp.GetRequiredService<ISubscriptionManager>();
            var managementCient = sp.GetRequiredService<IRabbitMqManagementClient>();
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>();

            Func<Task> act = async () => await manager.ConfigureAsync();

            act.Should().NotThrow();

            Func<Task> verify = async () => await managementCient.CreateExchangeAsync(name: options.Value.Exchange.Name, ExchangeType.Topic, durable: false);

            verify.Should().Throw<ExchangeAlreadyExistsException>()
                .And.ExchangeName.Should().Be(options.Value.Exchange.Name);
        }
        [IntegrationTest]
        public void WhenConfigureAsyncCalledThenShouldConfigureSubscriptions()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-queue-{Guid.NewGuid()}");
                         });
                    })
                    .AddJsonSerializers();

            var sp = services.BuildServiceProvider();
            var manager = sp.GetRequiredService<ISubscriptionManager>();
            var managementCient = sp.GetRequiredService<IRabbitMqManagementClient>();
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>();

            Func<Task> act = async () => await manager.ConfigureAsync();

            act.Should().NotThrow();

            var queue = options.Value.Subscriptions.First().Name;

            Func<Task> verify = async () => await managementCient.CreateQueueAsync(name: queue, durable: false);

            verify.Should().Throw<QueueAlreadyExistsException>()
                .And.QueueName.Should().Be(queue);
        }

        // TODO(Dan): Subscriptions, we can't get that information from the internal IModel currently.
    }
}
