using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenEventSourcing.Domain
{
    public abstract class ValueObject<T> : IEquatable<ValueObject<T>>
        where T : ValueObject<T>
    {
        protected abstract IEnumerable<object> EqualityComponents();

        public bool Equals(ValueObject<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(other, null))
                return false;

            if (GetType() != other.GetType())
                return false;

            return EqualityComponents().SequenceEqual(other.EqualityComponents());
        }

        public override bool Equals(object obj) => Equals(obj as ValueObject<T>);
        public override int GetHashCode()
        {
            var parts = EqualityComponents();

            var hashcode = new HashCode();

            foreach (var part in parts)
                hashcode.Add(part);

            return hashcode.ToHashCode();
        }

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }
        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            if (ReferenceEquals(a, null))
                return !ReferenceEquals(b, null);

            return !a.Equals(b);
        }
    }
}
