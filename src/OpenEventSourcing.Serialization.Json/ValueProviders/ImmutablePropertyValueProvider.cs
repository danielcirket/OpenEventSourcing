using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace OpenEventSourcing.Serialization.Json.ValueProviders
{
    internal class ImmutablePropertyValueProvider : IValueProvider
    {
        private readonly MemberInfo _member;

        public ImmutablePropertyValueProvider(MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (!(member is PropertyInfo) && !(member is FieldInfo))
                throw new ArgumentException($"MemberInfo '{member.Name}' must be of type {nameof(FieldInfo)} or {nameof(PropertyInfo)}");

            _member = member;
        }

        public object GetValue(object target)
        {
            // https://github.com/dotnet/corefx/issues/26053
            if (_member is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsByRef)
                throw new InvalidOperationException($"Could not create getter for '{propertyInfo.Name}'. ByRef return values are not supported.");

            switch (_member)
            {
                case PropertyInfo prop:
                    return prop.GetValue(target);
                case FieldInfo field:
                    return field.GetValue(target);
            }

            throw new ArgumentException($"MemberInfo '{_member.Name}' must be of type '{nameof(FieldInfo)}' or '{nameof(PropertyInfo)}'");
        }
        public void SetValue(object target, object value)
        {
            var type = target.GetType();

            var memberName = _member.Name;
            var compilerField = target.GetType().GetField($"<{memberName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (compilerField == null)
                throw new InvalidOperationException($"Cannot set value for '{memberName}' for '{type.Name}'. Compiler generated backing field '<{memberName}>k__BackingField' is missing.");

            compilerField.SetValue(target, value);
        }
    }
}
