using System;
using System.Linq;
using System.Linq.Expressions;
#if NETCOREAPP3_0 || NETCOREAPP3_1
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
#endif
#if NETSTANDARD2_0
using Microsoft.EntityFrameworkCore.Query.Internal;
#endif
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonArrayTraversalExpression : SqlExpression
    {
        public SqlExpression Expression { get; }
        public SqlExpression[] Path { get; }

        public JsonArrayTraversalExpression(SqlExpression expression,
                                            SqlExpression[] path,
                                            Type type, 
                                            RelationalTypeMapping typeMapping) 
            : base(type, typeMapping)
        {
            Expression = expression;
            Path = path;
        }

        protected override Expression Accept(ExpressionVisitor visitor)
            => visitor is JsonQuerySqlGenerator generator
                ? generator.VisitJsonArrayPathTraversal(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return Update((SqlExpression)visitor.Visit(Expression), Path.Select(p => (SqlExpression)visitor.Visit(p)).ToArray());
        }

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            throw new NotImplementedException();
        }

        public virtual JsonArrayTraversalExpression Append(SqlExpression pathComponent)
        {
            var oldPath = Path;
            var newPath = new SqlExpression[oldPath.Length + 1];
            Array.Copy(oldPath, newPath, oldPath.Length);
            newPath[newPath.Length - 1] = pathComponent;
            return new JsonArrayTraversalExpression(Expression, newPath, Type, TypeMapping);
        }

        public virtual JsonArrayTraversalExpression Update(
            SqlExpression expression,
            SqlExpression[] path)
            => expression == Expression &&
               path.Length == Path.Length &&
               path.Zip(Path, (x, y) => (x, y)).All(tup => tup.x == tup.y)
                ? this
                : new JsonArrayTraversalExpression(expression, path, Type, TypeMapping);

        public override string ToString() => $"{Expression}.{string.Join(".", Path.Select(p => p.ToString()))}";
    }
}
