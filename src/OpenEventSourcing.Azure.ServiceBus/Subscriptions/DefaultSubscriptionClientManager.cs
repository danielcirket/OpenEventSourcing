using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Azure.ServiceBus.Management;
using OpenEventSourcing.Azure.ServiceBus.Topics;

namespace OpenEventSourcing.Azure.ServiceBus.Subscriptions
{
    internal sealed class DefaultSubscriptionClientManager : ISubscriptionClientManager
    {
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly ISubscriptionClientFactory _subscriptionClientFactory;
        private readonly ITopicMessageReceiver _messageReceiver;
        private readonly IServiceBusManagementClient _managementClient;
        private List<ISubscriptionClient> _clients;

        public DefaultSubscriptionClientManager(IOptions<ServiceBusOptions> options,
                                                ISubscriptionClientFactory subscriptionClientFactory,
                                                ITopicMessageReceiver messageReceiver,
                                                IServiceBusManagementClient managementClient)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (subscriptionClientFactory == null)
                throw new ArgumentNullException(nameof(subscriptionClientFactory));
            if (messageReceiver == null)
                throw new ArgumentNullException(nameof(messageReceiver));
            if (managementClient == null)
                throw new ArgumentNullException(nameof(managementClient));

            _options = options;
            _subscriptionClientFactory = subscriptionClientFactory;
            _messageReceiver = messageReceiver;
            _managementClient = managementClient;
            _clients = new List<ISubscriptionClient>();
        }

        public async Task<IReadOnlyList<ISubscriptionClient>> ConfigureAsync()
        {
            // If we've already been configured, just close the existing clients for now.
            foreach (var client in _clients)
                await client.CloseAsync();

            var clients = new List<ISubscriptionClient>();
            var topicExists = await _managementClient.TopicExistsAsync(_options.Value.Topic.Name);

            if (!topicExists)
                await _managementClient.CreateTopicAsync(_options.Value.Topic.Name);
            
            foreach (var subscription in _options.Value.Subscriptions)
            {
                var client = _subscriptionClientFactory.Create(subscription);
                var subscriptionExists = await _managementClient.SubscriptionExistsAsync(client.SubscriptionName, client.TopicPath);

                if (!subscriptionExists)
                    await _managementClient.CreateSubscriptionAsync(client.SubscriptionName, client.TopicPath);

                var rules = await client.GetRulesAsync();

                if (subscription.Events.Any() && rules.Any(rule => rule.Name == RuleDescription.DefaultRuleName))
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName);

                var rulesToAdd = subscription.Events.Where(@event => !rules.Any(rule => rule.Name == @event.Name));
                var rulesToRemove = rules.Where(rule => !subscription.Events.Any(@event => @event.Name == rule.Name))
                                         .Where(rule => rule.Name != RuleDescription.DefaultRuleName);

                foreach (var type in rulesToAdd)
                    await client.AddRuleAsync(new RuleDescription(filter: new CorrelationFilter { Label = type.Name }, name: type.Name));

                foreach (var rule in rulesToRemove)
                    await client.RemoveRuleAsync(rule.Name);

                client.RegisterMessageHandler((message, cancelationToken) => _messageReceiver.ReceiveAsync(client, message, cancelationToken),
                                               new MessageHandlerOptions(_messageReceiver.OnErrorAsync) { AutoComplete = false });

                clients.Add(client);
            }

            _clients = clients;

            return _clients.AsReadOnly();
        }
    }
}
