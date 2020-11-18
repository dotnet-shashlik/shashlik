using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Test.TestClasses.DependencyCondition
{
    [ConditionDependsOn(typeof(EnvConditionProd), typeof(NeedTestOption1True), ConditionType = ConditionType.ANY)]
    [Singleton]
    public class DependsOnAny
    {
        // should be not null
    }
}