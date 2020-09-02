using Shashlik.Kernel.Dependency.Conditions;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [DependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True), ConditionType = ConditionType.ANY)]
    public class DependsOnAny : Shashlik.Kernel.Dependency.ISingleton
    {
        // should be not null
    }
}