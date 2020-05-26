using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Query.Expressions.Internal
{
    public class JsonMemberAccessExpression : SqlExpression
    {
        public string Member { get; }

        public JsonMemberAccessExpression(string member,
                                          Type type, 
                                          RelationalTypeMapping typeMapping) 
            : base(type, typeMapping)
        {
            if (string.IsNullOrWhiteSpace(member))
                throw new ArgumentException($"'{nameof(member)}' cannot be null or empty.", nameof(member));

            Member = member;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonMemberAccess(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append(".");
            expressionPrinter.Append(Member);
        }

        public override string ToString() => Member;
    }
}
