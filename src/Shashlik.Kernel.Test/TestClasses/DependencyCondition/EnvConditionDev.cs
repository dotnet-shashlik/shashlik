using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnHostEnvironment("Development")]
    [Singleton]
    public class EnvConditionDev
    {
        // should be not null
    }
}