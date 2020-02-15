using System;

namespace OpenEventSourcing.RabbitMQ.Extensions
{
    public static class RabbitMqExchangeOptionsExtensions
    {
        public static RabbitMqExchangeOptions UseTopicExchangeType(this RabbitMqExchangeOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.UseExchangeType(ExchangeTypes.Topic);

            return options;
        }
        public static RabbitMqExchangeOptions UseDirectExchangeType(this RabbitMqExchangeOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.UseExchangeType(ExchangeTypes.Direct);

            return options;
        }
        public static RabbitMqExchangeOptions UseFanoutExchangeType(this RabbitMqExchangeOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.UseExchangeType(ExchangeTypes.Fanout);

            return options;
        }
        public static RabbitMqExchangeOptions UseHeadersExchangeType(this RabbitMqExchangeOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.UseExchangeType(ExchangeTypes.Headers);

            return options;
        }
    }
}
