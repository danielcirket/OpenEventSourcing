namespace OpenEventSourcing.RabbitMQ.Management.Api.Responses
{
    internal class RabbitMqSubscriptionResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string source { get; set; }
        public string vhost { get; set; }
        public string destination { get; set; }
        public string destination_type { get; set; }
        public string routing_key { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
