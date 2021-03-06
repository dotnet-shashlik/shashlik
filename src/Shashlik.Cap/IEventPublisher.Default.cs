﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Shashlik.Cap
{
    internal class DefaultEventPublisher : IEventPublisher
    {
        public DefaultEventPublisher(ICapPublisher publisher, INameRuler nameRuler)
        {
            CapPublisher = publisher;
            NameRuler = nameRuler;
        }

        public ICapPublisher CapPublisher { get; }
        private INameRuler NameRuler { get; }

        public void Publish<T>(T eventModel, string? callbackName = null)
            where T : class, IEvent
        {
            if (eventModel is null)
                throw new ArgumentNullException(nameof(eventModel));
            CapPublisher.Publish(NameRuler.GetName(typeof(T)), eventModel, callbackName);
        }

        public Task PublishAsync<T>(T eventModel, string? callbackName = null,
            CancellationToken cancellationToken = default)
            where T : class, IEvent
        {
            if (eventModel is null)
                throw new ArgumentNullException(nameof(eventModel));
            return CapPublisher.PublishAsync(NameRuler.GetName(typeof(T)), eventModel, callbackName, cancellationToken);
        }
    }
}