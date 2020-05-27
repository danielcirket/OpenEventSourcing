using System;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes
{
    public class Person
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}
