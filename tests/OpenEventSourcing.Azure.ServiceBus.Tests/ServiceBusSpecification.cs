using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests
{
    public abstract class ServiceBusSpecification : IClassFixture<ConfigurationFixture>
    {
        private readonly ConfigurationFixture _fixture;

        protected IConfiguration Configuration { get; }
        protected IServiceProvider ServiceProvider { get; }

        public ServiceBusSpecification(ConfigurationFixture fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            _fixture = fixture;

            var services = new ServiceCollection();

            ConfigureServices(services);

            services.AddSingleton<IConfiguration>(_ => _fixture.Configuration);

            Configuration = _fixture.Configuration;

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        protected abstract void ConfigureServices(IServiceCollection services);
    }
}
