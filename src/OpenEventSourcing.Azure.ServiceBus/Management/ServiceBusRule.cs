using System;
using System.Collections.Generic;
using System.Text;

namespace OpenEventSourcing.Azure.ServiceBus.Management
{
    public class ServiceBusRule
    {
        public string Topic { get; set; }
        public string Rule { get; set; }
        public string Subscription { get; set; }
    }
}
