using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OpenEventSourcing.EntityFrameworkCore.ChangeTracking
{
    public class JsonValueComparer<T> : ValueComparer<T>
    //where T : class
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public JsonValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(T left, T right)
        {
            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            return ConvertTo(left).Equals(ConvertTo(right));
        }
        private static int HashCode(T model)
        {
            if (model == null)
                return 0;

            if (model is IEquatable<T>)
                return model.GetHashCode();

            return ConvertTo(model).GetHashCode();
        }
        private static T CreateSnapshot(T model)
        {
            if (model is ICloneable cloneable)
                return (T)cloneable.Clone();

            return ConvertFrom(ConvertTo(model));
        }
        private static string ConvertTo(T model)
        {
            if (model == null)
                return null;

            return JsonConvert.SerializeObject(model, _settings);
        }
        private static T ConvertFrom(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
