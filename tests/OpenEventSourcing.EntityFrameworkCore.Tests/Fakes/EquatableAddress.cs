using System;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class EquatableAddress : IEquatable<EquatableAddress>
    {
        public string AddressLine1 { get; set; }

        public bool Equals(EquatableAddress other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return AddressLine1.Equals(other.AddressLine1);
        }
    }
}
