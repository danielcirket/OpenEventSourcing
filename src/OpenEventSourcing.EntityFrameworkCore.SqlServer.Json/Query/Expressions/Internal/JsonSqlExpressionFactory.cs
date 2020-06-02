using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonSqlExpressionFactory : SqlExpressionFactory
    {
        private readonly IRelationalTypeMappingSource _relationalTypeMappingSource;

        public JsonSqlExpressionFactory(SqlExpressionFactoryDependencies dependencies,
            IRelationalTypeMappingSource relationalTypeMappingSource)
            : base(dependencies)
        {
            if (relationalTypeMappingSource == null)
                throw new ArgumentNullException(nameof(relationalTypeMappingSource));

            _relationalTypeMappingSource = relationalTypeMappingSource;
        }

        public virtual SqlExpression JsonValueExpression(JsonPathTraversalExpression expression, Type type, RelationalTypeMapping typeMapping = null)
        {
            return ConvertFromText(new JsonValueExpression(expression, type, typeMapping), type);
        }

        public virtual SqlExpression JsonPathTraversal(SqlExpression expression, IEnumerable<SqlExpression> path, Type type, RelationalTypeMapping typeMapping = null)
        {
            return new JsonPathTraversalExpression(ApplyDefaultTypeMapping(expression), path.Select(ApplyDefaultTypeMapping).ToArray(), type, typeMapping);
        }

        public virtual SqlExpression JsonMemberAccess(string member, Type type, RelationalTypeMapping typeMapping = null)
        {
            return new JsonMemberAccessExpression(member, type, typeMapping);
        }

        public virtual SqlExpression JsonMemberArrayIndex(SqlExpression array, SqlExpression index, RelationalTypeMapping typeMapping = null)
        {
            Type elementType = null;

            if (array.Type.IsArray)
                elementType = array.Type.GetElementType();
            else if (array.Type.IsGenericList())
                elementType = array.Type.GetGenericArguments()[0];
            else
                throw new ArgumentException($"Json array expression must be an Array or List<T> type.", nameof(array));

            if (typeMapping == null)
                typeMapping = FindMapping(elementType) ?? array.TypeMapping;

            if (index is SqlParameterExpression param && param.Type != typeof(string))
                index = Convert(ApplyDefaultTypeMapping(index), index.Type, FindMapping(typeof(string)));
            else
                index = ApplyDefaultTypeMapping(index);

            return new JsonMemberArrayIndexExpression(array, index, elementType, typeMapping);
        }

        public virtual SqlExpression JsonArrayIndex(SqlExpression array, SqlExpression index, RelationalTypeMapping typeMapping = null)
        {
            Type elementType = null;

            if (array.Type.IsArray)
                elementType = array.Type.GetElementType();
            else if (array.Type.IsGenericList())
                elementType = array.Type.GetGenericArguments()[0];
            else
                throw new ArgumentException($"Json array expression must be an Array or List<T> type.", nameof(array));

            if (typeMapping == null)
                typeMapping = FindMapping(elementType) ?? array.TypeMapping;

            if (index is SqlParameterExpression param && param.Type != typeof(string))
                index = Convert(ApplyDefaultTypeMapping(index), index.Type, FindMapping(typeof(string)));
            else
                index = ApplyDefaultTypeMapping(index);

            return ConvertFromText(new JsonArrayTraversalExpression(array, new[] { new JsonArrayIndexExpression(ApplyDefaultTypeMapping(index), elementType, typeMapping) }, array.Type, FindMapping(typeof(string))), elementType);
        }

        public virtual SqlExpression JsonArrayLength(SqlExpression expression)
        {
            // We get a ColumnExpression when we are acessing the length on a "top-level" column, rather than a nested json array.
            if (expression is ColumnExpression column)
                return new JsonArrayLengthExpression(JsonPathTraversal(column, Array.Empty<SqlExpression>(), column.Type, column.TypeMapping), FindMapping(typeof(int)));

            if (expression is JsonValueExpression value)
                return new JsonArrayLengthExpression(value.Expression, FindMapping(typeof(int)));

            return null;
        }

        private SqlExpression ConvertFromText(SqlExpression expression, Type returnType)
        {
            switch (Type.GetTypeCode(returnType.UnwrapNullableType()))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Convert(expression, returnType, FindMapping(returnType));
                default:
                    return (returnType == typeof(Guid))
                        ? Convert(expression, returnType, FindMapping(returnType))
                        : expression;
            }
        }
    }
}
