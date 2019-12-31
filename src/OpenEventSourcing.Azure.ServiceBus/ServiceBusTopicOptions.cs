using System;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusTopicOptions
    {
        internal string Name { get; set; }

        public ServiceBusTopicOptions WithName(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (topicName.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(topicName), $"'{nameof(topicName)}' exceeds the 50 character limit for then entity path name.");

            Name = topicName;

            return this;
        }
    }
}
