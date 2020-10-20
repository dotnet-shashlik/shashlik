using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.EventBus.Rabbit
{
    public class RabbitEventBusConfigure : IEventBusConfigure
    {
        public RabbitEventBusConfigure(IOptions<RabbitEventBusOptions> options)
        {
            Options = options.Value;
        }

        private RabbitEventBusOptions Options { get; }

        public void Configure(CapOptions capOptions)
        {
            capOptions.UseRabbitMQ(r => { Options.CopyTo(r); });
        }
    }
}