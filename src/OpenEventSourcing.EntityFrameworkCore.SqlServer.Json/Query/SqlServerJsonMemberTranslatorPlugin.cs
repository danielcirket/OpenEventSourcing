using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.ExpressionTranslators;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query
{
    public class SqlServerJsonMemberTranslatorPlugin : IMemberTranslatorPlugin
    {
        public IEnumerable<IMemberTranslator> Translators { get; }

        public SqlServerJsonMemberTranslatorPlugin(ISqlExpressionFactory sqlExpressionFactory)
        {
            Translators = new IMemberTranslator[]
            {
                new JsonSqlMemberTranslator(sqlExpressionFactory),
            };
        }
    }
}
