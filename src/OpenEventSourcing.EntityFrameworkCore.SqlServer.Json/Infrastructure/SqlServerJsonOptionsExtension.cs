using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Storage.Internal;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Infrastructure
{
    public class SqlServerJsonOptionsExtension : IDbContextOptionsExtension
    {
        private bool _strict = false;
        private SqlServerJsonCasingConvention _casing = SqlServerJsonCasingConvention.CamelCase;
        private DbContextOptionsExtensionInfo _info;

        public bool Strict => _strict;
        public SqlServerJsonCasingConvention CasingConvention => _casing;
        public DbContextOptionsExtensionInfo Info => _info ?? (_info = new SqlServerJsonOptionsExtensionInfo(this));

        public SqlServerJsonOptionsExtension() { }
        public SqlServerJsonOptionsExtension(SqlServerJsonOptionsExtension extension)
        {
            _strict = extension._strict;
            _casing = extension._casing;
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkCoreSqlServerJson();
        }

        public void Validate(IDbContextOptions options)
        {
            var internalServiceProvider = options.FindExtension<CoreOptionsExtension>()?.InternalServiceProvider;

            if (internalServiceProvider != null)
            {
                using (var scope = internalServiceProvider.CreateScope())
                {
                    if (scope.ServiceProvider.GetService<IEnumerable<IRelationalTypeMappingSourcePlugin>>()?.Any(s => s is SqlServerJsonTypeMappingSourcePlugin) != true)
                        throw new InvalidOperationException($"{nameof(SqlServerDbContextOptionsExtensions.EnableJsonSupport)} requires {nameof(Extensions.ServiceCollectionExtensions.AddEntityFrameworkCoreSqlServerJson)} to be called on the internal service provider used.");
                }
            }
        }

        public SqlServerJsonOptionsExtension UseStrict(bool strict)
        {
            var clone = new SqlServerJsonOptionsExtension(this);

            clone._strict = strict;

            return clone;
        }
        public SqlServerJsonOptionsExtension UseCamelCase()
        {
            var clone = new SqlServerJsonOptionsExtension(this);

            clone._casing = SqlServerJsonCasingConvention.CamelCase;

            return clone;
        }
        public SqlServerJsonOptionsExtension UsePascalCase()
        {
            var clone = new SqlServerJsonOptionsExtension(this);

            clone._casing = SqlServerJsonCasingConvention.PascalCase;

            return clone;
        }

        private class SqlServerJsonOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public override bool IsDatabaseProvider => false;
            public override string LogFragment { get; }
            public new SqlServerJsonOptionsExtension Extension => (SqlServerJsonOptionsExtension)base.Extension;

            public SqlServerJsonOptionsExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override long GetServiceProviderHashCode() => 0;
            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            { 
                debugInfo["OpenEventSourcing.EntityFrameworkCore.SqlServer.Json:" + nameof(SqlServerDbContextOptionsExtensions.EnableJsonSupport)] = "1";
                debugInfo["OpenEventSourcing.EntityFrameworkCore.SqlServer.Json:" + nameof(Extension.Strict)] = Extension.Strict ? "1" : "0";
                debugInfo["OpenEventSourcing.EntityFrameworkCore.SqlServer.Json:" + nameof(Extension.CasingConvention)] = Extension.CasingConvention.ToString();
            }
        }
    }

    public enum SqlServerJsonCasingConvention
    {
        CamelCase = 0,
        PascalCase = 1,
    }
}
