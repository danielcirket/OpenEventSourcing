using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Receiver
{
    public class ReceiverTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }
        public IConfiguration Configuration { get; }

        public ReceiverTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddEvents()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"receiver-queue-{Guid.NewGuid()}");
                             s.ForEvent<SampleReceiverEvent>();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
            Configuration = fixture.Configuration;
        }

        [IntegrationTest]
        public void WhenStartCalledThenShouldConsumePublishedEventWithSingleSubscription()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var @event = new SampleReceiverEvent();
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var receiver = scope.ServiceProvider.GetRequiredService<IQueueMessageReceiver>();
                var sentTime = DateTimeOffset.MinValue;

                Func<Task> act = async () => await receiver.StartAsync(CancellationToken.None);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    // Let the consumer actually startup, needs to open a connection which may take a short amount of time.
                    await Task.Delay(500);

                    await sender.SendAsync(@event);
                    sentTime = DateTimeOffset.UtcNow;

                    // Delay to ensure that we pick up the message.
                    await Task.Delay(250);
                };

                verify.Should().NotThrow();

                SampleReceiverEventHandler.Received.Should().Be(1);
            }
        }
        [IntegrationTest]
        public void WhenStartCalledThenShouldConsumePublishedEventWithMultipleSubscriptions()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddEvents()
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
                             s.UseName($"receiver-queue-{Guid.NewGuid()}");
                             s.ForEvent<MultipleSampleReceiverEventOne>();
                             s.AutoDelete();
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"receiver-queue-{Guid.NewGuid()}");
                             s.ForEvent<MultipleSampleReceiverEventTwo>();
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
                var event1 = new MultipleSampleReceiverEventOne();
                var event2 = new MultipleSampleReceiverEventTwo();
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var receiver = scope.ServiceProvider.GetRequiredService<IQueueMessageReceiver>();

                Func<Task> act = async () => await receiver.StartAsync(CancellationToken.None);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                // Let the consumer actually startup, needs to open a connection which may take a short amount of time.
                await Task.Delay(500);

                    await sender.SendAsync(new IEvent[] { event1, event2 });

                // Delay to ensure that we pick up the message.
                await Task.Delay(250);
                };

                verify.Should().NotThrow();

                MultipleSampleReceiverEventHandlerOne.Received.Should().Be(1);
                MultipleSampleReceiverEventHandlerTwo.Received.Should().Be(1);
            }
        }
        [IntegrationTest]
        public void WhenStartCalledThenShouldNotConsumePublishedEventWithoutSubscription()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddEvents()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"receiver-queue-{Guid.NewGuid()}");
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
                var @event = new SampleNonSubscriptionReceiverEvent();
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var receiver = scope.ServiceProvider.GetRequiredService<IQueueMessageReceiver>();

                Func<Task> act = async () => await receiver.StartAsync(CancellationToken.None);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                // Let the consumer actually startup, needs to open a connection which may take a short amount of time.
                await Task.Delay(500);

                    await sender.SendAsync(@event);

                // Delay to ensure that we pick up the message.
                await Task.Delay(250);
                };

                verify.Should().NotThrow();

                SampleNonSubscriptionReceiverEventHandler.Received.Should().Be(0);
            }
        }

        private class SampleReceiverEvent : Event
        {
            public SampleReceiverEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
        private class SampleReceiverEventHandler : IEventHandler<SampleReceiverEvent>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(SampleReceiverEvent @event)
            {
                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
        private class SampleNonSubscriptionReceiverEvent : Event
        {
            public SampleNonSubscriptionReceiverEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
        private class SampleNonSubscriptionReceiverEventHandler : IEventHandler<SampleNonSubscriptionReceiverEvent>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(SampleNonSubscriptionReceiverEvent @event)
            {
                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
        private class MultipleSampleReceiverEventOne : Event
        {
            public MultipleSampleReceiverEventOne() : base(Guid.NewGuid(), 1)
            {
            }
        }
        private class MultipleSampleReceiverEventHandlerOne : IEventHandler<MultipleSampleReceiverEventOne>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(MultipleSampleReceiverEventOne @event)
            {
                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
        private class MultipleSampleReceiverEventTwo : Event
        {
            public MultipleSampleReceiverEventTwo() : base(Guid.NewGuid(), 1)
            {
            }
        }
        private class MultipleSampleReceiverEventHandlerTwo : IEventHandler<MultipleSampleReceiverEventTwo>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(MultipleSampleReceiverEventTwo @event)
            {
                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
    }
}
