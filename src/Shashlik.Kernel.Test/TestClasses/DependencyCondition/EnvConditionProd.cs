using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionOnHostEnvironment("Production")]
    [Singleton]
    public class EnvConditionProd
    {
        // should be null
    }
}