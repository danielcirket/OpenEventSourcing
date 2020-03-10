using System;
using System.Collections.Generic;
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace OpenEventSourcing.EntityFrameworkCore.Postgres
{
    public class NpgsqlOptions
    {
        public string StoreConnectionString { get; internal set; }
        public string ProjectionConnectionString { get; internal set; }
        internal Action<NpgsqlDbContextOptionsBuilder> NpgsqlOptionsBuilder { get; set; }

        public NpgsqlOptions UseStoreConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            StoreConnectionString = connectionString;

            return this;
        }
        public NpgsqlOptions UseProjectionConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or empty.");

            ProjectionConnectionString = connectionString;

            return this;
        }
        public NpgsqlOptions UseNpgsqlOptions(Action<NpgsqlDbContextOptionsBuilder> optionsAction = null)
        {
            NpgsqlOptionsBuilder = optionsAction;

            return this;
        }
    }
}
