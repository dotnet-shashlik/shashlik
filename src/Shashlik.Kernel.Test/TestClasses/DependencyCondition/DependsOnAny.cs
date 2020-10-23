using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True), ConditionType = ConditionType.ANY)]
    public class DependsOnAny : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}