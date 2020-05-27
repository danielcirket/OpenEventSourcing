using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonMemberArrayIndexExpression : SqlExpression
    {
        public SqlExpression Array { get; }
        public SqlExpression Index { get; }

        public JsonMemberArrayIndexExpression(SqlExpression array,
                                    SqlExpression index,
                                    Type type,
                                    RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (!array.Type.IsArray && !array.Type.IsGenericList())
                throw new ArgumentException("Array expression must be of an array type", nameof(array));
            if (index.Type != typeof(int))
                throw new ArgumentException("Index expression must be of type int", nameof(index));

            Array = array;
            Index = index;
        } 
        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonMemberArrayIndex(this)
                : base.Accept(visitor);


        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => this;

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Visit(Array);
            expressionPrinter.Append("[");
            expressionPrinter.Visit(Index);
            expressionPrinter.Append("]");
        }

        public override string ToString() => $"{Array}[{Index}]";
    }
}
