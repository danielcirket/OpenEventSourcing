using System;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Tests.Fakes
{
    public class Order
    {
        public decimal Price { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime ShippingDate { get; set; }
    }
}
