using System;
using Microsoft.EntityFrameworkCore.Query;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Internal;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly SqlServerJsonOptions _options;

        public JsonQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies,
                                            SqlServerJsonOptions options)
        {
            if (dependencies == null)
                throw new ArgumentNullException(nameof(dependencies));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _dependencies = dependencies;
            _options = options;
        }

        public virtual QuerySqlGenerator Create()
            => new JsonQuerySqlGenerator(_dependencies, _options);
    }
}
