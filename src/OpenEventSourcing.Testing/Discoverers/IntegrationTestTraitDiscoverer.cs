using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace OpenEventSourcing.Testing.Discoverers
{
    public class IntegrationTestTraitDiscoverer : ITraitDiscoverer
    {
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            yield return new KeyValuePair<string, string>("Category", "IntegrationTest");
        }
    }
}
