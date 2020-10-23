using DotNetCore.CAP;
using Shashlik.Kernel.Attributes;

namespace Shashlik.EventBus.Rabbit
{
    [AutoOptions("Shashlik.EventBus.Rabbit")]
    public class RabbitEventBusOptions : RabbitMQOptions
    {
    }
}