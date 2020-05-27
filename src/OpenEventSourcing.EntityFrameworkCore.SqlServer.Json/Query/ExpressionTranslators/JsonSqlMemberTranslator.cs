using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.ExpressionTranslators
{
    public class JsonSqlMemberTranslator : IMemberTranslator
    {
        private readonly JsonSqlExpressionFactory _sqlExpressionFactory;
        private readonly RelationalTypeMapping _stringTypeMapping;

        public JsonSqlMemberTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = (JsonSqlExpressionFactory)sqlExpressionFactory;
            _stringTypeMapping = sqlExpressionFactory.FindMapping(typeof(string));
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType)
        {
            var memberAccess = _sqlExpressionFactory.JsonMemberAccess(member.Name, returnType, _stringTypeMapping);
            return TranslateMemberAccess(instance, memberAccess, returnType);
        }

        public virtual SqlExpression TranslateMemberAccess(SqlExpression instance,
                                                           SqlExpression member,
                                                           Type returnType)
        {
            if (instance is ColumnExpression column)
            {
                if (column.TypeMapping.Converter != null)
                {
                    if (column.TypeMapping.Converter.ProviderClrType == typeof(string))
                    {
                        var path = _sqlExpressionFactory.JsonPathTraversal(column, new[] { member }, returnType, _stringTypeMapping);

                        return _sqlExpressionFactory.JsonValueExpression((JsonPathTraversalExpression)path, returnType, _stringTypeMapping);
                    }
                }
            }

            if (instance is JsonPathTraversalExpression traversal)
            {
                var path = new List<SqlExpression>(traversal.Path);

                path.Add(_sqlExpressionFactory.ApplyDefaultTypeMapping(member));

                var newPath = _sqlExpressionFactory.JsonPathTraversal(traversal.Expression, path, returnType, traversal.TypeMapping);

                return _sqlExpressionFactory.JsonValueExpression((JsonPathTraversalExpression)newPath, returnType, _stringTypeMapping);
            }

            if (instance is JsonValueExpression value)
            {
                var path = new List<SqlExpression>(value.Expression.Path);

                path.Add(_sqlExpressionFactory.ApplyDefaultTypeMapping(member));

                var newPath = _sqlExpressionFactory.JsonPathTraversal(value.Expression.Expression, path, returnType, value.Expression.TypeMapping);

                return _sqlExpressionFactory.JsonValueExpression((JsonPathTraversalExpression)newPath, returnType, _stringTypeMapping);
            }

            return null;
        }
    }
}
