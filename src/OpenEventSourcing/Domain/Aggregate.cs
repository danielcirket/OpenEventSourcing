using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Domain
{
    public abstract class Aggregate<TState> : Entity
        where TState : IAggregateState
    {
        private readonly List<IEvent> _uncommitedEvents;
        private readonly Dictionary<Type, Action<IEvent>> _eventHandlers;
        protected readonly TState _state;

        protected Aggregate(TState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _uncommitedEvents = new List<IEvent>();
            _eventHandlers = new Dictionary<Type, Action<IEvent>>();
            _state = state;
        }

        public abstract TState GetState();
        public virtual void FromHistory(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                Apply(@event, isNew: false);
        }
        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _uncommitedEvents.AsReadOnly();
        }
        public void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            _eventHandlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }
        protected void Apply(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            Apply(@event, isNew: true);
        }

        private void Apply(IEvent @event, bool isNew)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            // TODO(Dan): The fact we have no handler registered for the event will need logging most likely.
            if (_eventHandlers.TryGetValue(@event.GetType(), out var handler))
                handler(@event);

            if (isNew)
                _uncommitedEvents.Add(@event);
            else
                Version = @event.Version;
        }
    }
}
