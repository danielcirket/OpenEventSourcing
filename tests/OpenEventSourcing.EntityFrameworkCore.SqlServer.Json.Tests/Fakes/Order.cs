using System;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes
{
    public class Order
    {
        public decimal Price { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime ShippingDate { get; set; }
    }
}
