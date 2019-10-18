using System;

namespace OpenEventSourcing.RabbitMQ.Exceptions
{
    public class QueueNotFoundException : Exception
    {
        public string QueueName { get; }

        public QueueNotFoundException(string name)
            : base($"A queue with name '{name}' could not be found.")
        {
            QueueName = name;
        }

    }
}
