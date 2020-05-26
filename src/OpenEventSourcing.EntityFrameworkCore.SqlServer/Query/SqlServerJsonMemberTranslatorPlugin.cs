using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Query.ExpressionTranslators;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Query
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
