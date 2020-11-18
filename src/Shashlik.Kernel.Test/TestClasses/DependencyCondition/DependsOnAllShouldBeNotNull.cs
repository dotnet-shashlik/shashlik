using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    // default is ALL
    [ConditionDependsOn(typeof(EnvConditionDev), typeof(NeedTestOption1True))]
    [Singleton]
    public class DependsOnAllShouldBeNotNull
    {
        // should be null
    }
}