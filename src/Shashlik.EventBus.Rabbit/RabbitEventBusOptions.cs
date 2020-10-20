using DotNetCore.CAP;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.EventBus.Rabbit
{
    [AutoOptions("Shashlik.EventBus.Rabbit")]
    public class RabbitEventBusOptions : RabbitMQOptions
    {
    }
}