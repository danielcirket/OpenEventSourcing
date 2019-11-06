using System;
using Xunit.Sdk;

namespace OpenEventSourcing.Testing.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("OpenEventSourcing.Testing.Discoverers.BugTraitDiscoverer", "OpenEventSourcing.Testing")]
    public sealed class BugAttribute : Attribute, ITraitAttribute 
    {
        public string Id { get; }

        public BugAttribute() { }
        public BugAttribute(string id)
        {
            Id = id;
        }
        public BugAttribute(long id)
        {
            Id = id.ToString();
        }
    }
}
