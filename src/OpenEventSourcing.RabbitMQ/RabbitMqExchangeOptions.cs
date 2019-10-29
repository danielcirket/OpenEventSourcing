using System;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqExchangeOptions
    {
        internal string Name { get; set; }
        internal string Type { get; set; }

        public RabbitMqExchangeOptions WithName(string exchange)
        {
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentException($"'{nameof(exchange)}' cannot be null or empty.", nameof(exchange));

            Name = exchange;

            return this;
        }
        public RabbitMqExchangeOptions UseExchangeType(string exchangeType)
        {
            if (string.IsNullOrEmpty(exchangeType))
                throw new ArgumentException($"'{nameof(exchangeType)}' cannot be null or empty.", nameof(exchangeType));

            Type = exchangeType;

            return this;
        }
    }
}
