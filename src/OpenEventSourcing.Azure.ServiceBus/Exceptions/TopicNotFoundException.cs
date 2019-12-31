using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class TopicNotFoundException : Exception
    {
        public string TopicName { get; }

        public TopicNotFoundException(string name)
            : base($"A topic with name '{name}' could not be found.")
        {
            TopicName = name;
        }
    }
}
