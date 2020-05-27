using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Internal
{
    public class SqlServerJsonOptions : ISingletonOptions
    {
        public bool Strict { get; private set; }
        public SqlServerJsonCasingConvention CasingConvention { get; private set; }

        public void Initialize(IDbContextOptions options)
        {
            var extension = options.FindExtension<SqlServerJsonOptionsExtension>() ?? new SqlServerJsonOptionsExtension();

            Strict = extension.Strict;
            CasingConvention = extension.CasingConvention;
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }
}
