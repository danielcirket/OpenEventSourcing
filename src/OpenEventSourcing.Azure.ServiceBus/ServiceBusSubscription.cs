using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusSubscription
    {
        private readonly List<Type> _events;

        public string Name { get; internal set; }
        public IEnumerable<Type> Events => _events.AsReadOnly();
        /// <summary>
        /// Defaults to 10.
        /// </summary>
        public int MaxDeliveryCount { get; internal set; }
        /// <summary>
        /// Defaults to false.
        /// </summary>
        public bool UseDeadLetterOnExpiration { get; internal set; }
        /// <summary>
        /// Defaults to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan TimeToLive { get; internal set; }
        /// <summary>
        /// Defaults to 1 minute.
        /// </summary>
        public TimeSpan LockDuration { get; internal set; }
        /// <summary>
        /// Defaults to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan DeleteOnIdleAfter { get; internal set; }

        public ServiceBusSubscription()
        {
            _events = new List<Type>();
            MaxDeliveryCount = ServiceBusSubscriptionDefaults.MaxDeliveryCount;
            UseDeadLetterOnExpiration = ServiceBusSubscriptionDefaults.UseDeadLetterOnExpiration;
            TimeToLive = ServiceBusSubscriptionDefaults.TimeToLive;
            LockDuration = ServiceBusSubscriptionDefaults.LockDuration;
            DeleteOnIdleAfter = ServiceBusSubscriptionDefaults.DeleteOnIdleAfter;
        }

        /// <summary>
        /// Sets the name for the subscription.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ServiceBusSubscription UseName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            Name = name;

            return this;
        }
        public ServiceBusSubscription ForEvent<TEvent>()
            where TEvent : IEvent
        {
            var type = typeof(TEvent);

            if (!_events.Contains(type))
                _events.Add(type);

            return this;
        }
        /// <summary>
        /// Sets the maximum number of times a message will attempt to be redlivered if it could not be successfully consumed.
        /// </summary>
        /// <param name="maxDeliveryCount"></param>
        /// <returns></returns>
        public ServiceBusSubscription WithMaxDeliveryCount(int maxDeliveryCount)
        {
            if (maxDeliveryCount < 1)
                throw new ArgumentException($"Max message delivery count cannot be less than 1.", nameof(maxDeliveryCount));

            MaxDeliveryCount = maxDeliveryCount;

            return this;
        }
        /// <summary>
        /// If the message becomes expired by exceeding the time to live the message may be sent to the dead-letter queue rather than being lost. Defaults to false.
        /// </summary>
        /// <param name="deadLetterOnExpiration"></param>
        /// <returns></returns>
        public ServiceBusSubscription UseDeadLetterOnMessageExpiration(bool deadLetterOnExpiration)
        {
            UseDeadLetterOnExpiration = deadLetterOnExpiration;

            return this;
        }
        /// <summary>
        /// Sets the time to live for the subscription. Defaults to TimeSpan.MaxValue if not set.
        /// </summary>
        /// <param name="timeToLive"></param>
        /// <returns></returns>
        public ServiceBusSubscription UseTimeToLive(TimeSpan timeToLive)
        {
            if (timeToLive < TimeSpan.FromSeconds(1.0))
                throw new ArgumentException($"Time to live cannot be less than 1 second.");

            TimeToLive = timeToLive;

            return this;
        }
        /// <summary>
        /// Sets the subscription message lock duration. Defaults to 1 minutes if not set.
        /// </summary>
        /// <param name="lockDuration"></param>
        /// <returns></returns>
        public ServiceBusSubscription UseLockDuration(TimeSpan lockDuration)
        {
            if (lockDuration < TimeSpan.FromSeconds(5.0))
                throw new ArgumentException($"Lock duration cannot be less than 5 seconds.");
            if (lockDuration > TimeSpan.FromMinutes(5.0))
                throw new ArgumentException($"Lock duration cannot be greater than 5 minutes.");

            LockDuration = lockDuration;

            return this;
        }
        /// <summary>
        /// Set the subscription to automatically delete after a defined period of inactivity. Defaults to TimeSpan.MaxValue.
        /// </summary>
        /// <param name="autoDeleteOnIdle"></param>
        /// <returns></returns>
        public ServiceBusSubscription AutoDeleteOnIdleAfter(TimeSpan autoDeleteOnIdle)
        {
            if (autoDeleteOnIdle < TimeSpan.FromMinutes(5.0))
                throw new ArgumentException($"Auto delete on idle cannot be less than 5 minutes.");

            DeleteOnIdleAfter = autoDeleteOnIdle;

            return this;
        }
    }
}
