using System;

namespace OpenEventSourcing.Domain
{
    public abstract class Entity
    {
        public virtual string Id { get; protected set; }
        public virtual int? Version { get; protected set; }
    }
}
