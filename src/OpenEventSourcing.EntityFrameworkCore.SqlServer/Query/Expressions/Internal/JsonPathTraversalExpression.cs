using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Query.Expressions.Internal
{
    public class JsonPathTraversalExpression : SqlExpression
    {
        public SqlExpression Expression { get; }
        public SqlExpression[] Path { get; }

        public JsonPathTraversalExpression(SqlExpression expression,
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
                ? generator.VisitJsonObjectPathTraversal(this)
                : base.Accept(visitor);

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return Update((SqlExpression)visitor.Visit(Expression), Path.Select(p => (SqlExpression)visitor.Visit(p)).ToArray());
        }

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            throw new NotImplementedException();
        }

        public virtual JsonPathTraversalExpression Append(SqlExpression pathComponent)
        {
            var oldPath = Path;
            var newPath = new SqlExpression[oldPath.Length + 1];
            Array.Copy(oldPath, newPath, oldPath.Length);
            newPath[newPath.Length - 1] = pathComponent;
            return new JsonPathTraversalExpression(Expression, newPath, Type, TypeMapping);
        }

        public virtual JsonPathTraversalExpression Update(
            SqlExpression expression,
            SqlExpression[] path)
            => expression == Expression &&
               path.Length == Path.Length &&
               path.Zip(Path, (x, y) => (x, y)).All(tup => tup.x == tup.y)
                ? this
                : new JsonPathTraversalExpression(expression, path, Type, TypeMapping);

        public override string ToString() => $"{Expression}.{string.Join(".", Path.Select(p => p.ToString()))}";
    }
}
