using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Query.Expressions.Internal
{
    public class JsonArrayIndexExpression : SqlExpression
    {
        public SqlExpression Index { get; }

        public JsonArrayIndexExpression(SqlExpression index,
                                        Type type,
                                        RelationalTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            if (index.Type != typeof(int))
                throw new ArgumentException("Index expression must be of type int", nameof(index));

            Index = index;
        } 
        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonArrayIndex(this)
                : base.Accept(visitor);


        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => this;

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append("[");
            expressionPrinter.Visit(Index);
            expressionPrinter.Append("]");
        }

        public override string ToString() => $"[{Index}]";
    }
}
