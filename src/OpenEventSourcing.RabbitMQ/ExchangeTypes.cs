namespace OpenEventSourcing.RabbitMQ
{
    public static class ExchangeTypes
    {
        public static readonly string Direct = "direct";
        public static readonly string Fanout = "fanout";
        public static readonly string Headers = "headers";
        public static readonly string Topic = "topic";
    }
}
