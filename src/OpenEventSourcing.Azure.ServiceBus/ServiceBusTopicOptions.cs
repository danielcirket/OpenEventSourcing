using System;

namespace OpenEventSourcing.Azure.ServiceBus
{
    public class ServiceBusTopicOptions
    {
        public string Name { get; internal set; }
        /// <summary>
        /// Defaults to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan TimeToLive { get; internal set; }
        /// <summary>
        /// Defaults to 5 minutes.
        /// </summary>
        public TimeSpan LockDuration { get; internal set; }
        /// <summary>
        /// Defaults to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan DeleteOnIdleAfter { get; internal set; }

        public ServiceBusTopicOptions()
        {
            TimeToLive = ServiceBusTopicDefaults.TimeToLive;
            LockDuration = ServiceBusTopicDefaults.LockDuration;
            DeleteOnIdleAfter = ServiceBusTopicDefaults.DeleteOnIdleAfter;
        }

        /// <summary>
        /// Sets the name of the topic.
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public ServiceBusTopicOptions WithName(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or empty.", nameof(topicName));
            if (topicName.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(topicName), $"'{nameof(topicName)}' exceeds the 50 character limit for then entity path name.");

            Name = topicName;

            return this;
        }
        /// <summary>
        /// Sets the time to live for the topic. Defaults to TimeSpan.MaxValue if not set.
        /// </summary>
        /// <param name="timeToLive"></param>
        /// <returns></returns>
        public ServiceBusTopicOptions UseTimeToLive(TimeSpan timeToLive)
        {
            if (timeToLive < TimeSpan.FromSeconds(1.0))
                throw new ArgumentException($"Time to live cannot be less than 1 second.");

            TimeToLive = timeToLive;

            return this;
        }
        /// <summary>
        /// Sets the topic lock duration. Defaults to 5 minutes if not set.
        /// </summary>
        /// <param name="lockDuration"></param>
        /// <returns></returns>
        public ServiceBusTopicOptions UseLockDuration(TimeSpan lockDuration)
        {
            if (lockDuration < TimeSpan.FromSeconds(5.0))
                throw new ArgumentException($"Lock duration cannot be less than 5 seconds.");
            if (lockDuration > TimeSpan.FromMinutes(5.0))
                throw new ArgumentException($"Lock duration cannot be greater than 5 minutes.");

            LockDuration = lockDuration;

            return this;
        }
        /// <summary>
        /// Set the topic to automatically delete after a defined period of inactivity. Defaults to TimeSpan.MaxValue.
        /// </summary>
        /// <param name="autoDeleteOnIdle"></param>
        /// <returns></returns>
        public ServiceBusTopicOptions AutoDeleteOnIdleAfter(TimeSpan autoDeleteOnIdle)
        {
            if (autoDeleteOnIdle < TimeSpan.FromMinutes(5.0))
                throw new ArgumentException($"Auto delete on idle cannot be less than 5 minutes.");

            DeleteOnIdleAfter = autoDeleteOnIdle;

            return this;
        }
    }
}
