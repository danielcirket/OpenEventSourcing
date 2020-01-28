using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Azure.ServiceBus.Exceptions;

namespace OpenEventSourcing.Azure.ServiceBus.Management
{
    internal sealed class ServiceBusManagementClient : IServiceBusManagementClient
    {
        private readonly ManagementClient _client;
        private readonly IOptions<ServiceBusOptions> _options;

        public ServiceBusManagementClient(ManagementClient client, IOptions<ServiceBusOptions> options)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _client = client;
            _options = options;
        }

        public async Task CreateRuleAsync(string ruleName, string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                throw new ArgumentException($"'{nameof(ruleName)}' cannot be null or empty.", nameof(ruleName));
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.", nameof(subscriptionName));

            var topicExists = await TopicExistsAsync(topicName);

            if (!topicExists)
                throw new TopicNotFoundException(topicName);

            var subscriptionExists = await SubscriptionExistsAsync(subscriptionName, topicName);

            if (!subscriptionExists)
                throw new SubscriptionNotFoundException(subscriptionName);

            var ruleExists = await RuleExistsAsync(ruleName, subscriptionName, topicName);

            if (ruleExists)
                throw new RuleAlreadyExistsException(ruleName);

            await _client.CreateRuleAsync(topicName, subscriptionName, new RuleDescription(filter: new CorrelationFilter { Label = ruleName }, name: ruleName));
        }
        public async Task CreateSubscriptionAsync(string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.", nameof(subscriptionName));

            try
            {
                var options = _options.Value.Subscriptions.FirstOrDefault(s => s.Name.Equals(subscriptionName, StringComparison.OrdinalIgnoreCase));

                var subscription = new SubscriptionDescription(topicName, subscriptionName)
                {
                    AutoDeleteOnIdle = options?.DeleteOnIdleAfter ?? ServiceBusSubscriptionDefaults.DeleteOnIdleAfter,
                    DefaultMessageTimeToLive = options?.TimeToLive ?? ServiceBusSubscriptionDefaults.TimeToLive,
                    EnableBatchedOperations = true,
                    EnableDeadLetteringOnMessageExpiration = options?.UseDeadLetterOnExpiration ?? ServiceBusSubscriptionDefaults.UseDeadLetterOnExpiration,
                    MaxDeliveryCount = options?.MaxDeliveryCount ?? ServiceBusSubscriptionDefaults.MaxDeliveryCount,
                    LockDuration = options?.LockDuration ?? ServiceBusSubscriptionDefaults.LockDuration,
                };

                await _client.CreateSubscriptionAsync(subscription);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                throw new SubscriptionAlreadyExistsException(subscriptionName);
            }
            catch (MessagingEntityNotFoundException)
            {
                throw new TopicNotFoundException(topicName);
            }
        }
        public async Task CreateTopicAsync(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));

            try
            {
                var options = _options.Value.Topic.Name.Equals(topicName, StringComparison.OrdinalIgnoreCase)
                    ? _options.Value.Topic
                    : null;

                var description = new TopicDescription(topicName)
                {
                    AutoDeleteOnIdle = options?.DeleteOnIdleAfter ?? ServiceBusTopicDefaults.DeleteOnIdleAfter,
                    DefaultMessageTimeToLive = options?.TimeToLive ?? ServiceBusTopicDefaults.TimeToLive,
                    EnableBatchedOperations = true,
                };

                await _client.CreateTopicAsync(description);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                throw new TopicAlreadyExistsException(topicName);
            }
        }
        public async Task RemoveRuleAsync(string ruleName, string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                throw new ArgumentException($"'{nameof(ruleName)}' cannot be null or empty.", nameof(ruleName));
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.", nameof(subscriptionName));

            var topicExists = await TopicExistsAsync(topicName);

            if (!topicExists)
                throw new TopicNotFoundException(topicName);

            var subscriptionExists = await SubscriptionExistsAsync(subscriptionName, topicName);

            if (!subscriptionExists)
                throw new SubscriptionNotFoundException(subscriptionName);

            var ruleExists = await RuleExistsAsync(ruleName, subscriptionName, topicName);

            if (!ruleExists)
                throw new RuleNotFoundException(ruleName);

            await _client.DeleteRuleAsync(topicName, subscriptionName, ruleName);
        }
        public async Task RemoveSubscriptionAsync(string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.");

            var topicExists = await TopicExistsAsync(topicName);

            if (!topicExists)
                throw new TopicNotFoundException(topicName);

            var subscriptionExists = await SubscriptionExistsAsync(subscriptionName, topicName);

            if (!subscriptionExists)
                throw new SubscriptionNotFoundException(subscriptionName);

            await _client.DeleteSubscriptionAsync(topicName, subscriptionName);
        }
        public async Task RemoveTopicAsync(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));

            try
            {
                await _client.DeleteTopicAsync(topicName);
            }
            catch (MessagingEntityNotFoundException)
            {
                throw new TopicNotFoundException(topicName);
            }
        }
        public async Task<bool> SubscriptionExistsAsync(string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.");

            var exists = await _client.SubscriptionExistsAsync(topicName, subscriptionName);

            return exists;
        }
        public Task<bool> TopicExistsAsync(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.");

            return _client.TopicExistsAsync(topicName);
        }
        public async Task<IEnumerable<ServiceBusRule>> RetrieveRulesAsync(string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.", nameof(subscriptionName));

            var pageSize = 25;
            var rules = await _client.GetRulesAsync(topicName, subscriptionName, count: pageSize);

            if (rules.Count < pageSize)
                return rules.Select(r => new ServiceBusRule { Topic = topicName, Subscription = subscriptionName, Rule = r.Name }).ToArray();

            var skip = rules.Count;
            var results = new List<ServiceBusRule>(rules.Select(r => new ServiceBusRule { Topic = topicName, Subscription = subscriptionName, Rule = r.Name }).ToArray());

            while (rules.Count == pageSize)
            {
                rules = await _client.GetRulesAsync(topicName, subscriptionName, count: pageSize, skip: skip);
                results.AddRange(rules.Select(r => new ServiceBusRule { Topic = topicName, Subscription = subscriptionName, Rule = r.Name }).ToArray());
                skip += rules.Count;
            }

            return results;
        }
        public async Task<bool> RuleExistsAsync(string ruleName, string subscriptionName, string topicName)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
                throw new ArgumentException($"'{nameof(ruleName)}' cannot be null or empty.", nameof(ruleName));
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException($"'{nameof(subscriptionName)}' cannot be null or empty.", nameof(subscriptionName));

            try
            {
                await _client.GetRuleAsync(topicName, subscriptionName, ruleName);

                return true;
            }
            catch (MessagingEntityNotFoundException)
            {
                return false;
            }
        }
    }
}
