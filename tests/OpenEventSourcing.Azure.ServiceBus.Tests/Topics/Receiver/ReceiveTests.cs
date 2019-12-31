using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using System.Threading.Tasks;
using FluentAssertions;
using OpenEventSourcing.Testing.Attributes;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System.Threading;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.Receiver
{
    public class ReceiveTests : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public ReceiveTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddEvents()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(e =>
                         {
                             e.WithName($"test-topic-{Guid.NewGuid()}");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"receiver-{Guid.NewGuid()}");
                             s.ForEvent<SampleReceiverEvent>();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [ServiceBusTest]
        public void WhenStartCalledThenShouldConsumePublishedEventWithSingleSubscription()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var @event = new SampleReceiverEvent();
                var sender = scope.ServiceProvider.GetRequiredService<IEventBusPublisher>();
                var receiver = scope.ServiceProvider.GetRequiredService<IEventBusConsumer>();
                var sentTime = DateTimeOffset.MinValue;

                Func<Task> act = async () => await receiver.StartAsync(CancellationToken.None);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    // Let the consumer actually startup, needs to open a connection which may take a short amount of time.
                    await Task.Delay(250);

                    await sender.PublishAsync(@event);
                    sentTime = DateTimeOffset.UtcNow;

                    // Delay to ensure that we pick up the message.
                    await Task.Delay(500);
                };

                verify.Should().NotThrow();

                SampleReceiverEventHandler.Received.Should().Be(1);
            }
        }

        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                if (managementClient.TopicExistsAsync(builder.EntityPath).GetAwaiter().GetResult())
                    managementClient.DeleteTopicAsync(builder.EntityPath).GetAwaiter().GetResult();
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
    }
}
