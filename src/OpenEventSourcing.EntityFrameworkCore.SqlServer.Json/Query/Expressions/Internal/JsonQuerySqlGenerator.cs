using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Internal;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Query.Expressions.Internal
{
    public class JsonQuerySqlGenerator : SqlServerQuerySqlGenerator
    {
        private readonly SqlServerJsonOptions _options;

        public JsonQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies,
                                     SqlServerJsonOptions options) 
            : base(dependencies)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        public virtual Expression VisitJsonValueExpression(JsonValueExpression expression)
        {
            Sql.Append("JSON_VALUE(");

            Visit(expression.Expression);

            Sql.Append(")");

            return expression;
        }

        public virtual Expression VisitJsonObjectPathTraversal(JsonPathTraversalExpression expression)
        {
            Visit(expression.Expression);

            Sql.Append(", ");

            if (_options.Strict)
                Sql.Append("'strict ");
            else
                Sql.Append("'");

            Sql.Append("$");

            if (expression.Path.Length > 0)
                Sql.Append(".");

            for (var i = 0; i < expression.Path.Length; i++)
            {
                Visit(expression.Path[i]);

                if (i < expression.Path.Length - 1)
                    Sql.Append(".");
            }

            Sql.Append("'");

            return expression;
        }

        public virtual Expression VisitJsonArrayPathTraversal(JsonArrayTraversalExpression expression)
        {
            Sql.Append("JSON_VALUE(");

            if (_options.Strict)
                Sql.Append("strict ");

            Visit(expression.Expression);

            Sql.Append(", ");
            Sql.Append("'$");

            for (var i = 0; i < expression.Path.Length; i++)
            {
                Visit(expression.Path[i]);

                if (i < expression.Path.Length - 1)
                    Sql.Append(".");
            }

            Sql.Append("')");

            return expression;
        }

        public virtual Expression VisitJsonMemberAccess(JsonMemberAccessExpression expression)
        {
            if (_options.CasingConvention == Infrastructure.SqlServerJsonCasingConvention.PascalCase)
                Sql.Append(expression.Member);
            else
                Sql.Append(char.ToLower(expression.Member[0]) + expression.Member.Substring(1));

            return expression;
        }

        public virtual Expression VisitJsonMemberArrayIndex(JsonMemberArrayIndexExpression expression)
        {
            Visit(expression.Array);
            Sql.Append("[");

            if (expression.Index is SqlParameterExpression parameter)
            {
                Sql.Append("[");
                Sql.Append("'+");
                Visit(parameter);
                Sql.Append("+'");
            }
            else
            {
                Visit(expression.Index);
            }
            Sql.Append("]");

            return expression;
        }

        public virtual Expression VisitJsonArrayIndex(JsonArrayIndexExpression expression)
        {
            Sql.Append("[");
            Visit(expression.Index);
            Sql.Append("]");

            return expression;
        }

        public virtual Expression VisitJsonArrayLength(JsonArrayLengthExpression expression)
        {
            Sql.Append("(");
            Sql.Append("SELECT COUNT(1) FROM ");
            Sql.Append("OPENJSON(");
            Visit(expression.Expression);
            Sql.Append(")");
            Sql.Append(")");

            return expression;
        }
    }
}
