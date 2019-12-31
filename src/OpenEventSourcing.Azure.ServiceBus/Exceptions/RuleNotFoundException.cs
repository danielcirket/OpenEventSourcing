using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class RuleNotFoundException : Exception
    {
        public string RuleName { get; }

        public RuleNotFoundException(string name)
            : base($"A rule with name '{name}' could not be found.")
        {
            RuleName = name;
        }
    }
}
