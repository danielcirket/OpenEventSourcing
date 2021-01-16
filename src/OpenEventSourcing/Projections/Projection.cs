using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Projections
{
    public abstract class Projection : IProjection
    {
        private readonly Dictionary<Type, Func<IEventContext<IEvent>, Task>> _handlers;

        protected Projection()
        {
            _handlers = new Dictionary<Type, Func<IEventContext<IEvent>, Task>>();
        }

        protected void Handles<TEvent>(Func<TEvent, Task> handler)
            where TEvent : IEvent
        {
            _handlers.Add(typeof(TEvent), e => handler((TEvent)e));
        }

        public async Task HandleAsync(IEventContext<IEvent> context)
        {
            if (_handlers.TryGetValue(context.GetType(), out var handler))
                await handler(context);
        }
    }
}
