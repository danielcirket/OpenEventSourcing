using System;

namespace OpenEventSourcing.Domain
{
    public abstract class Entity
    {
        public virtual Guid? Id { get; protected set; }
        public virtual int? Version { get; protected set; }
    }
}
