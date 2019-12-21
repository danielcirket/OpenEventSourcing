using System;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusTopicOptions
    {
        internal string Name { get; set; }

        public ServiceBusTopicOptions UseName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            Name = name;

            return this;
        }
    }
}
