using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Tests.Fakes
{
    public class NestedStatistics
    {
        public int SomeProperty { get; set; }
        public int? SomeNullableProperty { get; set; }
        public int[] IntArray { get; set; }
    }
}
