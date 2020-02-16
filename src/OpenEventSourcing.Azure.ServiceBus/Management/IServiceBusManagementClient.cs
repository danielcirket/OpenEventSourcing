using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenEventSourcing.Azure.ServiceBus.Management
{
    public interface IServiceBusManagementClient
    {
        Task CreateTopicAsync(string topicName, TimeSpan? deleteOnIdleAfter = null, TimeSpan? timeToLive = null);
        Task CreateSubscriptionAsync(string subscriptionName, string topicName, TimeSpan? deleteOnIdleAfter = null, TimeSpan? timeToLive = null, TimeSpan ? lockDuration = null, int? maxDeliveryCount = null, bool useDeadLetterOnExpiration = false);
        Task CreateRuleAsync(string ruleName, string subscriptionName, string topicName);

        Task<bool> TopicExistsAsync(string topicName);
        Task<bool> SubscriptionExistsAsync(string subscriptionName, string topicName);
        Task<bool> RuleExistsAsync(string ruleName, string subscriptionName, string topicName);

        Task RemoveTopicAsync(string topicName);
        Task RemoveSubscriptionAsync(string subscriptionName, string topicName);
        Task RemoveRuleAsync(string ruleName, string subscriptionName, string topicName);

        Task<IEnumerable<ServiceBusRule>> RetrieveRulesAsync(string subscriptionName, string topicName);
    }
}
