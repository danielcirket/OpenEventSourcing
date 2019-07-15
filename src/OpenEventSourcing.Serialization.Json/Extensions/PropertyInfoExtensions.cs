using System;
using System.Reflection;

namespace OpenEventSourcing.Serialization.Json.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool HasSetter(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return property.GetSetMethod(true) != null;
        }
    }
}
