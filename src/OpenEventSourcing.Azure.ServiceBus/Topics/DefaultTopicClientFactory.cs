using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    internal sealed class DefaultTopicClientFactory : ITopicClientFactory
    {
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly ServiceBusConnectionStringBuilder _connectionStringBuilder;
        private ITopicClient _topicClient;

        public DefaultTopicClientFactory(ServiceBusConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException(nameof(connectionStringBuilder));

            _connectionStringBuilder = connectionStringBuilder;
            _topicClient = new TopicClient(_connectionStringBuilder, RetryPolicy.Default);
        }
        public ITopicClient Create()
        {
            if (_topicClient.IsClosedOrClosing)
                _topicClient = new TopicClient(_connectionStringBuilder, RetryPolicy.Default);

            return _topicClient;
        }
    }
}
