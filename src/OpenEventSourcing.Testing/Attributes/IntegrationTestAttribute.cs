using System;
using Xunit;
using Xunit.Sdk;

namespace OpenEventSourcing.Testing.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("OpenEventSourcing.Testing.Discoverers.IntegrationTestTraitDiscoverer", "OpenEventSourcing.Testing")]
    public class IntegrationTestAttribute : FactAttribute, ITraitAttribute { }
}
