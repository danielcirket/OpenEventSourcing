namespace OpenEventSourcing.RabbitMQ.Management
{
    public class RabbitMqBinding
    {
        public string Queue { get; set; }
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
    }
}
