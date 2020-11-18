using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(int), ConditionType = ConditionType.ANY)]
    [Singleton]
    public class DependsOnAnyShouldBeNull
    {
        // should be null
    }
}