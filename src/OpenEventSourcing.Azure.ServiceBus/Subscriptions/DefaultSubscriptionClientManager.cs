using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Azure.ServiceBus.Topics;

namespace OpenEventSourcing.Azure.ServiceBus.Subscriptions
{
    internal sealed class DefaultSubscriptionClientManager : ISubscriptionClientManager
    {
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;
        private readonly ITopicMessageReceiver _messageReceiver;
        private readonly ServiceBusConnectionStringBuilder _connectionStringBuilder;
        private List<ISubscriptionClient> _clients;

        public DefaultSubscriptionClientManager(IOptions<ServiceBusOptions> options,
                                                ISubscriptionClientFactory subscriptionClientFactory,
                                                ITopicMessageReceiver messageReceiver,
                                                ServiceBusConnectionStringBuilder connectionStringBuilder)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (subscriptionClientFactory == null)
                throw new ArgumentNullException(nameof(subscriptionClientFactory));
            if (messageReceiver == null)
                throw new ArgumentNullException(nameof(messageReceiver));
            if (connectionStringBuilder == null)
                throw new ArgumentNullException(nameof(connectionStringBuilder));

            _options = options;
            _subscriptionClientFactory = subscriptionClientFactory;
            _messageReceiver = messageReceiver;
            _connectionStringBuilder = connectionStringBuilder;
            _clients = new List<ISubscriptionClient>();
        }

        public async Task<IReadOnlyList<ISubscriptionClient>> ConfigureAsync()
        {
            // If we've already been configured, just close the existing clients for now.
            foreach (var client in _clients)
                await client.CloseAsync();

            var clients = new List<ISubscriptionClient>();
            var managementClient = new ManagementClient(_connectionStringBuilder);
            
            foreach (var subscription in _options.Value.Subscriptions)
            {
                var client = _subscriptionClientFactory.Create(subscription);

                var subscriptionExists = await managementClient.SubscriptionExistsAsync(client.TopicPath, client.SubscriptionName);

                if (!subscriptionExists)
                    await managementClient.CreateSubscriptionAsync(client.TopicPath, client.SubscriptionName);

                var rules = await client.GetRulesAsync();

                if (subscription.Events.Any() && rules.Any(rule => rule.Name == RuleDescription.DefaultRuleName))
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName);

                var rulesToAdd = subscription.Events.Where(@event => !rules.Any(rule => rule.Name != @event.Name));
                var rulesToRemove = rules.Where(rule => !subscription.Events.Any(@event => @event.Name != rule.Name));

                foreach (var type in rulesToAdd)
                    await client.AddRuleAsync(new RuleDescription(filter: new CorrelationFilter { Label = type.Name }, name: type.Name));

                foreach (var rule in rulesToRemove)
                    await client.RemoveRuleAsync(rule.Name);

                client.RegisterMessageHandler((message, cancelationToken) => _messageReceiver.RecieveAsync(client, message, cancelationToken), _messageReceiver.OnErrorAsync);

                clients.Add(client);
            }

            _clients = clients;

            return _clients.AsReadOnly();
        }
    }
}
