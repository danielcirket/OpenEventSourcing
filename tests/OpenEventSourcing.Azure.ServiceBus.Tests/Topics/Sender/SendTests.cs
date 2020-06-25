using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.Sender
{
    public class SendTests : ServiceBusSpecification, IDisposable
    {
        public SendTests(ConfigurationFixture fixture) : base(fixture) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Configuration.GetValue<string>("Azure:ServiceBus:ConnectionString"))
                         .UseTopic(e =>
                         {
                             e.WithName($"test-topic-{Guid.NewGuid()}");
                             e.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();
        }

        [ServiceBusTest]
        public void WhenSendAsyncCalledWithSingleNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEventNotification<IEvent>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("context");
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEventNotification<IEvent>>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("contexts");
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            { 
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var @event = new SameSenderEvent();
                var notification = new EventNotification<SameSenderEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null); ;

                Func<Task> act = async () => await sender.SendAsync(notification);

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithMultipleEventsThenShouldSendEvents()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var events = new[] { new SameSenderEvent(), new SameSenderEvent() };
                var notifications = events.Select(@event => new EventNotification<SameSenderEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null));
                
                Func<Task> act = async () => await sender.SendAsync(notifications);

                act.Should().NotThrow();
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

        private class SameSenderEvent : Event
        {
            public SameSenderEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
    }
}
