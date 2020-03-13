using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    public interface IEventModelFactory
    {
        Entities.Event Create(IEvent @event, ICommand causation);
        Entities.Event Create(IEvent @event, IIntegrationEvent causation);
        Entities.Event Create(IEvent @event, Guid? causationId, Guid? correlationId, string userId);
    }
}
