using DotNetCore.CAP;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Cap.Rabbit
{
    [AutoOptions("Shashlik.Cap.Rabbit")]
    public class RabbitCapOptions : RabbitMQOptions
    {
    }
}