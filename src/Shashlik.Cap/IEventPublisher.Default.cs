using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Cap
{
    [ConditionDependsOnMissing(typeof(IEventPublisher))]
    internal class DefaultEventPublisher : IEventPublisher
    {
        public DefaultEventPublisher(ICapPublisher publisher)
        {
            CapPublisher = publisher;
        }

        public ICapPublisher CapPublisher { get; }

        public void Publish<T>(T eventModel, string? callbackName = null)
            where T : class, IEvent
        {
            if (eventModel == null)
                throw new ArgumentNullException(nameof(eventModel));
            CapPublisher.Publish(typeof(T).Name, eventModel, callbackName);
        }

        public Task PublishAsync<T>(T eventModel, string? callbackName = null,
            CancellationToken cancellationToken = default)
            where T : class, IEvent
        {
            if (eventModel == null)
                throw new ArgumentNullException(nameof(eventModel));
            return CapPublisher.PublishAsync(typeof(T).Name, eventModel, callbackName, cancellationToken);
        }
    }
}