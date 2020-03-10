using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer
{
    public class SqlServerOptions
    {
        public string StoreConnectionString { get; internal set; }
        public string ProjectionConnectionString { get; internal set; }
        internal Action<SqlServerDbContextOptionsBuilder> SqlServerOptionsBuilder { get; set; }

        public SqlServerOptions UseStoreConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            StoreConnectionString = connectionString;

            return this;
        }
        public SqlServerOptions UseProjectionConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            ProjectionConnectionString = connectionString;

            return this;
        }
        public SqlServerOptions UseSqlServerOptions(Action<SqlServerDbContextOptionsBuilder> optionsAction = null)
        {
            SqlServerOptionsBuilder = optionsAction;

            return this;
        }
    }
}
