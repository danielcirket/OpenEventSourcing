using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class Customer
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public EquatableAddress Address { get; set; }
    }
}
