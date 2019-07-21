using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Serialization.Json.ValueProviders;

namespace OpenEventSourcing.Serialization.Json.Resolvers
{
    internal class ImmutablePropertyCamelCasePropertyNamesContactResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (prop.Writable)
                return prop;

            if (member.MemberType == MemberTypes.Property)
            {
                var property = (PropertyInfo)member;

                prop.Writable = property.HasSetter();

                if (prop.Writable)
                    return prop;

                var declaringType = property.DeclaringType;
                var compilerField = declaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (compilerField != null)
                {
                    prop.Writable = true;
                    prop.ValueProvider = new ImmutablePropertyValueProvider(member);
                }
            }

            return prop;
        }
    }
}
