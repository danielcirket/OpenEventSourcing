using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Query.Expressions.Internal
{
    public class JsonArrayLengthExpression : SqlExpression
    {
        public SqlExpression Expression { get; }

        public JsonArrayLengthExpression(SqlExpression expression,
                                         RelationalTypeMapping typeMapping)
            : base(expression.Type, typeMapping)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Expression = expression;
        } 
        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonArrayLength(this)
                : base.Accept(visitor);


        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => this;

        public override void Print(ExpressionPrinter expressionPrinter)
        {   
        }
    }
}
