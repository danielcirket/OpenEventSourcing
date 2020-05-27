using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions
{
    public static class SqlServerDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder EnableJsonSupport(this DbContextOptionsBuilder optionsBuilder, Action<SqlServerJsonOptionsBuilder> sqlServerJsonOptionsAction = null)
        {
            if (optionsBuilder == null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);
            
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            sqlServerJsonOptionsAction?.Invoke(new SqlServerJsonOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        private static SqlServerJsonOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            return optionsBuilder.Options.FindExtension<SqlServerJsonOptionsExtension>() ?? new SqlServerJsonOptionsExtension();
        }
    }
}
