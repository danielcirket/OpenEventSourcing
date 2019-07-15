﻿using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
