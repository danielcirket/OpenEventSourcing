using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Extensions
{
    public class SqlServerJsonOptionsBuilder : IRelationalDbContextOptionsBuilderInfrastructure
    {
        public DbContextOptionsBuilder OptionsBuilder { get; }

        public SqlServerJsonOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            OptionsBuilder = optionsBuilder;
        }

        public SqlServerJsonOptionsBuilder UseStrict(bool strict = true)
            => WithOption(e => e.UseStrict(strict));

        public SqlServerJsonOptionsBuilder UseCamelCase()
            => WithOption(e => e.UseCamelCase());

        public SqlServerJsonOptionsBuilder UsePascalCase()
            => WithOption(e => e.UsePascalCase());

        protected virtual SqlServerJsonOptionsBuilder WithOption(Func<SqlServerJsonOptionsExtension, SqlServerJsonOptionsExtension> setAction)
        {
            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(setAction(OptionsBuilder.Options.FindExtension<SqlServerJsonOptionsExtension>() ?? new SqlServerJsonOptionsExtension()));
            return this;
        }
    }
}
