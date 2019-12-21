using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Extensions
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddAzureServiceBus(this IOpenEventSourcingBuilder builder, Action<ServiceBusOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.Configure(optionsAction);

            builder.Services.AddSingleton<IEventBusPublisher, AzureServiceBus>();
            builder.Services.AddSingleton<IEventBusConsumer, AzureServiceBus>();
            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<ISubscriptionClientFactory, DefaultSubscriptionClientFactory>();
            builder.Services.AddScoped<ISubscriptionClientManager, DefaultSubscriptionClientManager>();
            builder.Services.AddScoped<ITopicClientFactory, DefaultTopicClientFactory>();
            builder.Services.AddScoped<ITopicMessageReceiver, DefaultTopicMessageReceiver>();
            builder.Services.AddScoped<ITopicMessageSender, DefaultTopicMessageSender>();
            builder.Services.AddScoped<ServiceBusConnectionStringBuilder>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>();
                var connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.Value.ConnectionString);

                if (string.IsNullOrEmpty(connectionStringBuilder.EntityPath) && options.Value.Topic == null)
                    throw new InvalidOperationException($"Azure service bus connection string doesn't contain an entity path and 'UseTopic(...)' has not been called. Either include the entity path in the connection string or by calling 'UseTopic(...)' during startup when configuring Azure Service Bus.");

                if (options.Value.Topic != null)
                    connectionStringBuilder.EntityPath = options.Value.Topic.Name;

                return connectionStringBuilder;
            });

            return builder;
        }
    }
}
