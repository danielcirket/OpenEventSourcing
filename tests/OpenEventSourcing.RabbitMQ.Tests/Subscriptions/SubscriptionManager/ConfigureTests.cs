using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using OpenEventSourcing.Serialization.Json.Extensions;
using RabbitMQ.Client;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Subscriptions.SubscriptionManager
{
    public class ConfigureTests : IClassFixture<ConfigurationFixture>
    {
        public IConfiguration Configuration { get; }
        
        public ConfigureTests(ConfigurationFixture fixture)
        {
            Configuration = fixture.Configuration;
        }

        [RabbitMqTest]
        public void WhenConfigureAsyncCalledThenShouldConfigureExchange()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-queue-{Guid.NewGuid()}");
                             s.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            var sp = services.BuildServiceProvider(validateScopes: true);
#endif
            using (var scope = sp.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
                var managementCient = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();

                Func<Task> act = async () => await manager.ConfigureAsync();

                act.Should().NotThrow();

                Func<Task> verify = async () => await managementCient.CreateExchangeAsync(name: options.Value.Exchange.Name, ExchangeType.Topic, durable: false);

                verify.Should().Throw<ExchangeAlreadyExistsException>()
                    .And.ExchangeName.Should().Be(options.Value.Exchange.Name);
            }
        }
        [RabbitMqTest]
        public void WhenConfigureAsyncCalledThenShouldConfigureSubscriptions()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-queue-{Guid.NewGuid()}");
                             s.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            var sp = services.BuildServiceProvider(validateScopes: true);
#endif
            using (var scope = sp.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
                var managementCient = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();

                Func<Task> act = async () => await manager.ConfigureAsync();

                act.Should().NotThrow();

                var queue = options.Value.Subscriptions.First().Name;

                Func<Task> verify = async () => await managementCient.CreateQueueAsync(name: queue, durable: false);

                verify.Should().Throw<QueueAlreadyExistsException>()
                    .And.QueueName.Should().Be(queue);
            }
        }

        // TODO(Dan): Subscriptions, we can't get that information from the internal IModel currently.
    }
}
