using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
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

            builder.Services.AddSingleton<IEventBus, AzureServiceBus>();
            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<ISubscriptionClientFactory, DefaultSubscriptionClientFactory>();
            builder.Services.AddScoped<ISubscriptionClientManager, DefaultSubscriptionClientManager>();
            builder.Services.AddScoped<ITopicClientFactory, DefaultTopicClientFactory>();
            builder.Services.AddScoped<ITopicMessageReceiver, DefaultTopicMessageReceiver>();
            builder.Services.AddScoped<ITopicMessageSender, DefaultTopicMessageSender>();

            return builder;
        }
    }
}
