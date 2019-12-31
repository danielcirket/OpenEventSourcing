using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class TopicAlreadyExistsException : Exception
    {
        public string TopicName { get; }

        public TopicAlreadyExistsException(string name)
            : base($"A topic with name '{name}' already exists.")
        {
            TopicName = name;
        }
    }
}
