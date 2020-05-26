using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Tests.Fakes
{
    public class Statistics
    {
        public string Text { get; set; }
        public long Visits { get; set; }
        public int Purchases { get; set; }
        public NestedStatistics Nested { get; set; }
    }
}
