using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes
{
    public class JsonEntity
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public int[] ToplevelArray { get; set; }
        public Order[] TopLevelObjectArray { get; set; }
    }
}
