using System;
using System.Collections.Generic;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions
{
    internal static class TypeExtensions
    {
        public static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
        public static bool IsGenericList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}
