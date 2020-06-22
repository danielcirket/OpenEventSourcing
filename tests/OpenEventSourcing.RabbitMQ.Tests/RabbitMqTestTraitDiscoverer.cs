using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace OpenEventSourcing.RabbitMQ.Tests
{
    public class ServiceBusTestTraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Category", "RabbitMQ");
        }
    }
}
