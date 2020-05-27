using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonSqlTranslatingExpressionVisitorFactory : IRelationalSqlTranslatingExpressionVisitorFactory
    {
        private readonly RelationalSqlTranslatingExpressionVisitorDependencies _dependencies;

        public JsonSqlTranslatingExpressionVisitorFactory(RelationalSqlTranslatingExpressionVisitorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public virtual RelationalSqlTranslatingExpressionVisitor Create(IModel model,
                                                                        QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor)
        {
            return new JsonSqlTranslatingExpressionVisitor(
                _dependencies,
                model,
                queryableMethodTranslatingExpressionVisitor);
        }
    }
}
