﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    public interface ITopicMessageSender
    {
        Task SendAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEvent> events);
        //Task<string> SendAsync<TEvent>(TEvent @event, DateTimeOffset publishOnUtc) where TEvent : IEvent;
        //Task CancelScheduledMessageAsync(string identifier);
    }
}
