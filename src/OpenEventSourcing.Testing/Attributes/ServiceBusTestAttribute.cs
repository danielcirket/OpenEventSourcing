using System;
using Xunit;
using Xunit.Sdk;

namespace OpenEventSourcing.Testing.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("OpenEventSourcing.Testing.Discoverers.ServiceBusTestTraitDiscoverer", "OpenEventSourcing.Testing")]
    public class ServiceBusTestAttribute : FactAttribute, ITraitAttribute 
    {
        private static readonly string ENVIRONMENT_VARIABLE = "AZURE_SERVICE_BUS_CONNECTION_STRING";

        public override string Skip { get; set; }

        public ServiceBusTestAttribute()
        {
            if (!HasAzureServiceBusConnectionString())
                Skip = $"Skipping Azure Service Bus Test, no connection string configured. To enable service bus tests set the '{ENVIRONMENT_VARIABLE}' environment variable.";
        }

        private static bool HasAzureServiceBusConnectionString()
            => Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE) != null;
    }
}
