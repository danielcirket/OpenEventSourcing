using System;

namespace OpenEventSourcing.Azure.ServiceBus.Exceptions
{
    public class RuleAlreadyExistsException : Exception
    {
        public string RuleName { get; }

        public RuleAlreadyExistsException(string name)
            : base($"A rule with name '{name}' already exists.")
        {
            RuleName = name;
        }
    }
}
