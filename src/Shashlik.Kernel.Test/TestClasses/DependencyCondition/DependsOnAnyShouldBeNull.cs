using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(int), ConditionType = ConditionType.ANY)]
    public class DependsOnAnyShouldBeNull : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be null
    }
}