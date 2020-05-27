using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
    {
        private readonly JsonSqlExpressionFactory _sqlExpressionFactory;

        public JsonSqlTranslatingExpressionVisitor(RelationalSqlTranslatingExpressionVisitorDependencies dependencies,
                                                   IModel model,
                                                   QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor) 
            : base(dependencies, model, queryableMethodTranslatingExpressionVisitor)
        {
            _sqlExpressionFactory = (JsonSqlExpressionFactory)Dependencies.SqlExpressionFactory;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.ArrayIndex)
            {
                if (TranslationFailed(binaryExpression.Left, Visit(binaryExpression.Left), out var sqlLeft) || TranslationFailed(binaryExpression.Right, Visit(binaryExpression.Right), out var sqlRight))
                    return null;

                if (sqlLeft is ColumnExpression column)
                {
                    var arrayIndex = _sqlExpressionFactory.JsonArrayIndex(sqlLeft, sqlRight);

                    return arrayIndex;
                }                

                if (sqlLeft is JsonValueExpression json)
                {
                    // If the left hand side of the expression is a JSON value expression, we should create a new path
                    // copy removing the original path expression (i.e. the previous path member).
                    // This prevents any duplicate paths from having the following case:
                    //
                    //  Paths = JsonMember (Orders), JsonArrayIndex (Orders[0]), JsonMember (Price)
                    //
                    var path = json.Expression.Path;
                    var arrayIndex = _sqlExpressionFactory.JsonMemberArrayIndex(path.Last(), sqlRight);
                    var paths = new List<SqlExpression>();

                    for (var i = 0; i < path.Length - 1; i++)
                        paths.Add(path[i]);

                    paths.Add(arrayIndex);

                    var newPath = _sqlExpressionFactory.JsonPathTraversal(json.Expression.Expression, paths.ToArray(), arrayIndex.Type, arrayIndex.TypeMapping);
                    return _sqlExpressionFactory.JsonValueExpression((JsonPathTraversalExpression)newPath, newPath.Type, newPath.TypeMapping);
                }

                return null;
            }

            return base.VisitBinary(binaryExpression);
        }

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType == ExpressionType.ArrayLength)
            {
                if (TranslationFailed(unaryExpression.Operand, Visit(unaryExpression.Operand), out var sqlOperand))
                    return null;

                return _sqlExpressionFactory.JsonArrayLength(sqlOperand);
            }

            return base.VisitUnary(unaryExpression);
        }

        #region Copied from RelationalSqlTranslatingExpressionVisitor

        [DebuggerStepThrough]
        private bool TranslationFailed(Expression original, Expression translation, out SqlExpression castTranslation)
        {
            if (original != null && !(translation is SqlExpression))
            {
                castTranslation = null;
                return true;
            }

            castTranslation = translation as SqlExpression;
            return false;
        }

        #endregion Copied from RelationalSqlTranslatingExpressionVisitor
    }
}
