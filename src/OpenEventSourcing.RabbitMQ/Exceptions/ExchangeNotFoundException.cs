using System;

namespace OpenEventSourcing.RabbitMQ.Exceptions
{
    public class ExchangeNotFoundException : Exception
    {
        public string ExchangeName { get; }

        public ExchangeNotFoundException(string name)
            : base($"An exchange with name '{name}' does not exist.")
        {
            ExchangeName = name;
        }

    }
}
