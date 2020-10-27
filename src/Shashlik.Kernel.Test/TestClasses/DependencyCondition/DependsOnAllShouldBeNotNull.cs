using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    // default is ALL
    [ConditionDependsOn(typeof(EnvConditionDev), typeof(NeedTestOption1True))]
    public class DependsOnAllShouldBeNotNull : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}