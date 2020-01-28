using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace OpenEventSourcing.Azure.ServiceBus.Tests
{
    public class ServiceBusTestTraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Category", "ServiceBusTest");
        }
    }
}
