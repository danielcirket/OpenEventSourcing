using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonValueExpression : SqlExpression
    {
        public JsonPathTraversalExpression Expression { get; }

        public JsonValueExpression(JsonPathTraversalExpression expression,
                                   Type type, 
                                   RelationalTypeMapping typeMapping) 
            : base(type, typeMapping)
        {
            Expression = expression;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonValueExpression(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            throw new NotImplementedException();
        }
    }
}
