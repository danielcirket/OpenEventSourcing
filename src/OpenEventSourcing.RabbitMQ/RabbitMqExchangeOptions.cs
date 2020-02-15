using System;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqExchangeOptions
    {
        public string Name { get; internal set; }
        public string Type { get; internal set; }
        public bool ShouldAutoDelete { get; internal set; }
        public bool Durable { get; internal set; }

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
        /// <summary>
        /// Sets the exchange to auto delete once all queues have finished using the exchange. Defaults to false.
        /// </summary>
        /// <returns></returns>
        public RabbitMqExchangeOptions AutoDelete()
        {
            ShouldAutoDelete = true;

            return this;
        }
        /// <summary>
        /// Sets the exchange to be durable. Durable exchanges remain active when a server restarts. Non-durable exchanges (transient exchanges) are purged if/when a server restarts. Defaults to true.
        /// </summary>
        /// <param name="durable"></param>
        /// <returns></returns>
        public RabbitMqExchangeOptions WithDurable(bool durable)
        {
            Durable = durable;

            return this;
        }
    }
}
