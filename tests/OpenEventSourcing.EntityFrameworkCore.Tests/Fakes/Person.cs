using System;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class Person
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}
