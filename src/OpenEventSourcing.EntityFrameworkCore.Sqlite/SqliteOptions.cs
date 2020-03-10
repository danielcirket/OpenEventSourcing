using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.Sqlite
{
    public class SqliteOptions
    {
        public string StoreConnectionString { get; internal set; }
        public string ProjectionConnectionString { get; internal set; }
        internal Action<SqliteDbContextOptionsBuilder> SqliteOptionsBuilder { get; set; }

        public SqliteOptions UseStoreConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            StoreConnectionString = connectionString;

            return this;
        }
        public SqliteOptions UseProjectionConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            ProjectionConnectionString = connectionString;

            return this;
        }
        public SqliteOptions UseSqliteOptions(Action<SqliteDbContextOptionsBuilder> optionsAction = null)
        {
            SqliteOptionsBuilder = optionsAction;

            return this;
        }
    }
}
