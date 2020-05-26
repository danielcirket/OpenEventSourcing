using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Tests.Fakes
{
    public class Customer
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public bool IsVip { get; set; }
        public Statistics Statistics { get; set; }
        public Order[] Orders { get; set; }
    }
}
