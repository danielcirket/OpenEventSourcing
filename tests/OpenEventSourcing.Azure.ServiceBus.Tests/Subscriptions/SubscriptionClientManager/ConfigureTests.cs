using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Subscriptions.SubscriptionClientManager
{
    public class ConfigureTests : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public ConfigureTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(e =>
                         {
                             e.WithName($"test-topic-{Guid.NewGuid()}");
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-sub-{Guid.NewGuid()}");
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
        public void WhenConfigureAsyncCalledThenShouldConfigureTopic()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionClientManager>();

                Func<Task> act = async () => await manager.ConfigureAsync();

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var exists = await managementClient.TopicExistsAsync(topicPath: builder.EntityPath);

                    exists.Should().BeTrue();
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenConfigureAsyncCalledThenShouldConfigureSubscriptions()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<ServiceBusOptions>>();
                var managementClient = new ManagementClient(builder);
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionClientManager>();

                Func<Task> act = async () => await manager.ConfigureAsync();

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var topicExists = await managementClient.TopicExistsAsync(topicPath: builder.EntityPath);

                    topicExists.Should().BeTrue();

                    foreach (var subscription in options.Value.Subscriptions)
                    {
                        var exists = await managementClient.SubscriptionExistsAsync(builder.EntityPath, subscription.Name);

                        exists.Should().BeTrue();
                    }
                };

                verify.Should().NotThrow();
            }
        }

        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                managementClient.DeleteTopicAsync(builder.EntityPath).GetAwaiter().GetResult();
            }
        }
    }
}
