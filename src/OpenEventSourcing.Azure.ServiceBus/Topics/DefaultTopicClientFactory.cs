using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Azure.ServiceBus.Management;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    internal sealed class DefaultTopicClientFactory : ITopicClientFactory
    {
        private readonly IServiceBusManagementClient _managementClient;
        private readonly ServiceBusConnectionStringBuilder _connectionStringBuilder;
        private ITopicClient _topicClient;

        public DefaultTopicClientFactory(IServiceBusManagementClient managementClient,
                                         ServiceBusConnectionStringBuilder connectionStringBuilder)
        {
            if (managementClient == null)
                throw new ArgumentNullException(nameof(managementClient));
            if (connectionStringBuilder == null)
                throw new ArgumentNullException(nameof(connectionStringBuilder));

            _managementClient = managementClient;
            _connectionStringBuilder = connectionStringBuilder;
        }
        public async Task<ITopicClient> CreateAsync()
        {
            var exists = await _managementClient.TopicExistsAsync(_connectionStringBuilder.EntityPath);

            if (!exists)
                await _managementClient.CreateTopicAsync(_connectionStringBuilder.EntityPath);

            if (_topicClient == null || _topicClient.IsClosedOrClosing)
                _topicClient = new TopicClient(_connectionStringBuilder, RetryPolicy.Default);

            return _topicClient;
        }
    }
}
