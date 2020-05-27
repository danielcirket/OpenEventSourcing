using System;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Storage.Internal
{
    public class SqlServerJsonTypeMapping : RelationalTypeMapping
    {
        protected SqlServerJsonTypeMapping(RelationalTypeMappingParameters parameters)
            : base(parameters)
        {
        }

        public SqlServerJsonTypeMapping(Type clrType)
            : base("nvarchar(max)", clrType, System.Data.DbType.String, true)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
            => new SqlServerJsonTypeMapping(parameters);

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            var result = value switch
            {
                string @string => EscapeSqlLiteral(@string),
                _ => throw new NotImplementedException()
                //_ => EscapeSqlLiteral(JsonSerializer.Serialize(value, new JsonSerializerOptions
                //{
                //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //    PropertyNameCaseInsensitive = true,
                //    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                //}))
            };

            return result;
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            var value = parameter.Value;
            var length = (value as string)?.Length;

            parameter.Size = ((value == null || value == DBNull.Value || (length.HasValue && length <= 4000)) ? 4000 : (-1));
        }

        protected virtual string EscapeSqlLiteral(string literal)
        {
            if (literal == null)
                throw new ArgumentNullException(nameof(literal));

            return literal.Replace("'", "''");
        }
    }
}
