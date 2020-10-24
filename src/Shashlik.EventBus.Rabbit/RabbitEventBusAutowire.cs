using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.EventBus.Rabbit
{
    public class RabbitEventBusAutowire : IEventBusAutowire
    {
        public RabbitEventBusAutowire(IOptions<RabbitEventBusOptions> options,
            IOptions<EventBusOptions> eventBusOptions)
        {
            EventBusOptions = eventBusOptions.Value;
            Options = options.Value;
        }

        private RabbitEventBusOptions Options { get; }
        private EventBusOptions EventBusOptions { get; }

        public void Configure(CapOptions capOptions)
        {
            if (!EventBusOptions.Enable)
                return;
            capOptions.UseRabbitMQ(r => { Options.CopyTo(r); });
        }
    }
}