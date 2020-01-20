using System;
using Microsoft.Extensions.Configuration;

namespace OpenEventSourcing.RabbitMQ.Tests
{
    public class ConfigurationFixture
    {
        public IConfiguration Configuration { get; }

        public ConfigurationFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(ConfigurationFixture).Assembly, optional: true)
                .AddEnvironmentVariables(prefix: "OPENEVENTSOURCING_")
                .Build();

            Configuration = configuration;
        }
    }
}
