using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Cap.Rabbit
{
    public class RabbitCapAutowire : ICapAutowire
    {
        public RabbitCapAutowire(IOptions<RabbitCapOptions> options,
            IOptions<ShashlikCapOptions> capOptions)
        {
            CapOptions = capOptions.Value;
            Options = options.Value;
        }

        private RabbitCapOptions Options { get; }
        private ShashlikCapOptions CapOptions { get; }

        public void Configure(CapOptions capOptions)
        {
            if (!CapOptions.Enable)
                return;
            capOptions.UseRabbitMQ(r => { Options.CopyTo(r, true); });
        }
    }
}