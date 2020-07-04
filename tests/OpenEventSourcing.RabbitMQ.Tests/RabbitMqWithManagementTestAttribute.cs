using System;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Sdk;

namespace OpenEventSourcing.RabbitMQ.Tests
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("OpenEventSourcing.RabbitMQ.Tests.RabbitMqTestTraitDiscoverer", "OpenEventSourcing.RabbitMQ.Tests")]
    public class RabbitMqWithManagementTestAttribute : FactAttribute, ITraitAttribute 
    {
        private readonly IConfiguration _configuration;

        public override string Skip { get; set; }

        public RabbitMqWithManagementTestAttribute()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(RabbitMqTestAttribute).Assembly, optional: true)
                .AddEnvironmentVariables(prefix: "OPENEVENTSOURCING_")
                .Build();

            if (!HasRabbitMqConnectionString() || !HasRabbitMqManagementUri())
                Skip = $"Skipping RabbitMQ Test, no connection string or management URI configured.";
        }

        private bool HasRabbitMqConnectionString()
            => !string.IsNullOrWhiteSpace(_configuration.GetValue<string>("RabbitMQ:ConnectionString"));

        private bool HasRabbitMqManagementUri()
            => !string.IsNullOrWhiteSpace(_configuration.GetValue<string>("RabbitMQ:ManagementUri"));
    }
}
