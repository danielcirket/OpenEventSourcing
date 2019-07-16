using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Projections
{
    public abstract class Projection : IProjection
    {
        private readonly Dictionary<Type, Func<IEvent, Task>> _handlers;

        protected Projection()
        {
            _handlers = new Dictionary<Type, Func<IEvent, Task>>();
        }

        protected void Handles<TEvent>(Func<TEvent, Task> handler)
            where TEvent : IEvent
        {
            _handlers.Add(typeof(TEvent), e => handler((TEvent)e));
        }

        public async Task HandleAsync(IEvent @event)
        {
            if (_handlers.TryGetValue(@event.GetType(), out var handler))
                await handler(@event);
        }
    }
}
