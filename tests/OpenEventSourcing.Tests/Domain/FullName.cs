using System.Collections.Generic;
using OpenEventSourcing.Domain;

namespace OpenEventSourcing.Tests.Domain
{
    internal class FullName : ValueObject<FullName>
    {
        public string FirstName { get; }
        public string LastName { get; }

        public FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        protected override IEnumerable<object> EqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}
